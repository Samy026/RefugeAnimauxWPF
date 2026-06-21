namespace RefugeAnimaux.Metier;

public class Contact
{
    public int ContactIdentifiant { get; set; }
    public string Nom { get; set; } = "";
    public string Prenom { get; set; } = "";
    public string? RegistreNational { get; set; }
    public string? Rue { get; set; }
    public string? Cp { get; set; }
    public string? Localite { get; set; }
    public string? Gsm { get; set; }
    public string? Telephone { get; set; }
    public string? Email { get; set; }

    public override string ToString()
        => $"{ContactIdentifiant} - {Nom} {Prenom} - GSM:{Gsm} Tel:{Telephone} Email:{Email}";
}
