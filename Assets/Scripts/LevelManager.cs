using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int PlayerNumber = 2;
    public List<GameObject> players = new();
    public List<Transform> spawnPoints = new();
    public MultiplayerSplitCameraRig cameraRig;
    
    void Start()
    {
        cameraRig.InitializeCameras(PlayerNumber);
        for (int i = 0; i < PlayerNumber; i++)
        {
            GameObject player = Instantiate(players[i], spawnPoints[i].position, spawnPoints[i].rotation);
            cameraRig.CameraControllers[i].SetFollowTarget(player.transform);
        }
    }
}
