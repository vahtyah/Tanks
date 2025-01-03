﻿using System;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using UnityEngine;

public class DatabaseManager : Singleton<DatabaseManager>, IEventListener<GameEvent>
{
    private DatabaseReference reference;
    private string userName;
    public readonly List<User> users = new List<User>();

    protected override void Awake()
    {
        base.Awake();
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                var app = FirebaseApp.DefaultInstance;
                var databaseURL = "https://tanks-c93ec-default-rtdb.firebaseio.com/";
                reference = FirebaseDatabase.GetInstance(app, databaseURL).RootReference;
                GetUsersSortedByScore();
                Debug.Log("Firebase Database initialized successfully.");
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
    }

    private void WriteNewUser(int score)
    {
        if (reference != null)
        {
            User user = new User(userName, score);
            string json = JsonUtility.ToJson(user);
            reference.Child("users").Child(userName).SetRawJsonValueAsync(json).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("User data written successfully.");
                }
                else
                {
                    Debug.LogError("Failed to write user data: " + task.Exception);
                }
            });
        }
        else
        {
            Debug.LogError("Database reference is not initialized yet.");
        }
    }

    public void SetUserName(string inputFieldText) { userName = inputFieldText; }

    public void GetUsersSortedByScore()
    {
        if (reference != null)
        {
            reference.Child("users").OrderByChild("score").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    foreach (DataSnapshot userSnapshot in snapshot.Children)
                    {
                        string json = userSnapshot.GetRawJsonValue();
                        User user = JsonUtility.FromJson<User>(json);
                        users.Add(user);
                    }
                    users.Reverse();
                }
                else
                {
                    Debug.LogError("Failed to read user data: " + task.Exception);
                }
            });
        }
        else
        {
            Debug.LogError("Database reference is not initialized yet.");
        }
    }

    public void OnEvent(GameEvent e)
    {
        switch (e.EventType)
        {
            case GameEventType.GameStart:
                SetUserName(LevelManagerBotMatch.Instance.Username);
                break;
            case GameEventType.GameOver:
                WriteNewUser(LevelManagerBotMatch.Instance.Score);
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