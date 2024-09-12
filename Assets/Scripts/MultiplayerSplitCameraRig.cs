using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerSplitCameraRig : MonoBehaviour
{
    public List<CinemachineCameraController> CameraControllers;

    public void InitializeCameras(int playerNumber)
    {
        switch (playerNumber)
        {
            case 2:
                CameraControllers[0].SetViewportRect(new Rect(0, 0, 0.5f, 1)); // Left half
                CameraControllers[1].SetViewportRect(new Rect(0.5f, 0, 0.5f, 1)); // Right half
                CameraControllers[2].Hide();
                CameraControllers[3].Hide();
                break;
            case 3:
                CameraControllers[0].SetViewportRect(new Rect(0, 0, 1f / 3f, 1)); // Left third
                CameraControllers[1].SetViewportRect(new Rect(1f / 3f, 0, 1f / 3f, 1)); // Middle third
                CameraControllers[2].SetViewportRect(new Rect(2f / 3f, 0, 1f / 3f, 1)); // Right third
                CameraControllers[3].Hide();
                break;
            case 4:
                // Four-player split screen
                CameraControllers[0].SetViewportRect(new Rect(0, 0.5f, 0.5f, 0.5f)); // Top left
                CameraControllers[1].SetViewportRect(new Rect(0.5f, 0.5f, 0.5f, 0.5f)); // Top right
                CameraControllers[2].SetViewportRect(new Rect(0, 0, 0.5f, 0.5f)); // Bottom left
                CameraControllers[3].SetViewportRect(new Rect(0.5f, 0, 0.5f, 0.5f)); // Bottom right
                break;
            default:
                Debug.LogError("Unsupported number of players. Only 2 to 4 players are supported.");
                break;
        }
    }
    
    
}