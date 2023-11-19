namespace Miocas.Models
{
    public class Order
    {
        public long Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public DateTime OrderDate { get; set; }

        // Дополнительные поля заказа, если необходимо

        public List<OrderItem> OrderItems { get; set; }
    }
}
