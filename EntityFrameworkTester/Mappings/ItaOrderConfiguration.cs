namespace EntityFrameworkTester.Mappings
{
    public class ItaOrderConfiguration : OrderBaseConfiguration<ItaOrder>
    {
        public ItaOrderConfiguration()
        {
            this.Property(e => e.CustomerAddress)
                .HasColumnName("Indirizzo")
                .HasMaxLength(35)
                .IsOptional();

            this.Property(e => e.CustomerCity)
                .HasColumnName("Citta")
                .HasMaxLength(35)
                .IsOptional();

            this.Property(e => e.CustomerProvince)
               .HasColumnName("Pr")
               .HasMaxLength(2)
               .IsOptional();

            this.Property(e => e.PartitaIva)
                .HasColumnName("Piva")
                .HasMaxLength(16)
                .IsOptional();

            this.Property(e => e.CustomerTelephone)
              .HasColumnName("Telefono")
              .HasMaxLength(16)
              .IsOptional();

            this.Property(e => e.CustomerFax)
              .HasColumnName("Fax")
              .HasMaxLength(16)
              .IsOptional();

            this.Property(e => e.CustomerMail)
              .HasColumnName("email")
              .HasMaxLength(60)
              .IsOptional();

            this.Property(e => e.CustomerPostalCode)
               .HasColumnName("CAP")
               .HasMaxLength(5)
               .IsOptional();

            this.Property(e => e.CommentType)
               .HasColumnName("TipoCommenti")
               .HasMaxLength(1)
               .IsOptional();

            this.Property(e => e.Valuta)
               .HasColumnName("Valuta")
               .HasMaxLength(3)
               .IsOptional();

            this.Property(e => e.NumeroBid)
               .HasColumnName("NumeroAutBID")
               .HasMaxLength(45)
               .IsOptional();

            this.Property(e => e.TipoBid)
                .HasColumnName("tipoBid")
                .HasMaxLength(45)
                .IsOptional();

            this.Property(e => e.ClienteFatturazione)
                .HasColumnName("CLI_FATT")
                .HasMaxLength(6)
                .IsOptional();

            this.Property(e => e.CodiceAssotrade)
               .HasColumnName("COD_ASSOTRADE")
               .HasMaxLength(6)
               .IsOptional();

            this.Property(e => e.OrdineFiglio)
                .HasColumnName("ORD_FIG")
                .HasMaxLength(1)
                .IsOptional();

            this.Property(e => e.Pod)
               .HasColumnName("POD")
               .HasMaxLength(1)
               .IsOptional();

            this.Property(e => e.Privacy)
               .HasColumnName("LEGGE_PRIVACY")
               .HasMaxLength(1)
               .IsOptional();

            this.Property(e => e.Cig)
               .HasColumnName("CIG")
               .HasMaxLength(15)
               .IsOptional();

            this.Property(e => e.Cup)
               .HasColumnName("CUP")
               .HasMaxLength(15)
               .IsOptional();  
            
            ////this.HasMany(e => e.Lines)
            ////  .WithOptional()
            ////  .HasForeignKey(l => l.OrderId);

            // Configure table map and scheme. The scheme is created if not exists.
            this.ToTable("ordini_testata");
        }
    }
}
