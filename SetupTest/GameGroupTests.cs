using Setup.Models;

namespace SetupTest;

public class Tests
{
    private const string OWNER = "myConnectionId";

    [Test]
    [TestCase(1, 1)]
    [TestCase(2, 4)]
    public void GameGroup_CreateGameBoard_CheckInitialSize(int size, int expectedResult)
    {
        var gameBoard = CreateGameBoard(size);
        Assert.That(gameBoard.Length * gameBoard[0].Length, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GameGroup_CreateGameBoard_CheckInitialType()
    {
        var gameBoard = CreateGameBoard(2);
        foreach (var line in gameBoard)
        foreach (var item in line)
            Assert.That(item.BuildingType, Is.EqualTo(BuildingType.Grass));
    }

    [Test]
    [TestCase(BuildingType.Grass, "An unexpected error ocurred.")]
    [TestCase(BuildingType.Street, "placed!")]
    [TestCase(BuildingType.EnergyLarge, "placed!")]
    [TestCase(BuildingType.EnergySmall, "placed!")]
    [TestCase(BuildingType.School, "School should be connected to an energy source.")]
    [TestCase(BuildingType.Factory, "Factory should be connected to an energy source.")]
    [TestCase(BuildingType.Farm, "placed!")]
    [TestCase(BuildingType.Cinema, "Cinema should be placed next to a Street.")]
    [TestCase(BuildingType.House, "House should be placed next to a Street.")]
    public void GameGroup_TryPlaceBuilding_AllowedToPlaceBuilding(BuildingType buildingType, string expectedResult)
    {
        var gameGroup = CreateGameGroup();
        var placed = gameGroup.TryPlaceBuilding(0, 0, buildingType, OWNER);
        Assert.Multiple(() =>
        {
            Assert.That(gameGroup.GameBoard[0][0].BuildingType,
                Is.EqualTo(placed.Item1 ? buildingType : BuildingType.Grass));
            Assert.That(expectedResult, Is.EqualTo(placed.Item2));
        });
    }

    private static BuildingInfo[][] CreateGameBoard(int size)
    {
        //Act
        var gameBoard = GameGroup.CreateGameBoard(size);
        return gameBoard;
    }

    private static GameGroup CreateGameGroup()
    {
        //Arrange
        var name = "myName";
        var connectionId = OWNER;
        UserModel owner = new(connectionId, name);
        var key = "myKey";
        return new GameGroup(key, owner);
    }
}