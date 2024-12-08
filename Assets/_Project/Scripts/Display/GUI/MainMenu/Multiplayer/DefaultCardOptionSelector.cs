using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class RoomSetting : MonoBehaviour, ISetting, ISettingStorage
{
    public abstract void Initialize();
    public abstract string GetSettingName();
    public abstract string GetSettingDescription();
    public abstract void LoadSetting();

    public abstract void SaveSetting();
}

public abstract class MultiOptionRoomSetting : RoomSetting, IMultiOptionRoomSetting
{
    public UIMultiplayerController uiMultiplayerController;
    public CardOptionSelector cardOptionSelector;
    protected List<CardData> Values = new List<CardData>();
    protected int SelectedIndex;

    public void AddOption(CardData value)
    {
        Values.Add(value);
    }

    public List<CardData> GetValues()
    {
        return Values;
    }

    public int GetIndex() => SelectedIndex;
    public void SetIndex(int index) => SelectedIndex = index;

    protected void SelectOption(CardData value, int defaultIndex = 0)
    {
        if (TryGetIndex(value, out var index))
        {
            SetIndex(index);
        }
        else
        {
            SetIndex(defaultIndex);
        }
    }

    private bool TryGetIndex(CardData value, out int index)
    {
        return TryGetIndex(entry => entry.Equals(value), out index);
    }

    private bool TryGetIndex(Predicate<CardData> predicate, out int index)
    {
        index = Values.FindIndex(predicate);
        return index != -1;
    }
}

public interface IMultiOptionRoomSetting
{
    List<CardData> GetValues();
    int GetIndex();
    void SetIndex(int index);
}

public abstract class CardOptionSelector : MonoBehaviour
{
    public abstract void Initialize(List<CardData> cardData, int defaultIndex, UnityAction<int> onSelectionChanged);
}

public class DefaultCardOptionSelector : CardOptionSelector
{
    [Header("Card Setting")] [SerializeField]
    private Image chosenPreview;

    [SerializeField] private TextMeshProUGUI chosenName;
    [SerializeField] private TextMeshProUGUI chosenDescription;
    [SerializeField] private bool isLoop;

    [Header("Content Support")] [SerializeField]
    private Image chosenPreviewSupport;


    [SerializeField] private TextMeshProUGUI chosenNameSupport;

    [SerializeField] private TextMeshProUGUI chosenDescriptionSupport;


    [Header("Buttons")] [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    private int currentIndex;
    private int previousIndex;
    private List<CardData> values;
    private UnityAction<int> OnSelectionChanged;
    private Animator anim;
    
    private string rightAim = "Right";
    private string leftAim = "Left";
    

    public int CurrentIndex
    {
        get => currentIndex;
        set
        {
            previousIndex = currentIndex;
            if (value >= values.Count)
            {
                currentIndex = isLoop ? 0 : values.Count - 1;
            }
            else if (value < 0)
            {
                currentIndex = isLoop ? values.Count - 1 : 0;
            }
            else
            {
                currentIndex = value;
            }

            if (previousIndex != currentIndex)
            {
                OnSelectionChanged?.Invoke(currentIndex);
                UpdateValueText();
                UpdateSupportText();
            }
        }
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        nextButton.onClick.AddListener(SelectNextOption);
        previousButton.onClick.AddListener(SelectPreviousOption);
    }

    private void SelectNextOption() => Move(Vector2.right);
    private void SelectPreviousOption() => Move(Vector2.left);

    private void Move(Vector2 direction)
    {
        if (direction == Vector2.right)
        {
            CurrentIndex++;
            if(CurrentIndex != previousIndex) anim.Play(rightAim, 0, 0f);
        }
        else if (direction == Vector2.left)
        {
            CurrentIndex--;
            if(CurrentIndex != previousIndex) anim.Play(leftAim, 0, 0f);
        }
    }

    private void UpdateValueText()
    {
        chosenPreview.sprite = values[currentIndex].Preview;
        chosenName.text = values[currentIndex].Name.ToString();
        chosenDescription.text = values[currentIndex].Description;
    }
    
    private void UpdateSupportText()
    {
        chosenPreviewSupport.sprite = values[previousIndex].Preview;
        chosenNameSupport.text = values[previousIndex].Name.ToString();
        chosenDescriptionSupport.text = values[previousIndex].Description;
    }

    public override void Initialize(List<CardData> cardData, int defaultIndex, UnityAction<int> onSelectionChanged)
    {
        values = cardData;
        OnSelectionChanged += onSelectionChanged;
        currentIndex = defaultIndex;
        UpdateValueText();
        UpdateSupportText();
    }
}