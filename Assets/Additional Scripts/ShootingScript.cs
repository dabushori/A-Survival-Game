using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ShootingScript : MonoBehaviour
{
    public Camera Cam;
    /*
        private void Start()
        {
            Invoke(nameof(ShootingDay), 3);
        }

        void ShootingDay()
        {
            foreach (Transform obj in transform)
            {
                Debug.Log("Satrting " + obj.name);
                obj.gameObject.SetActive(true);

                Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

                // Read screen contents into the texture
                texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                texture.Apply();

                // Write to file
                byte[] bytes = texture.EncodeToPNG();
                File.WriteAllBytes("C:\\Users\\dabus\\Desktop\\Icons\\" + obj.name + ".png", bytes);

                // Clean up the used texture
                Destroy(texture);

                obj.gameObject.SetActive(false);
            }
        }
    */

    int i = 0;
    int state = 0;
    public Camera cam;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // transform.GetChild(i).gameObject.SetActive(true);
            ScreenCapture.CaptureScreenshot("C:\\Users\\dabus\\Desktop\\Icons\\" + transform.GetChild(i).gameObject.name + ".png");
            ++i;
        }
        /*
        if (i >= transform.childCount) return;
        GameObject obj = transform.GetChild(i).gameObject;
        switch (state)
        {
            case 0:
                obj.SetActive(true);
                obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y + 100 * i, obj.transform.position.z);
                cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y + 100 * i, cam.transform.position.z);
                cam.Render();
                break;
            case 1:
                ScreenCapture.CaptureScreenshot("C:\\Users\\dabus\\Desktop\\Icons\\" + obj.name + ".png");
                break;
            case 2:
                obj.SetActive(false);
                Destroy(obj);
                /*
                obj.TryGetComponent<MeshRenderer>(M).enabled = false;
                foreach (var v in obj.GetComponentsInChildren<MeshRenderer>())
                {
                    v.enabled = false;
                }
                ++i;
                break;
        }
        state = (state + 1) % 3;
        */
    }


    private IEnumerator CreateScreenshot(int i)
    {
        GameObject obj = transform.GetChild(i).gameObject;
        // obj.SetActive(true);

        yield return new WaitForEndOfFrame();

        Debug.Log("Satrting " + obj.name);

        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        // Read screen contents into the texture
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        // Write to file
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes("C:\\Users\\dabus\\Desktop\\Icons\\" + obj.name + ".png", bytes);

        // Clean up the used texture
        Destroy(texture);

        // obj.SetActive(false);

        yield return new WaitForEndOfFrame();
    }

    private void CreateScreenshots()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            GameObject obj = transform.GetChild(i).gameObject;
            obj.SetActive(true);

            if (i > 0)
            {
                // Destroy(transform.GetChild(i - 1).gameObject);
                transform.GetChild(i - 1).gameObject.SetActive(false);
            }

            Debug.Log("Satrting " + obj.name);

            Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

            // Read screen contents into the texture
            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            texture.Apply();

            // Write to file
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes("C:\\Users\\dabus\\Desktop\\Icons\\" + obj.name + ".png", bytes);

            // Clean up the used texture
            Destroy(texture);
        }
    }
}
