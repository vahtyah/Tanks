using System;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

public class AuthenticationManager : MonoBehaviour, IEventListener<AuthenticationRequest>, IEventListener<GameEvent>
{
    private FirebaseAuth auth;
    private FirebaseUser user;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                // Debug.Log("Firebase initialized successfully!");
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
    }

    [Button]
    public void LoginWithEmailPassword(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                AuthenticationEvent.Trigger(AuthenticationEventType.LoginFailed, user,
                    errorMessage: "Login was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                AuthenticationEvent.Trigger(AuthenticationEventType.LoginFailed,
                    errorMessage: "Email or password is incorrect.");
                return;
            }

            user = task.Result.User;
            Debug.Log($"User logged in successfully: {user.DisplayName} ({user.Email})");
            AuthenticationEvent.Trigger(AuthenticationEventType.LoginSuccessful, user);
        });
    }


    [Button]
    public void RegisterWithEmailPassword(string email, string password, string displayName)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Registration was canceled.");
                AuthenticationEvent.Trigger(AuthenticationEventType.RegisterFailed, user,
                    errorMessage: "Registration was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                AuthenticationEvent.Trigger(AuthenticationEventType.RegisterFailed, user,
                    errorMessage: GetErrorMessage(task.Exception));
                return;
            }

            user = task.Result.User;
            Debug.Log($"User registered successfully: {email}");

            // SendVerificationEmail();
            UpdateUserProfile(displayName);
        });
    }

    private void SendVerificationEmail()
    {
        if (user != null && !user.IsEmailVerified)
        {
            user.SendEmailVerificationAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SendEmailVerificationAsync was canceled.");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("Email verification sent successfully.");
            });
        }
    }

    private bool IsEmailVerified()
    {
        return user != null && user.IsEmailVerified;
    }

    public void ResendVerificationEmail()
    {
        if (user != null)
        {
            SendVerificationEmail();
        }
        else
        {
            Debug.LogError("No user is currently logged in.");
        }
    }

    private void UpdateUserProfile(string displayName)
    {
        if (user != null)
        {
            UserProfile profile = new UserProfile
            {
                DisplayName = displayName
            };

            user.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.LogError("Profile update failed: " + task.Exception);
                    return;
                }

                Debug.Log($"Profile updated successfully for {user.DisplayName}");
                AuthenticationEvent.Trigger(AuthenticationEventType.RegisterSuccessful, user);
            });
        }
    }

    [Button]
    public void Logout()
    {
        if (auth != null)
        {
            auth.SignOut();
            AuthenticationEvent.Trigger(AuthenticationEventType.LogoutSuccessful, user);
            user = null;
            Debug.Log("User logged out successfully.");
        }
    }

    private string GetErrorMessage(Exception exception)
    {
        if (exception == null)
            return "Unknown error occurred.";

        FirebaseException firebaseException = exception.GetBaseException() as FirebaseException;

        if (firebaseException != null)
        {
            var errorCode = firebaseException.ErrorCode;

            switch (errorCode)
            {
                case 17020: // Wrong password
                    return "Incorrect password.";
                case 17005: // Email doesn't exist
                    return "Email does not exist.";
                case 17007: // Email already in use
                    return "This email is already in use.";
                case 17008: // Invalid email
                    return "Invalid email.";
                case 17026: // Password too weak
                    return "Password is too weak.";
                default:
                    return $"{firebaseException.Message}";
            }
        }

        return exception.Message;
    }

    public void OnEvent(AuthenticationRequest e)
    {
        switch (e.Type)
        {
            case AuthenticationRequestType.Login:
                LoginWithEmailPassword(e.Email, e.Password);
                break;
            case AuthenticationRequestType.Register:
                RegisterWithEmailPassword(e.Email, e.Password, e.DisplayName);
                break;
            case AuthenticationRequestType.Logout:
                Logout();
                break;
        }
    }

    public void OnEvent(GameEvent e)
    {
        switch (e.EventType)
        {
            case GameEventType.GameAuthentication:
                if (!PlayerPrefs.HasKey(GlobalString.REMEMBER_ME)) return;
                if (auth.CurrentUser != null && PlayerPrefs.GetInt(GlobalString.REMEMBER_ME) == 1)
                {
                    user = auth.CurrentUser;
                    AuthenticationEvent.Trigger(AuthenticationEventType.LoginSuccessful, user);
                    Debug.Log($"User is already logged in: {user.DisplayName} ({user.Email})");
                }
                break;
            default:
                break;
        }
    }

    private void OnEnable()
    {
        this.StartListening<AuthenticationRequest>();
        this.StartListening<GameEvent>();
    }

    private void OnDisable()
    {
        this.StopListening<AuthenticationRequest>();
        this.StopListening<GameEvent>();
    }
}