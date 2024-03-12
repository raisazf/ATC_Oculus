using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        //Debug.Log(" Aircraft Activating the buttons");
    }

    public void SelectPlane()
    {

        //Debug.Log(" Aircraft Before checking for Selected");
        var localPlanes = ListJsonPlaneLocation_zero.planes;
        var localPlaneTags = localPlanes.Select(p => p.tag).ToList();
        int indxPlane = localPlaneTags.IndexOf("Selected");

        var localPlaneNames = localPlanes.Select(p => p.name).ToList();


        var localButtons = ListJsonPlaneLocation_zero.flightButtons;
        var localButtonTags = localButtons.Select(b => b.tag).ToList();
        int indxButton = localButtonTags.IndexOf("Selected");

        Debug.Log(" Start Button Aircraft " + gameObject.tag + " button name " + gameObject.name + " button index " + indxButton + " plane index " + indxPlane);

        if (indxPlane == -1 && indxButton == -1)
        {
            //Debug.Log("Aircraft Am I in?");
            gameObject.tag = "Selected";

            // Debug.Log("Aircraft Am I in 2?");
            localButtonTags = localButtons.Select(p => p.tag).ToList();
            indxButton = localButtonTags.IndexOf("Selected");

            originalMat = localPlanes[indxButton].gameObject.GetComponentsInChildren<Renderer>()[2].material;
            //Debug.Log("Aircraft Am I in 3?");
            localPlanes[indxButton].gameObject.GetComponentsInChildren<Renderer>()[2].material = selectedMat; // highlight with a different material
            localPlanes[indxButton].gameObject.GetComponentInChildren<TextMeshPro>().text = gameObject.name; // display registration name
            localPlanes[indxButton].gameObject.tag = "Selected";

            var colors = localButtons[indxButton].GetComponent<Button>().colors;
            colors.pressedColor = new Color(0f, 0f, 1f, 0.34f);
            colors.selectedColor = new Color(0f, 0f, 1f, 0.34f);
            localButtons[indxButton].gameObject.GetComponent<Button>().colors = colors;

            //Debug.Log("Aircraft Am I in 4?");
            previousButtonName = gameObject.name;
            Debug.Log("Aircraft Am I in 5? name is " + previousButtonName);
            Debug.Log("Sel Aircraft button " + gameObject.name + " tag: " + gameObject.tag + " plane name " + localPlanes[indxButton].name + " plane tag " + localButtons[indxPlane].gameObject.tag);

        }
        else if (indxPlane != -1 && indxButton != -1 && localButtons[indxButton].name != gameObject.name) 
        {
            Debug.Log("Keeping Aircraft previous buttons is " + previousButtonName + " current button " + gameObject.name);

            var colors = localButtons[indxButton].GetComponent<Button>().colors;
            colors.pressedColor = Color.red;
            colors.selectedColor = Color.red;
            localButtons[indxButton].GetComponent<Button>().colors = colors;

            Debug.Log("Keep colors Aircraft button name: " + localButtons[indxButton].name +
                " pressed: " + localButtons[indxButton].GetComponent<Button>().colors.pressedColor +
                " selected: " + localButtons[indxButton].GetComponent<Button>().colors.selectedColor);

            Debug.Log("Keep Aircraft button " + gameObject.name + " tag: " + gameObject.tag + " plane name " + localButtons[indxButton].name + " plane tag " + localButtons[indxPlane].tag);

        }
        else if (indxPlane != -1 && indxButton != -1 && localButtons[indxButton].name == gameObject.name)
        {
            //Debug.Log("Aircraft deselect 1?");
            localPlanes[indxButton].gameObject.GetComponentsInChildren<Renderer>()[2].material = originalMat;
            localPlanes[indxButton].gameObject.GetComponentInChildren<TextMeshPro>().text = "";
            localPlanes[indxButton].gameObject.tag = "Untagged";
            //Debug.Log("Aircraft deselect 2?");

            gameObject.tag = "Untagged";
            //Debug.Log("Aircraft deselect 3?");

            //localButtons[indxPlane].gameObject.GetComponent<Button>().
            var colors = localButtons[indxPlane].GetComponent<Button>().colors;
            colors.pressedColor = Color.white;
            colors.selectedColor = Color.white;
            localButtons[indxPlane].gameObject.GetComponent<Button>().colors = colors;
            //Debug.Log("Aircraft deselect 4?");
            //Debug.Log("Des Aircraft button name: " + localButtons[indxPlane].gameObject.name +
            //    " pressed: " + localButtons[indxPlane].gameObject.GetComponent<Button>().colors.pressedColor +
            //    " selected: " + localButtons[indxPlane].gameObject.GetComponent<Button>().colors.selectedColor);

            previousButtonName = "";
            Debug.Log("Des Aircraft button " + gameObject.name + " tag: " + gameObject.tag + " plane name " + localPlanes[indxButton].name + " plane tag " + localButtons[indxPlane].gameObject.tag);
        }
        else if ((indxPlane != -1 && indxButton == -1 && gameObject.name == previousButtonName) || (indxPlane == -1 && indxButton != -1 && gameObject.name == previousButtonName))
        {
            Debug.Log("reset all");

            foreach (var plane in localPlanes)
            {
                plane.gameObject.GetComponentsInChildren<Renderer>()[2].material = originalMat;
                plane.gameObject.GetComponentInChildren<TextMeshPro>().text = "";
                plane.gameObject.tag = "Untagged";
                Debug.Log("Aircraft reset 2?");

                var indx = localPlaneNames.IndexOf(plane.gameObject.name);

                localButtons[indx].gameObject.tag = "Untagged";

                Debug.Log("Aircraft reset 3?");


                //localButtons[indxPlane].gameObject.GetComponent<Button>().
                var colors = localButtons[indx].gameObject.GetComponent<Button>().colors;
                //Debug.Log("Aircraft reset 4?");
                colors.pressedColor = Color.white;
                //Debug.Log("Aircraft reset 5?");
                colors.selectedColor = Color.white;
                //Debug.Log("Aircraft reset 6?");
                localButtons[indx].gameObject.GetComponent<Button>().colors = colors;
                previousButtonName = "";

                //Debug.Log("Aircraft reset 7?");
                indxPlane = -1;
                indxButton = -1;

                //Debug.Log("Des Aircraft button name: " + plane.gameObject.name +
                 //   " pressed: " + localButtons[indx].gameObject.GetComponent<Button>().colors.pressedColor +
                 //   " selected: " + localButtons[indx].gameObject.GetComponent<Button>().colors.selectedColor);

                Debug.Log("Des Aircraft button reset " + gameObject.name + " tag: " + gameObject.tag + " plane name " + plane.name + " plane tag " + plane.gameObject.tag);
            }
        }

        Debug.Log("Aircraft done with buttons");

    }
}