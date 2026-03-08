using OOPProjectTool.Models;

namespace OOPProjectTool.Data
{
    public class AppState
    {
        public List<Benutzer> BenutzerListe { get; set; } = new();
        public List<Projekt> Projekte { get; set; } = new();
        public int NextProjektId { get; set; } = 1;
        public int NextInformationId { get; set; } = 1;
        public int NextKommentarId { get; set; } = 1;
        public int NextBenutzerId { get; set; } = 1;
    }
}