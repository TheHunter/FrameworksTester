using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using EntityFrameworkTester.Model;

namespace EntityFrameworkTester.Mappings
{
    public class MenuTreeMap
        : EntityTypeConfiguration<MenuTree>
    {
        public MenuTreeMap()
        {
            this.ToTable("AreaMenuTree", "dbo");

            this.HasKey(tree => new { tree.IdMenu, tree.IdLevel });

            this.Property(tree => tree.IdMenu).HasColumnName("IdMenu");

            this.Property(tree => tree.IdLevel).HasColumnName("IdLivello");

            this.Property(tree => tree.LevelDescription).HasColumnName("DescrizioneLivello");

            this.Property(tree => tree.Link).HasColumnName("Link");

            this.HasMany(tree => tree.Languages).WithOptional().HasForeignKey(lang => new { lang.IdMenu, lang.IdLevel });

            this.HasMany(tree => tree.Origins).WithOptional().HasForeignKey(origin => new { origin.IdMenu, origin.IdLevel });
            // this.Ignore(tree => tree.Origins);
            
            // non funziona in alcuni casi.
            // this.HasMany(tree => tree.Subtrees).WithOptional().Map(cfg => cfg.MapKey("FkMenuPadre", "FkMenuLivelloPadre"));
            this.Ignore(tree => tree.Subtrees);

            // this.Ignore(tree => tree.Root);
            
            // questo da errore... 
            // this.HasMany(tree => tree.Subtrees).WithMany().Map(cfg => cfg.MapLeftKey("IdMenu", "IdLivello").MapRightKey("FkMenuPadre", "FkMenuLivelloPadre"));
        }
    }

    public class MenuMap
        : ComplexTypeConfiguration<Menu>
    {
        public MenuMap()
        {
            this.Property(menu => menu.IdMenu).HasColumnName("FkMenuPadre");

            this.Property(menu => menu.IdLevel).HasColumnName("FkMenuLivelloPadre");
        }
    }
}
