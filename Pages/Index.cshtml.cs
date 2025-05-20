using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ICustomerRepository _customerRepository;

        private readonly IOrderRepository _orderRepository;

        private readonly IProductRepository _productRepository;

        public IList<Order> Orders { get; set; }

        public IList<Product> Products { get; set; }
        public IList<Order> OrdersByCustomer { get; set; }

        public IList<Customer> Customers { get; set; }

        public string username { get; set; }

        public IndexModel(ILogger<IndexModel> logger, ICustomerRepository customerRepository, IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _logger = logger;
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
            Products = new List<Product>();
            Customers = new List<Customer>();
            Orders = new List<Order>();
            OrdersByCustomer = new List<Order>();
        }

        public void OnGet()
        {
            Customers = _customerRepository.GetAllCustomers().ToList();
            Orders = _orderRepository.GetAllOrders().ToList();
            Products = _productRepository.GetAllProducts().ToList();
        }
    }
}
