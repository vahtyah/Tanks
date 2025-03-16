using System;
using Firebase.Auth;

[Serializable]
public class UserData
{
    public string UserId;
    public string DisplayName;
    public string Email;

    public long CreatedAt;
    public long LastLoginAt;

    public UserData() {}

    public UserData(FirebaseUser user)
    {
        UserId = user.UserId;
        DisplayName = user.DisplayName;
        Email = user.Email;
        CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        LastLoginAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public override string ToString()
    {
        return
            $"User ID: {UserId}, Display Name: {DisplayName}, Email: {Email}, Created At: {CreatedAt}, Last Login At: {LastLoginAt}";
    }
}