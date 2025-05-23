using DataAccessLayer;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class BestellingoverzichtModel : PageModel
    {   
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly MatrixIncDbContext _context;
        
        public IList<Order> Orders { get; set; } = new List<Order>();
        public string Username { get; set; } = string.Empty;
        public IList<Product> Products { get; set; } = new List<Product>();
        public IList<Customer> Customers { get; set; } = new List<Customer>();
        public List<OrderWithDetails> OrderDataForCustomer { get; set; } = new List<OrderWithDetails>();

        public class OrderProductQueryResult
        {
            public int ProductId { get; set; }
            public string Name { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public int Amount { get; set; }
        }

        public class OrderWithDetails
        {
            public required Order Order { get; set; }
            public List<OrderProductDetail> ProductDetails { get; set; } = new List<OrderProductDetail>();
            public decimal TotalOrderPrice => ProductDetails.Sum(p => p.TotalPrice);
        }

        public class OrderProductDetail
        {
            public required string ProductName { get; set; }
            public int Amount { get; set; }
            public decimal Price { get; set; }
            public decimal TotalPrice => Amount * Price;
        }

        public BestellingoverzichtModel(
            IOrderRepository orderRepository, 
            IProductRepository productRepository, 
            ICustomerRepository customerRepository,
            MatrixIncDbContext context)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;
            _context = context;
        }

        public void OnGet()
        {
            Username = TempData["username"]?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(Username))
            {
                return;
            }


            var orders = _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.Customer.Name == Username)
                .ToList();

            foreach (var order in orders)
            {
                var orderDetails = new OrderWithDetails { Order = order };
                
     
                var productDetails = _context.Database
                    .SqlQuery<OrderProductQueryResult>($@"
                        SELECT 
                            p.Id as ProductId,
                            p.Name,
                            p.Price,
                            op.Amount
                        FROM OrderProducts op
                        JOIN Products p ON p.Id = op.ProductId
                        WHERE op.OrderId = {order.Id}")
                    .ToList();

                foreach (var detail in productDetails)
                {
                    orderDetails.ProductDetails.Add(new OrderProductDetail
                    {
                        ProductName = detail.Name,
                        Amount = detail.Amount,
                        Price = detail.Price
                    });
                }

                OrderDataForCustomer.Add(orderDetails);
            }
        }
    }
}
