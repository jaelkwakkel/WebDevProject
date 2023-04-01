// using Microsoft.AspNetCore.Identity;
// using Moq;
// using NUnit.Framework.Internal;
// using Setup.Areas.Identity.Data;
// using Setup.Hubs;
//
// namespace SetupTest;
//
// internal class MockTest
// {
//     private const string NAME = "myUserName";
//     public const string USERID = "myId";
//
//     [Test]
//     public async Task TestNameAsync()
//     {
//         var userManager = new Mock<UserManager<SetupUser>>();
//         var context = new Mock<SetupContext>();
//         var gamehub = new Mock<GameHub>();
//
//         List<SetupUser> setupUsers = new()
//         {
//             new SetupUser()
//             {
//                 Id = USERID,
//                 UserName = NAME
//             }
//         };
//
//         IQueryable<SetupUser> setupUserQuery = setupUsers.AsQueryable();
//
//         userManager.Setup(p => p.Users).Returns(setupUserQuery);
//         gamehub.Setup(p => p.Context.ConnectionId = USERID).Returns(setupUserQuery);
//
//         //Act
//         hub.
//
//             //Assert
//             Assert.AreEqual(,);
//
//         //var gameHub = new GameHub(userManager.Object, context.Object);
//
//         //gameHub.Start();
//         //gameHub.SaveFinishedGameToAccount();
//         //userManager.Setup(u => u.GetUserName()).Returns(NAME);
//     }
// }

