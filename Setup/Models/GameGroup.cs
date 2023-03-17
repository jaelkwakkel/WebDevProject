namespace Setup.Models;

public class GameGroup
{
    public GameGroup(string key, UserModel owner)
    {
        HasStarted = false;
        HasFinished = false;
        Key = key;
        Owner = owner;
        UserTurn = owner;
        //TODO: C: allow multiple gameboard-sizes
        GameBoard = CreateGameBoard(10);
    }

    public bool HasStarted { get; set; }
    public bool HasFinished { get; set; }
    public string Key { get; set; }

    public UserModel Owner { get; set; }
    public UserModel UserTurn { get; set; }
    public List<UserModel> Users { get; set; } = new();
    private BuildingType[,] GameBoard { get; }

    private BuildingType[,] CreateGameBoard(int size)
    {
        var gameBoard = new BuildingType[size, size];
        for (var i = 0; i < gameBoard.GetLength(0); i++)
        for (var j = 0; j < gameBoard.GetLength(1); j++)
            gameBoard[i, j] = BuildingType.Grass;

        return gameBoard;
    }

    //Return if the tile has been placed, with an optional error/warning message
    public (bool, string) TryPlaceBuilding(int x, int y, BuildingType buildingType)
    {
        if (GameBoard[x, y] != BuildingType.Grass) return (false, "Tile not empty");
        if (buildingType == BuildingType.Grass) return (false, "Can not place grass");
        if (x < 0 || x > GameBoard.GetLength(0) || y < 0 || y > GameBoard.GetLength(1))
            return (false, "Tile falls outside board");

        GameBoard[x, y] = buildingType;

        return (true, "placed!");
    }

    public BuildingType StringToBuildingType(string buildingString)
    {
        return buildingString switch
        {
            "grass" => BuildingType.Grass,
            "street" => BuildingType.Street,
            "house" => BuildingType.House,
            "farm" => BuildingType.Farm,
            "cinema" => BuildingType.Cinema,
            "energy_small" => BuildingType.EnergySmall,
            "energy_large" => BuildingType.EnergyLarge,
            "school" => BuildingType.School,
            "factory" => BuildingType.Factory,
            _ => BuildingType.Grass
        };
    }

    public string BuildingTypetoString(BuildingType buildingType)
    {
        return buildingType switch
        {
            BuildingType.Grass => "grass",
            BuildingType.Street => "street",
            BuildingType.House => "house",
            BuildingType.Farm => "house",
            BuildingType.Cinema => "cinema",
            BuildingType.EnergySmall => "energy_small",
            BuildingType.EnergyLarge => "energy_large",
            BuildingType.School => "school",
            BuildingType.Factory => "factory",
            _ => "grass"
        };
    }
}