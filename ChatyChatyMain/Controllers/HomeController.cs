﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatyChaty.Services.GoogleFirebase;
using Microsoft.AspNetCore.Mvc;

namespace ChatyChaty.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(FirstTest firstTest)
        {

        }
        [Route("/")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/HubClient")]
        [HttpGet]
        public IActionResult HubClient()
        {
            return View();
        }
    }
}