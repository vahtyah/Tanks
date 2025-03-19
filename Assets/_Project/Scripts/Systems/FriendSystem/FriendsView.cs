using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendsView : MonoBehaviour
{
    [SerializeField] private GameObject contentPanel;
    [SerializeField] private GameObject friendItemViewPrefab;
    [SerializeField] private RectTransform sperator;
    [SerializeField] private RectTransform noFriendsOnline;
    [SerializeField] private RectTransform friendsViewContainer;
    [SerializeField] private TMP_InputField friendNameInput;
    
    [Header("Controls")]
    [SerializeField] private Button openCloseFriendsPanelButton;

    [Header("Add Friend")] 
    [SerializeField] private Button openAddFriendPanelButton;
    [SerializeField] private GameObject addFriendPanel;
    [SerializeField] private Button addFriendButton;

    [Header("View")] [SerializeField] private TextMeshProUGUI friendsCountText;

    // [SerializeField] private FriendUIStatus[] statusData;
    private readonly Dictionary<string, FriendItemView> friendItems = new();
    private FriendsController controller;
    
    //Amimation
    private Animator anim;
    const string openAim = "Open";
    const string closeAim = "Close";
    const string AddFriend = "AddFriend";
    const string CloseAddFriend = "CloseAddFriend";
    bool isAddFriendPanelOpen = false;
    
    private void Awake()
    {
        anim = GetComponent<Animator>();
        
        openCloseFriendsPanelButton.onClick.AddListener(() =>
        {
            anim.Play(contentPanel.activeSelf ? closeAim : openAim);
        });
        
        openAddFriendPanelButton.onClick.AddListener(() =>
        {
            friendNameInput.ActivateInputField();
            anim.Play(isAddFriendPanelOpen ? CloseAddFriend : AddFriend);
            isAddFriendPanelOpen = !isAddFriendPanelOpen;
        });
    }

    public void Initialize(FriendsController controller)
    {
        this.controller = controller;
        noFriendsOnline.gameObject.SetActive(false);
        sperator.gameObject.SetActive(false);
        addFriendButton.onClick.AddListener(OnAddFriendButtonClicked);
    }

    public void CleanUp()
    {
        RemoveAllFriendItemViews();
        noFriendsOnline.gameObject.SetActive(false);
        sperator.gameObject.SetActive(false);
    }

    public void SetVisible(bool isVisible)
    {
        contentPanel.SetActive(isVisible);
    }

    public void SetLoadingMessage(string message)
    {
        // Implement loading message logic here
    }

    public void UpdateFriendsList(List<PhotonFriendData> onlineFriends, List<PhotonFriendData> offlineFriends)
    {
        RemoveAllFriendItemViews();
        foreach (var friend in onlineFriends)
        {
            AddFriendItemView(friend);
        }

        sperator.gameObject.SetActive(onlineFriends.Count > 0 || offlineFriends.Count > 0);
        sperator.SetAsLastSibling();

        foreach (var friend in offlineFriends)
        {
            AddFriendItemView(friend);
        }

        noFriendsOnline.gameObject.SetActive(onlineFriends.Count == 0);
        friendsCountText.text = $"{onlineFriends.Count}/{onlineFriends.Count + offlineFriends.Count}";
    }

    public void AddFriendItemView(PhotonFriendData photonFriendData)
    {
        if (friendItems.ContainsKey(photonFriendData.DisplayName))
            return;

        var item = Pool.Spawn(friendItemViewPrefab, friendsViewContainer);
        item.transform.SetAsLastSibling();
        var itemView = item.GetComponent<FriendItemView>();
        itemView.Initialize(photonFriendData, controller);
        friendItems.Add(photonFriendData.DisplayName, itemView);
    }

    public void RemoveFriendItemView(string friendName)
    {
        if (!friendItems.TryGetValue(friendName, out var itemView))
            return;

        Pool.Despawn(itemView.gameObject);
        friendItems.Remove(friendName);
    }

    public void RemoveAllFriendItemViews()
    {
        foreach (var itemView in friendItems.Values)
        {
            Pool.Despawn(itemView.gameObject);
        }

        friendItems.Clear();
    }

    public void OnAddFriendButtonClicked()
    {
        var friendName = friendNameInput.text.Trim();
        if (string.IsNullOrEmpty(friendName))
            return;

        // Call the method to add a friend
        controller.AddFriend(friendName);
        friendNameInput.text = string.Empty;
        friendNameInput.DeactivateInputField();
        addFriendPanel.SetActive(false);
    }
}