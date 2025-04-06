using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Assertions;

namespace Testing.Photon
{
    public class PhotonTestHelper
    {
        public static IEnumerator ConnectToPhotonServer()
        {
            // Disconnect if already connected
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();

                // Wait for disconnection
                while (PhotonNetwork.IsConnected)
                    yield return null;
            }

            // Use OfflineMode for testing to avoid actual server connection
            // PhotonNetwork.OfflineMode = true; // Uncomment for offline testing

            // Connect to server
            PhotonNetwork.ConnectUsingSettings();

            // Wait for connection
            float timeout = 15f; // Increased timeout
            float timer = 0f;
            while (!PhotonNetwork.IsConnected && timer < timeout)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            
            PhotonNetwork.JoinLobby();
            
            // Wait for lobby join
            timer = 0f;
            while (!PhotonNetwork.InLobby && timer < timeout)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            Assert.IsTrue(PhotonNetwork.IsConnected, "Failed to connect to Photon server");
        }

        public static IEnumerator CreateRoom(string roomName)
        {
            // Wait until connected to master server
            if (!PhotonNetwork.IsConnected)
                yield return ConnectToPhotonServer();

            // Wait for master client connection to be ready
            float timeout = 10f;
            float timer = 0f;
            while (!PhotonNetwork.IsConnectedAndReady && timer < timeout)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            Assert.IsTrue(PhotonNetwork.IsConnectedAndReady, "Failed to connect and be ready for operations");

            // Now create the room
            PhotonNetwork.CreateRoom(roomName);

            // Wait to join room
            timer = 0f;
            while (!PhotonNetwork.InRoom && timer < timeout)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            Assert.IsTrue(PhotonNetwork.InRoom, "Failed to create and join room");
        }

        public static void Disconnect()
        {
            if (PhotonNetwork.IsConnected)
                PhotonNetwork.Disconnect();
        }
    }
}