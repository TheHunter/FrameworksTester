using System.Collections.Generic;

namespace EntityFrameworkTester
{
    public class ItaOrder : OrderBase
    {
        private HashSet<ItaOrderLine> lines;

        public ItaOrder()
        {
            this.lines = new HashSet<ItaOrderLine>();
        }

        public string CustomerAddress { get; set; }

        public string CustomerCity { get; set; }

        public string CustomerProvince { get; set; }

        public string PartitaIva { get; set; }

        public string CustomerTelephone { get; set; }

        public string CustomerFax { get; set; }

        public string CustomerMail { get; set; }

        public string CustomerPostalCode { get; set; }

        public string CommentType { get; set; }

        public string Valuta { get; set; }

        public string TipoBid { get; set; }

        public string ClienteFatturazione { get; set; }

        public string CodiceAssotrade { get; set; }

        public string OrdineFiglio { get; set; }

        public string Pod { get; set; }

        public string Privacy { get; set; }

        public string Cig { get; set; }

        public string Cup { get; set; }

        public virtual ICollection<ItaOrderLine> Lines
        {
            get { return this.lines; }
            set { this.lines = new HashSet<ItaOrderLine>(value); }
        }
    }
}
