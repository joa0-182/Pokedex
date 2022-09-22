using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pokedex.Models;
using Pokedex.ViewModels;
using Pokedex.Data;
using System.Linq;

namespace Pokedex.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        ViewData["Types"] = _context.Types.ToList();
        var pokemons = _context.Pokemons
            .Include(p => p.Types)
            .ThenInclude(t => t.Type)
            .ToList();
        return View(pokemons);
    }

    public IActionResult Details(uint Number)
    {
        var current = _context.Pokemons
            .Include(p => p.Types).ThenInclude(t => t.Type)
            .Include(p => p.Generation).Include(p => p.Gender)
            .Where(p => p.Number == Number).SingleOrDefault();
        var prior = _context.Pokemons
            .OrderByDescending(p => p.Number)
            .Where(p => p.Number < Number).FirstOrDefault();
        var next = _context.Pokemons
            .OrderBy(p => p.Number)
            .Where(p => p.Number > Number).FirstOrDefault();
        var pokemon = new Details()
        {
            Prior = prior,
            Current = current,
            Next = next
        };
        return View(pokemon);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
