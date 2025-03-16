using Firebase.Auth;
    
    public enum AuthenticationEventType
    {
        LoginSuccessful,
        LoginFailed,
        RegisterSuccessful,
        RegisterFailed,
        LogoutSuccessful
    }
    
    public struct AuthenticationEvent
    {
        public AuthenticationEventType EventType;
        public FirebaseUser User;
        public string ErrorMessage;
        
        private static AuthenticationEvent cacheEvent;
    
        public static void Trigger(AuthenticationEventType eventType, FirebaseUser user = null, string errorMessage = "")
        {
            cacheEvent.EventType = eventType;
            cacheEvent.User = user;
            cacheEvent.ErrorMessage = errorMessage;
            EventManager.TriggerEvent(cacheEvent);
        }
    }
    
    public enum AuthenticationRequestType
    {
        Login,
        Register,
        Logout
    }
    
    public struct AuthenticationRequest
    {
        public AuthenticationRequestType Type;
        public string Email;
        public string Password;
        public string DisplayName;
        
        private static AuthenticationRequest cacheEvent;
    
        public static void Trigger(AuthenticationRequestType type, string email = "", string password = "", string displayName = "")
        {
            cacheEvent.Type = type;
            cacheEvent.Email = email;
            cacheEvent.Password = password;
            cacheEvent.DisplayName = displayName;
            EventManager.TriggerEvent(cacheEvent);
        }   
    }