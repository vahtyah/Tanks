using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InfoGame
{
    public class Info
    {
        public string Title { get; private set; }
        public string Value { get; private set; }
        
        private Action<string> onValueChanged;

        private Info(string title, string value)
        {
            Title = title;
            Value = value;
        }

        private static InfoManager manager; 
        private static void EnsureManagerExists()
        {
            if (manager == null)
            {
                manager = InfoManager.Instance ?? new GameObject("InfoManager").AddComponent<InfoManager>();
            }
        }
        
        public static Info Register(string category,string title, string value = "")
        {
            EnsureManagerExists();
            var info = new Info(title, value);
            manager.Register(category, info);
            return info;
        }
        
        public void SetValue(string value)
        {
            Value = value;
            onValueChanged?.Invoke(value);
        }
        
        public void OnValueChanged(Action<string> onValueChanged)
        {
            this.onValueChanged += onValueChanged;
        }
    }
    
    public class InfoManager : MonoBehaviour
    {
        public static InfoManager Instance { get; private set; }
        
        private Dictionary<string, List<Info>> infos = new();

        [SerializeField] private Button openPanelButton;
        [SerializeField] private GameObject infoPanel;
        [SerializeField] private Button closePanelButton;
        
        
        [SerializeField] private GameObject infoPrefab;
        [SerializeField] private Transform infoContainer;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
            
            openPanelButton.onClick.AddListener(() =>
            {
                infoPanel.SetActive(true);
                openPanelButton.gameObject.SetActive(false);
            });
            closePanelButton.onClick.AddListener(() => 
            {
                infoPanel.SetActive(false);
                openPanelButton.gameObject.SetActive(true);
            });
        }
        
        public void Register(string category, Info info)
        {
            if (!infos.ContainsKey(category))
            {
                infos.Add(category, new List<Info>());
            }
            infos[category].Add(info);
            InfoDisplay.Create(infoPrefab, infoContainer, info);
        }
    }
}