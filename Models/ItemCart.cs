namespace Miocas.Models
{
    public class ItemCart
    {
        public long Id { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Total
        {
            get
            { return Quantity * Price; }
        }

        public byte[] Images { get; set; }


        public string UserId { get; set; }
        public User User { get; set; }

        public long? OrderId { get; set; }
        public Order Order { get; set; }

        public ItemCart() { }

        public ItemCart(Product product)
        {
            Id = product.Id;
            ProductName = product.Name;
            Quantity = 1;
            Price = product.Price;
            Images = product.Image;
        }
    }
}
