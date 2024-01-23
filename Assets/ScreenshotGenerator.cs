using UnityEngine;

public class ScreenshotGenerator : MonoBehaviour
{
    public float screenshotInterval = 5.0f; // Interval between screenshots in seconds
    private float nextScreenshotTime;

    private void Start()
    {
        // Set the initial time for the first screenshot
        nextScreenshotTime = Time.time + screenshotInterval;
    }

    private void Update()
    {
        // Check if it's time to capture a screenshot
        if (Time.time >= nextScreenshotTime)
        {
            // Capture a screenshot with the resolution of the main camera
            CaptureScreenshot();

            // Set the time for the next screenshot
            nextScreenshotTime = Time.time + screenshotInterval;
        }
    }

    private void CaptureScreenshot()
    {
        // Generate a unique filename based on the current date and time
        string screenshotFileName = "Dataset/Screenshot_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmssff") + ".png";

        // Capture the screenshot with the camera's resolution
        ScreenCapture.CaptureScreenshot(screenshotFileName);

        Debug.Log("Screenshot captured: " + screenshotFileName);
    }
}
