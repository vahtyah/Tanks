﻿using UnityEngine;
using UnityEngine.UI;

public class LevelSelectorButton : MonoBehaviour
{
    [SerializeField] private Scene.SceneName levelName;

    private void Awake() { GetComponent<Button>().onClick.AddListener(OnLevelSelect); }

    private void OnLevelSelect()
    {
        Scene.Load(levelName.ToString());
    }
}