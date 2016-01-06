using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using EntityFrameworkTester.Model;
using Newtonsoft.Json;
using Xunit;
using Xunit.Sdk;

namespace EntityFrameworkTester.Test
{
    public class DomaninReader
    {
        [Fact]
        public void TestOne()
        {
            int a = 11;
            double b = 2;

            var res = Math.Round(a / b);

            var aaa = Math.Ceiling(a/b);

            Console.WriteLine(res);
            Console.WriteLine(aaa);
        }

        [Theory]
        [InlineData("Data Source=SQLDEV;Initial Catalog=ORDINI_ITA;User ID=www;Password=www2000", 15374548)]
        public void Test1(string strConnection, int id)
        {
            using (var context = new MyDbContext(strConnection))
            {
                var instance = context.Set<ItaOrderLine>().FirstOrDefault(line => line.Id == id);
                Assert.NotNull(instance);
            }
        }

        [Theory]
        [InlineData("Data Source=LEONARDO-SQL;Initial Catalog=Utenti;Persist Security Info=True;User ID=www_rw;Password=farelt$k3j@aer24!fasf4234654;MultipleActiveResultSets=True;Application Name=ciupa", 112, 0)]
        [InlineData("Data Source=LEONARDO-SQL;Initial Catalog=Utenti;Persist Security Info=True;User ID=www_rw;Password=farelt$k3j@aer24!fasf4234654;MultipleActiveResultSets=True;Application Name=ciupa", 117, 1)]
        [InlineData("Data Source=LEONARDO-SQL;Initial Catalog=Utenti;Persist Security Info=True;User ID=www_rw;Password=farelt$k3j@aer24!fasf4234654;MultipleActiveResultSets=True;Application Name=ciupa", 288, 3)]
        [InlineData("Data Source=LEONARDO-SQL;Initial Catalog=Utenti;Persist Security Info=True;User ID=www_rw;Password=farelt$k3j@aer24!fasf4234654;MultipleActiveResultSets=True;Application Name=ciupa", 501, 5)]
        public void User_BackupOf_Test(string strConnection, int id, int count)
        {
            using (var context = new MyDbContext(strConnection))
            {
                // var instance1 = context.Set<Utente>().Include(utente => utente.BackupUsers).Single(utente => utente.Id == id);
                var instance1 = context.Set<Utente>().Single(utente => utente.Id == id);
                Assert.NotNull(instance1);

                Assert.Equal(instance1.BackupUsers.Count, count);
            }
        }

        [Theory]
        [InlineData("Data Source=SQLDEV;Initial Catalog=Utenti;User ID=www;Password=www2000", 965)]
        public void User_FakeUsers_Test(string strConnection, int id)
        {
            using (var context = new MyDbContext(strConnection))
            {
                var instance0 = context.Set<Utente>()
                    .Include(utente => utente.FakeUsers)
                    .Single(utente => utente.Id == id)
                    ;

                Assert.NotNull(instance0);
                Assert.NotEmpty(instance0.FakeUsers);
            }
        }

        //[Theory]
        //[InlineData("Data Source=LEONARDO-SQL;Initial Catalog=Utenti;User ID=www;Password=www2000", 36, 1)]
        //public void Menu_ReadingInstances(string strConnection, int id, int idlevel)
        //{
        //    using (var context = new MyDbContext(strConnection))
        //    {
        //        var result = context.Set<MenuTree>()
        //            .Include(tree => tree.Subtrees)
        //            .Include(tree => tree.Languages)
        //            .Where(tree => tree.IdMenu == id && tree.IdLevel == idlevel)
        //            .ToList();

        //        Assert.NotNull(result);

        //        var res2 = this.MakeTree(result.First(), "IT");
        //        Assert.NotNull(res2);
        //    }
        //}

        [Theory]
        [InlineData("Data Source=LEONARDO-SQL;Initial Catalog=Utenti;User ID=www;Password=www2000", 36, 1)]
        public void Menu_LanguagesTest(string strConnection, int id, int idlevel)
        {
            using (var context = new MyDbContext(strConnection))
            {
                var result = context.Set<MenuLanguage>()
                    //.Include(language => language.MenuTree)
                    //.Include(language => language.MenuTree.Subtrees)
                    .Where(language => language.IdMenu == id && language.IdLevel == idlevel)
                    .ToList();

                Assert.NotNull(result);
                Assert.NotEmpty(result);
            }
        }
        
        [Theory]
        [InlineData("Data Source=LEONARDO-SQL;Initial Catalog=Utenti;User ID=www;Password=www2000", 31, 1, 1)]
        public void TestOnOriginTree(string strConnection, int id, int idlevel, int idcompany)
        {
            //MenuTreeOrigin
            using (var context = new MyDbContext(strConnection))
            {
                var result = context.Set<MenuTreeOrigin>()
                    .Where(origin => origin.IdMenu == id && origin.IdLevel == idlevel && origin.IdCompany == idcompany)
                    .ToList();

                Assert.NotNull(result);
                Assert.NotEmpty(result);
            }
        }

        [Theory]
        [InlineData("Data Source=LEONARDO-SQL;Initial Catalog=Utenti;User ID=www;Password=www2000", 31, 1, 1)]
        public void TestOnMenuWithFilteredOrigins(string strConnection, int id, int idlevel, int idcompany)
        {
            using (var context = new MyDbContext(strConnection))
            {
                var result = context.Set<MenuTree>()
                    .Where(tree => tree.IdMenu == id && tree.IdLevel == idlevel)
                    .Select(n => new { Id = n.IdMenu, n.IdLevel, OrigCounter = n.Origins.Count(origin => origin.IdCompany == idcompany) })
                    .ToList();

                Assert.NotNull(result);
                Assert.NotEmpty(result);
            }
        }

        [Theory]
        [InlineData("Data Source=LEONARDO-SQL;Initial Catalog=Utenti;User ID=www;Password=www2000;Application Name=menutester", 31, 1, 1, 18, 914, 5623, false)]
        [InlineData("Data Source=LEONARDO-SQL;Initial Catalog=Utenti;User ID=www;Password=www2000;Application Name=menutester", 31, 1, 5, 19, 915, 5623, true)]
        public void TestWithViewModel(string strConnection, int id, int idlevel, int idcompany, int? idmacrogruppo, int? idgruppo, int? idutente, bool isnull)
        {
            using (var context = new MyDbContext(strConnection))
            {
                var result = context.Set<MenuTree>()
                    //.FirstOrDefault(tree => tree.Id == id && tree.IdLevel == idlevel);
                    .FirstOrDefault(tree => 
                        tree.IdMenu == id && tree.IdLevel == idlevel
                        && tree.Origins.Any(origin => origin.IdCompany == idcompany || origin.IdMacroGroup == idmacrogruppo || origin.IdGroup == idgruppo || origin.IdUser == idutente)
                    );

                #region
                //.Select(tree => new { tree.Id, tree.IdLevel, OrigCount = tree.Origins.Count(origin => origin.IdCompany == idcompany) }) // 7 righe
                //.Select(tree => new
                //{
                //    tree.Id, tree.IdLevel, OrigCount = tree.Origins.Count(origin => origin.IdCompany == idcompany || origin.IdMacroGroup == idmacrogruppo)
                //}) // 1621 righe

                //.Select(tree => new
                //{
                //    tree.Id, tree.IdLevel,
                //    OrigCount = tree.Origins.Count(origin => origin.IdCompany == idcompany || origin.IdMacroGroup == idmacrogruppo || origin.IdGroup == idgruppo)
                //}) // 1637 righe

                //.Select(tree => new
                //{
                //    tree.Id,
                //    tree.IdLevel,
                //    OrigCount = tree.Origins.Count(origin => origin.IdCompany == idcompany || origin.IdMacroGroup == idmacrogruppo || origin.IdGroup == idgruppo || origin.IdUser == idutente)
                //}) // 1637 righe
                //.FirstOrDefault(tree => tree.Id == id && tree.IdLevel == idlevel)
                //;
                #endregion

                if (isnull)
                {
                    Assert.Null(result);
                }
                else
                {
                    Assert.NotNull(result);

                    var ret = this.MakeTree(result, "IT");
                    Assert.NotNull(ret);
                }
            }
        }

        [Theory]
        // [InlineData("Data Source=LEONARDO-SQL;Initial Catalog=Utenti;User ID=www;Password=www2000;Application Name=menutester", 1, 1, 18, 914, 5623, false)]
        [InlineData("Data Source=LEONARDO-SQL;Initial Catalog=Utenti;Persist Security Info=True;User ID=www_rw;Password=farelt$k3j@aer24!fasf4234654;MultipleActiveResultSets=True;Application Name=menutester", 1, 1, 18, 914, 5623, false)]
        // [InlineData("Data Source=LEONARDO-SQL;Initial Catalog=Utenti;User ID=www;Password=www2000;Application Name=menutester", 1, 5, 19, 915, 5623, true)]
        [InlineData("Data Source=SQLDEV;Initial Catalog=Utenti;Persist Security Info=True;User ID=www_rw;Password=farelt$k3j@aer24!fasf4234654;MultipleActiveResultSets=True;Application Name=menutester", 1, 1, 19, 914, 5623, false)]
        public void TestWithViewModelRoot(string strConnection, int idlevel, int idcompany, int? idmacrogruppo, int? idgruppo, int? idutente, bool isnull)
        {
            // idazienda: 1, idMacrogruppo: 18, idGruppo: 914, idUtente: 5623
            using (var context = new MyDbContext(strConnection))
            {
                var result = context.Set<MenuTree>()
                    .Where(tree =>
                        tree.IdLevel == idlevel
                        && tree.Origins.Any(origin => origin.IdCompany == idcompany || origin.IdMacroGroup == idmacrogruppo || origin.IdGroup == idgruppo || origin.IdUser == idutente)
                    ).ToList();

                if (isnull)
                {
                    Assert.Empty(result);
                }
                else
                {
                    Assert.NotNull(result);
                    var trees = result.Select(n => this.MakeTree(n, "IT"));
                    Assert.NotNull(trees);
                    Assert.NotEmpty(trees);
                }
            }
        }

        [Theory]
        [InlineData("Data Source=LEONARDO-SQL;Initial Catalog=Utenti;Persist Security Info=True;User ID=www_rw;Password=farelt$k3j@aer24!fasf4234654;MultipleActiveResultSets=True;Application Name=menutester", 1, 1, 21, 927, 1547, false)]
        public void TestOnTreeModel(string strConnection, int idlevel, int idcompany, int? idmacrogruppo, int? idgruppo, int? idutente, bool isnull)
        {
            using (var context = new MyDbContext(strConnection))
            {
                var result = context.Set<MenuTree>()
                    .Where(tree =>
                        tree.IdLevel == idlevel
                        && tree.Origins.Any(origin => origin.IdCompany == idcompany || idmacrogruppo == origin.IdMacroGroup || origin.IdGroup == idgruppo || origin.IdUser == idutente)
                        // tree.IdLevel == 2 && tree.IdMenu == 62
                    );

                Assert.NotNull(result);

                var azienda = result.FirstOrDefault(n => n.IdMenu == 36 && n.IdLevel == 1);
                Assert.NotNull(azienda);

                //var subtreesAz =
                //    azienda.Subtrees.Where(
                //        tree =>
                //            tree.Origins.Any(
                //                origin =>
                //                    origin.IdCompany == idcompany || idmacrogruppo == origin.IdMacroGroup ||
                //                    origin.IdGroup == idgruppo || origin.IdUser == idutente));

                //Assert.NotNull(subtreesAz);


            }
        }

        [Theory]
        [InlineData("Data Source=LEONARDO-SQL;Initial Catalog=Utenti;Persist Security Info=True;User ID=www_rw;Password=farelt$k3j@aer24!fasf4234654;MultipleActiveResultSets=True;Application Name=menutester", 1, 1, 21, 927, 1547, false)]
        public void TestOnTreeModel2(string strConnection, int idlevel, int idcompany, int? idmacrogruppo, int? idgruppo, int? idutente, bool isnull)
        {
            using (var context = new MyDbContext(strConnection))
            {
                var result = context.Set<MenuTree>()//.Include(n => n.Subtrees)
                    .Where(tree =>
                        tree.IdLevel == idlevel
                        && tree.Origins.Any(origin => origin.IdCompany == idcompany || idmacrogruppo == origin.IdMacroGroup || origin.IdGroup == idgruppo || origin.IdUser == idutente)
                    // tree.IdLevel == 2 && tree.IdMenu == 62
                    ).ToList();

                Assert.NotNull(result);

            }
        }

        [Theory]
        [InlineData("Data Source=LEONARDO-SQL;Initial Catalog=Utenti;Persist Security Info=True;User ID=www_rw;Password=farelt$k3j@aer24!fasf4234654;MultipleActiveResultSets=True;Application Name=menutester", 1, 1, 21, 927, 1547)]
        public void TestBuildMenuView(string strConnection, int idlevel, int idcompany, int? idmacrogruppo, int? idgruppo, int? idutente)
        {
            List<MenuTree> result;
            using (var context = new MyDbContext(strConnection))
            {
                result = context.Set<MenuTree>().Include(tree => tree.Languages)
                    .Where(tree => tree.Root.IdMenu == null && tree.Root.IdLevel == null
                                   //&&
                                   //tree.Origins.Any(
                                   //    origin =>
                                   //        origin.IdCompany == idcompany || idmacrogruppo == origin.IdMacroGroup || origin.IdGroup == idgruppo || origin.IdUser == idutente)
                    ).ToList();

            }

            Assert.NotNull(result);

            var ress = result.Select(tree => this.MakeTreeModel(tree, "IT")).AsParallel().ToList();
            Assert.NotNull(ress);
        }

        [Theory]
        [InlineData("Data Source=LEONARDO-SQL;Initial Catalog=Utenti;Persist Security Info=True;User ID=www_rw;Password=farelt$k3j@aer24!fasf4234654;MultipleActiveResultSets=True;Application Name=menutester")]
        public void BuildNewTreNode(string strConnection)
        {
            var builder = new MenuBuilder(() => new MyDbContext(strConnection));

            var timer = new Stopwatch();

            timer.Start();
            var result = builder.Build();
            timer.Stop();

            Console.Write(timer.ElapsedMilliseconds);

            Assert.NotNull(result);

            var ress = this.MakeTree2(result, "IT");
            Assert.NotNull(ress);

            var json = JsonConvert.SerializeObject(ress, Formatting.Indented);
            Console.WriteLine(json);
        }

        public void MakeTreeModel(MenuTree root)
        {
            const string strConnection = "Data Source=LEONARDO-SQL;Initial Catalog=Utenti;Persist Security Info=True;User ID=www_rw;Password=farelt$k3j@aer24!fasf4234654;MultipleActiveResultSets=True;Application Name=menutester";
            List<MenuTree> result;
            using (var context = new MyDbContext(strConnection))
            {
                result = context.Set<MenuTree>().Include(tree => tree.Languages)
                    .Where(tree => tree.Root.IdMenu == root.IdMenu && tree.Root.IdLevel == root.IdLevel
                                   &&
                                   tree.Origins.Any(origin => origin.IdCompany == 1 || origin.IdMacroGroup == 21 || origin.IdGroup == 927 || origin.IdUser == 1547)
                    ).ToList();

                result.ForEach(this.MakeTreeModel);
            }
            root.Subtrees = result;
        }

        public MenuTreeModel MakeTreeModel(MenuTree root, string lang)
        {
            const string strConnection = "Data Source=LEONARDO-SQL;Initial Catalog=Utenti;Persist Security Info=True;User ID=www_rw;Password=farelt$k3j@aer24!fasf4234654;MultipleActiveResultSets=True;Application Name=menutester";

            List<MenuTree> result;
            using (var context = new MyDbContext(strConnection))
            {
                result = context.Set<MenuTree>().Include(tree => tree.Languages)
                    .Where(tree => tree.Root.IdMenu == root.IdMenu && tree.Root.IdLevel == root.IdLevel
                                   &&
                                   tree.Origins.Any(
                                       origin =>
                                           origin.IdCompany == 1 || origin.IdMacroGroup == 21 || origin.IdGroup == 927 ||
                                           origin.IdUser == 1547)
                    ).ToList();
            }
            
            return new MenuTreeModel
            {
                IdMenu = root.IdMenu,
                IdLevel = root.IdLevel,
                LevelDescription = root.LevelDescription,
                Link = root.Link,
                MenuDescription =
                    (root.Languages.FirstOrDefault(n => n.Language == lang)
                    ?? new MenuLanguage { Description = "nothing" }).Description,
                Subtrees = result.Select(tree => this.MakeTreeModel(tree, lang)).ToList()
            };
        }

        public MenuTreeModel MakeTree2(TreeNode tree, string lang)
        {
            return new MenuTreeModel
            {
                IdMenu = tree.IdMenu,
                IdLevel = tree.IdLevel,
                LevelDescription = tree.LevelDescription,
                Link = tree.Link,
                //CurrentLang = tree.Languages.FirstOrDefault(n => n.Language == lang),
                MenuDescription =
                    (tree.Languages.FirstOrDefault(n => n.Language == lang) ??
                     new MenuLanguage { Description = "nothing" }).Description,
                Subtrees = tree.Subtrees.Select(n => this.MakeTree2(n, lang)).OrderBy(model => model.MenuDescription).ToList()

                // Any(origin => origin.IdCompany == authInfo.IdAzienda || origin.IdMacroGroup == authInfo.IdMacroGruppo || origin.IdGroup == authInfo.IdGruppo || origin.IdUser == authInfo.IdUtente)
                //Subtrees = tree.Subtrees
                //    //.Where(
                //    //    menuTree =>
                //    //        menuTree.Origins.Any(origin => origin.IdCompany == 1 || origin.IdMacroGroup == 21 || origin.IdGroup == 927 || origin.IdUser == 1547)
                //    //)
                //    .Select(n => this.MakeTree(n, lang)).ToList()
            };
        }

        public MenuTreeModel MakeTree(MenuTree tree, string lang)
        {
            return new MenuTreeModel
            {
                IdMenu = tree.IdMenu,
                IdLevel = tree.IdLevel,
                LevelDescription = tree.LevelDescription,
                Link = tree.Link,
                //CurrentLang = tree.Languages.FirstOrDefault(n => n.Language == lang),
                MenuDescription =
                    (tree.Languages.FirstOrDefault(n => n.Language == lang) ??
                     new MenuLanguage {Description = "nothing"}).Description,
                // Subtrees = tree.Subtrees.Select(n => this.MakeTree(n, lang))
                // Any(origin => origin.IdCompany == authInfo.IdAzienda || origin.IdMacroGroup == authInfo.IdMacroGruppo || origin.IdGroup == authInfo.IdGruppo || origin.IdUser == authInfo.IdUtente)
                //Subtrees = tree.Subtrees
                //    //.Where(
                //    //    menuTree =>
                //    //        menuTree.Origins.Any(origin => origin.IdCompany == 1 || origin.IdMacroGroup == 21 || origin.IdGroup == 927 || origin.IdUser == 1547)
                //    //)
                //    .Select(n => this.MakeTree(n, lang)).ToList()
            };
        }
    }

    //[DebuggerDisplay("idMenu: {IdMenu}, idLevel: {IdLevel}, levelDescription: {LevelDescription}, language: {CurrentLang.Language}, link: {Link}")]
    [DebuggerDisplay("idMenu: {IdMenu}, idLevel: {IdLevel}, levelDescription: {LevelDescription}, menuDescription: {MenuDescription}, link: {Link}")]
    public class MenuTreeModel
    {
        public int IdMenu { get; set; }
        public int IdLevel { get; set; }
        public string LevelDescription { get; set; }
        public string Link { get; set; }
        // public MenuLanguage CurrentLang { get; set; }
        public string MenuDescription { get; set; }
        public List<MenuTreeModel> Subtrees { get; set; }

    }
}
