using System;
using System.Collections.Generic;
using UnityEngine;

public class ListHighScore : MonoBehaviour
{
    [SerializeField] private GameObject elementPrefab;
    private List<GameObject> elements = new List<GameObject>();

    private void OnEnable()
    {
        if(DatabaseManager.Instance.users == null) return;
        var users = DatabaseManager.Instance.users;
        for (int i = 0; i < 10; i++)
        {
            if(i >= users.Count) break;
            var element = Pool.Spawn(elementPrefab, null);
            element.GetComponent<HighScoreElement>().SetData(i + 1, users[i].username, users[i].score);
            elements.Add(element);
        }
    }
    
    private void OnDisable()
    {
        foreach (var element in elements)
        {
            Pool.Despawn(element);
        }
        elements.Clear();
    }
}

