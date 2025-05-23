using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class productenoverzichtModel : PageModel
    {
        private readonly IProductRepository _productRepository;
        public IList<Product> Products { get; set; }

        private readonly IPartRepository _partRepository;
        public IList<Part> Parts { get; set; }

        public productenoverzichtModel(IProductRepository productRepository, IPartRepository partRepository)
        {
            _productRepository = productRepository;
            _partRepository = partRepository;
            Products = new List<Product>();
            Parts = new List<Part>();

        }

        public void OnGet()
        {
            Products = _productRepository.GetAllProducts().ToList();
            Parts = _partRepository.GetAllParts().ToList();
            foreach (var product in Products)
            {
                if (product.Parts != null)
                {
                    foreach (var part in product.Parts)
                    {
                        var partFromRepo = _partRepository.GetPartById(part.Id);
                        if (partFromRepo != null)
                        {
                            product.Parts.Add(partFromRepo);
                        }
                    }
                }
            }
        }
        
        public IActionResult OnPost()
        {
            if (!Request.Form.TryGetValue("productId", out var productIdValue) ||
                !int.TryParse(productIdValue, out int productId))
            {
                return BadRequest("Invalid or missing productId.");
            }

            if (!Request.Form.TryGetValue("amount", out var amountValue) ||
                !int.TryParse(amountValue, out int amount))
            {
                return BadRequest("Invalid or missing amount.");
            }

            var product = _productRepository.GetProductById(productId);
            if (product == null)
            {
                return NotFound();
            }

            string cookieName = "shoppingcart";
            List<string> entries = new List<string>();
            bool found = false;

            if (Request.Cookies.TryGetValue(cookieName, out string? existingValue) && !string.IsNullOrEmpty(existingValue))
            {
                entries = existingValue.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();

                for (int i = 0; i < entries.Count; i++)
                {
                    var parts = entries[i].Split(':', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2 && int.TryParse(parts[0], out int entryId) && entryId == productId)
                    {
                        // Update the amount by adding
                        if (int.TryParse(parts[1], out int oldAmount))
                        {
                            int newAmount = oldAmount + amount;
                            entries[i] = $"{productId}:{newAmount}";
                            found = true;
                            break;
                        }
                    }
                }

                if (!found)
                {
                    entries.Add($"{productId}:{amount}");
                }

                Response.Cookies.Delete(cookieName);
                Response.Cookies.Append(cookieName, string.Join(";", entries));
            }
            else
            {
                // Create a new cookie
                Response.Cookies.Append(cookieName, $"{productId}:{amount}");
            }

            return RedirectToPage("/shoppingcart");
        }
    }
}