using NUnit.Framework;
using Testing;

[TestFixture]
public class UserDataServiceTests
{
    private AuthenticationService authenticationService;
    [Test]
    public void TestFindUserByDisplayName()
    {
        _ = new AuthenticationService(auth =>
        {
            _ = new DatabaseInitializer(dbRef =>
            {
                var userDataService = new UserDataService(auth, dbRef);
                var userId = auth.CurrentUser.UserId;
                var displayName = auth.CurrentUser.DisplayName;

                // Act
                userDataService.FindUserByDisplayName(displayName, (result, message) =>
                {
                    // Assert
                    Assert.IsNotNull(result);
                    Assert.AreEqual(userId, result.UserId);
                    Assert.AreEqual(displayName, result.DisplayName);
                });
            });
        });
    }
    
    [Test]
    public void TestGetUserData()
    {
        authenticationService = new AuthenticationService(auth =>
        {
            _ = new DatabaseInitializer(dbRef =>
            {
                string email = "8268826@gmail.com";
                string password = "8826Ty@h";
                authenticationService.LoginWithEmailPassword(email, password, ((user, s) => 
                {
                    var userDataService = new UserDataService(auth, dbRef);
                    var userId = auth.CurrentUser.UserId;

                    // Act
                    userDataService.GetUserData((result) =>
                    {
                        // Assert
                        Assert.IsNotNull(result);
                        Assert.AreEqual(userId, result.UserId);
                    });
                }));
                
            });
        });
    }
}