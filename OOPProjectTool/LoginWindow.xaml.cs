using System.Windows;
using OOPProjectTool.Data;
using OOPProjectTool.Models;

namespace OOPProjectTool
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            AppData.Load();
            CmbBenutzer.ItemsSource = AppData.BenutzerListe;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (CmbBenutzer.SelectedItem is not Benutzer benutzer)
            {
                MessageBox.Show("Bitte einen Benutzer auswählen.");
                return;
            }

            AppData.CurrentUser = benutzer;

            var mainWindow = new MainWindow();
            mainWindow.Show();

            Close();
        }
    }
}