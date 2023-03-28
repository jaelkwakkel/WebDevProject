namespace Setup.Models;

public enum BuildingType
{
    Grass,
    Street,
    House,
    Farm,
    Cinema,
    EnergySmall,
    EnergyLarge,
    School,
    Factory
}

public class BuildingInfo
{
    public BuildingType BuildingType { get; set; }
    public string? Owner;

    public override string ToString()
    {
        return "Building " + BuildingType + " is owned by " + Owner;
    }

    public BuildingInfo()
    {
        BuildingType = BuildingType.Grass;
        Owner = null;
    }
}