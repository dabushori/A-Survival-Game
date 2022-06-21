using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The class is in charge of the control settings
 */
public class ControlsSettings : MonoBehaviour
{
    float sensX = GameStateController.SensitivityX; // x sensitivity loacl
    float sensY = GameStateController.SensitivityY; // y sensitivity local
    float sensMax = 15f; // max sens
    float sensMin = 1f; // min sens

    [SerializeField]
    TMPro.TMP_InputField inputX; // x sensitivity input
    [SerializeField]
    TMPro.TMP_InputField inputY; // y sensitivity input

    public void Start()
    {
        // show the current sensitivity
        inputX.text = sensX.ToString();
        inputY.text = sensY.ToString();
    }

    /*
     * set x axis sensitivity (called when changing the input)
     */
    public void SetSensX(string sens)
    {
        // not a number
        if (!float.TryParse(sens, out float num))
        {
            sensX = 10f;
        }
        // put max value
        if (sensMax < num)
        {
            num = sensMax;
        }
        // put min value
        else if (num < sensMin)
        {
            num = sensMin;
        }
        sensX = num;
    }

    /*
     * set y axis sensitivity (called when changing the input)
     */
    public void SetSensY(string sens)
    {
        // not a number
        if (!float.TryParse(sens, out float num))
        {
            sensY = 10f;
        }
        // put max value
        if (sensMax < num)
        {
            num = sensMax;
        }
        // put min value
        else if (num < sensMin)
        {
            num = sensMin;
        }
        sensY = num;
    }

    /*
     * apply the new sensitivities
     */
    public void Apply()
    {
        inputX.text = sensX.ToString();
        GameStateController.SensitivityX = sensX; // save for the game

        inputY.text = sensY.ToString();
        GameStateController.SensitivityY = sensY; // save for the game
    }
}
