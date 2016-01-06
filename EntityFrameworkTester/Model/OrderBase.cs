using System;
using EntityFrameworkTester.Esprinet.Bcl;

namespace EntityFrameworkTester
{
    public abstract class OrderBase
    {                        
        protected OrderBase()
        {
            this.Creation = DateTime.Now;

            this.CarrierType = "03";

            this.Type = "I2";

            this.Coordinate = string.Empty;
            
            this.Carrier = string.Empty;

            this.Freight = "FR";

            this.GroupingOrders = "N";            

            this.SendingCompleted = "N";            

            this.Comments = string.Empty;            

            this.Payment = string.Empty;

            this.PaymentType = string.Empty;

            this.Destination = string.Empty;            

            this.DestinationName = string.Empty;

            this.DestinationAddress = string.Empty;

            this.DestinationPostalCode = string.Empty;

            this.DestinationCity = string.Empty;

            this.DestinationProvince = string.Empty;

            this.DestinationTelephone = string.Empty;            

            this.Orbpmi = string.Empty;
            
            this.CausaleLogo = string.Empty;                        
            
            this.BidVal = string.Empty;
            
            this.BidClif = string.Empty;

            this.BidAut = string.Empty;

            this.Seller = string.Empty;

            this.Nominativo = string.Empty;            

            this.NumeroBid = string.Empty;           

            this.Prenotazione = string.Empty;

            this.Processed = DateTime.Now;

            this.OrdineEndUser = false;
        }
        
        public decimal Id { get; set; }
        
        public string CompanyName { get; set; }
        
        public string CustomerCode { get; set; }
        
        public string UserCode { get; set; }
        
        public string Comments { get; set; }
        
        public DateTime Creation { get; set; }
       
        public string CarrierType { get; set; }
        
        public string Payment { get; set; }

        public string PaymentType { get; set; }

        public string Seller { get; set; }

        public string IpAddress { get; set; }

        public string CustomerReference { get; set; }        

        public string Type { get; set; }

        public DateTime? Processed { get; set; }

        public string Coordinate { get; set; }

        public string Carrier { get; set; }

        public string Freight { get; set; }

        public string GroupingOrders { get; set; }

        public string SendingCompleted { get; set; }

        public string DestinationId { get; set; }

        public string OrganizationType { get; set; }

        public IdAmbito ScopeId { get; set; }                

        public string Destination { get; set; }        

        public string DestinationName { get; set; }

        public string DestinationAddress { get; set; }

        public string DestinationPostalCode { get; set; }

        public string DestinationCity { get; set; }

        public string DestinationProvince { get; set; }

        public string DestinationTelephone { get; set; }            

        public string Orbpmi { get; set; }

        public string CausaleLogo { get; set; }        

        public string BidVal { get; set; }

        public string BidClif { get; set; }

        public string BidAut { get; set; }

        public string Nominativo { get; set; }                

        public string NumeroBid { get; set; }                

        public string Prenotazione { get; set; }

        public bool OrdineEndUser { get; set; }
    }
}
