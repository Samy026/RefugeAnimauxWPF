using Npgsql;

namespace RefugeAnimaux.Dal;

public class VaccinRepository
{
    public int AjouterVaccinSiAbsent(string nom)
    {
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand(@"
            INSERT INTO vaccin(nom) VALUES (@nom)
            ON CONFLICT (nom) DO UPDATE SET nom = EXCLUDED.nom
            RETURNING identifiant;", cnx);
        cmd.Parameters.AddWithValue("nom", nom);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void AjouterVaccination(string animalId, string nomVaccin, DateTime dateVaccination)
    {
        var idVaccin = AjouterVaccinSiAbsent(nomVaccin);
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand(@"
            INSERT INTO vaccination(vaccination_date, vac_animal, id_vaccin)
            VALUES (@date, @animal, @vaccin);", cnx);
        cmd.Parameters.AddWithValue("date", dateVaccination);
        cmd.Parameters.AddWithValue("animal", animalId);
        cmd.Parameters.AddWithValue("vaccin", idVaccin);
        cmd.ExecuteNonQuery();
    }
    public List<string> ListerVaccinations()
    {
        using var cnx = Database.GetConnection();

        using var cmd = new NpgsqlCommand(@"
        SELECT a.nom AS animal_nom, v.nom AS vaccin_nom, va.vaccination_date
        FROM vaccination va
        JOIN animal a ON a.identifiant = va.vac_animal
        JOIN vaccin v ON v.identifiant = va.id_vaccin
        ORDER BY va.vaccination_date", cnx);

        using var r = cmd.ExecuteReader();

        var liste = new List<string>();

        while (r.Read())
        {
            liste.Add(
                $"Animal : {r["animal_nom"]} - Vaccin : {r["vaccin_nom"]} - Date : {r["vaccination_date"]}"
            );
        }

        return liste;
    }
}
