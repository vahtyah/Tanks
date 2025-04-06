using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Testing;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class PlayerInputTests
{
    private GameObject playerObject;
    private PlayerMovement playerMovement;

    [SetUp]
    public void Setup()
    {
        // Create player object
        playerObject = new GameObject("Player");
        playerMovement = playerObject.AddComponent<PlayerMovement>();
    }
    
    [TearDown]
    public void Teardown()
    {
        // Cleanup
        Object.Destroy(playerObject);
    }
    
    [UnityTest]
    public IEnumerator TestHorizontalMovement()
    {
        // Arrange
        Vector3 startPosition = playerObject.transform.position;
        
        // Simulate horizontal input using custom InputSimulator
        InputSimulator.SetAxisValue("Horizontal", 1f); // Move right
        
        // Wait for a frame to allow movement to occur
        yield return null;
        
        // Assert
        Vector3 endPosition = playerObject.transform.position;
        Assert.Greater(endPosition.x, startPosition.x, "Player should move right when horizontal input is positive");
        
        // Cleanup
        InputSimulator.SetAxisValue("Horizontal", 0f);
    }

    [UnityTest]
    public IEnumerator TestVerticalMovement()
    {
        // Arrange
        Vector3 startPosition = playerObject.transform.position;
        
        // Simulate vertical input
        InputSimulator.SetAxisValue("Vertical", 1f); // Move forward
        
        // Wait for a frame
        yield return new WaitForSeconds(0.1f); // Wait a bit longer
        
        // Assert
        Vector3 endPosition = playerObject.transform.position;
        Assert.Greater(endPosition.z, startPosition.z, "Player should move forward when vertical input is positive");
        
        // Cleanup
        InputSimulator.SetAxisValue("Vertical", 0f);
    }

    [UnityTest]
    public IEnumerator TestDiagonalMovement()
    {
        // Arrange
        Vector3 startPosition = playerObject.transform.position;
        
        // Simulate diagonal input
        InputSimulator.SetAxisValue("Horizontal", 1f);
        InputSimulator.SetAxisValue("Vertical", 1f);
        
        // Wait for a frame
        yield return null;
        
        // Assert
        Vector3 endPosition = playerObject.transform.position;
        Assert.Greater(endPosition.x, startPosition.x, "Player should move right");
        Assert.Greater(endPosition.z, startPosition.z, "Player should move forward");
        
        // Cleanup
        InputSimulator.SetAxisValue("Horizontal", 0f);
        InputSimulator.SetAxisValue("Vertical", 0f);
    }
    
    [UnityTest]
    public IEnumerator TestShooting()
    {
        InputSimulator.SetButtonDown("Fire1");
        yield return null;
        Assert.IsTrue(playerMovement.HasShootingInput == true, "Player should shoot when Fire1 is pressed");
        InputSimulator.SetButtonUp("Fire1");
    }
}