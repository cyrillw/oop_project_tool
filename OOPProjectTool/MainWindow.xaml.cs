using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using OOPProjectTool.Data;
using OOPProjectTool.Models;

namespace OOPProjectTool
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LadeDaten();
            InitialisiereBenutzeroberflaeche();
        }

        private void InitialisiereBenutzeroberflaeche()
        {
            if (AppData.CurrentUser == null)
            {
                MessageBox.Show("Kein Benutzer angemeldet.");
                Close();
                return;
            }

            TxtAktuellerBenutzer.Text = $"Angemeldet als: {AppData.CurrentUser.Name} ({AppData.CurrentUser.Rolle})";
            TxtProjektleiterAnzeige.Text = AppData.CurrentUser.Name;

            bool istProjektleiter = AppData.CurrentUser.Rolle.Equals("Projektleiter", StringComparison.OrdinalIgnoreCase);

            if (!istProjektleiter)
            {
                MainTabControl.Items.Remove(TabProjektErfassen);
            }
        }

        private void LadeDaten()
        {
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
                AppData.CurrentUser == null)
            {
                MessageBox.Show("Bitte alle Pflichtfelder ausfüllen.");
                return;
            }

            var projekt = Projekt.EroeffneProjekt(
                TxtProjektName.Text.Trim(),
                TxtKunde.Text.Trim(),
                AppData.CurrentUser,
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
                CmbTyp.SelectedItem == null ||
                string.IsNullOrWhiteSpace(TxtInfoInhalt.Text) ||
                AppData.CurrentUser == null)
            {
                MessageBox.Show("Bitte alle Pflichtfelder ausfüllen.");
                return;
            }

            var projekt = (Projekt)CmbProjektFuerInfo.SelectedItem;
            var typ = ((ComboBoxItem)CmbTyp.SelectedItem).Content.ToString() ?? "Text";

            var info = Information.ErstelleInformation(typ, TxtInfoInhalt.Text.Trim(), AppData.CurrentUser);
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
            LstInformationen.SelectedItem = info;
            MessageBox.Show("Information wurde hinzugefügt.");
        }

        private void KommentarHinzufuegen_Click(object sender, RoutedEventArgs e)
        {
            if (LstInformationen.SelectedItem is not Information info ||
                string.IsNullOrWhiteSpace(TxtKommentar.Text) ||
                AppData.CurrentUser == null)
            {
                MessageBox.Show("Bitte zuerst eine Information auswählen und Kommentartext eingeben.");
                return;
            }

            var kommentar = Kommentar.ErstelleKommentar(TxtKommentar.Text.Trim(), AppData.CurrentUser);
            kommentar.KommentarId = AppData.NextKommentarId();

            info.FuegeKommentarHinzu(kommentar);
            AppData.Save();

            TxtKommentar.Clear();
            AktualisiereKommentare(info);
            MessageBox.Show("Kommentar wurde hinzugefügt.");
        }

        private void Suche_Click(object sender, RoutedEventArgs e)
        {
            if (CmbProjektFuerInfo.SelectedItem == null)
            {
                MessageBox.Show("Bitte ein Projekt auswählen.");
                return;
            }

            if (AppData.CurrentUser == null)
            {
                MessageBox.Show("Kein Benutzer angemeldet.");
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtSuchTag.Text))
            {
                MessageBox.Show("Bitte einen Tag eingeben.");
                return;
            }

            var projekt = (Projekt)CmbProjektFuerInfo.SelectedItem;
            var resultate = AppData.CurrentUser.SucheInformationen(projekt.Informationen, TxtSuchTag.Text.Trim());

            LstInformationen.ItemsSource = null;
            LstInformationen.ItemsSource = resultate;

            LeereDetailAnsicht();

            if (resultate.Count == 0)
            {
                MessageBox.Show("Keine passenden Informationen gefunden.");
            }
        }

        private void SucheZuruecksetzen_Click(object sender, RoutedEventArgs e)
        {
            TxtSuchTag.Clear();

            if (CmbProjektFuerInfo.SelectedItem is Projekt projekt)
            {
                AktualisiereInformationen(projekt);
            }
            else
            {
                LstInformationen.ItemsSource = null;
                LeereDetailAnsicht();
            }
        }

        private void LstProjekte_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstProjekte.SelectedItem is Projekt projekt)
            {
                CmbProjektFuerInfo.SelectedItem = projekt;
                MainTabControl.SelectedIndex = 1;
                ZeigeProjektKernanforderung(projekt);
                AktualisiereInformationen(projekt);
            }
        }

        private void CmbProjektFuerInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbProjektFuerInfo.SelectedItem is Projekt projekt)
            {
                ZeigeProjektKernanforderung(projekt);
                AktualisiereInformationen(projekt);
            }
            else
            {
                TxtProjektKernanforderungAnzeige.Clear();
                LstInformationen.ItemsSource = null;
                LeereDetailAnsicht();
            }
        }

        private void LstInformationen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstInformationen.SelectedItem is Information info)
            {
                ZeigeInformationDetails(info);
                AktualisiereKommentare(info);
                ZeigeBildVorschau(info);
            }
            else
            {
                LeereDetailAnsicht();
            }
        }

        private void AktualisiereInformationen(Projekt projekt)
        {
            LstInformationen.ItemsSource = null;
            LstInformationen.ItemsSource = projekt.Informationen;
            LeereDetailAnsicht();
        }

        private void ZeigeProjektKernanforderung(Projekt? projekt)
        {
            if (projekt == null)
            {
                TxtProjektKernanforderungAnzeige.Clear();
                return;
            }

            TxtProjektKernanforderungAnzeige.Text = projekt.KernanforderungImProjekt ?? "";
        }

        private void AktualisiereKommentare(Information info)
        {
            LstKommentare.ItemsSource = null;
            LstKommentare.ItemsSource = info.Kommentare;
        }

        private void ZeigeInformationDetails(Information info)
        {
            TxtDetailTyp.Text = info.Typ;
            TxtDetailErsteller.Text = info.Ersteller?.Name ?? "Unbekannt";
            TxtDetailInhalt.Text = info.Inhalt;
            TxtDetailTags.Text = info.Tags.Count > 0 ? string.Join(", ", info.Tags) : "";
        }

        private void LeereDetailAnsicht()
        {
            TxtDetailTyp.Clear();
            TxtDetailErsteller.Clear();
            TxtDetailInhalt.Clear();
            TxtDetailTags.Clear();
            TxtKommentar.Clear();

            LstKommentare.ItemsSource = null;
            ZeigeBildVorschau(null);
        }

        private void ZeigeBildVorschau(Information? info)
        {
            ImgVorschau.Source = null;

            if (info == null)
            {
                TxtBildHinweis.Text = "Keine Bild-URL ausgewählt.";
                TxtBildHinweis.Visibility = Visibility.Visible;
                return;
            }

            if (!string.Equals(info.Typ, "Bild-URL", StringComparison.OrdinalIgnoreCase))
            {
                TxtBildHinweis.Text = "Die ausgewählte Information ist kein Bild.";
                TxtBildHinweis.Visibility = Visibility.Visible;
                return;
            }

            if (string.IsNullOrWhiteSpace(info.Inhalt))
            {
                TxtBildHinweis.Text = "Keine Bild-URL vorhanden.";
                TxtBildHinweis.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                var uri = new Uri(info.Inhalt, UriKind.Absolute);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = uri;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                ImgVorschau.Source = bitmap;
                TxtBildHinweis.Visibility = Visibility.Collapsed;
            }
            catch
            {
                ImgVorschau.Source = null;
                TxtBildHinweis.Text = "Bild konnte nicht geladen werden.";
                TxtBildHinweis.Visibility = Visibility.Visible;
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            AppData.CurrentUser = null;

            var loginWindow = new LoginWindow();
            loginWindow.Show();

            Close();
        }
    }
}