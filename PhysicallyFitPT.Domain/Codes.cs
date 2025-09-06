namespace PhysicallyFitPT.Domain;

public class CptCode : Entity
{
  public string Code { get; set; } = null!;
  public string Description { get; set; } = null!;
}

public class Icd10Code : Entity
{
  public string Code { get; set; } = null!;
  public string Description { get; set; } = null!;
}
