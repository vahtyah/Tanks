using System;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

public class DataBaseManager : MonoBehaviour, IEventListener<AuthenticationEvent>
{
    private DatabaseReference databaseRef;
    private FirebaseUser user;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                databaseRef = FirebaseDatabase.GetInstance("https://tanks-c93ec-default-rtdb.firebaseio.com/")
                    .RootReference;
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
    }

    [Button]
    public void GetCurrentUserData()
    {
        if (user == null)
        {
            Debug.LogError("User is null, cannot get user data.");
            return;
        }

        databaseRef.Child("users").Child(user.UserId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("GetUserData was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("GetUserData encountered an error: " + task.Exception);
                return;
            }

            if (!task.Result.Exists) return;
            var userData = JsonUtility.FromJson<UserData>(task.Result.GetRawJsonValue());
            Debug.Log(userData);
        });
    }

    public void GetUserData(Action<UserData> callback)
    {
        if (user == null)
        {
            Debug.LogError("User is null, cannot get user data.");
            callback?.Invoke(null);
            return;
        }

        databaseRef.Child("users").Child(user.UserId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("GetUserData was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("GetUserData encountered an error: " + task.Exception);
                return;
            }

            if (!task.Result.Exists) return;
            var userData = JsonUtility.FromJson<UserData>(task.Result.GetRawJsonValue());
            callback?.Invoke(userData);
        });
    }

    private void SaveUserToDatabase()
    {
        if (user == null)
        {
            Debug.LogError("User is null, cannot save user data.");
            return;
        }

        if (databaseRef == null)
        {
            Debug.LogError("Database reference is null, cannot save user data.");
            return;
        }

        var userData = new UserData(user);
        var json = JsonUtility.ToJson(userData);
        databaseRef.Child("users").Child(user.UserId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SaveUserToDatabase was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("SaveUserToDatabase encountered an error: " + task.Exception);
                return;
            }

            Debug.Log($"User data saved to database successfully: {user.UserId}");
        });
    }

    private void UpdateLastLogin()
    {
        if (user == null)
        {
            Debug.LogError("User is null, cannot update last login.");
            return;
        }

        databaseRef.Child("users").Child(user.UserId).Child("LastLoginAt")
            .SetValueAsync(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("UpdateLastLogin was canceled.");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("UpdateLastLogin encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log($"Last login updated successfully for user: {user.UserId}");
            });
    }

    public void OnEvent(AuthenticationEvent e)
    {
        switch (e.EventType)
        {
            case AuthenticationEventType.LoginSuccessful:
                user = e.User;
                UpdateLastLogin();
                break;
            case AuthenticationEventType.LogoutSuccessful:
                user = null;
                break;
            case AuthenticationEventType.RegisterSuccessful:
                user = e.User;
                SaveUserToDatabase();
                break;
        }
    }

    private void OnEnable()
    {
        this.StartListening();
    }

    private void OnDisable()
    {
        this.StopListening();
    }
}