namespace OOPProjectTool.Models
{
    public class Benutzer
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Rolle { get; set; } = "";

        public List<Information> SucheInformationen(List<Information> informationen, string suchbegriff)
        {
            return informationen
                .Where(i => i.Tags.Any(t => t.Contains(suchbegriff, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        public override string ToString()
        {
            return $"{Name} ({Rolle})";
        }
    }
}