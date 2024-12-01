using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DefaultMultiOptionSelector : MultiOptionSelector, IMoveHandler
{
    [Header("Text")] [SerializeField] TMPro.TextMeshProUGUI labelText;

    [SerializeField] TMPro.TextMeshProUGUI valueText;

    [Header("Audio")] [SerializeField] AudioClip selectionChangedSFX;

    [SerializeField] AudioClip verticalMovementSFX;

    [Header("Settings")]
    [SerializeField]
    bool loop = true;
    
    [Header("Buttons")] [SerializeField]
    private PointerDownHandler nextButton;
    
    [SerializeField] private PointerDownHandler previousButton;

    int currentIndex = 0;

    public int CurrentIndex
    {
        get => currentIndex;
        set
        {
            int previousIndex = currentIndex;
            if (value >= Values.Count)
            {
                currentIndex = loop ? 0 : Values.Count - 1;
            }
            else if (value < 0)
            {
                currentIndex = loop ? Values.Count - 1 : 0;
            }
            else
            {
                currentIndex = value;
            }
            
            if(previousIndex != currentIndex)
            {
                OnSelectionChanged?.Invoke(currentIndex);
                UpdateValueText();
                PlayAudioFeedback(selectionChangedSFX);
            }
        }
    }

    List<string> Values;

    UnityAction<int> OnSelectionChanged { get; set; }

    AudioSource AudioSource { get; set; }

    private void Awake()
    {
        AudioSource = GetComponent<AudioSource>();
        nextButton.OnClick.AddListener(SelectNextOption);
        previousButton.OnClick.AddListener(SelectPreviousOption);
    }

    public override void Initialize(string label, List<string> values, int index, UnityAction<int> onSelectionChanged)
    {
        OnSelectionChanged += onSelectionChanged;

        labelText.text = label;
        Values = values;
        currentIndex = index;

        UpdateValueText();
    }

    public override void SetIndex(int index)
    {
        CurrentIndex = index;
    }


    public void OnMove(AxisEventData eventData)
    {
        Move(eventData.moveVector);
    }

    /// <summary>
    /// Selects the next option
    /// </summary>
    public void SelectNextOption() => Move(Vector2.right);

    /// <summary>
    /// Selects the previous Option
    /// </summary>
    public void SelectPreviousOption() => Move(Vector2.left);

    private void Move(Vector2 direction)
    {
        // Increase the index when moving to the right
        if (direction.x > 0)
        {
            CurrentIndex++;
        }

        // Decrease the index when moving to the left
        if (direction.x < 0)
        {
            CurrentIndex--;
        }
        
        if (direction.y != 0)
        {
            if (AudioSource != null && verticalMovementSFX != null)
            {
                PlayAudioFeedback(verticalMovementSFX);
            }
        }
    }

    void UpdateValueText()
    {
        valueText.text = Values[currentIndex];
    }

    void PlayAudioFeedback(AudioClip audioClip)
    {
        if (AudioSource != null && audioClip != null)
        {
            AudioSource.PlayOneShot(audioClip);
        }
    }
}