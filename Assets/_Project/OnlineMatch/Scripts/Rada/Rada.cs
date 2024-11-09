using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Rada : MonoBehaviour, IEventListener<CharacterEvent>
{
    [ShowInInspector] readonly Dictionary<LocatableComponent, LocatableIconComponent> locatableIconDictionary = new();

    [ShowInInspector] readonly Dictionary<LocatableComponent, LocatableIconComponent> teammateIconDictionary = new();
    [ShowInInspector] readonly Dictionary<LocatableComponent, LocatableIconComponent> enemyIconDictionary = new();


    [SerializeField] private RectTransform iconContainer;

    [SerializeField, Min(1)] private float range = 20;

    [SerializeField] private bool applyRotation = true;

    [SerializeField] private Image visibleArea;

    private Camera mainCamera;


    public float Range
    {
        get => range;
        set => range = value;
    }

    public bool ApplyRotation
    {
        get => applyRotation;
        set => applyRotation = value;
    }

    public GameObject Player;
    private Player player;
    LocatableIconComponent playerIcon;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        LocatableManager.OnLocatableAdded += OnLocatableAdded;
        LocatableManager.OnLocatableRemoved += OnLocatableRemoved;
        this.StartListening();
    }

    private void OnDisable()
    {
        LocatableManager.OnLocatableAdded -= OnLocatableAdded;
        LocatableManager.OnLocatableRemoved -= OnLocatableRemoved;
        this.StopListening();
    }

    public void OnEvent(CharacterEvent e)
    {
        if (e.EventType == CharacterEventType.CharacterSpawned)
        {
            Player = e.Character.GetComponent<CharacterTank>().Turret;
            player = e.Character.PhotonView.Owner;
        }
    }

    // private void OnLocatableAdded(LocatableComponent locatable)
    // {
    //     // Create the icon for the locatable and add a new entry to the dictionary
    //     if (locatable != null && !locatableIconDictionary.ContainsKey(locatable))
    //     {
    //         var locatableCharacter = locatable.GetComponent<Character>();
    //         var color = Color.red;
    //         var isDetected = false;
    //         if (locatableCharacter)
    //         {
    //             if (locatableCharacter.PhotonView.IsMine)
    //                 return;
    //
    //             if (locatableCharacter.GetTeamType() == PhotonNetwork.LocalPlayer.GetTeam().TeamType)
    //             {
    //                 color = Color.green;
    //                 isDetected = true;
    //             }
    //         }
    //
    //         var icon = locatable.CreateIcon(color);
    //         icon.transform.SetParent(iconContainer.transform, false);
    //
    //         locatable.Detected = isDetected;
    //         locatableIconDictionary.Add(locatable, icon);
    //     }
    // }

    private void AddTeammateIconDic(LocatableComponent locatable, LocatableIconComponent icon)
    {
        icon.ChangeColor(Color.green);
        locatable.Detected = true;
        teammateIconDictionary.Add(locatable, icon);
    }

    private void AddEnemyIconDic(LocatableComponent locatable, LocatableIconComponent icon)
    {
        icon.ChangeColor(Color.red);
        locatable.Detected = false;
        enemyIconDictionary.Add(locatable, icon);
    }

    private void OnLocatableAdded(LocatableComponent locatable)
    {
        // Create the icon for the locatable and add a new entry to the dictionary
        if (locatable != null && !locatableIconDictionary.ContainsKey(locatable))
        {
            var locatableCharacter = locatable.GetComponent<Character>();
            if (locatableCharacter)
            {
                if (locatableCharacter.PhotonView.IsMine)
                {
                    playerIcon = locatable.CreateIcon(Color.white, true);
                    playerIcon.transform.SetParent(iconContainer.transform, false);
                    return;
                }

                var icon = locatable.CreateIcon(Color.red);
                icon.transform.SetParent(iconContainer.transform, false);
                if (locatableCharacter.GetTeamType() == PhotonNetwork.LocalPlayer.GetTeam().TeamType)
                {
                    AddTeammateIconDic(locatable, icon);
                }
                else
                {
                    AddEnemyIconDic(locatable, icon);
                }
            }
        }
    }

    private bool IsTargetOnScreen(Vector3 position)
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(position);

        return screenPosition.z > 0 && screenPosition.x > 0 && screenPosition.x < screenWidth &&
               screenPosition.y > 0 && screenPosition.y < screenHeight;
    }

    private void OnLocatableRemoved(LocatableComponent locatable)
    {
        // if (locatable != null && locatableIconDictionary.TryGetValue(locatable, out LocatableIconComponent icon))
        // {
        //     locatableIconDictionary.Remove(locatable);
        //
        //     Destroy(icon.gameObject);
        // }

        if (locatable != null)
        {
            if (teammateIconDictionary.Remove(locatable, out LocatableIconComponent teammateIcon))
            {
                Destroy(teammateIcon.gameObject);
            }
            else if (enemyIconDictionary.Remove(locatable, out LocatableIconComponent enemyIcon))
            {
                Destroy(enemyIcon.gameObject);
            }
        }
    }

    private void Update()
    {
        if (Player != null)
        {
            // UpdateLocatableIcons();
            UpdateTeammateLocatableIcons();
            UpdateEnemyLocatableIcons();
            UpdatePlayerIcon();
        }
    }

    private void UpdatePlayerIcon()
    {
        //rotate player icon
        if (playerIcon != null)
        {
            float playerRotationZ = Player.transform.eulerAngles.y; // Lấy góc quay Z (yaw) từ đối tượng Player
            playerIcon.transform.rotation = Quaternion.Euler(0, 0, -playerRotationZ); // Áp dụng góc quay vào playerIcon
        }
    }

    private void UpdateTeammateLocatableIcons()
    {
        foreach (var kvp in teammateIconDictionary)
        {
            var locatable = kvp.Key;
            var icon = kvp.Value;

            if (TryGetIconLocation(locatable, out var iconLocation))
            {
                icon.SetVisible(true);
                var rectTransform = icon.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = iconLocation;
            }
            else
            {
                icon.SetVisible(false);
            }
        }
    }

    private void UpdateEnemyLocatableIcons()
    {
        foreach (var kvp in enemyIconDictionary)
        {
            var locatable = kvp.Key;
            var icon = kvp.Value;


            if (TryGetIconLocation(locatable, out var iconLocation))
            {
                locatable.SetDetectedForTeam(player, IsTargetOnScreen(locatable.transform.position));
            }

            icon.SetVisible(locatable.Detected);

            if (locatable.Detected)
            {
                var rectTransform = icon.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = iconLocation;
            }
        }
    }


    private void UpdateLocatableIcons()
    {
        foreach (var kvp in locatableIconDictionary)
        {
            var locatable = kvp.Key;
            var icon = kvp.Value;

            if (TryGetIconLocation(locatable, out var iconLocation))
            {
                icon.SetVisible(true);

                var rectTransform = icon.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = iconLocation;
            }
            else
            {
                icon.SetVisible(false);
            }
        }
    }


    private bool TryGetIconLocation(LocatableComponent locatable, out Vector2 iconLocation)
    {
        iconLocation = GetDistanceToPlayer(locatable);

        float radarSize = GetRadarUISize();

        var scale = radarSize / Range;

        iconLocation *= scale;

        if (ApplyRotation)
        {
            var playerForwardDirectionXZ = Vector3.ProjectOnPlane(Player.transform.forward, Vector3.up);

            var rotation = Quaternion.LookRotation(playerForwardDirectionXZ);

            var euler = rotation.eulerAngles;
            euler.y = -euler.y;
            rotation.eulerAngles = euler;

            var rotatedIconLocation = rotation * new Vector3(iconLocation.x, 0.0f, iconLocation.y);

            iconLocation = new Vector2(rotatedIconLocation.x, rotatedIconLocation.z);
        }

        if (iconLocation.sqrMagnitude < radarSize * radarSize || locatable.ClampOnRadar)
        {
            iconLocation = Vector2.ClampMagnitude(iconLocation, radarSize);

            return true;
        }

        return false;
    }


    private float GetRadarUISize()
    {
        return iconContainer.rect.width / 2;
    }

    private Vector2 GetDistanceToPlayer(LocatableComponent locatable)
    {
        Vector3 distanceToPlayer = locatable.transform.position - Player.transform.position;

        return new Vector2(distanceToPlayer.x, distanceToPlayer.z);
    }
}