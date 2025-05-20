using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class BestellingoverzichtModel : PageModel
    {   
        
        private readonly IOrderRepository _orderRepository;
        public IList<Order> Orders { get; set; }
        public string Username { get; set; }

        private readonly IProductRepository _productRepository;
        public IList<Product> Products { get; set; }

        private readonly ICustomerRepository _customerRepository;
        public IList<Customer> Customers { get; set; }

        public List<Order> OrderDataForCustomer { get; set; } = new List<Order>();

        public BestellingoverzichtModel(IOrderRepository orderRepository, IProductRepository productRepository, ICustomerRepository customerRepository)
        {
            _productRepository = productRepository;
            Products = new List<Product>();
            _customerRepository = customerRepository;
            Customers = new List<Customer>();
            _orderRepository = orderRepository;
            Orders = new List<Order>();
        }

        public void OnGet()
        {
            Orders = _orderRepository.GetAllOrders().ToList();
            Username = TempData["username"]?.ToString() ?? string.Empty;
            List<Order> orderDataForCustomer = new List<Order>();
            for (int i = 0; i < Orders.Count; i++)
            {
                if (Orders[i].Customer != null && Orders[i].Customer.Name == Username)
                {
                    Orders[i].CustomerId = Orders[i].Customer.Id;
                    orderDataForCustomer.Add(Orders[i]);
                    Products = _productRepository.GetAllProducts().ToList();
                    Customers = _customerRepository.GetAllCustomers().ToList();
                    foreach (var product in Products)
                    {
                        if (Orders[i].Products.Contains(product))
                        {
                            Orders[i].Products.Add(product);
                        }
                    }
                }
            }
            OrderDataForCustomer = orderDataForCustomer;
        }
    }
}
