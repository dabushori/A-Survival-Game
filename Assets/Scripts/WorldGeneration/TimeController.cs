using System;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class TimeController : MonoBehaviour
{
    // time controll
    [SerializeField]
    private float timeMultiplayer;

    [SerializeField]
    private float startHour;

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

    private int day;
    private bool changedDay;
    [SerializeField]
    TMP_Text dayText;

    public int GetDay()
    {
        return day;
    }

    public void ShowDay()
    {
        dayText.gameObject.SetActive(true);
    }

    private void Start()
    {
        day = 0;
        changedDay = false;
        sunLight = GameObject.Find("SunLight").GetComponent<Light>();
        moonLight = GameObject.Find("MoonLight").GetComponent<Light>();

        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);

        sunriseTime = TimeSpan.FromHours(sunriseHour);
        sunsetTime = TimeSpan.FromHours(sunsetHour);

        GameStateController.timeController = this;
    }

    private void Update()
    {
        UpdateTimeOfDay();
        RotateSun();
        UpdateLight();
        UpdateSky();
        UpdateDay();
    }

    private void UpdateDay()
    {
        TimeSpan newDay = new TimeSpan(12, 0, 0);
        TimeSpan now = currentTime.TimeOfDay;
        if (now > newDay)
        {
            if (!changedDay)
            {
                changedDay = true;
                day++;
                dayText.text = "Day: " + day.ToString();
            }
        }
        else
        {
            changedDay = false;
        }
    }

    void TrySetLights()
    {
        sunLight = GameObject.Find("SunLight").GetComponent<Light>();
        moonLight = GameObject.Find("MoonLight").GetComponent<Light>();
    }

    private void UpdateLight()
    {
        if (sunLight == null || moonLight == null)
        {
            TrySetLights();
            if (sunLight == null || moonLight == null)
            {
                return;
            }
        }
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
    }

    private void UpdateSky()
    {
        if (currentTime.TimeOfDay > sunriseTime + TimeSpan.FromHours(-1.5) && currentTime.TimeOfDay < sunsetTime + TimeSpan.FromHours(1.5))
        {
            moonLight.enabled = false;
            RenderSettings.skybox = daySky;
        }
        else
        {
            moonLight.enabled = true;
            RenderSettings.skybox = nightSky;
        }
    }

    public bool IsNight()
    {
        return !(currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime);
    }

    private void RotateSun()
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
