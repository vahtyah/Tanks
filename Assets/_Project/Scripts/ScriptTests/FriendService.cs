using System;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Testing
{
    [Serializable]
    public class PhotonFriendData
    {
        public string UserId;
        public string DisplayName;
    }

    public class FriendService
    {
        private UserDataService userDataService;
        private DatabaseReference databaseRef;
        private FirebaseAuth auth;
        private FirebaseUser user => auth?.CurrentUser;

        public FriendService(FirebaseAuth auth, DatabaseReference databaseRef, UserDataService userDataService)
        {
            this.userDataService = userDataService;
            this.auth = auth;
            this.databaseRef = databaseRef;
        }


        #region Friend Requests

        [Button]
        public void SendFriendRequest(string friendName, Action<bool, string> callback = null)
        {
            if (!ValidateUserAndDatabase(callback)) return;

            userDataService.FindUserByDisplayName(friendName, (friendData, error) =>
            {
                if (!ValidateFriendData(friendData, error, callback)) return;
                if (!ValidateFriendshipRequest(friendData, callback)) return;

                var friendRequest = new FriendData(friendData.UserId, friendData.DisplayName, friendData.Email, "pending");

                // Add friend to current user's friends list
                UpdateFriendship(user.UserId, friendData.UserId, friendRequest, (success, updateError) =>
                {
                    if (!success)
                    {
                        callback?.Invoke(false, updateError);
                        return;
                    }

                    // Add current user to friend's friends list
                    var senderUserData = new FriendData(
                        user.UserId,
                        user.DisplayName,
                        user.Email,
                        "received"
                    );

                    UpdateFriendship(friendData.UserId, user.UserId, senderUserData, (innerSuccess, innerError) =>
                    {
                        if (innerSuccess)
                        {
                            Debug.Log($"Friend request sent successfully to {friendData.DisplayName}");
                            callback?.Invoke(true, "Friend request sent successfully");
                        }
                        else
                        {
                            callback?.Invoke(false, innerError);
                        }
                    });
                });
            });
        }

        [Button]
        public void AcceptFriendRequest(string friendName, Action<bool, string> callback = null)
        {
            if (!ValidateUserAndDatabase(callback)) return;

            userDataService.FindUserByDisplayName(friendName, (friendData, error) =>
            {
                if (!ValidateFriendData(friendData, error, callback)) return;
                if (!ValidateFriendshipRequest(friendData, callback)) return;

                // Update status in current user's friends list
                UpdateFriendshipStatus(user.UserId, friendData.UserId, "accepted", (success, updateError) =>
                {
                    if (!success)
                    {
                        callback?.Invoke(false, updateError);
                        return;
                    }

                    // Update status in friend's friends list
                    UpdateFriendshipStatus(friendData.UserId, user.UserId, "accepted", (innerSuccess, innerError) =>
                    {
                        if (innerSuccess)
                        {
                            Debug.Log($"Friend request accepted successfully for user: {friendData.DisplayName}");
                            callback?.Invoke(true, "Friend request accepted successfully");
                        }
                        else
                        {
                            callback?.Invoke(false, innerError);
                        }
                    });
                });
            });
        }

        [Button]
        public void RejectFriendRequest(string friendName, Action<bool, string> callback = null)
        {
            if (!ValidateUserAndDatabase(callback)) return;

            userDataService.FindUserByDisplayName(friendName, (friendData, error) =>
            {
                if (!ValidateFriendData(friendData, error, callback)) return;
                if (!ValidateFriendshipRequest(friendData, callback)) return;

                // Remove from current user's friends list
                RemoveFriendship(user.UserId, friendData.UserId, (success, removeError) =>
                {
                    if (!success)
                    {
                        callback?.Invoke(false, removeError);
                        return;
                    }

                    // Remove from friend's friends list
                    RemoveFriendship(friendData.UserId, user.UserId, (innerSuccess, innerError) =>
                    {
                        if (innerSuccess)
                        {
                            Debug.Log($"Friend request rejected successfully for user: {friendData.DisplayName}");
                            callback?.Invoke(true, "Friend request rejected successfully");
                        }
                        else
                        {
                            callback?.Invoke(false, innerError);
                        }
                    });
                });
            });
        }

        [Button]
        public void RemoveFriend(string friendName, Action<bool, string> callback = null)
        {
            RejectFriendRequest(friendName, callback);
        }

        #endregion

        #region Friend Lists

        public void GetFriendsList(Action<List<FriendData>, string> callback)
        {
            if (user == null)
            {
                callback?.Invoke(null, "You're not logged in!");
                return;
            }

            if (databaseRef == null)
            {
                callback?.Invoke(null, "Database reference is null.");
                return;
            }

            databaseRef.Child("users").Child(user.UserId).Child("Friends").GetValueAsync()
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled)
                    {
                        callback?.Invoke(null, "Operation was canceled");
                        return;
                    }

                    if (task.IsFaulted)
                    {
                        callback?.Invoke(null, "Operation failed: " + task.Exception.Message);
                        return;
                    }

                    var friendsList = new List<FriendData>();
                    foreach (var child in task.Result.Children)
                    {
                        var friendData = JsonUtility.FromJson<FriendData>(child.GetRawJsonValue());
                        friendsList.Add(friendData);
                    }

                    callback?.Invoke(friendsList, null);
                });
        }

        public void GetFriendByStatus(string status, Action<List<FriendData>, string> callback)
        {
            GetFriendsList((allFriends, error) =>
            {
                if (error != null)
                {
                    callback?.Invoke(null, error);
                    return;
                }

                var filteredFriends = allFriends.FindAll(friend => friend.Status == status);
                callback?.Invoke(filteredFriends, null);
            });
        }

        #endregion

        #region Helper Methods

        private bool ValidateUserAndDatabase(Action<bool, string> callback)
        {
            if (user == null)
            {
                callback?.Invoke(false, "You are not logged in.");
                return false;
            }

            if (databaseRef == null)
            {
                callback?.Invoke(false, "Database reference is null.");
                return false;
            }

            return true;
        }

        private bool ValidateFriendData(UserData friendData, string error, Action<bool, string> callback)
        {
            if (friendData == null)
            {
                callback?.Invoke(false, error ?? "User not found");
                return false;
            }

            return true;
        }

        private bool ValidateFriendshipRequest(UserData friendData, Action<bool, string> callback)
        {
            if (friendData.UserId == user.UserId)
            {
                callback?.Invoke(false, "You cannot perform this action with yourself.");
                return false;
            }

            if (friendData.Friends != null && friendData.Friends.ContainsKey(user.UserId))
            {
                callback?.Invoke(false, "You are already friends with this user.");
                return false;
            }

            return true;
        }

        private void UpdateFriendship(string userIdToUpdate, string friendIdToAdd, FriendData friendData,
            Action<bool, string> callback)
        {
            databaseRef.Child("users").Child(userIdToUpdate).Child("Friends").Child(friendIdToAdd)
                .SetRawJsonValueAsync(JsonUtility.ToJson(friendData))
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled)
                    {
                        callback?.Invoke(false, "Operation was canceled");
                        return;
                    }

                    if (task.IsFaulted)
                    {
                        callback?.Invoke(false, "Operation failed: " + task.Exception.Message);
                        return;
                    }

                    callback?.Invoke(true, null);
                });
        }

        private void UpdateFriendshipStatus(string userIdToUpdate, string friendIdToUpdate, string status,
            Action<bool, string> callback)
        {
            databaseRef.Child("users").Child(userIdToUpdate).Child("Friends").Child(friendIdToUpdate).Child("status")
                .SetValueAsync(status).ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled)
                    {
                        callback?.Invoke(false, "Operation was canceled");
                        return;
                    }

                    if (task.IsFaulted)
                    {
                        callback?.Invoke(false, "Operation failed: " + task.Exception.Message);
                        return;
                    }

                    callback?.Invoke(true, null);
                });
        }

        private void RemoveFriendship(string userIdToUpdate, string friendIdToRemove, Action<bool, string> callback)
        {
            databaseRef.Child("users").Child(userIdToUpdate).Child("Friends").Child(friendIdToRemove).RemoveValueAsync()
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled)
                    {
                        callback?.Invoke(false, "Operation was canceled");
                        return;
                    }

                    if (task.IsFaulted)
                    {
                        callback?.Invoke(false, "Operation failed: " + task.Exception.Message);
                        return;
                    }

                    callback?.Invoke(true, null);
                });
        }

        #endregion
    }
    
    [Serializable]
    public class FriendData
    {
        public string UserId;
        public string DisplayName;
        public string Email;
        public long FriendshipCreatedAt;
        public string Status; // "pending", "accepted", "blocked"

        public FriendData() { } // Required for Firebase serialization

        public FriendData(string userId, string displayName, string email, string status)
        {
            UserId = userId;
            DisplayName = displayName;
            Email = email;
            Status = status;
        }
    }
}