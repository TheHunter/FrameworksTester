using System.Collections.Generic;
using System.Diagnostics;
using EntityFrameworkTester.Mappings;

namespace EntityFrameworkTester.Model
{
    [DebuggerDisplay("idMenu: {IdMenu}, idLevel: {IdLevel}, levelDescription: {LevelDescription}, root: [{Root.IdMenu} - {Root.IdLevel}], link: {Link}")]
    public class MenuTree
    {
        public int IdMenu { get; set; }

        public int IdLevel { get; set; }

        public string LevelDescription { get; set; }

        public string Link { get; set; }

        //public int? IdRootMenu { get; set; }

        //public int? IdRootLevel { get; set; }

        public Menu Root { get; set; }

        public virtual ICollection<MenuLanguage> Languages { get; set; }
        
        public virtual ICollection<MenuTreeOrigin> Origins { get; set; }

        public virtual ICollection<MenuTree> Subtrees { get; set; }

    }

    [DebuggerDisplay("idMenu: {IdMenu}, idLevel: {IdLevel}")]
    public class Menu
    {
        public int? IdMenu { get; set; }
        public int? IdLevel { get; set; }
    }
}
