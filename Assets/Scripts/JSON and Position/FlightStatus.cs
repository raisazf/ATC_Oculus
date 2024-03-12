using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

// This class is responsible for identifying an airplane touched be the player.
// 

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
            previousPlaneName = gameObject.name;

        }
        else if (other.CompareTag("IndexFinger") && indxPlane != -1 && indxButton != -1 && gameObject.name == previousPlaneName)
        {
            gameObject.GetComponentsInChildren<Renderer>()[2].material = originalMat;
            gameObject.GetComponentInChildren<TextMeshPro>().text = "";
            gameObject.tag = "Untagged";
            localButtons[indxPlane].gameObject.tag = "Untagged";

            var colors = localButtons[indxPlane].GetComponent<Button>().colors;
            colors.pressedColor = Color.white;
            colors.selectedColor = Color.white;
            localButtons[indxPlane].gameObject.GetComponent<Button>().colors = colors;

        }
        else if ((indxPlane != -1 && indxButton == -1 && gameObject.name != previousPlaneName) || (indxPlane == -1 && indxButton != -1 && gameObject.name != previousPlaneName))
        {

            foreach (var plane in localPlanes)
            {
                plane.gameObject.GetComponentsInChildren<Renderer>()[2].material = originalMat;
                plane.gameObject.GetComponentInChildren<TextMeshPro>().text = "";
                plane.gameObject.tag = "Untagged";

                var indx = localButtonNames.IndexOf(plane.gameObject.name);

                localButtons[indx].gameObject.tag = "Untagged";

                //localButtons[indxPlane].gameObject.GetComponent<Button>().
                var colors = localButtons[indx].gameObject.GetComponent<Button>().colors;
                colors.pressedColor = Color.white;
                colors.selectedColor = Color.white;
                localButtons[indx].gameObject.GetComponent<Button>().colors = colors;

                indxPlane = -1;
                indxButton = -1;

            }
        }
    }
}