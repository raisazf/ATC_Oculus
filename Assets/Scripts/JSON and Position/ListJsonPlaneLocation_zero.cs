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

public class ListJsonPlaneLocation_zero : MonoBehaviour
{
    // Start is called before the first frame update
    //public GameObject marker;
    [SerializeField] public GameObject GlobalSystem;
    [SerializeField] public GameObject marker;
    [SerializeField] public GameObject flightButtonsTemplate;
    [SerializeField] public GameObject flightButtonsParent;

    [SerializeField] public float radiusAdjustment = 0.605f; // globe ball radius (unity units 1m)
    [SerializeField] public float airportLatitude = 38.94846f;
    [SerializeField] public float airpotLongitude = -77.44057f;
    
    [SerializeField] public Material haloMat;

    [SerializeField] public int altitudeScale = 1;
    [SerializeField] public float positionScale = 0.1f;
    [SerializeField] public float indexAdjustment=10f;
    [SerializeField] private List<GameObject> flightButtons;
    private List<string> flightNames;
    private List<GameObject> planes;
    private List<string> flightNamesPrevious;
    private List<flights> requestFlightResponses;
    private flights flightResponse;

    StreamReader streamReader;

    private float latitude = 0f; // lat
    private float longitude = 0f; // lng
    private float altitude = 0f; // alt
    private float direction = 0f;

    private float radius;
    private float newRadius;
    private float xPos, yPos, zPos;

    private List<float> latList;
    private List<float> lngList;
    private List<float> altList;

    private float latIndex;
    private float lngIndex;
    private float altIndex;

    private int donotIncludRequest = 1;
    private List<int> requestNumbers; // how many requests
    private string jsonString;

    private int count = 0;
    private float interval = 1f;
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

        radius = radiusAdjustment * GlobalSystem.transform.localScale.x;

        ReadJson();

    }

    public void Update()
    {
        radius = radiusAdjustment * GlobalSystem.transform.localScale.x;
        // loop through all requests. use interval to set speed of calling PlaneLocation()
        for (int i = 0; i < requestFlightResponses.Count; i++)
        {
            time += Time.deltaTime;
            while (time >= interval)
            {
                flightResponse = requestFlightResponses[i];
                PlaneLocation();
                time -= interval;
                count = 0;
            }

            count++;
        }
    }

    // Remove flights that left the region from the list of active flights.
    private void RemoveFromList(int index)
    {

        if (index < 0) return;

        Destroy(planes[index]);
        planes.RemoveAt(index);

        Destroy(flightButtons[index]);
        flightButtons.RemoveAt(index);

        flightNames.RemoveAt(index);
    }

    private void GetXYZPositions()
    {

        newRadius = (float)((float)(2.093e7 + altitude) * radius / 2.093e7);

        xPos = (newRadius) * Mathf.Cos(latitude + latIndex) * Mathf.Cos(longitude + lngIndex) + GlobalSystem.transform.position.x;
        zPos = (newRadius) * Mathf.Cos(latitude + latIndex) * Mathf.Sin(longitude + lngIndex) + GlobalSystem.transform.position.z;
        yPos = (newRadius) * Mathf.Sin(latitude + latIndex) + GlobalSystem.transform.position.y;


    }

    // If a flight doesn't exist, add it to the list of active flights
    private void AddToList(FlightsEmbeddedField flight)
    {

        flightNames.Add(flight.reg_number);

        altitude = flight.alt * altitudeScale;

        direction = -flight.dir;

        latitude = flight.lat * Mathf.Deg2Rad;
        longitude = flight.lng * Mathf.Deg2Rad;

        GetXYZPositions();

        planes.Add(Instantiate(marker, new Vector3(xPos, yPos, zPos), Quaternion.identity, GlobalSystem.transform));


        planes[flightNames.Count - 1].transform.LookAt(new Vector3(GlobalSystem.transform.position.x, GlobalSystem.transform.position.y, GlobalSystem.transform.position.z));
        planes[flightNames.Count - 1].name = flight.reg_number;

        flightButtons.Add(Instantiate(flightButtonsTemplate,
            new Vector3(flightButtonsTemplate.transform.position.x,
            flightButtonsTemplate.transform.position.y, flightButtonsTemplate.transform.position.z),
            Quaternion.Euler(0f, 0f, 0f), flightButtonsParent.transform));

        // convert back for display purposes
        latitude = latitude * Mathf.Rad2Deg;
        longitude = longitude * Mathf.Rad2Deg;

        // remove scale for altitude separation in VR
        altitude = altitude / altitudeScale;

        string temp = flight.reg_number + "    " + latitude.ToString("F4") + "   " + longitude.ToString("F4") +
            "    " + altitude.ToString("F0") + "    " + direction.ToString("F0");

        flightButtons[flightNames.Count - 1].GetComponentInChildren<TextMeshProUGUI>().text = temp;

    }


    public void PlaneLocation()
    {
        int index = 0;
        var tempResponseNames = new List<string>();

        var tempLatList = new List<float>();

        latList = reorderParameterList("lat", airportLatitude);
        lngList = reorderParameterList("lng", airpotLongitude);

        foreach (var flight in flightResponse.response)
        {
            tempResponseNames.Add(flight.reg_number);
        }

        foreach (var flight in flightResponse.response)
        {

            if (flight.lat < airportLatitude)
            {
                latIndex = -(latList.IndexOf(flight.lat)- latList.IndexOf(flight.lat)/ indexAdjustment) * positionScale;
            }
            else if (flight.lat > airportLatitude)
            {
                latIndex = (latList.IndexOf(flight.lat)- latList.IndexOf(flight.lat)/ indexAdjustment) * positionScale;
            }
            else
            {
                latIndex = 0;
            }

            if (flight.lng < airpotLongitude)
            {
                lngIndex = -(lngList.IndexOf(flight.lng) - lngList.IndexOf(flight.lng)/ indexAdjustment) * positionScale;
            }
            else if (flight.lat > airportLatitude)
            {
                lngIndex = lngList.IndexOf(flight.lng - lngList.IndexOf(flight.lng) / indexAdjustment) * positionScale;
            }
            else
            {
                lngIndex = 0;
            }


            altIndex = lngList.IndexOf(flight.alt);

            // if flight is in the active list, update its position
            if (flightNames.Contains(flight.reg_number))
            {
                index = flightNames.FindIndex(a => a.Contains(flight.reg_number));
                UpdatePlanePosition(index, flight);
            }
            else // add a new flight to the list
            {
                AddToList(flight);
            }
        }

        for (int i = 0; i < flightNamesPrevious.Count - 1; i++)
        {
            var flight = flightNamesPrevious[i];

            if (!tempResponseNames.Contains(flight))
            {
                // if a flight left the area if interest, remove it from the list of active flights
                index = flightNames.FindIndex(a => a.Contains(flight));
                RemoveFromList(index);
            }
        }

        flightNamesPrevious = flightNames;

        //SaveFlightInfo(flightNames, flightNames.Count);

    }

    private List<float> reorderParameterList(string infoType, float splitvalue)
    {
        Debug.Log("Info Type = " + infoType + " splitvalue = " + splitvalue);

        List<float> localList;

        localList = new List<float>();

        if (infoType == "lat")
        {
            localList = flightResponse.response.Select(flt => flt.lat).ToList();
        }
        else if (infoType == "lng")
        {
            localList = flightResponse.response.Select(flt => flt.lng).ToList();
        }
        else if (infoType == "alt")
        {
            localList = flightResponse.response.Select(flt => flt.alt).ToList();
        }

        //Airport location
       localList.Insert(0, splitvalue);

        //splitvalue = localList.Average();
        localList.Insert(0, splitvalue);

        localList = localList.OrderBy(x => x).ToList();

        int splitIndex = localList.IndexOf(splitvalue);

        Debug.Log("Info Type = " + infoType + " splitvalue = " + splitvalue + " split index" + splitIndex);
        List<float> firstHalf = localList.GetRange(0, splitIndex);
        List<float> secondHalf = localList.GetRange(splitIndex, localList.Count - splitIndex);

        firstHalf.Reverse();

        List<float> reorderedList = firstHalf.Concat(secondHalf).ToList();

        string values = String.Join(", ", reorderedList);
        Debug.Log("reorederList for " + infoType + " : " + values);

        return reorderedList;
    }

    private void UpdatePlanePosition(int currentIndex, FlightsEmbeddedField flight)
    {

        altitude = flight.alt * altitudeScale; // scale altitude to get a better separation in Oculus

        direction = -flight.dir;

        latitude = flight.lat * Mathf.Deg2Rad;
        longitude = flight.lng * Mathf.Deg2Rad;

        GetXYZPositions();

        planes[currentIndex].transform.position = new Vector3(xPos, yPos, zPos);
        planes[currentIndex].transform.rotation = Quaternion.identity;

        // adjust plane location to GlobalSystem rotation
        Quaternion rotation = Quaternion.Euler(0f, -(90f - GlobalSystem.transform.rotation.eulerAngles.y), 0f);
        planes[currentIndex].transform.position = GlobalSystem.transform.position + rotation * (planes[currentIndex].transform.position - GlobalSystem.transform.position);
        planes[currentIndex].transform.rotation = rotation * planes[currentIndex].transform.rotation;

        // plane is perpendicular to surface normal
        planes[currentIndex].transform.LookAt(new Vector3(GlobalSystem.transform.position.x, GlobalSystem.transform.position.y, GlobalSystem.transform.position.z));

        if (flight.arr_iata == "IAD")
        {
            planes[currentIndex].gameObject.GetComponentsInChildren<Renderer>()[2].material = haloMat; // highlight with a different material
        }

        // adjust route direction. Set to zero if grounded
        if (altitude <= 90f)
        {
            planes[currentIndex].transform.Rotate(0f, 0f, 0f, Space.Self);
            direction = 0;
        }
        else
        {
            planes[currentIndex].transform.Rotate(0f, 0f, direction, Space.Self);
            direction = -direction;
        }


        // remove altitude scale used for separation in VR
        altitude = altitude / altitudeScale;

        // parameters are converted back to degrees
        latitude = latitude * Mathf.Rad2Deg;
        longitude = longitude * Mathf.Rad2Deg;

        // update button information
        string temp = flight.reg_number + "     " + latitude.ToString("F3") + "      " + longitude.ToString("F3") + "      " + altitude.ToString("F0") + "     " + direction.ToString("F0");
        flightButtons[currentIndex].GetComponentInChildren<TextMeshProUGUI>().text = temp;

    }

    public void SaveFlightInfo(List<string> flightInfo, int indx)
    {

        StreamWriter writer = new StreamWriter("C:/Users/raisa/Documents/GitHub/ATC_test_v2/Assets/Resources/TryingToWrite.txt", true);
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
        //StreamReader sr = new StreamReader("C:/Users/raisa/Documents/GitHub/ATC_test_v2 - Oculus/Assets/Resources/AirLab_data.json");
        //StreamReader sr = new StreamReader("/storage/emulated/0/Android/data/com.DefaultCompany.ATC_test_v2/files/AirLab_data_test.json");
        streamReader = new StreamReader("C:/Users/raisa/AppData/LocalLow/DefaultCompany/ATC_test_v2/AirLab_data_test.json");
        streamReader.BaseStream.Position = 0;
        jsonString = streamReader.ReadToEnd();
        streamReader.Close();

        Int32 next = 0;

        // break json line (the entire request) into individual responses
        for (int i = 1; i < jsonString.Length; i++)
        {
            Int32 indx = jsonString.IndexOf("res", i);
            if (indx > next)
            {
                requestNumbers.Add(indx);
                next = indx;
            }

        }

        // break each response into individual flights. 
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

            flightResponse = JsonConvert.DeserializeObject<flights>(jsonString[start..end]);

            if (i == 0)
            {
                foreach (FlightsEmbeddedField flight in flightResponse.response)
                {
                    flightNamesPrevious.Add(flight.reg_number);

                }
            }
            requestFlightResponses.Add(flightResponse);

        }
    }
}