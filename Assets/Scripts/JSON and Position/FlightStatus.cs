using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

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
        var localPlaneTags = localPlanes.Select(p => p.tag).ToList();
        int indxPlane = localPlaneTags.IndexOf("Selected");

        var localButtons = ListJsonPlaneLocation_zero.flightButtons;
        var localButtonTags = localButtons.Select(b => b.tag).ToList();
        int indxButton = localButtonTags.IndexOf("Selected");

        var localPlaneNames = localPlanes.Select(p => p.name).ToList();
        var localButtonNames = localButtons.Select(b => b.name).ToList();

        //Debug.Log("Aircraft " + other.tag + " name: " + gameObject.name + " " + gameObject.tag + " plane index " + indxPlane + " button tag " + indxButton);

        if (other.CompareTag("IndexFinger") && indxPlane == -1 && indxButton == -1 && gameObject.name != "PlaneHolderInside")
        {
            originalMat = gameObject.GetComponentsInChildren<Renderer>()[2].material;

            gameObject.GetComponentsInChildren<Renderer>()[2].material = selectedMat; // highlight with a different material
            gameObject.GetComponentInChildren<TextMeshPro>().text = gameObject.name; // display registration name
            gameObject.tag = "Selected";
            localPlaneTags = localPlanes.Select(p => p.tag).ToList();
            indxPlane = localPlaneTags.IndexOf("Selected");
            localButtons[indxPlane].gameObject.tag = "Selected";

            localButtons[indxPlane].gameObject.GetComponent<Button>().Select();

            var colors = localButtons[indxPlane].GetComponent<Button>().colors;
            colors.pressedColor = new Color(0f, 0f, 1f, 0.34f);
            colors.selectedColor = new Color(0f, 0f, 1f, 0.34f);
            localButtons[indxPlane].gameObject.GetComponent<Button>().colors = colors;

            // Debug.Log("Inside Aircraft tag " + gameObject.tag);

            previousPlaneName = gameObject.name;

            Debug.Log("Sel Aircraft " + other.tag + " name: " + gameObject.name + " " + gameObject.tag + " button tag " + localButtons[indxPlane].gameObject.tag);


        }
        else if (other.CompareTag("IndexFinger") && indxPlane != -1 && indxButton != -1 && gameObject.name == previousPlaneName)
        {
            gameObject.GetComponentsInChildren<Renderer>()[2].material = originalMat;
            gameObject.GetComponentInChildren<TextMeshPro>().text = "";
            gameObject.tag = "Untagged";
            localButtons[indxPlane].gameObject.tag = "Untagged";

            //localButtons[indxPlane].gameObject.GetComponent<Button>().

            var colors = localButtons[indxPlane].GetComponent<Button>().colors;
            colors.pressedColor = Color.white;
            colors.selectedColor = Color.white;
            localButtons[indxPlane].gameObject.GetComponent<Button>().colors = colors;

            Debug.Log("Des Aircraft " + other.tag + " name: " + gameObject.name + " " + gameObject.tag + " button tag " + localButtons[indxPlane].gameObject.tag);
        }
        else if ((indxPlane != -1 && indxButton == -1 && gameObject.name != previousPlaneName) || (indxPlane == -1 && indxButton != -1 && gameObject.name != previousPlaneName))
        {
            Debug.Log("reset all");

            foreach (var plane in localPlanes)
            {
                Debug.Log("reset each plane");
                plane.gameObject.GetComponentsInChildren<Renderer>()[2].material = originalMat;
                plane.gameObject.GetComponentInChildren<TextMeshPro>().text = "";
                plane.gameObject.tag = "Untagged";
                Debug.Log("Aircraft reset 2?");

                var indx = localButtonNames.IndexOf(plane.gameObject.name);

                localButtons[indx].gameObject.tag = "Untagged";

                Debug.Log("Aircraft reset 3?");


                //localButtons[indxPlane].gameObject.GetComponent<Button>().
                var colors = localButtons[indx].gameObject.GetComponent<Button>().colors;
                Debug.Log("Aircraft reset 4?");
                colors.pressedColor = Color.white;
                Debug.Log("Aircraft reset 5?");
                colors.selectedColor = Color.white;
                Debug.Log("Aircraft reset 6?");
                localButtons[indx].gameObject.GetComponent<Button>().colors = colors;


                Debug.Log("Aircraft reset 7?");
                Debug.Log("Des Aircraft button name: " + plane.gameObject.name +
                    " pressed: " + localButtons[indx].gameObject.GetComponent<Button>().colors.pressedColor +
                    " selected: " + localButtons[indx].gameObject.GetComponent<Button>().colors.selectedColor);

                indxPlane = -1;
                indxButton = -1;

                Debug.Log("Des Aircraft button reset " + gameObject.name + " tag: " + gameObject.tag + " plane name " + plane.name + " plane tag " + plane.gameObject.tag);
            }
        }
    }
}