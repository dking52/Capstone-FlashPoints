﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FlashPoints.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
    }
}