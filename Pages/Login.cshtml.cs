using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class LoginModel : PageModel
    {

        private readonly ICustomerRepository _customerRepository;
        public IList<Customer> Customers { get; set; }
        public LoginModel(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
            Customers = new List<Customer>();
        }

        public void OnGet()
        {
            Customers = _customerRepository.GetAllCustomers().ToList();
            TempData.Clear();
        }

        public IActionResult OnPost()
        {
            string username = Request.Form["username"];
            Customers = _customerRepository.GetAllCustomers().ToList(); // Fetch customers again

            foreach (var customer in Customers)
            {
                if (customer.Name == username)
                {
                    TempData["username"] = username;
                    TempData.Keep("username");
                    return Redirect("/Index");
                }
            }
            TempData["error"] = "Er is geen juiste login gegeven.";
            return Redirect("/login");
        }
    }
}

