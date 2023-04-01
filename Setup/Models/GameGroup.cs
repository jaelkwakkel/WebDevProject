using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static System.Math;

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
    public string Key { get; }

    public UserModel Owner { get; }
    public UserModel UserTurn { get; set; }
    public List<UserModel> Users { get; } = new();

    [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
    public BuildingInfo[][] GameBoard { get; }

    public static BuildingInfo[][] CreateGameBoard(int size)
    {
        var gameBoard = new BuildingInfo[size][];
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
        return JsonConvert.SerializeObject(GameBoard);
    }

    public string GetUsersAsJson()
    {
        var userInfo = new List<UserInfo>();
        foreach (var user in Users) userInfo.Add(new UserInfo(user.Name, user.Score.ToString()));
        return JsonConvert.SerializeObject(userInfo);
    }

    //Return if the tile has been placed, with an optional error/warning message
    public (bool, string) TryPlaceBuilding(int x, int y, BuildingType buildingType, string owner)
    {
        //Gebouwen mogen alleen op lege plekken geplaatst worden
        if (GameBoard[x][y].BuildingType != BuildingType.Grass) return (false, "An unexpected error ocurred.");
        //Gras mag niet geplaatst worden
        if (buildingType == BuildingType.Grass) return (false, "An unexpected error ocurred.");
        //Gebouwen moeten op een geldige positie geplaatst worden
        if (x < 0 || x > GameBoard.Length || y < 0 || y > GameBoard.Length)
            return (false, "Not a board position.");

        //Een aantal gebouwen moeten naast een straat geplaatst worden
        if (buildingType is BuildingType.House or BuildingType.Cinema)
            if (GetAdjecentBuildingTypes(x, y, 1).All(type => type.BuildingType != BuildingType.Street))
                return (false, $"{BuildingTypetoString(buildingType)} should be placed next to a Street.");

        //Een aantal gebouwen moeten in de buurt van een energie-bron geplaatst worden
        if (buildingType is BuildingType.Factory or BuildingType.School or BuildingType.Cinema)
            if (!(GetAdjecentBuildingTypes(x, y, 1)
                      .Any(type => type.BuildingType is BuildingType.EnergySmall or BuildingType.EnergyLarge) ||
                  GetAdjecentBuildingTypes(x, y, 2).Any(type => type.BuildingType == BuildingType.EnergyLarge)))
                return (false, $"{BuildingTypetoString(buildingType)} should be connected to an energy source.");

        GameBoard[x][y].BuildingType = buildingType;
        GameBoard[x][y].Owner = owner;

        return (true, "placed!");
    }

    public bool CheckGameFinished()
    {
        foreach (var buildingInfo in GameBoard)
            for (var y = 0; y < GameBoard[0].Length; y++)
                if (buildingInfo[y].BuildingType == BuildingType.Grass)
                    return false;

        return true;
    }

    public int CalculateScore(string owner)
    {
        var score = 0;
        for (var x = 0; x < GameBoard.Length; x++)
        for (var y = 0; y < GameBoard[0].Length; y++)
        {
            if (GameBoard[x][y].Owner != owner) continue;
            //Calculate score
            switch (GameBoard[x][y].BuildingType)
            {
                case BuildingType.Grass:
                    continue;
                case BuildingType.Street:
                    continue;
                case BuildingType.House:
                    score++;
                    score += GetAdjecentBuildingTypes(x, y, 2)
                        .Count(buildingInfo => buildingInfo.BuildingType == BuildingType.School);
                    continue;
                case BuildingType.Farm:
                    score += GetAdjecentBuildingTypes(x, y, 1)
                        .Count(buildingInfo => buildingInfo.BuildingType == BuildingType.Grass);
                    continue;
                case BuildingType.Cinema:
                    score += GetAdjecentBuildingTypes(x, y, 1)
                        .Count(buildingInfo => buildingInfo.BuildingType == BuildingType.House);
                    continue;
                case BuildingType.EnergySmall:
                    score += GetAdjecentBuildingTypes(x, y, 1).Count(buildingInfo =>
                        buildingInfo.BuildingType is BuildingType.Cinema or BuildingType.Factory
                            or BuildingType.School);
                    continue;
                case BuildingType.EnergyLarge:
                    score += GetAdjecentBuildingTypes(x, y, 2).Count(buildingInfo =>
                        buildingInfo.BuildingType is BuildingType.Cinema or BuildingType.Factory
                            or BuildingType.School);
                    continue;
                case BuildingType.School:
                    //Score is calculated at house
                    continue;
                case BuildingType.Factory:
                    score += GetAdjecentBuildingTypes(x, y, 10).Count(buildingInfo =>
                        buildingInfo.BuildingType is BuildingType.Cinema or BuildingType.School) * 3;
                    continue;
                default:
                    Console.WriteLine("Als je dit op de console ziet is er iets flink misgegaan ^O^");
                    break;
            }
        }

        return score;
    }

    private List<BuildingInfo> GetAdjecentBuildingTypes(int cellX, int cellY, int range)
    {
        List<BuildingInfo> buildings = new();

        var minX = Max(cellX - range, GameBoard.GetLowerBound(0));
        var maxX = Min(cellX + range, GameBoard.GetUpperBound(0));
        var minY = Max(cellY - range, GameBoard[0].GetLowerBound(0));
        var maxY = Min(cellY + range, GameBoard[0].GetUpperBound(0));

        for (var x = minX; x <= maxX; x++)
        for (var y = minY; y <= maxY; y++)
            buildings.Add(GameBoard[x][y]);
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

    private string BuildingTypetoString(BuildingType buildingType)
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

    public int GetPriceOfBuilding(BuildingType buildingType)
    {
        return buildingType switch
        {
            BuildingType.Grass => 0,
            BuildingType.Street => 1,
            BuildingType.House => 4,
            BuildingType.Farm => 9,
            BuildingType.Cinema => 12,
            BuildingType.EnergySmall => 3,
            BuildingType.EnergyLarge => 6,
            BuildingType.School => 14,
            BuildingType.Factory => 15,
            _ => 0
        };
    }
}

public class UserInfo
{
    public UserInfo(string name, string score)
    {
        Name = name;
        Score = score;
    }

    public string Name { get; set; }
    public string Score { get; set; }
}