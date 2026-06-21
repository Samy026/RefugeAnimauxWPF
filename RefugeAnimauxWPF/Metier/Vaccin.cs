namespace RefugeAnimaux.Metier;
public class Vaccin { public int Identifiant {get;set;} public string Nom {get;set;}=""; public override string ToString()=> $"{Identifiant} - {Nom}"; }
