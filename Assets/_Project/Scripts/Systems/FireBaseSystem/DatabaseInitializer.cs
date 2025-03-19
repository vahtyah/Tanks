using System;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class DatabaseInitializer
{
    private DatabaseReference databaseRef;

    public DatabaseInitializer(Action<DatabaseReference> onInitialized)
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
            onInitialized?.Invoke(databaseRef);
        });
    }

    public DatabaseReference GetDatabaseReference()
    {
        return databaseRef;
    }
}