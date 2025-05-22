using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class ShoppingCartModel : PageModel
    {
        private readonly IProductRepository _productRepository;
        public IList<Product> Products { get; set; }



        public List<(int ProductId, int Amount, decimal Price, string ProductName)> ShoppingCartContents { get; set; } = new List<(int, int, decimal, string)>();
        public ShoppingCartModel(IProductRepository productRepository)
        {
            _productRepository = productRepository;
            Products = new List<Product>();
        }

        public void OnGet()
        {
            string cookieName = "shoppingcart";
            if (Request.Cookies.TryGetValue(cookieName, out string? existingValue) && !string.IsNullOrEmpty(existingValue))
            {
                string[] entries = existingValue.Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach (var entry in entries)
                {
                    string[] parts = entry.Split(':', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2 &&
                    int.TryParse(parts[0], out int productId) &&
                    int.TryParse(parts[1], out int amount))
                    {
                        var product = _productRepository.GetProductById(productId);
                        if (product != null)
                        {
                            ShoppingCartContents.Add((productId, amount, product.Price, product.Name));  
                            Console.WriteLine(ShoppingCartContents);
                        }
                    }
                }

            }
        }

        public void OnPost()
        {
            
        }
    }
}
