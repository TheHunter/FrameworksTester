using System.Data.Entity.ModelConfiguration;
using EntityFrameworkTester.Model;

namespace EntityFrameworkTester.Mappings
{
    public class MenuLanguageMap
        : EntityTypeConfiguration<MenuLanguage>
    {
        public MenuLanguageMap()
        {
            this.ToTable("AreaMenuTreeLang", "dbo");

            this.Property(lang => lang.IdMenu).HasColumnName("FkMenu");

            this.Property(lang => lang.IdLevel).HasColumnName("FkLivello");

            this.Property(lang => lang.Description).HasColumnName("Descrizione");

            this.Property(lang => lang.Language).HasColumnName("Lang");

            this.HasKey(lang => new { lang.IdMenu, lang.IdLevel, lang.Language });
            
        }
    }
}
