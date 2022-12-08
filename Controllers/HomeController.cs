using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pokedex.Models;
using Pokedex.Data;
using Pokedex.ViewModels;

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

    public IActionResult Index(string type)
    {
        var index = new IndexVM(){
            Types =  _context.Types.ToList(),
            Pokemons = _context.Pokemons.OrderBy(p => p.Number)
                .Include(p => p.Types).ThenInclude(pt => pt.Type).ToList()
        };
        ViewData["Filter"] = type ?? "all";
        return View(index);
    }

    public IActionResult Details(uint Number)
    {
        var current = _context.Pokemons
            .Include(p => p.Types).ThenInclude(pt => pt.Type)
            .Include(p => p.Gender).Include(p => p.Generation)
            .Where(p => p.Number == Number).SingleOrDefault();
        var prior = _context.Pokemons.OrderByDescending(p => p.Number)
            .FirstOrDefault(p => p.Number < Number);
        var next = _context.Pokemons.OrderBy(p => p.Number)
            .FirstOrDefault(p => p.Number > Number);
        
        var pokemon = new DetailsVM()
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
