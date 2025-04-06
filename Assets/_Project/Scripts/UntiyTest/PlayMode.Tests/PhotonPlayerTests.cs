using System.Collections;
using NUnit.Framework;
using Photon;
using Photon.Pun;
using Testing;
using Testing.Photon;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class PhotonPlayerTests
{
    private GameObject playerObject;
    private PhotonView photonView;
    private NetworkPlayerMovement networkPlayerMovement;
    
    [SetUp]
    public void Setup()
    {
        // Create player object with PhotonView
        playerObject = new GameObject("NetworkPlayer");
        photonView = playerObject.AddComponent<PhotonView>();
        networkPlayerMovement = playerObject.AddComponent<NetworkPlayerMovement>();
    }
    
    [TearDown]
    public void Teardown()
    {
        // Cleanup
        Object.Destroy(playerObject);
        PhotonTestHelper.Disconnect();
    }
    
    [UnityTest]
    public IEnumerator TestNetworkPlayerMovement()
    {
        // Connect to Photon
        yield return PhotonTestHelper.ConnectToPhotonServer();
        yield return PhotonTestHelper.CreateRoom("TestRoom");
        
        // Set the player as mine
        photonView.ViewID = 1;
        photonView.OwnershipTransfer = OwnershipOption.Takeover;
        photonView.ControllerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
        
        // Start position
        Vector3 startPosition = playerObject.transform.position;
        
        // Simulate input
        InputSimulator.SetAxisValue("Horizontal", 1f);
        
        // Wait for movement to occur
        yield return new WaitForSeconds(0.1f);
        
        // Check if moved
        Vector3 endPosition = playerObject.transform.position;
        Assert.Greater(endPosition.x, startPosition.x, "Network player should move when controlled locally");
        
        // Cleanup
        InputSimulator.SetAxisValue("Horizontal", 0f);
    }
    
    [UnityTest]
    public IEnumerator TestNetworkPlayerRPC()
    {
        // Connect to Photon
        yield return PhotonTestHelper.ConnectToPhotonServer();
        yield return PhotonTestHelper.CreateRoom("TestRoom");
        
        // Set PhotonView ownership
        photonView.ViewID = 1;
        photonView.ControllerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
        
        // Test RPC functionality
        bool rpcCalled = false;
        networkPlayerMovement.OnRPCReceived += () => rpcCalled = true;
        
        // Trigger RPC
        InputSimulator.SetButtonDown("Fire1");
        yield return null;
        
        // Verify RPC was called
        Assert.IsTrue(rpcCalled, "RPC should be called when Fire1 button is pressed");
        
        // Cleanup
        InputSimulator.SetButtonUp("Fire1");
    }
    
    /*[UnityTest]
    public IEnumerator TestPingSatification()
    {
        // Connect to Photon
        yield return PhotonTestHelper.ConnectToPhotonServer();
        yield return PhotonTestHelper.CreateRoom("TestRoom");
        
        // Set PhotonView ownership
        photonView.ViewID = 1;
        photonView.ControllerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
        
        // Test ping functionality
        float ping = PhotonNetwork.GetPing();
        Assert.Greater(ping, 0, "Ping should be greater than 0");
        Debug.Log(ping);
        
        // Cleanup
        PhotonTestHelper.Disconnect();
    }*/
    
    
    [UnityTest]
    public IEnumerator TestPingPerformance()
    {
        // Connect to Photon
        yield return PhotonTestHelper.ConnectToPhotonServer();
        yield return PhotonTestHelper.CreateRoom("TestRoom");
    
        // Wait a few frames for ping to update
        for (int i = 0; i < 3; i++)
            yield return null;
    
        // Get current ping
        int currentPing = PhotonNetwork.GetPing();
        Debug.Log($"Current Photon ping: {currentPing}ms");
    
        // Check if ping is below threshold
        Assert.Less(currentPing, 100, "Ping should be less than 100ms for optimal gameplay experience");
    
        // Cleanup already handled in TearDown
    }
}