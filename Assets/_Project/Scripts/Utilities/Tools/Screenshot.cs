using System;
using UnityEngine;

public class Screenshot : PersistentSingleton<Screenshot>
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            TakeScreenshot();
        }
    }

    private void TakeScreenshot()
    {
        //C:\Users\82688\OneDrive\ВКР\Picture\Tecnical
        
        string screenshotPath = $"C:\\Users\\82688\\OneDrive\\ВКР\\Picture\\Tecnical";
        System.IO.Directory.CreateDirectory(screenshotPath);
        string screenshotName = $"{screenshotPath}/Screenshot_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
        
        ScreenCapture.CaptureScreenshot(screenshotName);
        Debug.Log($"Screenshot saved to: {screenshotName}");
    }
}