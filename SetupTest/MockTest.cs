using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework.Internal;
using Setup.Areas.Identity.Data;
using Setup.Hubs;
using Setup.Models;
using System.Security.Claims;
using System.Security.Principal;

namespace SetupTest;

internal class GameHubTests
{
    private const string NAME = "myName";
    private const string KEY = "myKeyMoreThen6LessThen25";
    private readonly string CONNECTION_ID = Guid.NewGuid().ToString();
    private IReadOnlyList<object> callerArguments = null;

    [Test]
    public async Task GameHub_LifeCycle()
    {
        GameHub gameHub = SetupGameHubWithMocks();

        await gameHub.OnConnectedAsync();
        AssertCaller("HideNameInput");

        await gameHub.CreateOrJoin(KEY, "myName");
        AssertCaller("JoinedGroup", KEY);
        GameGroup game = gameHub.GetGame(KEY);

        await gameHub.Start();
        Assert.That(game.HasStarted, Is.EqualTo(true));
        Assert.That(game.HasFinished, Is.EqualTo(false));

        string move = "{\"BuildingType\":1,\"XPosition\":0,\"YPosition\":0}";
        await gameHub.PlacedBuilding(move);
        Assert.That(game.HasFinished, Is.EqualTo(true));

        // TODO: mock usermanager
        // await gameHub.SaveFinishedGameToAccount();
        // assertCaller("GamePlayError", "Can not save the game.");
    }


    private GameHub SetupGameHubWithMocks()
    {
        GameGroup.gameBoardSize = 1;
        GameHub gameHub = new(null, null);

        Mock<IHubCallerClients> clients = new();
        Mock<IClientProxy> mockGroup = new();
        clients.Setup(cs => cs.Group(KEY)).Returns(mockGroup.Object);

        Mock<IClientProxy> caller = new();
        caller.Setup(_ => _.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .Callback(new InvocationAction(i => { callerArguments = i.Arguments; }));

        clients.Setup(cs => cs.Caller).Returns(caller.Object);

        clients.Setup(cs => cs.Client(CONNECTION_ID)).Returns(caller.Object);
        gameHub.Clients = clients.Object;

        Mock<HubCallerContext> clientContext = new();
        clientContext.Setup(c => c.ConnectionId).Returns(CONNECTION_ID);

        Mock<IIdentity> identity = new();
        identity.Setup(i => i.Name).Returns(NAME);
        Mock<ClaimsPrincipal> user = new();
        user.Setup(u => u.Identity).Returns(identity.Object);
        clientContext.Setup(c => c.User).Returns(user.Object);

        gameHub.Context = clientContext.Object;

        Mock<IGroupManager> groups = new();
        gameHub.Groups = groups.Object;

        Mock<SetupContext> context = new();
        return gameHub;
    }

    private void AssertCaller(string expectedMethod)
    {
        AssertCaller(expectedMethod, new object[0]);
    }

    private void AssertCaller(string expectedMethod, params object[] expectedArgs)
    {
        Assert.That((string)callerArguments[0], Is.EqualTo(expectedMethod));
        Assert.That((object[])callerArguments[1], Is.EqualTo(expectedArgs));
    }


}

