using System;
using UnityEngine;
using TMPro;

public class TimeController : MonoBehaviour
{
    // time controll
    [SerializeField]
    private float timeMultiplayer;

    [SerializeField]
    private float startHour;

    [SerializeField]
    private TextMeshProUGUI timeText;

    // sunlight rotation
    [SerializeField]
    private Light sunLight;

    [SerializeField]
    private float sunriseHour;

    [SerializeField]
    private float sunsetHour;

    // night time switch
    [SerializeField]
    private Color dayAmbientLight;

    [SerializeField]
    private Color nightAmbientLight;

    [SerializeField]
    private AnimationCurve lightChangeCurve;

    [SerializeField]
    private float maxSunLightIntensity;

    [SerializeField]
    private Light moonLight;

    [SerializeField]
    private float maxMoonLightIntensity;

    [SerializeField]
    private Material daySky;

    [SerializeField]
    private Material nightSky;

    private DateTime currentTime;

    private TimeSpan sunriseTime;

    private TimeSpan sunsetTime;

    private void Start()
    {
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);
        
        sunriseTime = TimeSpan.FromHours(sunriseHour);
        sunsetTime = TimeSpan.FromHours(sunsetHour);
    }

    private void Update()
    {
        UpdateTimeOfDay();
        RotateSun();
        UpdateLight();
        UpdateSky();
    }

    private void UpdateLight()
    {
        // compare the difference between the vectors (between 1 and -1)
        float doProduct = Vector3.Dot(sunLight.transform.forward, Vector3.down);
        // calculate intesity of light for day and night
        sunLight.intensity = Mathf.Lerp(0, maxSunLightIntensity, lightChangeCurve.Evaluate(doProduct));
        moonLight.intensity = Mathf.Lerp(maxMoonLightIntensity, 0, lightChangeCurve.Evaluate(doProduct));
        // change the ambient light
        RenderSettings.ambientLight = Color.Lerp(nightAmbientLight, dayAmbientLight, lightChangeCurve.Evaluate(doProduct));
    }


    private void UpdateTimeOfDay()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplayer);
        if (timeText != null)
        {
            timeText.text = currentTime.ToString("HH:mm");
        }
    }

    private void UpdateSky()
    {
        if (currentTime.TimeOfDay > sunriseTime + TimeSpan.FromHours(-1) && currentTime.TimeOfDay < sunsetTime + TimeSpan.FromHours(1))
        {
            RenderSettings.skybox = daySky;
        } else
        {
            RenderSettings.skybox = nightSky;
        }
    }

    private  void RotateSun()
    {
        float lightRotation;
        
        if (currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime)
        {
            TimeSpan sunriseToSunsetDuration = CalculateTimeDiff(sunriseTime, sunsetTime);
            TimeSpan timeSinceSunrise = CalculateTimeDiff(sunriseTime, currentTime.TimeOfDay);

            double precntage = timeSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;

            lightRotation = Mathf.Lerp(0, 180, (float)precntage); // day light
        }
        else
        {
            TimeSpan sunsetToSunriseDuration = CalculateTimeDiff(sunsetTime, sunriseTime);
            TimeSpan timeSinceSunset = CalculateTimeDiff(sunsetTime, currentTime.TimeOfDay);

            double precntage = timeSinceSunset.TotalMinutes / sunsetToSunriseDuration.TotalMinutes;
            
            lightRotation = Mathf.Lerp(180, 360, (float)precntage); // night light
        }

        sunLight.transform.rotation = Quaternion.AngleAxis(lightRotation, Vector3.right);
    }

    private TimeSpan CalculateTimeDiff(TimeSpan fromTime, TimeSpan toTime)
    {
        TimeSpan diff = toTime - fromTime;
        
        // if diff < 0 time is between two differnt days
        if (diff.TotalSeconds < 0)
        {
            diff += TimeSpan.FromHours(24); // adding a day
        }

        return diff;
    }

}
