namespace OOPProjectTool.Models
{
    public class Information
    {
        public int InformationId { get; set; }
        public string Typ { get; set; } = "";
        public string Inhalt { get; set; } = "";
        public DateTime ErstellDatum { get; set; } = DateTime.Now;
        public List<string> Tags { get; set; } = new();
        public List<Kommentar> Kommentare { get; set; } = new();
        public Benutzer? Ersteller { get; set; }

        public static Information ErstelleInformation(string typ, string inhalt, Benutzer? ersteller)
        {
            return new Information
            {
                Typ = typ,
                Inhalt = inhalt,
                ErstellDatum = DateTime.Now,
                Ersteller = ersteller
            };
        }

        public bool WeiseTagZu(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return false;

            if (Tags.Count >= 3)
                return false;

            if (Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                return false;

            Tags.Add(tag.Trim());
            return true;
        }

        public void FuegeKommentarHinzu(Kommentar kommentar)
        {
            Kommentare.Add(kommentar);
        }

        public override string ToString()
        {
            string tagsText = Tags.Count > 0 ? $" [{string.Join(", ", Tags)}]" : "";
            return $"{Typ}: {Inhalt}{tagsText}";
        }
    }
}