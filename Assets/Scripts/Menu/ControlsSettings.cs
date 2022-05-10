using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsSettings : MonoBehaviour
{
    private float sensX = GameStateController.SensitivityX;
    private float sensY = GameStateController.SensitivityY;
    float sensMax = 15f;
    float sensMin = 1f;

    public TMPro.TMP_InputField inputX;
    public TMPro.TMP_InputField inputY;

    public void Start()
    {
        inputX.text = sensX.ToString();
        inputY.text = sensY.ToString();
    }

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
            num = sensMin;
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
            num = sensMin;
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
