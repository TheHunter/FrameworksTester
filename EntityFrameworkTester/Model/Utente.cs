using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkTester
{
    public class Utente : UtenteBase
    {
        private readonly int[] utentiCashAndCarryFidelity = { 1588, 931, 4155, 328, 439, 4570, 4901, 1308, 1625 };

        //public virtual ICollection<ParametriPurchasing> BrandParametriPurchasingAcquisti { get; set; }

        //public virtual ICollection<ParametriPurchasing> BrandParametriPurchasingBackupAcquisti { get; set; }

        //public virtual ICollection<ParametriPurchasing> BrandParametriPurchasingResi { get; set; } // ParametriPurchasing.FK_ParametriPurchasing_TB_UTENTI_resi

        //public virtual ICollection<ParametriPurchasing> BrandParametriPurchasingTeamleader { get; set; }

        //public virtual ICollection<BusinessDivisionAssociata> BusinessDivisionsAssociate { get; set; }

        //public virtual ICollection<CanaleAssociato> CanaliAssociati { get; set; }

        public string Cap { get; set; }

        public string Citta { get; set; }

        public string CodiceAgente { get; set; }

        public string CodiceFiscale { get; set; }

        //public IEnumerable<string> CodiciProduttoriAssociati
        //{
        //    get
        //    {
        //        return this.ProduttoriAssociati != null ? this.ProduttoriAssociati.Select(c => c.Codice).Distinct() : Enumerable.Empty<string>();
        //    }
        //}

        //public virtual ICollection<Gruppo> Gruppi { get; set; }

        public int? Id { get; set; }

        public string IdGuid { get; set; }

        //public IdAzienda IdAzienda { get; set; }

        public string Indirizzo { get; set; }

        public bool IsAbilitato { get; set; }

        public bool IsAgente
        {
            get
            {
                return !string.IsNullOrEmpty(this.CodiceAgente);
            }
        }

        //public bool IsBrandManager
        //{
        //    get
        //    {
        //        return (this.ProduttoriAssociati != null) && this.ProduttoriAssociati.Any();
        //    }
        //}

        //public bool IsBrandManagerTeamLeader
        //{
        //    get
        //    {
        //        return (this.ProduttoriAssociati != null) && this.ProduttoriAssociati.Any(p => p.IdTipoBrandManager == "T");
        //    }
        //}

        //public bool IsBusinessDivisionManager
        //{
        //    get
        //    {
        //        return this.BusinessDivisionsAssociate.Any(c => c.UtenteId == this.Id);
        //    }
        //}

        //public bool IsBusinessUnitManager
        //{
        //    get
        //    {
        //        return (this.ProduttoriAssociati != null) && this.ProduttoriAssociati.Any(p => p.IdTipoBrandManager == "B");
        //    }
        //}

        //public bool IsCommercialeAttivo { get; set; }

        //public bool IsCountryManager
        //{
        //    get
        //    {
        //        return (this.Gruppi != null) && this.Gruppi.Any(g => new[] { 782, 1173 }.Contains(g.IdGruppo));
        //    }
        //}

        //public bool IsMarketAreaManager
        //{
        //    get
        //    {
        //        return (this.ProduttoriAssociati != null) && this.ProduttoriAssociati.Any(p => p.IdTipoBrandManager == "S");
        //    }
        //}

        //public bool IsMemberOfCashAndCarry
        //{
        //    get
        //    {
        //        return this.utentiCashAndCarryFidelity.Contains(this.Id);
        //    }
        //}

        //public bool IsMemberOfCdg
        //{
        //    get
        //    {
        //        return (this.Gruppi != null) && this.Gruppi.Any(g => Configuration.Intranet.Gruppi.Default.GruppiCdG.Contains(g.IdGruppo));
        //    }
        //}

        //public bool IsMemberOfDirezione
        //{
        //    get
        //    {
        //        return (this.Gruppi != null) && this.Gruppi.Any(g => Configuration.Intranet.Gruppi.Default.GruppiDirezione.Contains(g.IdGruppo));
        //    }
        //}

        //public bool IsMemberOfPurchasing
        //{
        //    get
        //    {
        //        return (this.Gruppi != null) && this.Gruppi.Any(g => Configuration.Intranet.Gruppi.Default.GruppiPurchasing.Contains(g.IdGruppo));
        //    }
        //}

        //public bool IsMemberOfUfficioBid
        //{
        //    get
        //    {
        //        return (this.Gruppi != null) && this.Gruppi.Any(g => new[] { 560, 720 }.Contains(g.IdGruppo));
        //    }
        //}

        //public bool IsMemberOfUfficioLogistico
        //{
        //    get
        //    {
        //        return (this.Gruppi != null) && this.Gruppi.Any(g => new[] { 718, 808 }.Contains(g.IdGruppo));
        //    }
        //}

        //public bool IsMemberOfUfficioResi
        //{
        //    get
        //    {
        //        return (this.Gruppi != null) && this.Gruppi.Any(g => Configuration.Intranet.Gruppi.Default.GruppiResi.Contains(g.IdGruppo));
        //    }
        //}

        //public bool IsMemberOfWeb
        //{
        //    get
        //    {
        //        return (this.Gruppi != null) && (this.Gruppi.Any(g => new[] { 8, 18, 43, 52 }.Contains(g.IdMacroGruppo)) || this.Gruppi.Any(g => new[] { 653 }.Contains(g.IdGruppo)));
        //    }
        //}

        //public bool IsMemberOfHr
        //{
        //    get
        //    {
        //        return (this.Gruppi != null) && (this.Gruppi.Any(g => new[] { 14 }.Contains(g.IdMacroGruppo)) || this.Gruppi.Any(g => new[] { 614 }.Contains(g.IdGruppo)));
        //    }
        //}

        //public bool IsMemberOfWebBusinessArea
        //{
        //    get
        //    {
        //        return (this.Gruppi != null) && this.Gruppi.Any(g => new[] { 913 }.Contains(g.IdGruppo));
        //    }
        //}

        //public bool IsResponsabileDiCanale
        //{
        //    get
        //    {
        //        return (this.CanaliAssociati != null) && this.CanaliAssociati.Any();
        //    }
        //}

        public string Matricola { get; set; }

        public string Nazione { get; set; }

        public string NomeUtente { get; set; }

        //public virtual ICollection<ProduttoreAssociato> ProduttoriAssociati { get; set; }

        public string Provincia { get; set; }

        public int IdResponsabile { get; set; }

        public string Telefono { get; set; }

        public string Cellulare { get; set; }

        public bool IsFakeUser { get; set; }
        
        public Utente BackupOf { get; set; }

        public virtual ICollection<Utente> BackupUsers { get; set; }

        public virtual ICollection<Utente> FakeUsers { get; set; }
    }
}
