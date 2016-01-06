using System.Diagnostics;

namespace EntityFrameworkTester.Model
{
    [DebuggerDisplay("idMenu: {IdMenu}, idLevel: {IdLevel}, idMenuOrigin: {IdMenuOrigin}")]
    public class MenuTreeOrigin
    {
        public int Id { get; set; }

        public int IdMenu { get; set; }

        public int IdLevel { get; set; }

        public int IdMenuOrigin { get; set; }

        public int? IdCompany { get; set; }

        public int? IdMacroGroup { get; set; }

        public int? IdGroup { get; set; }

        public int? IdUser { get; set; }
    }
}
