using OOPProjectTool.Models;

namespace OOPProjectTool.Data
{
    public static class AppData
    {
        public static List<Benutzer> BenutzerListe { get; } = new();
        public static List<Projekt> Projekte { get; } = new();

        private static int _projektId = 1;
        private static int _informationId = 1;
        private static int _kommentarId = 1;
        private static int _benutzerId = 1;

        public static int NextProjektId() => _projektId++;
        public static int NextInformationId() => _informationId++;
        public static int NextKommentarId() => _kommentarId++;
        public static int NextBenutzerId() => _benutzerId++;

        public static void Seed()
        {
            if (BenutzerListe.Count > 0)
                return;

            BenutzerListe.Add(new Benutzer
            {
                Id = NextBenutzerId(),
                Name = "Max Muster",
                Rolle = "Projektleiter"
            });

            BenutzerListe.Add(new Benutzer
            {
                Id = NextBenutzerId(),
                Name = "Anna Beispiel",
                Rolle = "Mitarbeiter"
            });
        }
    }
}