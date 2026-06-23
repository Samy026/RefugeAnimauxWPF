using Npgsql;
using RefugeAnimaux.Metier;

namespace RefugeAnimaux.Dal;

public class AnimalRepository
{
    public void Ajouter(Animal animal)
    {
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand(@"
            CALL sp_ajouter_animal(
                @id, @nom, @type, @sexe, @part, @deces, @desc, @sterDate, @ster, @naiss
            );", cnx);

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
        using var cmd = new NpgsqlCommand("SELECT * FROM fn_lister_animaux();", cnx);
        using var r = cmd.ExecuteReader();

        var animaux = new List<Animal>();
        while (r.Read()) animaux.Add(LireAnimal(r));
        return animaux;
    }

    public Animal? Consulter(string id)
    {
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand("SELECT * FROM fn_consulter_animal(@id);", cnx);
        cmd.Parameters.AddWithValue("id", id);

        using var r = cmd.ExecuteReader();
        return r.Read() ? LireAnimal(r) : null;
    }

    public void Supprimer(string id)
    {
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand("CALL sp_supprimer_animal(@id);", cnx);
        cmd.Parameters.AddWithValue("id", id);
        cmd.ExecuteNonQuery();
    }

    public void ModifierContact(int id, string adresse, string gsm, string telephone, string email)
    {
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand("CALL sp_modifier_contact(@id, @adresse, @gsm, @telephone, @email);", cnx);

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
        using var cmd = new NpgsqlCommand("CALL sp_ajouter_famille_accueil(@animalId, @contactId, @dateDebut);", cnx);

        cmd.Parameters.AddWithValue("animalId", animalId);
        cmd.Parameters.AddWithValue("contactId", contactId);
        cmd.Parameters.AddWithValue("dateDebut", dateDebut);
        cmd.ExecuteNonQuery();
    }

    public void AjouterAdoption(string animalId, int contactId, DateOnly dateDemande)
    {
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand("CALL sp_ajouter_adoption(@animalId, @contactId, @dateDemande);", cnx);

        cmd.Parameters.AddWithValue("animalId", animalId);
        cmd.Parameters.AddWithValue("contactId", contactId);
        cmd.Parameters.AddWithValue("dateDemande", dateDemande);
        cmd.ExecuteNonQuery();
    }

    public void ModifierStatutAdoption(int idAdoption, string statut)
    {
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand("CALL sp_modifier_statut_adoption(@idAdoption, @statut);", cnx);

        cmd.Parameters.AddWithValue("idAdoption", idAdoption);
        cmd.Parameters.AddWithValue("statut", statut);
        cmd.ExecuteNonQuery();
    }

    public List<int> ListerIdsAdoption()
    {
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand("SELECT adoption_id FROM fn_lister_ids_adoption();", cnx);
        using var r = cmd.ExecuteReader();

        var ids = new List<int>();
        while (r.Read()) ids.Add(r.GetInt32(0));
        return ids;
    }

    public void ListerFamillesAccueilAnimal(string animalId)
    {
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand(@"
            SELECT fa_contact, date_debut, date_fin
            FROM famille_accueil
            WHERE fa_ani_identifiant = @animalId", cnx);

        cmd.Parameters.AddWithValue("animalId", animalId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            int contact = reader.GetInt32(0);
            DateOnly dateDebut = reader.GetFieldValue<DateOnly>(1);
            string dateFin = reader.IsDBNull(2) ? "En cours" : reader.GetFieldValue<DateOnly>(2).ToString();
            Console.WriteLine($"Contact : {contact} | Début : {dateDebut} | Fin : {dateFin}");
        }
    }

    public bool AnimalDejaAdopte(string animalId)
    {
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand("SELECT fn_animal_deja_adopte(@animalId);", cnx);
        cmd.Parameters.AddWithValue("animalId", animalId);
        return Convert.ToBoolean(cmd.ExecuteScalar());
    }

    public bool AnimalEnFamilleAccueil(string animalId)
    {
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand("SELECT fn_animal_en_famille_accueil(@animalId);", cnx);
        cmd.Parameters.AddWithValue("animalId", animalId);
        return Convert.ToBoolean(cmd.ExecuteScalar());
    }

    public void AjouterSortie(string animalId, int contactId, string raison, DateOnly dateSortie)
    {
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand("CALL sp_ajouter_sortie(@animalId, @contactId, @raison, @dateSortie);", cnx);

        cmd.Parameters.AddWithValue("animalId", animalId);
        cmd.Parameters.AddWithValue("contactId", contactId);
        cmd.Parameters.AddWithValue("raison", raison);
        cmd.Parameters.AddWithValue("dateSortie", dateSortie);
        cmd.ExecuteNonQuery();
    }

    public void AjouterCompatibilite(string animalId, int idCompatibilite, string valeur)
    {
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand("CALL sp_ajouter_compatibilite(@animalId, @compIdentifiant, @valeur);", cnx);

        cmd.Parameters.AddWithValue("animalId", animalId);
        cmd.Parameters.AddWithValue("compIdentifiant", idCompatibilite);
        cmd.Parameters.AddWithValue("valeur", valeur);
        cmd.ExecuteNonQuery();
    }

    public List<string> ListerFamillesAccueil()
    {
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand("SELECT * FROM fn_lister_familles_accueil();", cnx);
        using var r = cmd.ExecuteReader();

        var liste = new List<string>();
        while (r.Read())
        {
            string dateFin = r["date_fin"] == DBNull.Value ? "En cours" : r["date_fin"]?.ToString() ?? "";
            liste.Add($"FA {r["accueil_id"]} - Animal {r["fa_ani_identifiant"]} - Contact {r["fa_contact"]} - Début {r["date_debut"]} - Fin {dateFin}");
        }
        return liste;
    }

    public List<string> ListerAdoptions()
    {
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand("SELECT * FROM fn_lister_adoptions();", cnx);
        using var r = cmd.ExecuteReader();

        var liste = new List<string>();
        while (r.Read())
        {
            liste.Add($"Adoption {r["adoption_id"]} - Animal {r["ani_identifiant"]} - Contact {r["adop_contact"]} - Statut {r["statut"]} - Date {r["date_demande"]}");
        }
        return liste;
    }

    public List<string> ListerSorties()
    {
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand("SELECT * FROM fn_lister_sorties();", cnx);
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
        using var cmd = new NpgsqlCommand("SELECT * FROM fn_lister_compatibilites();", cnx);
        using var r = cmd.ExecuteReader();

        var liste = new List<string>();
        while (r.Read())
        {
            liste.Add($"Animal : {r["animal_nom"]} - Compatibilité : {r["compatibilite_nom"]} - Valeur : {r["valeur"]}");
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

