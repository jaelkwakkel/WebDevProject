﻿using Microsoft.AspNetCore.Mvc;

namespace Setup.Controllers;

public class GameController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}