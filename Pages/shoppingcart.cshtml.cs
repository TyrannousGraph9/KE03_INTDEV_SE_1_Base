using DataAccessLayer;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class ShoppingCartModel : PageModel
    {
        private readonly IProductRepository _productRepository;
        private readonly MatrixIncDbContext _context;
        public IList<Product> Products { get; set; }
        public List<(int ProductId, int Amount, decimal Price, string ProductName)> ShoppingCartContents { get; set; } = new List<(int, int, decimal, string)>();

        public ShoppingCartModel(IProductRepository productRepository, MatrixIncDbContext context)
        {
            _productRepository = productRepository;
            Products = new List<Product>();
            _context = context;
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
                        }
                    }
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!TempData.TryGetValue("username", out var usernameObj) || usernameObj is null)
            {
                return RedirectToPage("Login");
            }

            var customer = _context.Customers.FirstOrDefault(c => c.Name == usernameObj.ToString());
            if (customer == null)
            {
                return RedirectToPage("Login");
            }

            string cookieName = "shoppingcart";
            ShoppingCartContents.Clear();
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
                        }
                    }
                }
            }

            var order = new Order
            {
                OrderDate = DateTime.Now,
                CustomerId = customer.Id,
                Customer = customer
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); 

            foreach(var item in ShoppingCartContents)
            {
                var product = _productRepository.GetProductById(item.ProductId);
                if (product != null)
                {
                    var relationship = new
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Amount = item.Amount
                    };
                    await _context.Database.ExecuteSqlInterpolatedAsync(
                        $"INSERT INTO OrderProducts (OrderId, ProductId, Amount) VALUES ({relationship.OrderId}, {relationship.ProductId}, {relationship.Amount})");
                }
            }

            await _context.SaveChangesAsync();

            Response.Cookies.Delete("shoppingcart");
            return RedirectToPage("bestellinggeplaatst");
        }
    }
}
