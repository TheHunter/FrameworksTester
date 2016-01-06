using System.Globalization;

namespace EntityFrameworkTester
{
    public abstract class OrderLineBase
    {
        private decimal price;

        protected OrderLineBase()
        {            
            this.Coordinate = string.Empty;

            this.Subtype = string.Empty;
        }

        public decimal Id { get; set; }

        public decimal OrderId { get; set; }

        public string ProductCode { get; set; }

        public int Quantity { get; set; }

        public decimal Price
        {
            get
            {
                return this.price;
            }

            set
            {
                this.price = value;

                this.OnPriceSet();
            }
        }

        public string ProductDescription { get; set; }

        public string Warehouse { get; set; }

        public string Coordinate { get; set; }

        ////public string Type { get; set; }

        public string Subtype { get; set; }

        ////public string QtyPrice { get; set; }

        public int Bundle { get; set; }

        public int Supply { get; set; }

        public string Program { get; set; }

        public string PriceToString
        {
            get { return this.price.ToString(CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// Gets the price for quantity converted to string.
        /// </summary>
        public string PriceForQuantity
        {
            get { return (this.price * this.Quantity).ToString(CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// Occurs whenever the <see cref="Price"/> property is set.
        /// </summary>
        protected abstract void OnPriceSet();
   }
}
