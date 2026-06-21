using RefugeAnimaux.Dal;
using RefugeAnimaux.Metier;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace RefugeAnimauxWPF
{
    public partial class MainWindow : Window
    {
        private readonly AnimalRepository _animalRepo = new();
        private readonly ContactRepository _contactRepo = new();
        private readonly EntreeRepository _entreeRepo = new();
        private readonly VaccinRepository _vaccinRepo = new();

        public MainWindow()
        {
            InitializeComponent();
            ChargerAnimaux();
            ChargerContacts();
            ChargerAdoptions();
            ChargerListesAdoption();
            ChargerListesAdoption();
        }

        private void ChargerAnimaux()
        {
            var animaux = _animalRepo.ListerTous();

            cmbAnimalConsulter.ItemsSource = animaux;
            cmbAnimalSupprimer.ItemsSource = animaux;

            cmbFAAnimal.ItemsSource = animaux;
            cmbAdoptionAnimal.ItemsSource = animaux;
            cmbCompAnimal.ItemsSource = animaux;
            cmbSortieAnimal.ItemsSource = animaux;
            cmbEntreeAnimal.ItemsSource = animaux;
            cmbVaccinAnimal.ItemsSource = animaux;

            lstAnimaux.ItemsSource = animaux;
        }

        private void ChargerContacts()
        {
            var contacts = _contactRepo.ListerTous();

            cmbContactSupprimer.ItemsSource = contacts;
            cmbContactModifier.ItemsSource = contacts;

            cmbFAContact.ItemsSource = contacts;
            cmbAdoptionContact.ItemsSource = contacts;
            cmbSortieContact.ItemsSource = contacts;
            cmbEntreeContact.ItemsSource = contacts;

            lstContacts.ItemsSource = contacts;
        }
        private void ChargerAdoptions()
        {
            cmbIdAdoption.ItemsSource = _animalRepo.ListerIdsAdoption();
        }
        private void ChargerListesAdoption()
        {
            lstFamillesAccueil.ItemsSource = null;
            lstFamillesAccueil.ItemsSource = _animalRepo.ListerFamillesAccueil();

            lstAdoptions.ItemsSource = null;
            lstAdoptions.ItemsSource = _animalRepo.ListerAdoptions();
        }

        private void BtnAfficher_Click(object sender, RoutedEventArgs e)
        {
            ChargerAnimaux();
        }

        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            var animal = new Animal
            {
                Identifiant = txtId.Text,
                Nom = txtNom.Text,
                Type = (cmbType.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "",
                Sexe = (cmbSexe.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "",
                Particularites = string.IsNullOrWhiteSpace(txtParticularites.Text) ? null : txtParticularites.Text,
                Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text,
                Sterilise = chkSterilise.IsChecked == true,
                DateNaissance = dpDateNaissance.SelectedDate ?? DateTime.Now,
                DateSterilisation = dpDateSterilisation.SelectedDate,
                DateDeces = dpDateDeces.SelectedDate
            };

            _animalRepo.Ajouter(animal);
            MessageBox.Show("Animal ajouté !");
            ChargerAnimaux();
        }

        private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            var animal = cmbAnimalSupprimer.SelectedItem as Animal;

            if (animal == null)
            {
                MessageBox.Show("Sélectionnez un animal.");
                return;
            }

            _animalRepo.Supprimer(animal.Identifiant);
            MessageBox.Show("Animal supprimé !");
            ChargerAnimaux();
        }

        private void BtnConsulter_Click(object sender, RoutedEventArgs e)
        {
            var selection = cmbAnimalConsulter.SelectedItem as Animal;

            if (selection == null)
            {
                MessageBox.Show("Sélectionnez un animal.");
                return;
            }

            var animal = _animalRepo.Consulter(selection.Identifiant);

            if (animal == null)
            {
                MessageBox.Show("Animal introuvable.");
                return;
            }

            string sterDate = animal.DateSterilisation == null
                ? "Aucune date"
                : animal.DateSterilisation.Value.ToString("yyyy-MM-dd");

            MessageBox.Show(
                $"Identifiant : {animal.Identifiant}\n" +
                $"Nom : {animal.Nom}\n" +
                $"Type : {animal.Type}\n" +
                $"Sexe : {animal.Sexe}\n" +
                $"Stérilisé : {(animal.Sterilise ? "Oui" : "Non")}\n" +
                $"Date stérilisation : {sterDate}\n" +
                $"Particularités : {animal.Particularites ?? "Aucune"}\n" +
                $"Description : {animal.Description ?? "Aucune"}"
            );
        }

        private void BtnAfficherContacts_Click(object sender, RoutedEventArgs e)
        {
            ChargerContacts();
        }

        private void BtnAjouterContact_Click(object sender, RoutedEventArgs e)
        {
            var contact = new Contact
            {
                Nom = txtContactNom.Text,
                Prenom = txtContactPrenom.Text,
                RegistreNational = txtContactRN.Text,
                Rue = txtContactRue.Text,
                Cp = txtContactCp.Text,
                Localite = txtContactLocalite.Text,
                Gsm = string.IsNullOrWhiteSpace(txtContactGsm.Text) ? null : txtContactGsm.Text,
                Telephone = string.IsNullOrWhiteSpace(txtContactTelephone.Text) ? null : txtContactTelephone.Text,
                Email = string.IsNullOrWhiteSpace(txtContactEmail.Text) ? null : txtContactEmail.Text
            };

            int id = _contactRepo.Ajouter(contact);
            MessageBox.Show($"Contact ajouté. ID = {id}");
            ChargerContacts();
        }

        private void BtnModifierContact_Click(object sender, RoutedEventArgs e)
        {
            var contact = cmbContactModifier.SelectedItem as Contact;

            if (contact == null)
            {
                MessageBox.Show("Sélectionnez un contact.");
                return;
            }

            _contactRepo.ModifierContact(
                contact.ContactIdentifiant,
                txtAdresse.Text,
                txtGsm.Text,
                txtTelephone.Text,
                txtEmail.Text
            );

            MessageBox.Show("Contact modifié !");
            ChargerContacts();
        }

        private void BtnSupprimerContact_Click(object sender, RoutedEventArgs e)
        {
            var contact = cmbContactSupprimer.SelectedItem as Contact;

            if (contact == null)
            {
                MessageBox.Show("Sélectionnez un contact.");
                return;
            }

            _contactRepo.Supprimer(contact.ContactIdentifiant);
            MessageBox.Show("Contact supprimé !");
            ChargerContacts();
        }

        private void BtnAjouterFamilleAccueil_Click(object sender, RoutedEventArgs e)
        {
            var animal = cmbFAAnimal.SelectedItem as Animal;
            var contact = cmbFAContact.SelectedItem as Contact;

            if (animal == null || contact == null)
            {
                MessageBox.Show("Sélectionnez un animal et un contact.");
                return;
            }

            if (_animalRepo.AnimalDejaAdopte(animal.Identifiant))
            {
                MessageBox.Show("Impossible : cet animal est déjà adopté.");
                return;
            }

            if (_animalRepo.AnimalEnFamilleAccueil(animal.Identifiant))
            {
                MessageBox.Show("Impossible : cet animal est déjà en famille d'accueil.");
                return;
            }

            _animalRepo.AjouterFamilleAccueil(
                animal.Identifiant,
                contact.ContactIdentifiant,
                DateOnly.FromDateTime(dpFADateDebut.SelectedDate ?? DateTime.Now)
            );

            _animalRepo.AjouterSortie(
                animal.Identifiant,
                contact.ContactIdentifiant,
                "famille_accueil",
                DateOnly.FromDateTime(dpSortieDate.SelectedDate ?? DateTime.Now)
            );

            MessageBox.Show("Famille d'accueil ajoutée !");
        }

        private void BtnAjouterAdoption_Click(object sender, RoutedEventArgs e)
        {
            var animal = cmbAdoptionAnimal.SelectedItem as Animal;
            var contact = cmbAdoptionContact.SelectedItem as Contact;

            if (animal == null || contact == null)
            {
                MessageBox.Show("Sélectionnez un animal et un contact.");
                return;
            }

            if (_animalRepo.AnimalDejaAdopte(animal.Identifiant))
            {
                MessageBox.Show("Impossible : cet animal est déjà adopté.");
                return;
            }

            if (_animalRepo.AnimalEnFamilleAccueil(animal.Identifiant))
            {
                MessageBox.Show("Impossible : cet animal est déjà en famille d'accueil.");
                return;
            }


            _animalRepo.AjouterAdoption(
                animal.Identifiant,
                contact.ContactIdentifiant,
                DateOnly.FromDateTime(dpAdoptionDateDemande.SelectedDate ?? DateTime.Now)
            );

            _animalRepo.AjouterSortie(
                animal.Identifiant,
                contact.ContactIdentifiant,
                "adoption",
                DateOnly.FromDateTime(dpAdoptionDateDemande.SelectedDate ?? DateTime.Now)
            );

            MessageBox.Show("Adoption ajoutée !");
        }

        private void BtnAfficherFamillesAccueil_Click(object sender, RoutedEventArgs e)
        {
            lstFamillesAccueil.ItemsSource = null;
            lstFamillesAccueil.ItemsSource = _animalRepo.ListerFamillesAccueil();
        }

        private void BtnAfficherAdoptions_Click(object sender, RoutedEventArgs e)
        {
            lstAdoptions.ItemsSource = null;
            lstAdoptions.ItemsSource = _animalRepo.ListerAdoptions();
        }
        private void BtnModifierStatutAdoption_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbIdAdoption.SelectedItem == null || cmbStatutAdoption.SelectedItem == null)
                {
                    MessageBox.Show("Sélectionnez une adoption et un statut.");
                    return;
                }

                int idAdoption = (int)cmbIdAdoption.SelectedItem;
                string statut = ((ComboBoxItem)cmbStatutAdoption.SelectedItem).Content?.ToString() ?? "";

                _animalRepo.ModifierStatutAdoption(idAdoption, statut);

                MessageBox.Show("Statut adoption modifié !");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void BtnAjouterCompatibilite_Click(object sender, RoutedEventArgs e)
        {
            var animal = cmbCompAnimal.SelectedItem as Animal;

            if (animal == null)
            {
                MessageBox.Show("Sélectionnez un animal.");
                return;
            }

            if (cmbCompatibilite.SelectedItem == null ||
                cmbValeurCompatibilite.SelectedItem == null)
            {
                MessageBox.Show("Sélectionnez une compatibilité et une valeur.");
                return;
            }

            string compatibilite =
                ((ComboBoxItem)cmbCompatibilite.SelectedItem).Content.ToString() ?? "";

            string valeur =
                ((ComboBoxItem)cmbValeurCompatibilite.SelectedItem).Content.ToString() ?? "";

            int idCompatibilite = compatibilite switch
            {
                "chat" => 1,
                "chien" => 2,
                "jeune enfant" => 3,
                "enfant" => 4,
                "jardin" => 5,
                "poney" => 6,
                _ => 0
            };

            _animalRepo.AjouterCompatibilite(
                animal.Identifiant,
                idCompatibilite,
                valeur
            );

            MessageBox.Show("Compatibilité ajoutée !");
        }

        private void BtnAjouterSortie_Click(object sender, RoutedEventArgs e)
        {
            var animal = cmbSortieAnimal.SelectedItem as Animal;
            var contact = cmbSortieContact.SelectedItem as Contact;

            if (animal == null || contact == null)
            {
                MessageBox.Show("Sélectionnez un animal et un contact.");
                return;
            }

            _animalRepo.AjouterSortie(
                animal.Identifiant,
                contact.ContactIdentifiant,
                ((ComboBoxItem)cmbSortieRaison.SelectedItem).Content?.ToString() ?? "",
                DateOnly.FromDateTime(dpSortieDate.SelectedDate ?? DateTime.Now)
            );

            MessageBox.Show("Sortie ajoutée !");
        }

        private void BtnAjouterEntree_Click(object sender, RoutedEventArgs e)
        {
            var animal = cmbEntreeAnimal.SelectedItem as Animal;
            var contact = cmbEntreeContact.SelectedItem as Contact;

            if (animal == null || contact == null)
            {
                MessageBox.Show("Sélectionnez un animal et un contact.");
                return;
            }

            var entree = new Entree
            {
                AniIdentifiant = animal.Identifiant,
                EntreeContact = contact.ContactIdentifiant,
                Raison = ((ComboBoxItem)cmbEntreeRaison.SelectedItem).Content?.ToString() ?? "",
                DateEntree = dpEntreeDate.SelectedDate ?? DateTime.Now
            };

            _entreeRepo.Ajouter(entree);
            MessageBox.Show("Entrée ajoutée !");
        }
        private void BtnAfficherEntrees_Click(object sender, RoutedEventArgs e)
        {
            lstEntrees.ItemsSource = _entreeRepo.ListerEntrees();
        }

        private void BtnAfficherSorties_Click(object sender, RoutedEventArgs e)
        {
            lstSorties.ItemsSource = _animalRepo.ListerSorties();
        }

        private void BtnAjouterVaccination_Click(object sender, RoutedEventArgs e)
        {
            var animal = cmbVaccinAnimal.SelectedItem as Animal;

            if (animal == null)
            {
                MessageBox.Show("Sélectionnez un animal.");
                return;
            }

            _vaccinRepo.AjouterVaccination(
                animal.Identifiant,
                txtVaccinNom.Text,
                dpVaccinDate.SelectedDate ?? DateTime.Now
            );

            MessageBox.Show("Vaccination ajoutée !");
        }
        private void BtnAfficherVaccinations_Click(object sender, RoutedEventArgs e)
        {
            lstVaccinations.ItemsSource = null;
            lstVaccinations.ItemsSource = _vaccinRepo.ListerVaccinations();
        }

        private void BtnAfficherCompatibilites_Click(object sender, RoutedEventArgs e)
        {
            lstCompatibilites.ItemsSource = null;
            lstCompatibilites.ItemsSource = _animalRepo.ListerCompatibilites();
        }
    }
}