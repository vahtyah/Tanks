using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CharacterData
{
    public string Name;
    public int ID;
    public GameObject DisplayPrefab;
    public GameObject CharacterPrefab;
    public Transform DisplayTransform;
}

public class CharacterSelector : MonoBehaviour
{
    public CharacterData[] AvailableCharacters;
    public CharacterData CurrentSelection;

    [SerializeField] private CameraMainMenu cameraMainMenu;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;


    private Transform emptyTransform;
    private bool canChangeSelection = false;

    private void Start()
    {
        for (var i = 0; i < AvailableCharacters.Length; i++)
        {
            var tankDisplay = Instantiate(AvailableCharacters[i].DisplayPrefab, Vector3.zero.Add(x: i * 50),
                AvailableCharacters[i].DisplayPrefab.transform.rotation);
            AvailableCharacters[i].DisplayTransform = tankDisplay.transform;
        }

        emptyTransform = new GameObject("EmptyTransform").transform;
        SelectNextCharacter();
        TurnOffDisplay();
        AddListenerButtons();
    }

    private void AddListenerButtons()
    {
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(SelectNextCharacter);
        }

        if (previousButton != null)
        {
            previousButton.onClick.AddListener(SelectPreviousCharacter);
        }
    }

    public void TurnOffDisplay()
    {
        cameraMainMenu.ChangeTarget(emptyTransform);
    }

    public void TurnOnDisplay()
    {
        cameraMainMenu.ChangeTarget(CurrentSelection.DisplayTransform);
    }

    private void Update()
    {
        if (canChangeSelection)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SelectNextCharacter();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SelectPreviousCharacter();
            }
        }
    }

    private void SelectNextCharacter()
    {
        if (CurrentSelection == null)
        {
            CurrentSelection = AvailableCharacters[0];
        }
        else
        {
            var index = Array.IndexOf(AvailableCharacters, CurrentSelection);
            index = (index + 1) % AvailableCharacters.Length;
            CurrentSelection = AvailableCharacters[index];
        }

        SetEmptyTransformPosition();
        TurnOnDisplay();
        GameManager.Instance.SelectedCharacter = CurrentSelection;
    }

    private void SelectPreviousCharacter()
    {
        if (CurrentSelection == null)
        {
            CurrentSelection = AvailableCharacters[0];
        }
        else
        {
            var index = Array.IndexOf(AvailableCharacters, CurrentSelection);
            index = (index - 1 + AvailableCharacters.Length) % AvailableCharacters.Length;
            CurrentSelection = AvailableCharacters[index];
        }

        SetEmptyTransformPosition();
        TurnOnDisplay();
        GameManager.Instance.SelectedCharacter = CurrentSelection;
    }

    private void SetEmptyTransformPosition()
    {
        emptyTransform.position = CurrentSelection.DisplayTransform.position.Add(x: -25);
    }

    public void SetCanChangeSelection(bool value)
    {
        canChangeSelection = value;
    }
}