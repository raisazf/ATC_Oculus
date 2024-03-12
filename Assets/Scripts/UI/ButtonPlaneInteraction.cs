using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

// This class is responsible for identifying an airplane based on the button clicked.
// Below is the comment about the issue in CAPS


public class ButtonPlaneInteraction : MonoBehaviour
{
    public Material selectedMat;


    private Material originalMat;
    private string previousButtonName;

    private void Awake()
    {
        previousButtonName = "";
    }
        public void Start()
    {
        //var planesMat = new List<Renderer>();
        //originalMat = gameObject.GetComponentsInChildren<Renderer>()[2].material;
    }

    public void SelectPlane()
    {

        var localPlanes = ListJsonPlaneLocation_zero.planes;
        var localPlaneTags = localPlanes.Select(p => p.tag).ToList();
        int indxPlane = localPlaneTags.IndexOf("Selected");

        var localPlaneNames = localPlanes.Select(p => p.name).ToList();


        var localButtons = ListJsonPlaneLocation_zero.flightButtons;
        var localButtonTags = localButtons.Select(b => b.tag).ToList();
        int indxButton = localButtonTags.IndexOf("Selected");


        if (indxPlane == -1 && indxButton == -1) // select the corresponding plane
        {
            gameObject.tag = "Selected";

            localButtonTags = localButtons.Select(p => p.tag).ToList();
            indxButton = localButtonTags.IndexOf("Selected");

            originalMat = localPlanes[indxButton].gameObject.GetComponentsInChildren<Renderer>()[2].material;

            localPlanes[indxButton].gameObject.GetComponentsInChildren<Renderer>()[2].material = selectedMat; // highlight with a different material
            localPlanes[indxButton].gameObject.GetComponentInChildren<TextMeshPro>().text = gameObject.name; // display registration name
            localPlanes[indxButton].gameObject.tag = "Selected";

            var colors = localButtons[indxButton].GetComponent<Button>().colors;
            colors.pressedColor = new Color(0f, 0f, 1f, 0.34f);
            colors.selectedColor = new Color(0f, 0f, 1f, 0.34f);
            localButtons[indxButton].gameObject.GetComponent<Button>().colors = colors;

            previousButtonName = gameObject.name;

        }
        else if (indxPlane != -1 && indxButton != -1 && localButtons[indxButton].name != gameObject.name)  // highlight the correct button if the wrong one is clicked
        {

            // I'M TRYING TO IDENTIFY AIRCRAFT CORRESPONDING BUTTON IN RED IF ANOTHER BUTTON WAS CLICKED BY ACCIDENT
            // IT DOESN"T BEHAVE THIS WAY. INSTEAD IT GOES BACK TO UNSELECTED MATERIAL, WHILE THE WRONG BUTTON GETS SELECTED COLORS
            // WHEN THE CORRECT BUTTON CLICKED TO DEACTIVATE THE PLANE, IT TURNS RED
            var colors = localButtons[indxButton].GetComponent<Button>().colors;
            colors.pressedColor = Color.red;
            colors.selectedColor = Color.red;
            localButtons[indxButton].GetComponent<Button>().colors = colors;

            Debug.Log("Keep colors Aircraft button name: " + localButtons[indxButton].name +
                " pressed: " + localButtons[indxButton].GetComponent<Button>().colors.pressedColor +
                " selected: " + localButtons[indxButton].GetComponent<Button>().colors.selectedColor);

            Debug.Log("Keep Aircraft button " + gameObject.name + " tag: " + gameObject.tag + " plane name " + localButtons[indxButton].name + " plane tag " + localButtons[indxPlane].tag);

        }
        else if (indxPlane != -1 && indxButton != -1 && localButtons[indxButton].name == gameObject.name) // Deselect the plane and return button to its default color
        {
            localPlanes[indxButton].gameObject.GetComponentsInChildren<Renderer>()[2].material = originalMat;
            localPlanes[indxButton].gameObject.GetComponentInChildren<TextMeshPro>().text = "";
            localPlanes[indxButton].gameObject.tag = "Untagged";

            gameObject.tag = "Untagged";

            //localButtons[indxPlane].gameObject.GetComponent<Button>().
            var colors = localButtons[indxPlane].GetComponent<Button>().colors;
            colors.pressedColor = Color.white;
            colors.selectedColor = Color.white;
            localButtons[indxPlane].gameObject.GetComponent<Button>().colors = colors;

            previousButtonName = "";
            //Debug.Log("Desactivate Aircraft button " + gameObject.name + " tag: " + gameObject.tag + " plane name " + localPlanes[indxButton].name + " plane tag " + localButtons[indxPlane].gameObject.tag);
        }
        else if ((indxPlane != -1 && indxButton == -1 && gameObject.name == previousButtonName) || (indxPlane == -1 && indxButton != -1 && gameObject.name == previousButtonName)) // reset everything
        {

            foreach (var plane in localPlanes)
            {
                plane.gameObject.GetComponentsInChildren<Renderer>()[2].material = originalMat;
                plane.gameObject.GetComponentInChildren<TextMeshPro>().text = "";
                plane.gameObject.tag = "Untagged";

                var indx = localPlaneNames.IndexOf(plane.gameObject.name);

                localButtons[indx].gameObject.tag = "Untagged";

                //localButtons[indxPlane].gameObject.GetComponent<Button>().
                var colors = localButtons[indx].gameObject.GetComponent<Button>().colors;
                colors.pressedColor = Color.white;
                colors.selectedColor = Color.white;
                localButtons[indx].gameObject.GetComponent<Button>().colors = colors;
                previousButtonName = "";

                indxPlane = -1;
                indxButton = -1;

                //Debug.Log("Reset Aircraft button reset " + gameObject.name + " tag: " + gameObject.tag + " plane name " + plane.name + " plane tag " + plane.gameObject.tag);
            }
        }

    }
}