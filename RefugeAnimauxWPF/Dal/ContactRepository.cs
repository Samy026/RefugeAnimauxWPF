using Npgsql;
using RefugeAnimaux.Metier;

namespace RefugeAnimaux.Dal;

public class ContactRepository
{
    public int Ajouter(Contact contact)
    {
        using var cnx = Database.GetConnection();
        const string sql = @"
            INSERT INTO contact(nom, prenom, registre_national, rue, cp, localite, gsm, telephone, email)
            VALUES (@nom, @prenom, @rn, @rue, @cp, @localite, @gsm, @tel, @mail)
            RETURNING contact_identifiant;";
        using var cmd = new NpgsqlCommand(sql, cnx);
        cmd.Parameters.AddWithValue("nom", contact.Nom);
        cmd.Parameters.AddWithValue("prenom", contact.Prenom);
        cmd.Parameters.AddWithValue("rn", (object?)contact.RegistreNational ?? DBNull.Value);
        cmd.Parameters.AddWithValue("rue", (object?)contact.Rue ?? DBNull.Value);
        cmd.Parameters.AddWithValue("cp", (object?)contact.Cp ?? DBNull.Value);
        cmd.Parameters.AddWithValue("localite", (object?)contact.Localite ?? DBNull.Value);
        cmd.Parameters.AddWithValue("gsm", (object?)contact.Gsm ?? DBNull.Value);
        cmd.Parameters.AddWithValue("tel", (object?)contact.Telephone ?? DBNull.Value);
        cmd.Parameters.AddWithValue("mail", (object?)contact.Email ?? DBNull.Value);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public List<Contact> ListerTous()
    {
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand("SELECT * FROM contact ORDER BY contact_identifiant", cnx);
        using var r = cmd.ExecuteReader();
        var contacts = new List<Contact>();
        while (r.Read()) contacts.Add(LireContact(r));
        return contacts;
    }

    public void Supprimer(int id)
    {
        using var cnx = Database.GetConnection();
        using var cmd = new NpgsqlCommand("DELETE FROM contact WHERE contact_identifiant=@id", cnx);
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

    private static Contact LireContact(NpgsqlDataReader r) => new()
    {
        ContactIdentifiant = r.GetInt32(r.GetOrdinal("contact_identifiant")),
        Nom = r.GetString(r.GetOrdinal("nom")),
        Prenom = r.GetString(r.GetOrdinal("prenom")),
        RegistreNational = r["registre_national"] as string,
        Rue = r["rue"] as string,
        Cp = r["cp"] as string,
        Localite = r["localite"] as string,
        Gsm = r["gsm"] as string,
        Telephone = r["telephone"] as string,
        Email = r["email"] as string
    };
}
