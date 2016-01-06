using System.Collections.Generic;
using System.Diagnostics;

namespace EntityFrameworkTester.Model
{
    [DebuggerDisplay("idMenu: {IdMenu}, idLevel: {IdLevel}, levelDescription: {LevelDescription}, link: {Link}")]
    public class TreeNode
    {
        public int IdMenu { get; set; }

        public int IdLevel { get; set; }

        public string LevelDescription { get; set; }

        public string Link { get; set; }

        public virtual ICollection<MenuLanguage> Languages { get; set; }

        public virtual ICollection<TreeNode> Subtrees { get; set; }
    }
}
