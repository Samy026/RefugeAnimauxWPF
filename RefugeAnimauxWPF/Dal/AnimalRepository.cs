using Npgsql;
using RefugeAnimaux.Metier;

namespace RefugeAnimaux.Dal;

public class AnimalRepository
{
    public void Ajouter(Animal animal)
    {
        using var cnx = Database.GetConnection();
        const string sql = @"
            INSERT INTO animal(identifiant, nom, type, sexe, particularites, date_deces, description, date_sterilisation, sterilise, date_naissance)
            VALUES (@id, @nom, @type, @sexe, @part, @deces, @desc, @sterDate, @ster, @naiss);";
        using var cmd = new NpgsqlCommand(sql, cnx);
        cmd.Parameters.AddWithValue("id", animal.Identifiant);
        cmd.Parameters.AddWithValue("nom", animal.Nom);
        cmd.Parameters.AddWithValue("type", animal.Type);
        cmd.Parameters.AddWithValue("sexe", animal.Sexe);
        cmd.Parameters.AddWithValue("part", (object?)animal.Particularites ?? DBNull.Value);
        cmd.Parameters.AddWithValue("deces", (object?)animal.DateDeces ?? DBNull.Value);
        cmd.Parameters.AddWithValue("desc", (object?)animal.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("sterDate", (object?)animal.DateSterilisation ?? DBNull.Value);
        cmd.Parameters.AddWithValue("ster", animal.Sterilise);
        cmd.Parameters.AddWithValue("naiss", animal.DateNaissance);
        cmd.ExecuteNonQuery();
    }

    public List<Animal> ListerTous()
    {
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand("SELECT * FROM animal ORDER BY identifiant", cnx);
        using var r = cmd.ExecuteReader();
        var animaux = new List<Animal>();
        while (r.Read()) animaux.Add(LireAnimal(r));
        return animaux;
    }

    public Animal? Consulter(string id)
    {
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand("SELECT * FROM animal WHERE identifiant=@id", cnx);
        cmd.Parameters.AddWithValue("id", id);
        using var r = cmd.ExecuteReader();
        return r.Read() ? LireAnimal(r) : null;
    }

    public void Supprimer(string id)
    {
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand("DELETE FROM animal WHERE identifiant=@id", cnx);
        cmd.Parameters.AddWithValue("id", id);
        cmd.ExecuteNonQuery();
    }
    public void ModifierContact(int id, string adresse, string gsm, string telephone, string email)
    {
        using var cnx = Database.GetConnection();

        string sql = @"
            UPDATE contact
            SET rue = @adresse,
                gsm = @gsm,
                telephone = @telephone,
                email = @email
            WHERE contact_identifiant = @id";

        using var cmd = new NpgsqlCommand(sql, cnx);

        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("adresse", adresse);
        cmd.Parameters.AddWithValue("gsm", gsm);
        cmd.Parameters.AddWithValue("telephone", telephone);
        cmd.Parameters.AddWithValue("email", email);

        cmd.ExecuteNonQuery();
    }

    public void AjouterFamilleAccueil(string animalId, int contactId, DateOnly dateDebut)
    {
        using var cnx = Database.GetConnection();

        string sql = @"
            INSERT INTO famille_accueil
            (date_debut, fa_ani_identifiant, fa_contact)
            VALUES
            (@dateDebut, @animalId, @contactId)";

        using var cmd = new NpgsqlCommand(sql, cnx);

        cmd.Parameters.AddWithValue("dateDebut", dateDebut);
        cmd.Parameters.AddWithValue("animalId", animalId);
        cmd.Parameters.AddWithValue("contactId", contactId);

        cmd.ExecuteNonQuery();
    }

    public void AjouterAdoption(string animalId, int contactId, DateOnly dateDemande)
    {
        using var cnx = Database.GetConnection();

        string sql = @"
            INSERT INTO adoption
            (statut, date_demande, ani_identifiant, adop_contact)
            VALUES
            ('demande', @dateDemande, @animalId, @contactId)";

        using var cmd = new NpgsqlCommand(sql, cnx);

        cmd.Parameters.AddWithValue("dateDemande", dateDemande);
        cmd.Parameters.AddWithValue("animalId", animalId);
        cmd.Parameters.AddWithValue("contactId", contactId);

        cmd.ExecuteNonQuery();
    }

    public void ModifierStatutAdoption(int idAdoption, string statut)
    {
        using var cnx = Database.GetConnection();

        var sql = @"
            UPDATE adoption
            SET statut = @statut
            WHERE adoption_id = @idAdoption";

        using var cmd = new NpgsqlCommand(sql, cnx);

        cmd.Parameters.AddWithValue("statut", statut);
        cmd.Parameters.AddWithValue("idAdoption", idAdoption);

        cmd.ExecuteNonQuery();
    }
    public List<int> ListerIdsAdoption()
    {
        using var cnx = Database.GetConnection();

        using var cmd = new NpgsqlCommand(
            "SELECT adoption_id FROM adoption ORDER BY adoption_id",
            cnx
        );

        using var r = cmd.ExecuteReader();

        var ids = new List<int>();

        while (r.Read())
            ids.Add(r.GetInt32(0));

        return ids;
    }

    public void ListerFamillesAccueilAnimal(string animalId)
    {
        using var cnx = Database.GetConnection();

        string sql = @"
            SELECT fa_contact, date_debut, date_fin
            FROM famille_accueil
            WHERE fa_ani_identifiant = @animalId";

        using var cmd = new NpgsqlCommand(sql, cnx);

        cmd.Parameters.AddWithValue("animalId", animalId);

        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            int contact = reader.GetInt32(0);

            DateOnly dateDebut =
                reader.GetFieldValue<DateOnly>(1);

            string dateFin = reader.IsDBNull(2)
                ? "En cours"
                : reader.GetFieldValue<DateOnly>(2).ToString();

            Console.WriteLine(
                $"Contact : {contact} | Début : {dateDebut} | Fin : {dateFin}"
            );
        }
    }
    public bool AnimalDejaAdopte(string animalId)
    {
        using var cnx = Database.GetConnection();

        using var cmd = new NpgsqlCommand(@"
        SELECT COUNT(*) 
        FROM adoption
        WHERE ani_identifiant = @animalId
        AND statut = 'acceptee'", cnx);

        cmd.Parameters.AddWithValue("animalId", animalId);

        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public bool AnimalEnFamilleAccueil(string animalId)
    {
        using var cnx = Database.GetConnection();

        using var cmd = new NpgsqlCommand(@"
        SELECT COUNT(*) 
        FROM famille_accueil
        WHERE fa_ani_identifiant = @animalId
        AND date_fin IS NULL", cnx);

        cmd.Parameters.AddWithValue("animalId", animalId);

        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public void AjouterSortie(
        string animalId,
        int contactId,
        string raison,
        DateOnly dateSortie
    )
    {
        using var cnx = Database.GetConnection();

        string sql = @"
            INSERT INTO ani_sortie
            (raison, date_sortie, ani_identifiant, sortie_contact)
            VALUES
            (@raison, @dateSortie, @animalId, @contactId)";

        using var cmd = new NpgsqlCommand(sql, cnx);

        cmd.Parameters.AddWithValue("raison", raison);
        cmd.Parameters.AddWithValue("dateSortie", dateSortie);
        cmd.Parameters.AddWithValue("animalId", animalId);
        cmd.Parameters.AddWithValue("contactId", contactId);

        cmd.ExecuteNonQuery();
    }

    public void AjouterCompatibilite(
        string animalId,
        int idCompatibilite,
        string valeur
    )
    {
        using var cnx = Database.GetConnection();

        string sql = @"
            INSERT INTO ani_compatibilite
            (ani_identifiant, comp_identifiant, valeur)
            VALUES
            (@animalId, @compIdentifiant, @valeur)";

        using var cmd = new NpgsqlCommand(sql, cnx);

        cmd.Parameters.AddWithValue("animalId", animalId);

        cmd.Parameters.AddWithValue(
            "compIdentifiant",
            idCompatibilite
        );

        cmd.Parameters.AddWithValue("valeur", valeur);

        cmd.ExecuteNonQuery();
    }
    public List<string> ListerFamillesAccueil()
    {
        using var cnx = Database.GetConnection();

        using var cmd = new NpgsqlCommand(@"
        SELECT accueil_id, fa_ani_identifiant, fa_contact, date_debut, date_fin
        FROM famille_accueil
        ORDER BY accueil_id", cnx);

        using var r = cmd.ExecuteReader();

        var liste = new List<string>();

        while (r.Read())
        {
            string dateFin = r["date_fin"] == DBNull.Value
                ? "En cours"
                : r["date_fin"]?.ToString() ?? "";

            liste.Add(
                $"FA {r["accueil_id"]} - Animal {r["fa_ani_identifiant"]} - Contact {r["fa_contact"]} - Début {r["date_debut"]} - Fin {dateFin}"
            );
        }

        return liste;
    }

    public List<string> ListerAdoptions()
    {
        using var cnx = Database.GetConnection();

        using var cmd = new NpgsqlCommand(@"
        SELECT adoption_id, statut, date_demande, ani_identifiant, adop_contact
        FROM adoption
        ORDER BY adoption_id", cnx);

        using var r = cmd.ExecuteReader();

        var liste = new List<string>();

        while (r.Read())
        {
            liste.Add(
                $"Adoption {r["adoption_id"]} - Animal {r["ani_identifiant"]} - Contact {r["adop_contact"]} - Statut {r["statut"]} - Date {r["date_demande"]}"
            );
        }

        return liste;
    }
    public List<string> ListerSorties()
    {
        using var cnx = Database.GetConnection();

        using var cmd = new NpgsqlCommand(@"
        SELECT raison, date_sortie, ani_identifiant, sortie_contact
        FROM ani_sortie
        ORDER BY date_sortie", cnx);

        using var r = cmd.ExecuteReader();

        var liste = new List<string>();

        while (r.Read())
        {
            liste.Add($"Animal {r["ani_identifiant"]} - Contact {r["sortie_contact"]} - Raison {r["raison"]} - Date {r["date_sortie"]}");
        }

        return liste;
    }
    public List<string> ListerCompatibilites()
    {
        using var cnx = Database.GetConnection();

        using var cmd = new NpgsqlCommand(@"
        SELECT a.nom AS animal_nom,
               c.type AS compatibilite_nom,
               ac.valeur
        FROM ani_compatibilite ac
        JOIN animal a ON a.identifiant = ac.ani_identifiant
        JOIN compatibilite c ON c.identifiant = ac.comp_identifiant
        ORDER BY a.nom", cnx);

        using var r = cmd.ExecuteReader();

        var liste = new List<string>();

        while (r.Read())
        {
            liste.Add(
                $"Animal : {r["animal_nom"]} - Compatibilité : {r["compatibilite_nom"]} - Valeur : {r["valeur"]}"
            );
        }

        return liste;
    }

    private static Animal LireAnimal(NpgsqlDataReader r) => new()
    {
        Identifiant = r.GetString(r.GetOrdinal("identifiant")),
        Nom = r.GetString(r.GetOrdinal("nom")),
        Type = r.GetString(r.GetOrdinal("type")),
        Sexe = r.GetString(r.GetOrdinal("sexe")),
        Particularites = r["particularites"] as string,
        DateDeces = r["date_deces"] == DBNull.Value ? null : r.GetFieldValue<DateOnly>(r.GetOrdinal("date_deces")).ToDateTime(TimeOnly.MinValue),
        Description = r["description"] as string,
        DateSterilisation = r["date_sterilisation"] == DBNull.Value ? null : r.GetFieldValue<DateOnly>(r.GetOrdinal("date_sterilisation")).ToDateTime(TimeOnly.MinValue),
        Sterilise = r.GetBoolean(r.GetOrdinal("sterilise")),
        DateNaissance = r.GetFieldValue<DateOnly>(r.GetOrdinal("date_naissance")).ToDateTime(TimeOnly.MinValue)
    };
}
