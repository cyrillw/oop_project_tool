namespace OOPProjectTool.Models
{
    public class Projekt
    {
        public int ProjektId { get; set; }
        public string Name { get; set; } = "";
        public string Kunde { get; set; } = "";
        public Benutzer? Projektleiter { get; set; }
        public string KernanforderungImProjekt { get; set; } = "";

        public List<Information> Informationen { get; set; } = new();

        public static Projekt EroeffneProjekt(string name, string kunde, Benutzer? projektleiter, string kernanforderung)
        {
            return new Projekt
            {
                Name = name,
                Kunde = kunde,
                Projektleiter = projektleiter,
                KernanforderungImProjekt = kernanforderung
            };
        }

        public void FuegeInformationHinzu(Information information)
        {
            Informationen.Add(information);
        }

        public override string ToString()
        {
            return $"{Name} ({Kunde})";
        }
    }
}