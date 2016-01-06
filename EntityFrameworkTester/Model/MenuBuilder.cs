using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkTester.Model
{
    public class MenuBuilder
    {
        private readonly Func<MyDbContext> contextFunc;

        public MenuBuilder(Func<MyDbContext> contextFunc)
        {
            this.contextFunc = contextFunc;
        }

        public TreeNode Build()
        {
            List<TreeNode> result;
            using (var context = this.contextFunc.Invoke())
            {
                result = context.Database.SqlQuery<TreeNode>(
                        "SELECT fnt.IdMenu, fnt.IdLivello AS IdLevel, fnt.DescrizioneLivello AS LevelDescription, fnt.FkMenuPadre, fnt.FkMenuLivelloPadre, fnt.Link FROM dbo.AreaMenuTreeFunc(1, 21, 927, 1547) fnt WHERE fnt.FkMenuPadre is null AND fnt.FkMenuLivelloPadre is null")
                        .ToList();
            }

            Parallel.ForEach(result, node => this.InitSubTrees(node));

            return new TreeNode
            {
                IdMenu = 0, IdLevel = 0, LevelDescription = "Root", Subtrees = result, Languages = new List<MenuLanguage>()
            };
        }

        private void InitSubTrees(TreeNode root)
        {
            Task<List<TreeNode>> subtrees;
            var idmenu = new SqlParameter("@idmenu", root.IdMenu);
            var idlevel = new SqlParameter("@idlevel", root.IdLevel);

            var languages = Task.Run(() =>
            {
                using (var context = this.contextFunc.Invoke())
                {
                    return context.Set<MenuLanguage>()
                        .Where(language => language.IdMenu == root.IdMenu && language.IdLevel == root.IdLevel).ToList();
                }
            });

            subtrees = Task.Run(() =>
            {
                using (var context = this.contextFunc.Invoke())
                {
                    return
                        context.Database.SqlQuery<TreeNode>(
                            "SELECT fnt.IdMenu, fnt.IdLivello AS IdLevel, fnt.DescrizioneLivello AS LevelDescription, fnt.FkMenuPadre, fnt.FkMenuLivelloPadre, fnt.Link FROM dbo.AreaMenuTreeFunc(1, 21, 927, 1547) fnt WHERE fnt.FkMenuPadre = @idmenu AND fnt.FkMenuLivelloPadre = @idlevel",
                            idmenu, idlevel)
                            .ToList();
                }
            });

            Parallel.ForEach(subtrees.Result, node => this.InitSubTrees(node));

            root.Languages = languages.Result;
            root.Subtrees = subtrees.Result;
        }
    }
}
