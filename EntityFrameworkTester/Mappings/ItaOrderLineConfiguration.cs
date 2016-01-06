namespace EntityFrameworkTester.Mappings
{
    public class ItaOrderLineConfiguration : OrderLineBaseConfiguration<ItaOrderLine>
    {
        public ItaOrderLineConfiguration()
        {
            this.Property(e => e.PriceDb)
               .HasColumnName("PrVendita")
               .IsOptional();

            this.Property(e => e.Sconto)
              .HasColumnName("Sconto")
              .HasMaxLength(50)
              .IsOptional();

            this.Property(e => e.Promo)
              .HasColumnName("Promo")
              .HasMaxLength(50)
              .IsOptional();        

            // Configure table map and scheme. The scheme is created if not exists.
            this.ToTable("ordini_dettaglio");
        }
    }
}
