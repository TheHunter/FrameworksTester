using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkTester.Mappings
{
    public class UtenteMap : UtenteBaseMap<Utente>
    {
        public UtenteMap()
        {
            this.ToTable("UtentiConIdAzienda");

            this.Property(u => u.Id).HasColumnName("ID_UTENTE");

            this.Property(u => u.IdGuid).HasColumnName("GUID_ACTIVE_DIRECTORY");

            this.Property(u => u.NomeUtente).HasColumnName("CUSER_NAME").IsUnicode(false);
            this.Property(u => u.Password).HasColumnName("CPWD").IsUnicode(false);
            this.Property(u => u.IsAbilitato).HasColumnName("UteAttivo");
            this.Property(u => u.Nome).HasColumnName("CNOME").IsUnicode(false);
            this.Property(u => u.Cognome).HasColumnName("CCOGNOME2").IsUnicode(false);
            this.Property(u => u.Email).HasColumnName("CEMAIL").IsUnicode(false);
            //this.Property(u => u.IsCommercialeAttivo).HasColumnName("IsCommercialeAttivo");
            this.Property(u => u.CodiceAgente).HasColumnName("Cod_Customer");
            this.Property(u => u.Indirizzo).HasColumnName("INDIRIZZO").IsUnicode(false);
            this.Property(u => u.Cap).HasColumnName("CAP").IsUnicode(false);
            this.Property(u => u.Citta).HasColumnName("CITTA").IsUnicode(false);
            this.Property(u => u.Provincia).HasColumnName("PROVINCIA").IsUnicode(false);
            this.Property(u => u.Nazione).HasColumnName("NAZIONE").IsUnicode(false);
            this.Property(u => u.CodiceFiscale).HasColumnName("COD_FISCALE").IsUnicode(false);
            this.Property(u => u.Matricola).HasColumnName("N_MATRI").IsUnicode(false);
            this.Property(u => u.IdResponsabile).HasColumnName("ID_resp");
            this.Property(u => u.Telefono).HasColumnName("CTELEFONO").IsUnicode(false);
            this.Property(u => u.Cellulare).HasColumnName("CCELLULARE").IsUnicode(false);

            this.Property(u => u.IsFakeUser).HasColumnName("IsFakeUser");

            ////this.HasOptional(utente => utente.BackupOf)
            ////    .WithOptionalDependent()
            ////    .Map(configuration => configuration.MapKey("BKP_UTENTE"))
            ////    ;

            //this.HasOptional(utente => utente.BackupOf)
            //    .WithMany()
            //    .Map(configuration => configuration.MapKey("BKP_UTENTE"));

            // this.HasOptional(utente => utente.BackupOf).WithMany().HasForeignKey(utente => utente.Idbackup);
            this.Ignore(utente => utente.BackupOf);

            // this.Ignore(utente => utente.FakeUsers);
            this.HasMany(utente => utente.FakeUsers)
                .WithMany()
                .Map(configuration => configuration
                    .ToTable("FakeUserImpersonator", "dbo")
                    .MapLeftKey("IdUser")
                    .MapRightKey("IdFakeUser")
                    )
                ;

            this.HasMany(utente => utente.BackupUsers).WithOptional().Map(cfg => cfg.MapKey("BKP_UTENTE"));
            // this.Ignore(utente => utente.BackupUsers);

            this.HasKey(u => u.Id);

            //this.HasMany(u => u.Gruppi).WithMany(g => g.Utenti).Map(m => m.ToTable("TP_GRUPPO_UTENTE").MapLeftKey("ID_UTENTE").MapRightKey("ID_GRUPPO"));

            //this.HasMany(u => u.ProduttoriAssociati).WithRequired(p => p.Utente).HasForeignKey(p => p.IdUtente);
            //this.HasMany(u => u.CanaliAssociati).WithRequired(c => c.Utente).HasForeignKey(c => c.IdUtente);
        }
    }
}
