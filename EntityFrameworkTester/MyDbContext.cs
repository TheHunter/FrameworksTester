using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Configuration;
using EntityFrameworkTester.Mappings;
using EntityFrameworkTester.Model;

namespace EntityFrameworkTester
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ////modelBuilder.Configurations.Add(new ItaOrderConfiguration());
            modelBuilder.Configurations.Add(new ItaOrderLineConfiguration());
            modelBuilder.Configurations.Add(new UtenteMap());

            modelBuilder.Configurations.Add(new MenuTreeMap());
            modelBuilder.Configurations.Add(new MenuLanguageMap());

            modelBuilder.Configurations.Add(new MenuTreeOriginMap());

            // modelBuilder.ComplexType<MenuMap>();
            modelBuilder.Configurations.Add(new MenuMap());

            //FunctionImportReturnTypeScalarPropertyMapping

            //modelBuilder.Conventions.Add(new FunctionsConvention<>);
            //var aaa = ((IObjectContextAdapter) this).ObjectContext;
            ////aaa.CreateQuery<MenuTree>();
            //aaa.ExecuteFunction("");

        }
    }
}
