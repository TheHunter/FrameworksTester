using System.Data.Entity.ModelConfiguration;
using EntityFrameworkTester.Model;

namespace EntityFrameworkTester.Mappings
{
    public class MenuTreeOriginMap
        : EntityTypeConfiguration<MenuTreeOrigin>
    {
        public MenuTreeOriginMap()
        {
            this.ToTable("MenuTreeOrigin", "dbo");

            this.HasKey(origin => origin.Id);

            this.Property(origin => origin.IdMenu).HasColumnName("IdMenu");

            this.Property(origin => origin.IdLevel).HasColumnName("IdLivello");
            
            this.Property(origin => origin.IdMenuOrigin).HasColumnName("IdMenuOrig");

            this.Property(origin => origin.IdCompany).HasColumnName("IdSocieta");
            
            this.Property(origin => origin.IdMacroGroup).HasColumnName("IdmacroGruppo");
            
            this.Property(origin => origin.IdGroup).HasColumnName("IdGruppo");
            
            this.Property(origin => origin.IdUser).HasColumnName("IdUtente");
        }
    }
}
