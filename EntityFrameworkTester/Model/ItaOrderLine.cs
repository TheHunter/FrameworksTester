namespace EntityFrameworkTester
{
   public class ItaOrderLine : OrderLineBase
   {      
      public decimal PriceDb { get; set; }

      public string Sconto { get; set; }

      public string Promo { get; set; }

       protected override void OnPriceSet()
       {
           throw new System.NotImplementedException();
       }
   }
}