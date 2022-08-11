using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FacebookLoginExample.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Page1()
        {
            return View();
        }
        [Authorize]
        public IActionResult Page2()
        {
            return View();
        }
        [Authorize]
        public IActionResult Page3()
        {
            return View();
        }
    }
}