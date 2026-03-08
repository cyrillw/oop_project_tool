using System.Windows;
using System.Windows.Controls;
using OOPProjectTool.Data;
using OOPProjectTool.Models;
using System.Windows.Media.Imaging;

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

        private void LadeDaten()
        {
            LstProjekte.ItemsSource = null;
            LstProjekte.ItemsSource = AppData.Projekte;

            CmbProjektFuerInfo.ItemsSource = null;
            CmbProjektFuerInfo.ItemsSource = AppData.Projekte;

            CmbSuchProjekt.ItemsSource = null;
            CmbSuchProjekt.ItemsSource = AppData.Projekte;
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

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            AppData.CurrentUser = null;

            var loginWindow = new LoginWindow();
            loginWindow.Show();

            Close();
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
            var ersteller = AppData.CurrentUser;
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
                string.IsNullOrWhiteSpace(TxtKommentar.Text) ||
                AppData.CurrentUser == null)
            {
                MessageBox.Show("Bitte alle Pflichtfelder ausfüllen.");
                return;
            }

            var info = (Information)CmbKommentarInformation.SelectedItem;
            var ersteller = AppData.CurrentUser;

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
                ZeigeBildVorschau(info);
            }
            else
            {
                ZeigeBildVorschau(null);
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

            ZeigeBildVorschau(null);
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

        private void Suche_Click(object sender, RoutedEventArgs e)
        {
            if (CmbSuchProjekt.SelectedItem == null)
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

            var projekt = (Projekt)CmbSuchProjekt.SelectedItem;
            var benutzer = AppData.CurrentUser;
            var suchTag = TxtSuchTag.Text.Trim();

            var resultate = benutzer.SucheInformationen(projekt.Informationen, suchTag);

            LstSuchresultate.ItemsSource = null;
            LstSuchresultate.ItemsSource = resultate;

            if (resultate.Count == 0)
            {
                MessageBox.Show("Keine passenden Informationen gefunden.");
            }
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

    }


}