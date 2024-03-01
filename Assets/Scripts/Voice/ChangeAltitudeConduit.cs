
using Oculus.Voice;
using System;
using System.Linq;
using Meta.WitAi;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class ChangeAltitudeConduit : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI altChangeText;
    [SerializeField] private AppVoiceExperience appVoiceExperience;
    [SerializeField] bool appVoiceActive;

    private const string CHANGE_ALTITUDE_INTENT = "change_altitude";


        [MatchIntent(CHANGE_ALTITUDE_INTENT)]


    public void ChangeAltitude(string[] values)
    {
        string distance = values[0];
        string phone_number = values[1];

        if (!string.IsNullOrEmpty(distance) && !string.IsNullOrEmpty(phone_number))
        {

            string temp = "N"+phone_number + " change altitude to  " + distance + " feet";

            altChangeText.GetComponentInChildren<TextMeshProUGUI>().text = temp;
        }
    }

}
