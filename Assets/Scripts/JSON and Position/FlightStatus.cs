using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

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
    public Material selectedMat;

    private Material originalMat;
    private string previousPlaneName;

    //private List<Renderer> planesMat;

    public void Start()
    {
        //var planesMat = new List<Renderer>();
        originalMat = gameObject.GetComponentsInChildren<Renderer>()[2].material;
    }
    public void OnTriggerEnter(Collider other)
    {
        

        var localPlanes = ListJsonPlaneLocation_zero.planes;
        var localTags = localPlanes.Select(p => p.tag).ToList();
        int inndx = localTags.IndexOf("Selected");

        if (other.CompareTag("IndexFinger") && inndx == -1 && gameObject.name != "PlaneHolderInside")
        { 
            gameObject.GetComponentsInChildren<Renderer>()[2].material = selectedMat; // highlight with a different material
            gameObject.GetComponentInChildren<TextMeshPro>().text = gameObject.name; // display registration name
            gameObject.tag = "Selected";
            previousPlaneName = gameObject.name;

            Debug.Log("Sel Aircraft" + other.tag + " name: "+ gameObject.name  + " " + gameObject.tag);
        }
        else if (other.CompareTag("IndexFinger") && inndx != 0 && gameObject.name==previousPlaneName)
        {
            gameObject.GetComponentsInChildren<Renderer>()[2].material = originalMat;
            gameObject.GetComponentInChildren<TextMeshPro>().text = "";
            gameObject.tag = "Untagged";

            Debug.Log("Des Aircraft " + other.tag + " name: " + gameObject.name + " " + gameObject.tag);
        }
    }
}