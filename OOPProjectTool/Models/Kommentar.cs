namespace OOPProjectTool.Models
{
    public class Kommentar
    {
        public int KommentarId { get; set; }
        public string Text { get; set; } = "";
        public DateTime Datum { get; set; } = DateTime.Now;
        public Benutzer? Ersteller { get; set; }

        public static Kommentar ErstelleKommentar(string text, Benutzer? ersteller)
        {
            return new Kommentar
            {
                Text = text,
                Datum = DateTime.Now,
                Ersteller = ersteller
            };
        }

        public override string ToString()
        {
            var name = Ersteller?.Name ?? "Unbekannt";
            return $"{Datum:dd.MM.yyyy HH:mm} - {name}: {Text}";
        }
    }
}