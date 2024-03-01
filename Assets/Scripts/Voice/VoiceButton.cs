using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Oculus.Voice;
using Meta.WitAi;

public class VoiceButton : MonoBehaviour
{
    //[SerializeField] AppVoiceExperience appVoiceExperience;
    [SerializeField] GameObject volumeButton;
    [SerializeField] bool isVolumeOn;    // Start is called before the first frame update
    [SerializeField] Material volumeOn;
    [SerializeField] Material volumeOff;


    void Start()
    {
        //Debug.Log("Lights Start " + lightSwitchButton.GetComponent<Image>().color);
        isVolumeOn = false;
    }

    public void VoiceOnOff()
    {
        if (isVolumeOn)
        {
            Debug.Log("Lights Off " + volumeButton.GetComponent<Renderer>().material);
            volumeButton.GetComponent<Renderer>().material = volumeOff;
            isVolumeOn = false;
        }
        else
        {
            Debug.Log("Lights On " + volumeButton.GetComponent<Renderer>().material);
            volumeButton.GetComponent<Renderer>().material = volumeOn;
            isVolumeOn = true;
        }
    }
}
