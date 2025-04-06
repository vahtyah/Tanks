using System;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

namespace Testing
{
    [Serializable]
    public class UserData
    {
        public string UserId;
        public string DisplayName;
        public string Email;

        public long CreatedAt;
        public long LastLoginAt;
        public Dictionary<string, PhotonFriendData> Friends = new();

        public UserData()
        {
        }

        public UserData(FirebaseUser user)
        {
            UserId = user.UserId;
            DisplayName = user.DisplayName;
            Email = user.Email;
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            LastLoginAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public override string ToString()
        {
            return
                $"User ID: {UserId}, Display Name: {DisplayName}, Email: {Email}, Created At: {CreatedAt}, Last Login At: {LastLoginAt}";
        }
    }

    public class UserDataService
    {
        private FirebaseAuth auth;
        private DatabaseReference databaseRef;

        public UserDataService(FirebaseAuth auth, DatabaseReference databaseRef)
        {
            this.auth = auth;
            this.databaseRef = databaseRef;
        }

        #region User Management

        public FirebaseUser User => auth?.CurrentUser;

        public void UpdateUserProfile(string displayName)
        {
            if (User == null)
            {
                Debug.LogError("Cannot update profile: User is null");
                return;
            }

            UserProfile profile = new UserProfile
            {
                DisplayName = displayName
            };

            User.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.LogError("Profile update failed: " + task.Exception);
                    return;
                }

                Debug.Log($"Profile updated successfully for {User.DisplayName}");
            });
        }

        #endregion

        #region Database Operations

        public void GetUserData(Action<UserData> callback)
        {
            if (User == null || databaseRef == null)
            {
                Debug.LogError(
                    "Cannot get user data: " + (User == null ? "User is null" : "Database reference is null"));
                callback?.Invoke(null);
                return;
            }

            databaseRef.Child("users").Child(User.UserId).GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    if (task.Exception != null)
                        Debug.LogError("GetUserData error: " +
                                       (task.IsFaulted ? task.Exception.ToString() : "Operation canceled"));
                    callback?.Invoke(null);
                    return;
                }

                if (!task.Result.Exists) return;
                var userData = JsonUtility.FromJson<UserData>(task.Result.GetRawJsonValue());
                callback?.Invoke(userData);
            });
        }

        public void SaveUserToDatabase()
        {
            if (User == null || databaseRef == null)
            {
                Debug.LogError("Cannot save user data: " +
                               (User == null ? "User is null" : "Database reference is null"));
                return;
            }

            var userData = new UserData(User);
            var json = JsonUtility.ToJson(userData);
            databaseRef.Child("users").Child(User.UserId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    if (task.Exception != null)
                        Debug.LogError("SaveUserToDatabase error: " +
                                       (task.IsFaulted ? task.Exception.ToString() : "Operation canceled"));
                    return;
                }

                Debug.Log($"User data saved to database successfully: {User.UserId}");
            });
        }

        public void UpdateLastLogin()
        {
            if (User == null || databaseRef == null)
            {
                Debug.LogError(
                    "Cannot update last login: " + (User == null ? "User is null" : "Database reference is null"));
                return;
            }

            databaseRef.Child("users").Child(User.UserId).Child("LastLoginAt")
                .SetValueAsync(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()).ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled || task.IsFaulted)
                    {
                        if (task.Exception != null)
                            Debug.LogError("UpdateLastLogin error: " +
                                           (task.IsFaulted ? task.Exception.ToString() : "Operation canceled"));
                        return;
                    }

                    Debug.Log($"Last login updated successfully for user: {User.UserId}");
                });
        }

        public void FindUserByDisplayName(string displayName, Action<UserData, string> callback)
        {
            if (databaseRef == null)
            {
                Debug.LogError("Cannot find user by display name: Database reference is null");
                callback?.Invoke(null, "Database reference is null");
                return;
            }

            databaseRef.Child("users").OrderByChild("DisplayName").EqualTo(displayName).GetValueAsync()
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled || task.IsFaulted)
                    {
                        if (task.Exception != null)
                        {
                            string errorMsg = task.IsFaulted ? task.Exception.Message : "Operation was canceled";
                            Debug.LogError("FindUserByDisplayName error: " + errorMsg);
                            callback?.Invoke(null, errorMsg);
                        }

                        return;
                    }

                    if (!task.Result.Exists || task.Result.ChildrenCount == 0)
                    {
                        callback?.Invoke(null, "User not found");
                        return;
                    }

                    UserData foundUserData = null;

                    foreach (var child in task.Result.Children)
                    {
                        foundUserData = JsonUtility.FromJson<UserData>(child.GetRawJsonValue());
                        break;
                    }

                    callback?.Invoke(foundUserData, null);
                });
        }

        #endregion
    }
}