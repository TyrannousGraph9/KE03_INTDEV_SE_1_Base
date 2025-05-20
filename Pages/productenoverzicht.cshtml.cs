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
    }
}
