using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pokedex.Data;
using Pokedex.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Pokedex.Controllers
{
    public class PokemonsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public PokemonsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Pokemons
        public async Task<IActionResult> Index()
        {
            var pokemons = _context.Pokemons.Include(p => p.Gender).Include(p => p.Generation).Include(p => p.PokemonBase);
            return View(await pokemons.OrderBy(p => p.Number).ToListAsync());
        }

        // GET: Pokemons/Details/5
        public async Task<IActionResult> Details(uint? id)
        {
            if (id == null || _context.Pokemons == null)
            {
                return NotFound();
            }

            var pokemons = await _context.Pokemons
                .Include(p => p.Gender)
                .Include(p => p.Generation)
                .Include(p => p.PokemonBase)
                .Include(p => p.Types).ThenInclude(pt => pt.Type)
                .FirstOrDefaultAsync(m => m.Number == id);
            if (pokemons == null)
            {
                return NotFound();
            }

            return View(pokemons);
        }

        // GET: Pokemons/Create
        public IActionResult Create()
        {
            ViewData["GenderId"] = new SelectList(_context.Genders.OrderBy(g => g.Id), "Id", "Name");
            ViewData["GenerationId"] = new SelectList(_context.Generations.OrderBy(g => g.Id), "Id", "Name");
            ViewData["EvolvedFrom"] = new SelectList(_context.Pokemons.OrderBy(p => p.Number), "Number", "Name");
            ViewData["Types"] = new SelectList(_context.Types.OrderBy(t => t.Name), "Id", "Name");
            return View();
        }

        // POST: Pokemons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Number,EvolvedFrom,GenerationId,GenderId,Name,Description,Height,Weight,Image,AnimatedImg,SelectTypes")] Pokemons pokemons, IFormFile file, List<string> SelectTypes)
        {
            ViewData["GenderId"] = new SelectList(_context.Genders.OrderBy(g => g.Id), "Id", "Name");
            ViewData["GenerationId"] = new SelectList(_context.Generations.OrderBy(g => g.Id), "Id", "Name");
            ViewData["EvolvedFrom"] = new SelectList(_context.Pokemons.OrderBy(p => p.Number), "Number", "Name");
            ViewData["Types"] = new SelectList(_context.Types.OrderBy(t => t.Name), "Id", "Name");
            if (ModelState.IsValid)
            {
                if (PokemonsExists(pokemons.Number))
                {
                    ModelState.AddModelError("", "Número já cadastrado, verifique!");
                    return View(pokemons);
                }

                if (file != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = pokemons.Number.ToString("000") + Path.GetExtension(file.FileName);
                    string uploads = Path.Combine(wwwRootPath, @"img\pokemons");
                    string newFile = Path.Combine(uploads, fileName);
                    using (var stream = new FileStream(newFile, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    pokemons.Image = @"\img\pokemons\" + fileName;
                }
                pokemons.Types = new List<PokemonTypes>();
                foreach (var item in SelectTypes)
                {
                    pokemons.Types.Add(
                        new PokemonTypes
                        {
                            PokemonNumber = pokemons.Number,
                            TypeId = uint.Parse(item)
                        }
                    );
                }
                _context.Add(pokemons);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(pokemons);
        }

        // GET: Pokemons/Edit/5
        public async Task<IActionResult> Edit(uint? id)
        {
            if (id == null || _context.Pokemons == null)
            {
                return NotFound();
            }

            var pokemons = await _context.Pokemons.Include(p => p.Types).ThenInclude(t => t.Type)
                .Where(p => p.Number == id).SingleOrDefaultAsync();
            if (pokemons == null)
            {
                return NotFound();
            }
            ViewData["GenderId"] = new SelectList(_context.Genders.OrderBy(g => g.Id), "Id", "Name", pokemons.GenderId);
            ViewData["GenerationId"] = new SelectList(_context.Generations.OrderBy(g => g.Id), "Id", "Name", pokemons.GenerationId);
            ViewData["EvolvedFrom"] = new SelectList(_context.Pokemons.OrderBy(p => p.Number), "Number", "Name", pokemons.EvolvedFrom);
            ViewData["Types"] = new MultiSelectList(_context.Types.OrderBy(t => t.Name), "Id", "Name", pokemons.Types.Select(t => t.Type.Id.ToString()));
            return View(pokemons);
        }

        // POST: Pokemons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(uint id, [Bind("Number,EvolvedFrom,GenerationId,GenderId,Name,Description,Height,Weight,Image,AnimatedImg,SelectTypes")] Pokemons pokemons, IFormFile file, List<string> SelectTypes)
        {
            if (id != pokemons.Number)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (file != null)
                    {
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        if (pokemons.Image != null)
                        {
                            string oldFile = Path.Combine(wwwRootPath, pokemons.Image.TrimStart('\\'));
                            if (System.IO.File.Exists(oldFile))
                            {
                                System.IO.File.Delete(oldFile);
                            }
                        }

                        string fileName = pokemons.Number.ToString("000") + Path.GetExtension(file.FileName);
                        string uploads = Path.Combine(wwwRootPath, @"img\pokemons");
                        string newFile = Path.Combine(uploads, fileName);
                        using (var stream = new FileStream(newFile, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                        pokemons.Image = @"\img\pokemons\" + fileName;
                    }
                    pokemons.Types = _context.PokemonsTypes.Where(pt => pt.PokemonNumber == pokemons.Number).ToList();
                    _context.Update(pokemons);
                    _context.RemoveRange(pokemons.Types);
                    await _context.SaveChangesAsync();

                    pokemons.Types = new List<PokemonTypes>();
                    foreach (var item in SelectTypes)
                    {
                        _context.Add(
                            new PokemonTypes
                            {
                                PokemonNumber = pokemons.Number,
                                TypeId = uint.Parse(item)
                            }
                        );
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PokemonsExists(pokemons.Number))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenderId"] = new SelectList(_context.Genders, "Id", "Name", pokemons.GenderId);
            ViewData["GenerationId"] = new SelectList(_context.Generations, "Id", "Name", pokemons.GenerationId);
            ViewData["EvolvedFrom"] = new SelectList(_context.Pokemons, "Number", "Name", pokemons.EvolvedFrom);
            ViewData["Types"] = new MultiSelectList(_context.Types.OrderBy(t => t.Name), "Id", "Name", pokemons.Types.Select(t => t.Type.Id.ToString()));
            return View(pokemons);
        }

        // GET: Pokemons/Delete/5
        public async Task<IActionResult> Delete(uint? id)
        {
            if (id == null || _context.Pokemons == null)
            {
                return NotFound();
            }

            var pokemons = await _context.Pokemons
                .Include(p => p.Gender)
                .Include(p => p.Generation)
                .Include(p => p.PokemonBase)
                .Include(p => p.Types).ThenInclude(pt => pt.Type)
                .FirstOrDefaultAsync(m => m.Number == id);
            if (pokemons == null)
            {
                return NotFound();
            }

            return View(pokemons);
        }

        // POST: Pokemons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(uint id)
        {
            if (_context.Pokemons == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Pokemons'  is null.");
            }
            var pokemons = await _context.Pokemons.FindAsync(id);
            if (pokemons != null)
            {
                pokemons.Types = _context.PokemonsTypes.Where(pt => pt.PokemonNumber == pokemons.Number).ToList();
                _context.RemoveRange(pokemons.Types);
                pokemons.Abilities = _context.PokemonsAbilities.Where(pa => pa.PokemonNumber == pokemons.Number).ToList();
                _context.RemoveRange(pokemons.Abilities);
                pokemons.Weaknesses = _context.Weaknesses.Where(w => w.PokemonNumber == pokemons.Number).ToList();
                _context.RemoveRange(pokemons.Weaknesses);
                _context.Pokemons.Remove(pokemons);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PokemonsExists(uint id)
        {
            return (_context.Pokemons?.Any(e => e.Number == id)).GetValueOrDefault();
        }
    }
}
