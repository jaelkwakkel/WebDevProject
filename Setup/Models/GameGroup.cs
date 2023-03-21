using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text.Json;

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
    [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
    public BuildingInfo[][] GameBoard { get; }

    private BuildingInfo[][] CreateGameBoard(int size)
    {
        BuildingInfo[][] gameBoard = new BuildingInfo[size][];
        for (var i = 0; i < size; i++)
        {
            gameBoard[i] = new BuildingInfo[size];
            for (var j = 0; j < size; j++)
                gameBoard[i][j] = new BuildingInfo();

        }

        return gameBoard;
    }

    public string GetBoardAsJsonString()
    {
        Console.WriteLine(JsonConvert.SerializeObject(GameBoard));
        //BuildingType[][] gameBoard;

        return JsonConvert.SerializeObject(GameBoard);
        //return JsonConvert.SerializeObject(GameBoard);
        //string[,] buildings = new string[GameBoard.GetLength(0), GameBoard.GetLength(1)];
        //for (int i = 0; i < GameBoard.GetLength(0); i++)
        //{
        //    for (int j = 0; j < GameBoard.GetLength(1); j++)
        //    {
        //        buildings[i, j] = BuildingTypetoString(GameBoard[i, j]);
        //    }
        //}

        //return JsonConvert.SerializeObject(buildings);
    }

    //public static string SerializeWithStringEnum(object obj)
    //{
    //    var options = new JsonSerializerOptions();
    //    options.Converters.Add(new JsonStringEnumConverter());
    //    return System.Text.Json.JsonSerializer.Serialize(obj, options);
    //}

    //Return if the tile has been placed, with an optional error/warning message
    public (bool, string) TryPlaceBuilding(int x, int y, BuildingType buildingType)
    {
        if (GameBoard[x][y].BuildingType != BuildingType.Grass) return (false, "Tile not empty.");
        if (buildingType == BuildingType.Grass) return (false, "Can not place Grass.");
        if (x < 0 || x > GameBoard.Length || y < 0 || y > GameBoard.Length)
            return (false, "Not a board position.");

        GetAdjecentBuildingTypes(x, y, 1).ForEach(Console.WriteLine);

        //Buildingtypes needing to be placed next to a Street.
        if (buildingType is BuildingType.House or BuildingType.Cinema)
            if (GetAdjecentBuildingTypes(x, y, 1).All(type => type.BuildingType != BuildingType.Street))
                return (false, $"{BuildingTypetoString(buildingType)} should be placed next to a Street.");

        //BuildingTypes needing to be connected to a energy source
        if (buildingType is BuildingType.Factory or BuildingType.School or BuildingType.Cinema)
            if (!(GetAdjecentBuildingTypes(x, y, 1)
                      .Any(type => type.BuildingType is BuildingType.EnergySmall or BuildingType.EnergyLarge) ||
                  GetAdjecentBuildingTypes(x, y, 2).Any(type => type.BuildingType == BuildingType.EnergyLarge)))
                return (false, $"{BuildingTypetoString(buildingType)} should be connected to an energy source.");

        GameBoard[x][y].BuildingType = buildingType;

        return (true, "placed!");
    }

    //private List<BuildingInfo> GetAdjecentBuildingTypes(int x, int y, int range)
    //{
    //    List<BuildingInfo> buildings = new();
    //    for (var i = x - range; i < x + range; i++)
    //        for (var j = y - range; j < y + range; j++)
    //        {
    //            if (i != x || j != y)
    //            {
    //                Console.WriteLine("[" + i + "," + j + "]");
    //                buildings.Add(GameBoard[i][j]);
    //            }
    //        }

    //    return buildings;
    //}

    public List<BuildingInfo> GetAdjecentBuildingTypes(int CellX, int CellY, int range)
    {
        List<BuildingInfo> buildings = new();

        int minX = Math.Max(CellX - range, GameBoard.GetLowerBound(0));
        int maxX = Math.Min(CellX + range, GameBoard.GetUpperBound(0));
        int minY = Math.Max(CellY - range, GameBoard[0].GetLowerBound(0));
        int maxY = Math.Min(CellY + range, GameBoard[0].GetUpperBound(0));

        //System.Diagnostics.Debug.WriteLine(string.Format("[{0},{1}] : [{2},{3}] - [{4},{5}]",
        //CellX, CellY, minX, maxX, minY, maxY), "NeighbourCount");

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                buildings.Add(GameBoard[x][y]);
            }
        }
        return buildings;
    }

    public BuildingType StringToBuildingType(string buildingString)
    {
        return buildingString switch
        {
            "Grass" => BuildingType.Grass,
            "Street" => BuildingType.Street,
            "House" => BuildingType.House,
            "Farm" => BuildingType.Farm,
            "Cinema" => BuildingType.Cinema,
            "EnergySmall" => BuildingType.EnergySmall,
            "EnergyLarge" => BuildingType.EnergyLarge,
            "School" => BuildingType.School,
            "Factory" => BuildingType.Factory,
            _ => BuildingType.Grass
        };
    }

    public string BuildingTypetoString(BuildingType buildingType)
    {
        return buildingType switch
        {
            BuildingType.Grass => "Grass",
            BuildingType.Street => "Street",
            BuildingType.House => "House",
            BuildingType.Farm => "Farm",
            BuildingType.Cinema => "Cinema",
            BuildingType.EnergySmall => "EnergySmall",
            BuildingType.EnergyLarge => "EnergyLarge",
            BuildingType.School => "School",
            BuildingType.Factory => "Factory",
            _ => "Grass"
        };
    }
}