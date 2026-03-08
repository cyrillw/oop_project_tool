using System.Windows;
using System.Windows.Controls;
using OOPProjectTool.Data;
using OOPProjectTool.Models;

namespace OOPProjectTool
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AppData.Load();
            LadeDaten();
        }

        private void LadeDaten()
        {
            CmbProjektleiter.ItemsSource = null;
            CmbProjektleiter.ItemsSource = AppData.BenutzerListe;

            CmbInfoErsteller.ItemsSource = null;
            CmbInfoErsteller.ItemsSource = AppData.BenutzerListe;

            CmbKommentarErsteller.ItemsSource = null;
            CmbKommentarErsteller.ItemsSource = AppData.BenutzerListe;

            LstProjekte.ItemsSource = null;
            LstProjekte.ItemsSource = AppData.Projekte;

            CmbProjektFuerInfo.ItemsSource = null;
            CmbProjektFuerInfo.ItemsSource = AppData.Projekte;
        }

        private void ProjektErstellen_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtProjektName.Text) ||
                string.IsNullOrWhiteSpace(TxtKunde.Text) ||
                string.IsNullOrWhiteSpace(TxtKernanforderung.Text) ||
                CmbProjektleiter.SelectedItem == null)
            {
                MessageBox.Show("Bitte alle Pflichtfelder ausfüllen.");
                return;
            }

            var projektleiter = (Benutzer)CmbProjektleiter.SelectedItem;

            var projekt = Projekt.EroeffneProjekt(
                TxtProjektName.Text.Trim(),
                TxtKunde.Text.Trim(),
                projektleiter,
                TxtKernanforderung.Text.Trim());

            projekt.ProjektId = AppData.NextProjektId();

            AppData.Projekte.Add(projekt);
            AppData.Save();

            TxtProjektName.Clear();
            TxtKunde.Clear();
            TxtKernanforderung.Clear();

            LadeDaten();
            MessageBox.Show("Projekt wurde erstellt.");
        }

        private void InformationHinzufuegen_Click(object sender, RoutedEventArgs e)
        {
            if (CmbProjektFuerInfo.SelectedItem == null ||
                CmbInfoErsteller.SelectedItem == null ||
                CmbTyp.SelectedItem == null ||
                string.IsNullOrWhiteSpace(TxtInfoInhalt.Text))
            {
                MessageBox.Show("Bitte alle Pflichtfelder ausfüllen.");
                return;
            }

            var projekt = (Projekt)CmbProjektFuerInfo.SelectedItem;
            var ersteller = (Benutzer)CmbInfoErsteller.SelectedItem;
            var typ = ((ComboBoxItem)CmbTyp.SelectedItem).Content.ToString() ?? "Text";

            var info = Information.ErstelleInformation(typ, TxtInfoInhalt.Text.Trim(), ersteller);
            info.InformationId = AppData.NextInformationId();

            var tags = TxtTags.Text
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToList();

            foreach (var tag in tags)
            {
                if (!info.WeiseTagZu(tag))
                {
                    MessageBox.Show("Es sind maximal 3 eindeutige Tags erlaubt.");
                    break;
                }
            }

            projekt.FuegeInformationHinzu(info);
            AppData.Save();

            TxtInfoInhalt.Clear();
            TxtTags.Clear();

            AktualisiereInformationen(projekt);
            AktualisiereKommentarInformationen();
            MessageBox.Show("Information wurde hinzugefügt.");
        }

        private void KommentarHinzufuegen_Click(object sender, RoutedEventArgs e)
        {
            if (CmbKommentarInformation.SelectedItem == null ||
                CmbKommentarErsteller.SelectedItem == null ||
                string.IsNullOrWhiteSpace(TxtKommentar.Text))
            {
                MessageBox.Show("Bitte alle Pflichtfelder ausfüllen.");
                return;
            }

            var info = (Information)CmbKommentarInformation.SelectedItem;
            var ersteller = (Benutzer)CmbKommentarErsteller.SelectedItem;

            var kommentar = Kommentar.ErstelleKommentar(TxtKommentar.Text.Trim(), ersteller);
            kommentar.KommentarId = AppData.NextKommentarId();

            info.FuegeKommentarHinzu(kommentar);
            AppData.Save();

            TxtKommentar.Clear();
            AktualisiereKommentare(info);
            MessageBox.Show("Kommentar wurde hinzugefügt.");
        }

        private void LstProjekte_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstProjekte.SelectedItem is Projekt projekt)
            {
                CmbProjektFuerInfo.SelectedItem = projekt;
                AktualisiereInformationen(projekt);
            }
        }

        private void CmbProjektFuerInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbProjektFuerInfo.SelectedItem is Projekt projekt)
            {
                AktualisiereInformationen(projekt);
            }
        }

        private void LstInformationen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstInformationen.SelectedItem is Information info)
            {
                CmbKommentarInformation.SelectedItem = info;
                AktualisiereKommentare(info);
            }
        }

        private void CmbKommentarInformation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbKommentarInformation.SelectedItem is Information info)
            {
                AktualisiereKommentare(info);
            }
        }

        private void AktualisiereInformationen(Projekt projekt)
        {
            LstInformationen.ItemsSource = null;
            LstInformationen.ItemsSource = projekt.Informationen;

            AktualisiereKommentarInformationen();
        }

        private void AktualisiereKommentarInformationen()
        {
            var infos = AppData.Projekte.SelectMany(p => p.Informationen).ToList();
            CmbKommentarInformation.ItemsSource = null;
            CmbKommentarInformation.ItemsSource = infos;
        }

        private void AktualisiereKommentare(Information info)
        {
            LstKommentare.ItemsSource = null;
            LstKommentare.ItemsSource = info.Kommentare;
        }
    }
}