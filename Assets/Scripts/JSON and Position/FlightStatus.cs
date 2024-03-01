using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


// This class is responsible for identifying an airplane touched be the player.
// 
// The idea is once the collider of the touched airplane is triggered the following has to happen:
// 1) the airplane material should change its "halo" to selectedMaterial
// 2) the registration name should appear over that plane

// The issues are:
// 1) More than one airplane gets triggered, although isSelected flag is set to true for the first airplane touched. 
// Another airplane should not be selected until isSelected is set to false.
// Debuggin log shows two or three airplanes with isSelected flags set to true, which I can't figure out why. 
// 2) TextMeshProUGUI text is now showing over the plane, although I'm successfully displaying text within other game objects.

public class FlightStatus : MonoBehaviour
{
    public bool isSelected = false;
    public Material selectedMat;
    public TextMeshProUGUI flightName;

    private Material originalMat;
    private string previousPlane;

    public void Start()
    {
        originalMat = gameObject.GetComponent<Renderer>().material;
    }
    public void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player") && !isSelected && gameObject.name != "PlaneHolderInside")
        {
            isSelected = true;
            
            gameObject.GetComponentsInChildren<Renderer>()[2].material = selectedMat; // highlight with a different material

            //flightName.text = gameObject.name;
            flightName.GetComponent<TextMeshProUGUI>().text = gameObject.name;
            
            previousPlane = gameObject.name; // make sure

            Debug.Log("Selected Aircraft " + gameObject.name + " " + isSelected + " " + previousPlane);
        }

        else if (other.CompareTag("Player") && isSelected && gameObject.name == previousPlane)
        
        {
            isSelected = false;
           
            gameObject.GetComponentsInChildren<Renderer>()[2].material = originalMat;

            flightName.GetComponent<TextMeshProUGUI>().text = "";

            Debug.Log("Deselected Aircraft " + gameObject.name + " " + isSelected + " " + previousPlane);
        }
    }
}