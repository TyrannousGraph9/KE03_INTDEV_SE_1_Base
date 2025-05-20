using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class LoguitModel : PageModel
    {
        public LoguitModel()
        {

        }

        public void OnGet()
        {
            TempData.Clear();
        }
    }
}

