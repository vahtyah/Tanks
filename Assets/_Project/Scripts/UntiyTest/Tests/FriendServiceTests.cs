using NUnit.Framework;
using Testing;
using UnityEngine;

public class FriendServiceTests
{
    private FriendService friendService;
    
    [Test]
    public void TestSendFriendRequest()
    {
        _ = new AuthenticationService(auth =>
        {
            _ = new DatabaseInitializer(databaseRef =>
            {
                var user = new UserDataService(auth, databaseRef);
                friendService = new FriendService(auth, databaseRef, user);
                
                friendService.SendFriendRequest("vahtyah", (result, error) =>
                {
                    Assert.IsTrue(result);
                    Debug.Log("Friend request sent successfully.");
                });
            });
        });
    }
    
    [Test]
    public void TestAcceptFriendRequest()
    {
        _ = new AuthenticationService(auth =>
        {
            _ = new DatabaseInitializer(databaseRef =>
            {
                var user = new UserDataService(auth, databaseRef);
                friendService = new FriendService(auth, databaseRef, user);
                
                friendService.AcceptFriendRequest("vahtyah", (result, error) =>
                {
                    Assert.IsTrue(result);
                    Debug.Log("Friend request accepted successfully.");
                });
            });
        });
    }
    
    [Test]
    public void TestRejectFriendRequest()
    {
        _ = new AuthenticationService(auth =>
        {
            _ = new DatabaseInitializer(databaseRef =>
            {
                var user = new UserDataService(auth, databaseRef);
                friendService = new FriendService(auth, databaseRef, user);
                
                friendService.RejectFriendRequest("vahtyah", (result, error) =>
                {
                    Assert.IsTrue(result);
                    Debug.Log("Friend request rejected successfully.");
                });
            });
        });
    }
    
    [Test]
    public void TestRemoveFriend()
    {
        _ = new AuthenticationService(auth =>
        {
            _ = new DatabaseInitializer(databaseRef =>
            {
                var user = new UserDataService(auth, databaseRef);
                friendService = new FriendService(auth, databaseRef, user);
                
                friendService.RemoveFriend("vahtyah", (result, error) =>
                {
                    Assert.IsTrue(result);
                    Debug.Log("Friend removed successfully.");
                });
            });
        });
    }
    
    [Test]
    public void TestGetFriendsList()
    {
        _ = new AuthenticationService(auth =>
        {
            _ = new DatabaseInitializer(databaseRef =>
            {
                var user = new UserDataService(auth, databaseRef);
                friendService = new FriendService(auth, databaseRef, user);
                
                friendService.GetFriendsList((friends, error) =>
                {
                    Assert.IsNotNull(friends);
                    Debug.Log("Friends list retrieved successfully.");
                });
            });
        });
    }
}
