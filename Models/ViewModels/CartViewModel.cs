using Miocas.Models;

namespace Miocas.Models.ViewModels
{
    public class CartViewModel
    {
        public List<ItemCart> ItemCarts { get; set; }

        public decimal GrandTotal { get; set; }

        public string UserId { get; set; }  // Добавьте UserId для связи с пользователем
    }
}
