using System.IO;
using System.Text.Json;
using OOPProjectTool.Models;

namespace OOPProjectTool.Data
{
    public static class AppData
    {
        private static readonly string DataFolder =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataStore");

        private static readonly string DataFile =
            Path.Combine(DataFolder, "appdata.json");

        public static List<Benutzer> BenutzerListe { get; private set; } = new();
        public static List<Projekt> Projekte { get; private set; } = new();
        public static Benutzer? CurrentUser { get; set; }

        private static int _projektId = 1;
        private static int _informationId = 1;
        private static int _kommentarId = 1;
        private static int _benutzerId = 1;

        public static int NextProjektId() => _projektId++;
        public static int NextInformationId() => _informationId++;
        public static int NextKommentarId() => _kommentarId++;
        public static int NextBenutzerId() => _benutzerId++;

        public static void Load()
        {
            if (!Directory.Exists(DataFolder))
            {
                Directory.CreateDirectory(DataFolder);
            }

            if (!File.Exists(DataFile))
            {
                Seed();
                Save();
                return;
            }

            var json = File.ReadAllText(DataFile);

            if (string.IsNullOrWhiteSpace(json))
            {
                Seed();
                Save();
                return;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            var state = JsonSerializer.Deserialize<AppState>(json, options);

            if (state == null)
            {
                Seed();
                Save();
                return;
            }

            BenutzerListe = state.BenutzerListe ?? new List<Benutzer>();
            Projekte = state.Projekte ?? new List<Projekt>();

            _projektId = state.NextProjektId;
            _informationId = state.NextInformationId;
            _kommentarId = state.NextKommentarId;
            _benutzerId = state.NextBenutzerId;

            if (BenutzerListe.Count == 0)
            {
                Seed();
                Save();
            }
        }

        public static void Save()
        {
            if (!Directory.Exists(DataFolder))
            {
                Directory.CreateDirectory(DataFolder);
            }

            var state = new AppState
            {
                BenutzerListe = BenutzerListe,
                Projekte = Projekte,
                NextProjektId = _projektId,
                NextInformationId = _informationId,
                NextKommentarId = _kommentarId,
                NextBenutzerId = _benutzerId
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(state, options);
            File.WriteAllText(DataFile, json);
        }

        public static void Seed()
        {
            BenutzerListe.Clear();
            Projekte.Clear();

            _projektId = 1;
            _informationId = 1;
            _kommentarId = 1;
            _benutzerId = 1;

            BenutzerListe.Add(new Benutzer
            {
                Id = NextBenutzerId(),
                Name = "Hans",
                Rolle = "Projektleiter"
            });

            BenutzerListe.Add(new Benutzer
            {
                Id = NextBenutzerId(),
                Name = "Lisa",
                Rolle = "Mitarbeiter"
            });

            BenutzerListe.Add(new Benutzer
            {
                Id = NextBenutzerId(),
                Name = "Max",
                Rolle = "Mitarbeiter"
            });
        }
    }
}