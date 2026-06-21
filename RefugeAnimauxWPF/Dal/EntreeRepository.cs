using Npgsql;
using RefugeAnimaux.Metier;

namespace RefugeAnimaux.Dal;

public class EntreeRepository
{
    public void Ajouter(Entree entree)
    {
        using var cnx = Database.GetConnection();
        const string sql = @"
            INSERT INTO ani_entree(raison, date_entree, ani_identifiant, entree_contact)
            VALUES (@raison, @date, @animal, @contact);";
        using var cmd = new NpgsqlCommand(sql, cnx);
        cmd.Parameters.AddWithValue("raison", entree.Raison);
        cmd.Parameters.AddWithValue("date", entree.DateEntree);
        cmd.Parameters.AddWithValue("animal", entree.AniIdentifiant);
        cmd.Parameters.AddWithValue("contact", entree.EntreeContact);
        cmd.ExecuteNonQuery();
    }
    public List<string> ListerEntrees()
    {
        using var cnx = Database.GetConnection();

        using var cmd = new NpgsqlCommand(@"
        SELECT raison, date_entree, ani_identifiant, entree_contact
        FROM ani_entree
        ORDER BY date_entree", cnx);

        using var r = cmd.ExecuteReader();

        var liste = new List<string>();

        while (r.Read())
        {
            liste.Add($"Animal {r["ani_identifiant"]} - Contact {r["entree_contact"]} - Raison {r["raison"]} - Date {r["date_entree"]}");
        }

        return liste;
    }
}
