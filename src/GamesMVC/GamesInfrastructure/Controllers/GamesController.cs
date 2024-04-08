using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GamesDomain.Model;
using GamesInfrastructure;
using Humanizer.Localisation;

namespace GamesInfrastructure.Controllers
{
    public class GamesController : Controller
    {
        private readonly DbgamesContext _context;

        public GamesController(DbgamesContext context)
        {
            _context = context;
        }

        // GET: Games
        public async Task<IActionResult> Index(int? id, string? name)
        {
            if(id == null) return RedirectToAction("Genres","Index");
            ViewBag.GenreId = id;
            ViewBag.GenreName = name.Trim();
            var gamesByGenre = _context.Games.Include(g => g.Developer).Include(g => g.Genres).Where(g => g.Genres.Any(genre => genre.Id == id));
            return View(await gamesByGenre.ToListAsync());
            //var dbgamesContext = _context.Games.Include(g => g.Developer);
            //return View(await dbgamesContext.ToListAsync());
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id, int GenreId, string GenreName)
        {
            ViewBag.GenreId = GenreId;
            ViewBag.GenreName = GenreName;
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Developer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: Games/Create
        public IActionResult Create(int genreId)
        {
            ViewData["DeveloperId"] = new SelectList(_context.Developers, "Id", "Name");
            ViewBag.DeveloperId = new SelectList(_context.Developers, "Id", "Name");
            var genre = _context.Genres.FirstOrDefault(g => g.Id == genreId);
            ViewBag.GenreName = genre.Name;
            ViewBag.GenreId = genreId;
            return View();
        }

        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int genreId, int DeveloperId, Game game)
        {
            var existingGame = _context.Games.FirstOrDefault(g => g.Name.Trim() == game.Name.Trim() && g.ReleaseDate == game.ReleaseDate);
            if (existingGame != null)
            {
                ModelState.AddModelError("Name", "Game with this name and release date already exists.");
                ViewData["DeveloperId"] = new SelectList(_context.Developers, "Id", "Name");
                ViewBag.DeveloperId = new SelectList(_context.Developers, "Id", "Name");
                var genre1 = _context.Genres.FirstOrDefault(g => g.Id == genreId);
                ViewBag.GenreName = genre1.Name;
                ViewBag.GenreId = genreId;
                return View();
            }

            var developer = _context.Developers.FirstOrDefault(d => d.Id == DeveloperId);
            game.DeveloperId = DeveloperId;
            game.Developer = developer;

            var genre = _context.Genres.FirstOrDefault(g => g.Id == genreId);
            game.Genres.Add(genre);

            ModelState.Clear();
            TryValidateModel(game);

            if (ModelState.IsValid)
            {
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Games", new { id = genreId, name = genre.Name });
            }

            return View();
        }

        public IActionResult Add(int genreId)
        {
            var gamesWithoutGenre = _context.Games.Include(g => g.Developer).Include(g => g.Genres).Where(g => !g.Genres.Any(genre => genre.Id == genreId)).ToList();

            ViewData["GameId"] = new SelectList(gamesWithoutGenre, "Id", "Name");
            ViewBag.GameId = new SelectList(gamesWithoutGenre, "Id", "Name");

            var genre = _context.Genres.FirstOrDefault(g => g.Id == genreId);
            ViewBag.GenreName = genre.Name;
            ViewBag.GenreId = genreId;

            return View(gamesWithoutGenre);
        }

        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int GenreId, int DeveloperId, [Bind("Id")]Game game)
        {
            var gameInfo = _context.Games.AsNoTracking().FirstOrDefault(g => g.Id == game.Id);

            game.ReleaseDate = gameInfo.ReleaseDate;
            game.Name = gameInfo.Name;
            var developer = _context.Developers.Where(d => d.Id == DeveloperId).FirstOrDefault();
            game.DeveloperId = DeveloperId;
            game.Developer = developer;

            var gameGenres = gameInfo.Genres;
            foreach (var item in gameGenres)
            {
                game.Genres.Add(item);
            }

            var genre = _context.Genres.Where(g => g.Id == GenreId).FirstOrDefault();
            game.Genres.Add(genre);

            ModelState.Clear();
            TryValidateModel(game);

            if (ModelState.IsValid)
            {
                _context.Update(game);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Games", new { id = GenreId, name = genre.Name });
            }

            return View();
        }

        // GET: Games/Edit/5
        public async Task<IActionResult> Edit(int? id, int GenreId, string GenreName)
        {
            ViewBag.GenreId = GenreId;
            ViewBag.GenreName = GenreName;
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            ViewData["DeveloperId"] = new SelectList(_context.Developers, "Id", "Name", game.DeveloperId);
            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int GenreId, string GenreName, [Bind("Id","DeveloperId","Name","ReleaseDate","GenreId")]Game game)
        {
            var existingGame = _context.Games.FirstOrDefault(g => g.Name.Trim() == game.Name.Trim() && g.ReleaseDate == game.ReleaseDate);
            if (existingGame != null)
            {
                ModelState.AddModelError("Name", "Game with this name and release date already exists.");
                ViewData["DeveloperId"] = new SelectList(_context.Developers, "Id", "Name");
                ViewBag.DeveloperId = new SelectList(_context.Developers, "Id", "Name");
                var genre1 = _context.Genres.FirstOrDefault(g => g.Id == GenreId);
                ViewBag.GenreName = genre1.Name;
                ViewBag.GenreId = GenreId;
                return View();
            }

            var developer = await _context.Developers.Where(d => d.Id == game.DeveloperId).FirstOrDefaultAsync();
            game.Developer = developer;
            ModelState.Clear();
            TryValidateModel(game);

            if (id != game.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Games", new { id = GenreId, name = GenreName });
            }
            ViewData["DeveloperId"] = new SelectList(_context.Developers, "Id", "Name", game.DeveloperId);
            return View(game);
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id, int GenreId, string GenreName)
        {
            ViewBag.GenreId = GenreId;
            ViewBag.GenreName = GenreName;
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Developer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int GenreId, string GenreName)
        {
            var game = await _context.Games
                .Include(g => g.Genres)
                .Include(g => g.Comments)
                .Include(g => g.Ratings)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game != null)
            {
                _context.Comments.RemoveRange(game.Comments);

                _context.Ratings.RemoveRange(game.Ratings);

                foreach (var genre in game.Genres)
                {
                    genre.Games.Remove(game);
                }

                _context.Games.Remove(game);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Games", new { id = GenreId, name = GenreName });
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("DeleteFromGenre")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFromGenreConfirmed(int id, int GenreId, string GenreName)
        {
            var game = await _context.Games.Include(g => g.Genres).FirstOrDefaultAsync(g => g.Id == id);

            if (game != null)
            {
                foreach (var genre in game.Genres)
                {
                    if(genre.Id == GenreId)
                        genre.Games.Remove(game);
                }

                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Games", new { id = GenreId, name = GenreName });
        }

        private bool GameExists(int id)
        {
            return _context.Games.Any(e => e.Id == id);
        }
    }
}
