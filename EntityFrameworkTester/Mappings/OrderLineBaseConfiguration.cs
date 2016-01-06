using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EntityFrameworkTester.Mappings
{
    public abstract class OrderLineBaseConfiguration<T> : EntityTypeConfiguration<T>
      where T : OrderLineBase
   {
        protected OrderLineBaseConfiguration()
        {
            // Define primary key.
            this.HasKey(e => e.Id);

            // Define properties.
            this.Property(e => e.Id)
                .HasColumnName("ID")
                .HasPrecision(18, 0)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(e => e.OrderId)
                .HasColumnName("ID_Ordine");
       
            this.Property(e => e.ProductCode)
                .HasColumnName("Cod")
                .HasMaxLength(50)
                .IsRequired();

            this.Property(e => e.Quantity)
                .HasColumnName("Qta")
                .IsRequired();

            this.Property(e => e.ProductDescription)
                .HasColumnName("Descrizione")
                .HasMaxLength(35)
                .IsOptional();

            this.Ignore(e => e.Price);

            this.Property(e => e.Warehouse)
                .HasColumnName("Deposito")
                .HasMaxLength(2)
                .IsOptional();

            this.Property(e => e.Coordinate)
                .HasColumnName("Coordinata")
                .HasMaxLength(2)
                .IsOptional();

            ////this.Property(e => e.Type)
            ////    .HasColumnName("TipoOrdine")
            ////    .HasMaxLength(2)
            ////    .IsRequired();
          
            this.Property(e => e.Subtype)
                .HasColumnName("SottoTipo")
                .HasMaxLength(3)
                .IsRequired();

            ////this.Property(e => e.QtyPrice)
            ////    .HasColumnName("PrezzoQta")
            ////    .HasMaxLength(50)
            ////    .IsOptional();

            this.Property(e => e.Bundle)                
                .IsOptional();

            this.Property(e => e.Supply)
                .HasColumnName("Offerta")
                .IsOptional();

            this.Property(e => e.Program)
                .HasColumnName("Programma")
                .HasMaxLength(15)
                .IsOptional();
        }
    }
}
