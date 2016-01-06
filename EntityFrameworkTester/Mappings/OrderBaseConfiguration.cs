using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EntityFrameworkTester.Mappings
{
    public abstract class OrderBaseConfiguration<T> : EntityTypeConfiguration<T>
        where T : OrderBase
    {
        protected OrderBaseConfiguration()
        {
            // Define primary key.
            this.HasKey(e => e.Id);

            // Define properties.
            this.Property(e => e.Id)
                .HasColumnName("ID")
                .HasPrecision(18, 0)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(e => e.CompanyName)
                .HasColumnName("RagSoc")
                .HasMaxLength(35)
                .IsOptional();

            this.Property(e => e.CustomerCode)
                .HasColumnName("Codice")
                .HasMaxLength(6)
                .IsRequired();

            this.Property(e => e.UserCode)
                .HasColumnName("CodUtente")
                .HasMaxLength(10)
                .IsOptional();

            this.Property(e => e.Comments)
                .HasColumnName("Commenti")
                .HasMaxLength(255)
                .IsOptional();

            this.Property(e => e.Creation)
                .HasColumnName("Data")
                .IsOptional();

            this.Property(e => e.CarrierType)
                .HasColumnName("AMezzo")
                .HasMaxLength(2)
                .IsOptional();

            this.Property(e => e.Payment)
                .HasColumnName("Pagamento")
                .HasMaxLength(35)
                .IsOptional();

            this.Property(e => e.PaymentType)
                .HasColumnName("MOD_PAGAM")
                .HasMaxLength(3)
                .IsOptional();

            this.Property(e => e.Seller)
                .HasColumnName("Venditore")
                .HasMaxLength(35)
                .IsMaxLength();

            this.Property(e => e.IpAddress)
                .HasColumnName("IP")
                .HasMaxLength(15)
                .IsOptional();

            this.Property(e => e.CustomerReference)
                .HasColumnName("RifCli")
                .HasMaxLength(50)
                .IsOptional();

            this.Property(e => e.Type)
                .HasColumnName("TipoOrdine")
                .HasMaxLength(2)
                .IsRequired();

            this.Property(e => e.Processed)
                .HasColumnName("DataEvasione")
                .IsOptional();

            this.Property(e => e.Coordinate)
                .HasColumnName("Coordinata")
                .HasMaxLength(2)
                .IsOptional();

            this.Property(e => e.Carrier)
                .HasColumnName("Vettore")
                .HasMaxLength(3)
                .IsOptional();

            this.Property(e => e.Freight)
                .HasColumnName("orresa")
                .HasMaxLength(2)
                .IsOptional();

            this.Property(e => e.GroupingOrders)
                .HasColumnName("RAGGR_ORDINI")
                .HasMaxLength(1)
                .IsOptional();

            this.Property(e => e.SendingCompleted)
                .HasColumnName("InvioCompletato")
                .HasMaxLength(1)
                .IsOptional();

            this.Property(e => e.DestinationId)
                .HasColumnName("IdDestinazione")
                .HasMaxLength(15)
                .IsRequired();

            this.Property(e => e.OrganizationType)
                .HasColumnName("TipoEnte")
                .HasMaxLength(3)
                .IsRequired();

            this.Property(e => e.ScopeId)
                .HasColumnName("IdAmbito")
                .IsRequired();                     

            this.Property(e => e.Destination)
                .HasColumnName("Destinazione")
                .HasMaxLength(35)
                .IsOptional();          

            this.Property(e => e.DestinationName)
                .HasColumnName("D_Nome")
                .HasMaxLength(35)
                .IsOptional();

            this.Property(e => e.DestinationAddress)
                .HasColumnName("D_Indirizzo")
                .HasMaxLength(35)
                .IsOptional();

            this.Property(e => e.DestinationPostalCode)
                .HasColumnName("D_CAP")
                .HasMaxLength(5)
                .IsOptional();

            this.Property(e => e.DestinationCity)
                .HasColumnName("D_Citta")
                .HasMaxLength(24)
                .IsOptional();

            this.Property(e => e.DestinationProvince)
                .HasColumnName("D_Pr")
                .HasMaxLength(2)
                .IsOptional();

            this.Property(e => e.DestinationTelephone)
                .HasColumnName("D_Tel")
                .HasMaxLength(15)
                .IsOptional();          

            this.Property(e => e.Orbpmi)
                .HasColumnName("ORBPMI")
                .HasMaxLength(1)
                .IsOptional();

            this.Property(e => e.CausaleLogo)
                .HasColumnName("CAUSALE_LOGO")
                .HasMaxLength(2)
                .IsOptional();

            this.Property(e => e.BidVal)
                .HasColumnName("BID_VAL")
                .HasMaxLength(50)
                .IsOptional();

            this.Property(e => e.BidClif)
                .HasColumnName("BID_CLIF")
                .HasMaxLength(50)
                .IsOptional();

            this.Property(e => e.BidAut)
                .HasColumnName("BID_AUT")
                .HasMaxLength(50)
                .IsOptional();

            this.Property(e => e.Nominativo)
                .HasColumnName("Nominativo")
                .HasMaxLength(35)
                .IsOptional();                             

            this.Property(e => e.Prenotazione)
                .HasColumnName("Prenotazione")
                .HasMaxLength(1)
                .IsOptional();
        }
    }
}
