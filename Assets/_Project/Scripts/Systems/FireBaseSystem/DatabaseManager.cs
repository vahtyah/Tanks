using UnityEngine;

public class DatabaseManager : Singleton<DatabaseManager>, IEventListener<AuthenticationEvent>,
    IEventListener<AuthenticationRequest>, IEventListener<GameEvent>
{
    public FriendService FriendService { get; private set; }
    public AuthenticationService AuthenticationService { get; private set; }

    private UserDataService userDataService;

    protected override void Awake()
    {
        base.Awake();
        _ = new DatabaseInitializer((reference =>
        {
            AuthenticationService = new AuthenticationService((auth) =>
            {
                userDataService = new UserDataService(auth, reference);
                FriendService = new FriendService(auth, reference, userDataService);
                GameEvent.Trigger(GameEventType.GameAuthentication); // TODO: Move to GameManager with all references ready
            });
        }));
    }

    // Sign in from previous session with toggle remember me on
    public void OnEvent(GameEvent e)
    {
        if (e.EventType != GameEventType.GameAuthentication) return;
        if (!PlayerPrefs.HasKey(GlobalString.REMEMBER_ME)) return;

        if (AuthenticationService.CurrentUser != null && PlayerPrefs.GetInt(GlobalString.REMEMBER_ME) == 1)
        {
            AuthenticationEvent.Trigger(AuthenticationEventType.LoginSuccessful, AuthenticationService.CurrentUser);
            Debug.Log(
                $"User is already logged in: {AuthenticationService.CurrentUser.DisplayName} ({AuthenticationService.CurrentUser.Email})");
        }
    }

    public void OnEvent(AuthenticationRequest e)
    {
        switch (e.Type)
        {
            case AuthenticationRequestType.Login:
                AuthenticationService.LoginWithEmailPassword(e.Email, e.Password);
                break;
            case AuthenticationRequestType.Register:
                AuthenticationService.RegisterWithEmailPassword(e.Email, e.Password, e.DisplayName);
                break;
            case AuthenticationRequestType.Logout:
                AuthenticationService.Logout();
                break;
        }
    }

    public void OnEvent(AuthenticationEvent e)
    {
        switch (e.EventType)
        {
            case AuthenticationEventType.LoginSuccessful:
                userDataService.UpdateLastLogin();
                break;
            case AuthenticationEventType.RegisterSuccessful:
                userDataService.SaveUserToDatabase();
                break;
        }
    }

    private void OnEnable()
    {
        this.StartListening<AuthenticationEvent>();
        this.StartListening<AuthenticationRequest>();
        this.StartListening<GameEvent>();
    }

    private void OnDisable()
    {
        this.StopListening<AuthenticationEvent>();
        this.StopListening<AuthenticationRequest>();
        this.StopListening<GameEvent>();
    }
}