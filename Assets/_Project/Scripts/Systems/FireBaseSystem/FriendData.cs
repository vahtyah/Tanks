using System;

public enum FriendStatus
{
    Pending,
    Accepted,
    Received,
    Blocked
}

[Serializable]
public class FriendData
{
    public string UserId;
    public string DisplayName;
    public string Email;
    public long FriendshipCreatedAt;
    public string Status; // "pending", "accepted", "blocked"

    public FriendData() { } // Required for Firebase serialization
    
    public FriendStatus FriendStatusEnum
    {
        get => Enum.TryParse<FriendStatus>(Status, true, out var status) ? status : FriendStatus.Pending;
        set => Status = value.ToString().ToLower();
    }

    public FriendData(UserData userData, FriendStatus status = FriendStatus.Pending)
    {
        UserId = userData.UserId;
        DisplayName = userData.DisplayName;
        Email = userData.Email;
        FriendshipCreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        FriendStatusEnum = status;
    }

    public FriendData(string userId, string displayName, string email, FriendStatus status = FriendStatus.Pending)
    {
        UserId = userId;
        DisplayName = displayName;
        Email = email;
        FriendshipCreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        FriendStatusEnum = status;
    }
}