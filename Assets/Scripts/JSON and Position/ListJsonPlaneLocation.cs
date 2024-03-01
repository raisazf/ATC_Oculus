using System.Collections;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;
using System.IO;
using RestSharp;
using Newtonsoft.Json;
using System.Linq;

public class ListJsonPlaneLocation : MonoBehaviour
{
    // Start is called before the first frame update
    //public GameObject marker;
    [SerializeField] public GameObject GlobalSystem;
    [SerializeField] public GameObject marker;
    [SerializeField] public GameObject flightButtonsTemplate;
    [SerializeField] public GameObject flightButtonsParent;

    [SerializeField] public float radiusAdjustment = 0f; // globe ball radius (unity units 1m)
    [SerializeField] public float airportLatitude = 38.94846f; 
    [SerializeField] public float airpotLongitude = -77.44057f; 

    [SerializeField] private List<GameObject> flightButtons;
    private List<string> flightNames;
    [SerializeField] private List<GameObject> planes;
    private List<string> flightNamesPrevious;
    private List<flights> requestFlightResponses;

    private float latitude = 0f; // lat
    private float longitude = 0f; // long
    private float altitude = 0f;
    private float direction = 0f;
    private OVRVirtualKeyboard overlayKeyboard;

    private flights flightResponse;

    private float radius;
    private float newRadius;
    private float xPos, yPos, zPos;
    private int altitudeScale = 20;

    private int donotIncludRequest = 1;
    private List<int> requestNumbers; // how many requests
    string jsonString;

    private int count = 0;
    private float interval = 60f;
    private float time = 0.0f;

    void Start()
    {
        planes = new List<GameObject>();
        flightButtons = new List<GameObject>();
        flightNamesPrevious = new List<string>();
        flightNames = new List<string>();
        flightResponse = new flights();
        requestNumbers = new List<int>();
        requestFlightResponses = new List<flights>();

        GlobalSystem.transform.Find("Earth").eulerAngles = new Vector3(0f, 90f, 0f);
        radius = 2.093e7f / 1000000f * GlobalSystem.transform.localScale.x - radiusAdjustment;
        

        ReadJson();

    }

    private void Init()
    {

        int inxd = flightNames.Count-1;
        do {
            Debug.Log("Init indx  = " + inxd);
            RemoveFromList(inxd);
            inxd--;
        } while (flightNames!=null && inxd >= 0);

        planes.Clear();
        flightButtons.Clear();
        flightNamesPrevious.Clear();
        flightNames.Clear();

        flightNamesPrevious = flightNames;

        //flightResponse

        Debug.Log("During debug: " + planes.Count + "  " + flightButtons.Count + " flight Names count " + flightNames.Count);

    }

    public void Update()
    {
        //Init();
        //ReadJson();
        //Loopping();

        for (int i = 0; i < requestFlightResponses.Count; i++) {
            time += Time.deltaTime;
            while (time >= interval)
            {
                flightResponse = requestFlightResponses[i]; 
                PlaneLocation();
                time -= interval;
                count = 0;
            }

            //GetDataFromAirLabApi();
            count++;
            //Debug.Log("Count whatever = " + count);
        }
    }

    private void RemoveFromList(int index)
    {
        //Debug.Log("Remove " + flightNames[index] + " from flight Names list at index " + index);

        if (index < 0) return;

        //planes.RemoveAt(index);
        Destroy(planes[index]);
        planes.RemoveAt(index);

        //flightButtons.RemoveAt(index);
        Destroy(flightButtons[index]);
        flightButtons.RemoveAt(index);

        flightNames.RemoveAt(index);
    }

    private void GetXYZPositions()
    {

        newRadius = (float)((float)(2.093e7 + altitude) * radius / 2.093e7);

        xPos = (newRadius) * Mathf.Cos(latitude) * Mathf.Cos(longitude) + GlobalSystem.transform.position.x;
        zPos = (newRadius) * Mathf.Cos(latitude) * Mathf.Sin(longitude) + GlobalSystem.transform.position.z;
        yPos = (newRadius) * Mathf.Sin(latitude) + GlobalSystem.transform.position.y;
    }
    private void AddToList(FlightsEmbeddedField flight)
    {

        flightNames.Add(flight.reg_number);

        altitude = flight.alt * altitudeScale;

        direction = -flight.dir;
        latitude = Mathf.PI * flight.lat / 180;
        longitude = Mathf.PI * flight.lng / 180;

        GetXYZPositions();

        planes.Add(Instantiate(marker, new Vector3(xPos, yPos, zPos), Quaternion.identity, GlobalSystem.transform));

        planes[flightNames.Count - 1].transform.LookAt(new Vector3(GlobalSystem.transform.position.x, GlobalSystem.transform.position.y, GlobalSystem.transform.position.z));
        planes[flightNames.Count - 1].name = flight.reg_number;

       Debug.Log(message: $"Plane {flightNames[flightNames.Count - 1]} altitude {latitude * 180 / Mathf.PI}, {longitude * 180 / Mathf.PI}, {altitude}, {direction} at index {flightNames.Count} and plane list size {planes.Count}");
       //Debug.Log(message: $"Plane {flightNames[flightNames.Count - 1]}  at index {flightNames.Count - 1} and plane list size {planes.Count - 1}");

        //return;

        flightButtons.Add(Instantiate(flightButtonsTemplate,
            new Vector3(flightButtonsTemplate.transform.position.x,
            flightButtonsTemplate.transform.position.y, flightButtonsTemplate.transform.position.z),
            Quaternion.Euler(0f, 0f, 0f), flightButtonsParent.transform));

        // convert back for buttons information
        latitude = latitude * 180 / Mathf.PI;
        longitude = longitude * 180 / Mathf.PI;

        altitude = altitude / altitudeScale;

        string temp = flight.reg_number + "    " + latitude.ToString("F4") + "   " + longitude.ToString("F4") +
            "    " + altitude.ToString("F0") + "    " + direction.ToString("F0");

        flightButtons[flightNames.Count - 1].GetComponentInChildren<TextMeshProUGUI>().text = temp;

    }

    
    public void PlaneLocation()
    {
        int index = 0;
        var tempResponseNames = new List<string>();

        foreach (var flight in flightResponse.response)
        {
            tempResponseNames.Add(flight.reg_number);
        }

        foreach (var flight in flightResponse.response)
        {

            //Debug.Log(" FLIGHT Name length " + flightNames.Count);
            if (flightNames.Contains(flight.reg_number))
            {
                //Debug.Log("Flight " + flight.reg_number + " in both lists");
                index = flightNames.FindIndex(a => a.Contains(flight.reg_number));
                //Debug.Log("Index = " + index + " flight Name length " + flightNames.Count);
                UpdatePlanePosition(index, flight);
            }
            else
            {
                AddToList(flight);
            }
        }

        //foreach (var flight in flightNamesPrevious)
        for (int i = 0; i < flightNamesPrevious.Count - 1; i++)
        {
            var flight = flightNamesPrevious[i];

            if (!tempResponseNames.Contains(flight))
            {
                index = flightNames.FindIndex(a => a.Contains(flight));
                RemoveFromList(index);
            }
        }

        flightNamesPrevious = flightNames;

        //SaveFlightInfo(flightNames, flightNames.Count);

    }

    private void UpdatePlanePosition(int currentIndex, FlightsEmbeddedField flight)
    {
        //Debug.Log(" flight Name " + flight.reg_number + " is at index " + currentIndex + " Planes count" + planes.Count);

        altitude = flight.alt * altitudeScale;// * UnityEngine.Random.Range(-2.0f, 2.0f);

        direction = -flight.dir;
        latitude = Mathf.PI * flight.lat / 180;
        longitude = Mathf.PI * flight.lng / 180;


        GetXYZPositions();


        planes[currentIndex].transform.position = new Vector3(xPos, yPos, zPos);
        planes[currentIndex].transform.rotation = Quaternion.identity;
        planes[currentIndex].transform.parent = GlobalSystem.transform;

        //planes[i].transform.LookAt(Vector3.zero);
        planes[currentIndex].transform.LookAt(new Vector3(GlobalSystem.transform.position.x, GlobalSystem.transform.position.y, GlobalSystem.transform.position.z));

        if (altitude <= 0f)
        {
            planes[currentIndex].transform.Rotate(0f, 0f, 0f, Space.Self);
        }
        else
        {
            planes[currentIndex].transform.Rotate(0f, 0f, direction, Space.Self);
            direction = -direction;
        }

        altitude = altitude / altitudeScale;
        //Debug.Log(message: $"Plane altitude {latitude * 180 / Mathf.PI}, {longitude * 180 / Mathf.PI}, {altitude}, {direction}");

        latitude = latitude * 180 / Mathf.PI;
        longitude = longitude * 180 / Mathf.PI;
        string temp = flight.reg_number + "     " + latitude.ToString("F3") + "      " + longitude.ToString("F3") + "      " + altitude.ToString("F0") + "     " + direction.ToString("F0");
        flightButtons[currentIndex].GetComponentInChildren<TextMeshProUGUI>().text = temp;

    }

    // Update is called once per frame
    public void Keyboard()
    {
        //overlayKeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
        overlayKeyboard.TextCommitField.keyboardType = (TouchScreenKeyboardType)(-1);
        //if (overlayKeyboard != null)
        //    //inputText = overlayKeyboard.text;
        //    Debug.Log("keyboard is activated");

    }

    public void SaveFlightInfo(List<string> flightInfo, int indx)
    {

        StreamWriter writer = new StreamWriter("Assets/Resources/TryingToWrite.txt", true);
        for (int i = 0; i < indx; i++)
        {
            writer.WriteLine(flightInfo[i]); //writer.Write(" ");
            //writer.WriteLine(planes[i].transform.position);
        }
        writer.WriteLine("-------------------");
        writer.Close();
    }

    public void ReadJson()
    {

        StreamReader sr = new StreamReader("/storage/emulated/0/Android/data/com.DefaultCompany.ATC_test_v2/files/AirLab_data.json");
        sr.BaseStream.Position = 0;
        jsonString = sr.ReadToEnd();
        sr.Close();
        Debug.Log("Done with ReadJson");
        Int32 next = 0;

        // break the entire line in json file into individual responses
        for (int i = 1; i < jsonString.Length; i++)
        {
            Int32 indx = jsonString.IndexOf("res", i);
            if (indx > next)
            {
                requestNumbers.Add(indx);
                next = indx;
            }

        }

        // break each response into individual flights
        int start = 0;
        int end = 0;

        for (int i = 0; i < requestNumbers.Count - donotIncludRequest; i++) // the last request could be corrupted
        {
            start = requestNumbers[i] - 2;
            if (i == requestNumbers.Count - donotIncludRequest)
            {
                end = jsonString.Length;
            }
            else
            {
                end = requestNumbers[i + 1] - 2;
            }
            //Debug.Log(i + " " + start + " " + end);
            flightResponse = JsonConvert.DeserializeObject<flights>(jsonString[start..end]);

            if (i == 0)
            {
                foreach (FlightsEmbeddedField flight in flightResponse.response)
                {
                    flightNamesPrevious.Add(flight.reg_number);
                    //Debug.Log("init flights " + flight.reg_number);
                }
            }

            //Debug.Log(" Calling Plane Location for " + i + "!!!!!!!!!!!!!");

            //Debug.Log(" flight Name length in read " + flightNames.Count);
            // PlaneLocation();
            requestFlightResponses.Add(flightResponse);
            Debug.Log(" megaData " + requestFlightResponses.Count);

        }
    }


    public void Loopping()
    {


        // break each response into individual flights
        int start = 0;
        int end = 0;

        for (int i = 0; i < requestNumbers.Count - donotIncludRequest; i++) // the last request could be corrupted
        {
            start = requestNumbers[i] - 2;
            if (i == requestNumbers.Count - donotIncludRequest)
            {
                end = jsonString.Length;
            }
            else
            {
                end = requestNumbers[i + 1] - 2;
            }
            //Debug.Log(i + " " + start + " " + end);
            flightResponse = JsonConvert.DeserializeObject<flights>(jsonString[start..end]);

            if (i == 0)
            {
                foreach (FlightsEmbeddedField flight in flightResponse.response)
                {
                    flightNamesPrevious.Add(flight.reg_number);
                    //Debug.Log("init flights previous " + flight.reg_number);
                }
                flightNames.Clear();
                foreach (string flight in flightNames)
                {
                    flightNames.Clear();
                    //Debug.Log("init flights names " + flight);
                }
            }

            //Debug.Log(" repeat for " + i + "!!!!!!!!!!!!!");

            //PlaneLocation();
            requestFlightResponses.Add(flightResponse);
            Debug.Log(" megaData " + requestFlightResponses.Count);
        }
        
    }
}
