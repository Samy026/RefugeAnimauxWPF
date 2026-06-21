namespace RefugeAnimaux.Metier;

public class Animal
{
    public string Identifiant { get; set; } = "";
    public string Nom { get; set; } = "";
    public string Type { get; set; } = "";
    public string Sexe { get; set; } = "";
    public string? Particularites { get; set; }
    public DateTime? DateDeces { get; set; }
    public string? Description { get; set; }
    public DateTime? DateSterilisation { get; set; }
    public bool Sterilise { get; set; }
    public DateTime DateNaissance { get; set; }

    public override string ToString()
        => $"{Identifiant} - {Nom} - {Type} - {Sexe} - né le {DateNaissance:yyyy-MM-dd}";
}
