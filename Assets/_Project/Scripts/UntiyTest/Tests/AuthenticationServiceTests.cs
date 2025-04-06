using NUnit.Framework;
// For mocking
using Testing;

[TestFixture]
public class AuthenticationServiceTests
{
    private AuthenticationService _authenticationService;
    
    [SetUp]
    public void SetUp()
    {
        // Initialize the AuthenticationService with a mock FirebaseAuth
    }
    
    [Test]
    public void TestRegisterWithEmailAndPassword()
    {
        _authenticationService = new AuthenticationService(init =>
        {
            string email = "8268826@gmail.com";
            string password = "8826Ty@h";
            string username = "TestUser";
            _authenticationService.RegisterWithEmailPassword(email, password,username, ((user, s) => 
            {
                Assert.IsNotNull(user, "User should not be null after registration.");
            }));
        });
    }

    [Test]
    public void TestLoginWithEmailAndPassword()
    {
        _authenticationService = new AuthenticationService(init =>
        {
            string email = "8268826@gmail.com";
            string password = "8826Ty@h";
            _authenticationService.LoginWithEmailPassword(email, password, ((user, s) => 
            {
                Assert.IsNotNull(user, "User should not be null after login.");
            }));
        });
    }
    
    
}