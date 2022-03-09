using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsSettings : MonoBehaviour
{
    private float sensX = 10f;
    private float sensY = 10f;
    float sensMax = 15f;
    float sensMin = 0f;

    public TMPro.TMP_InputField inputX;
    public TMPro.TMP_InputField inputY;


    public void SetSensX(string sens)
    {
        if (!float.TryParse(sens, out float num))
        {
            sensX = 10f;
        }
        if (sensMax < num)
        {
            num = sensMax;
        } else if (num < sensMin)
        {
            num = 1f;
        }
        sensX = num;
    }

    public void SetSensY(string sens)
    {
        if (!float.TryParse(sens, out float num))
        {
            sensY = 10f;
        }
        if (sensMax < num)
        {
            num = sensMax;
        }
        else if (num < sensMin)
        {
            num = 1f;
        }
        sensY = num;
    }
    public void Apply()
    {
        inputX.text = sensX.ToString();
        GameStateController.SensitivityX = sensX;

        inputY.text = sensY.ToString();
        GameStateController.SensitivityY = sensY;
    }
}
