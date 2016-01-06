using System.Diagnostics;

namespace EntityFrameworkTester.Model
{
    [DebuggerDisplay("description: {Description}, language: {Language}")]
    public class MenuLanguage
    {
        public int IdMenu { get; set; }

        public int IdLevel { get; set; }

        public string Description { get; set; }

        public string Language { get; set; }
    }
}
