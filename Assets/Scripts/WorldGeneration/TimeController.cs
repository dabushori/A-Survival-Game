using System;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class TimeController : MonoBehaviour
{
    // time controll
    [SerializeField]
    float timeMultiplayer; // how fast times move

    [SerializeField]
    float startHour; // start of day

    // sunlight rotation
    [SerializeField]
    Light sunLight; // sun light

    [SerializeField]
    float sunriseHour; // time of sunrise

    [SerializeField]
    float sunsetHour; // time of sunset

    // night time switch
    [SerializeField]
    Color dayAmbientLight; // color of ambient light in the day

    [SerializeField]
    Color nightAmbientLight; // color of ambient light in the night

    [SerializeField]
    AnimationCurve lightChangeCurve; // curve for light intensity change between moon and sun

    [SerializeField]
    float maxSunLightIntensity; // max intensity for the sun

    [SerializeField]
    Light moonLight; // moon light
    [SerializeField]
    float maxMoonLightIntensity; // max intensity for the moon

    [SerializeField]
    Material daySky; // the day sky

    [SerializeField]
    Material nightSky; // the nighy sky

    DateTime currentTime; // hold the current time
    TimeSpan sunriseTime; // sunrise time
    TimeSpan sunsetTime; // sunset time

    int day; // day counter
    private bool changedDay; // bool to check if the day has changed
    [SerializeField]
    TMP_Text dayText; // text that shows the day

    /*
     * The function return the current day
     */
    public int GetDay()
    {
        return day;
    }

    /*
     * The function makes the dayText visible
     */
    public void ShowDay()
    {
        dayText.gameObject.SetActive(true);
    }

    private void Start()
    {
        // start of time
        day = 0;
        changedDay = false;
        // get the lights
        sunLight = GameObject.Find("SunLight").GetComponent<Light>();
        moonLight = GameObject.Find("MoonLight").GetComponent<Light>();
        // save the current time, sunset and sunrise
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);
        sunriseTime = TimeSpan.FromHours(sunriseHour);
        sunsetTime = TimeSpan.FromHours(sunsetHour);
        // save instance
        GameStateController.timeController = this;
    }

    private void Update()
    {
        // update time
        UpdateTimeOfDay();
        // rotate the sun according to the time
        RotateSun();
        // change the light according to the time
        UpdateLight();
        // change the sky when day and night change
        UpdateSky();
        // update what day is it
        UpdateDay();
    }

    /*
     * The function change the day count every new day at noon
     */
    private void UpdateDay()
    {
        // set time of new day as noon
        TimeSpan newDay = new TimeSpan(12, 0, 0);
        TimeSpan now = currentTime.TimeOfDay;
        // if its after noon
        if (now > newDay)
        {
            // if we did not change the day count already: change it
            if (!changedDay)
            {
                changedDay = true;
                day++;
                dayText.text = "Day: " + day.ToString();
            }
        }
        else
        {
            // when its after midnight we can change day count again next time it is noon
            changedDay = false;
        }
    }

    /*
     * The timecontroller try to get the lights if he did not get them at the start
     */
    void TrySetLights()
    {
        GameObject light = GameObject.Find("SunLight");
        if (light != null) sunLight = light.GetComponent<Light>();
        light = GameObject.Find("MoonLight");
        if (light != null) moonLight = light.GetComponent<Light>();
    }
    
    /*
     * 
     */
    private void UpdateLight()
    {
        // try get lights if they are null
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
        // calculate intesity of light for day and night using the curve and the time of the day (the doProduct above calculation)
        sunLight.intensity = Mathf.Lerp(0, maxSunLightIntensity, lightChangeCurve.Evaluate(doProduct));
        moonLight.intensity = Mathf.Lerp(maxMoonLightIntensity, 0, lightChangeCurve.Evaluate(doProduct));
        // change the ambient light
        RenderSettings.ambientLight = Color.Lerp(nightAmbientLight, dayAmbientLight, lightChangeCurve.Evaluate(doProduct));
    }

    /*
     * The function update the time
     */
    private void UpdateTimeOfDay()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplayer);
    }

    /*
     * The function change the sky according to if it is day or night
     */
    private void UpdateSky()
    {
        //  1.5 hours before sunrise to 1.5 hours after sunset is day sky
        if (currentTime.TimeOfDay > sunriseTime + TimeSpan.FromHours(-1.5) && currentTime.TimeOfDay < sunsetTime + TimeSpan.FromHours(1.5))
        {
            moonLight.gameObject.SetActive(false);
            RenderSettings.skybox = daySky;
        }
        else
        {
            // the rest is night sky
            moonLight.gameObject.SetActive(true);
            RenderSettings.skybox = nightSky;
        }
    }

    /*
     * The function check if the time is night
     */
    public bool IsNight()
    {
        return !(currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime);
    }

    /*
     * The function rotate the sun according to the time
     */
    private void RotateSun()
    {
        // try get lights if they are null
        if (sunLight == null || moonLight == null)
        {
            TrySetLights();
            if (sunLight == null || moonLight == null)
            {
                return;
            }
        }

        float lightRotation;
        // day time
        if (currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime)
        {
            // calculate time between sunrise and sunset
            TimeSpan sunriseToSunsetDuration = CalculateTimeDiff(sunriseTime, sunsetTime);
            // calculate time between sunrise until now
            TimeSpan timeSinceSunrise = CalculateTimeDiff(sunriseTime, currentTime.TimeOfDay);
            // check how much of the day has past
            double precntage = timeSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;
            // use the precntage to set the rotation of the sun between 0 and 180
            lightRotation = Mathf.Lerp(0, 180, (float)precntage); // day light
        }
        // night time
        else
        {
            // calculate time between sunset and sunrise
            TimeSpan sunsetToSunriseDuration = CalculateTimeDiff(sunsetTime, sunriseTime);
            // calculate time between sunset until now
            TimeSpan timeSinceSunset = CalculateTimeDiff(sunsetTime, currentTime.TimeOfDay);
            // check how much of the night has past
            double precntage = timeSinceSunset.TotalMinutes / sunsetToSunriseDuration.TotalMinutes;
            // use the precntage to set the rotation of the sun between 180 and 360
            lightRotation = Mathf.Lerp(180, 360, (float)precntage); // night light
        }
        // rotate the sun according to the calculations
        sunLight.transform.rotation = Quaternion.AngleAxis(lightRotation, Vector3.right);
    }

    /*
     * The fuction calculate the difference between two time spans
     */
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
