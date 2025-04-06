using System;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;

namespace Testing
{
    public class AuthenticationService
    {
        private FirebaseAuth auth;

        public AuthenticationService(Action<FirebaseAuth> onInitialized = null)
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    auth = FirebaseAuth.DefaultInstance;
                    onInitialized?.Invoke(auth);
                }
                else
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                    onInitialized?.Invoke(null);
                }
            });
        }

        #region Authentication Operations

        public bool IsLoggedIn => auth?.CurrentUser != null;
        public FirebaseUser CurrentUser => auth?.CurrentUser;

        public void LoginWithEmailPassword(string email, string password, Action<FirebaseUser, string> onloggin = null)
        {
            auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    return;
                }

                if (task.IsFaulted)
                {
                    return;
                }

                var user = task.Result.User;
                
                onloggin?.Invoke(user, "Login successful");
                Debug.Log($"User logged in successfully: {user.DisplayName} ({user.Email})");
            });
        }

        public void RegisterWithEmailPassword(string email, string password, string displayName, Action<FirebaseUser, string> onRegister = null)
        {
            auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    return;
                }

                if (task.IsFaulted)
                {
                    return;
                }

                var user = task.Result.User;

                // Update user profile with display name
                var profile = new UserProfile
                {
                    DisplayName = displayName
                };
                user.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(updateTask =>
                {
                    if (updateTask.IsCanceled || updateTask.IsFaulted)
                    {
                        Debug.LogError("Profile update failed: " + updateTask.Exception);
                        return;
                    }
                });
                
                onRegister?.Invoke(user, "Registration successful");

                Debug.Log($"User registered successfully: {email}");
            });
        }

        public void Logout()
        {
            if (auth != null)
            {
                auth.SignOut();
                Debug.Log("User logged out successfully.");
            }
        }

        #endregion

        #region Email Verification

        private void SendVerificationEmail(FirebaseUser user)
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

        public void ResendVerificationEmail(FirebaseUser user)
        {
            if (user != null)
            {
                SendVerificationEmail(user);
            }
            else
            {
                Debug.LogError("No user is currently logged in.");
            }
        }

        public bool IsEmailVerified(FirebaseUser user)
        {
            return user != null && user.IsEmailVerified;
        }

        #endregion

        #region Error Handling

        private string GetErrorMessage(Exception exception)
        {
            if (exception == null)
                return "Unknown error occurred.";

            if (exception.GetBaseException() is not FirebaseException firebaseException) return exception.Message;
            var errorCode = firebaseException.ErrorCode;

            switch (errorCode)
            {
                case 17020: return "Incorrect password.";
                case 17005: return "Email does not exist.";
                case 17007: return "This email is already in use.";
                case 17008: return "Invalid email.";
                case 17026: return "Password is too weak.";
                default: return $"{firebaseException.Message}";
            }
        }

        #endregion
    }
}