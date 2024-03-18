using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GamesDomain.Model;
using GamesInfrastructure;

namespace GamesInfrastructure.Controllers
{
    public class RatingsController : Controller
    {
        private readonly DbgamesContext _context;

        public RatingsController(DbgamesContext context)
        {
            _context = context;
        }

        // GET: Ratings
        public async Task<IActionResult> Index()
        {
            var dbgamesContext = _context.Ratings.Include(r => r.Game).Include(r => r.Player);
            return View(await dbgamesContext.ToListAsync());
        }

        // GET: Ratings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rating = await _context.Ratings
                .Include(r => r.Game)
                .Include(r => r.Player)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rating == null)
            {
                return NotFound();
            }

            return View(rating);
        }

        // GET: Ratings/Create
        public IActionResult Create()
        {
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Name");
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Name");
            return View();
        }

        // POST: Ratings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlayerId,GameId,Rating1,RatingDate,Id")] Rating rating)
        {
            rating.Game = _context.Games.Include(g => g.Developer).Where(g => g.Id == rating.GameId).FirstOrDefault();
            rating.Player = _context.Players.Where(p => p.Id == rating.PlayerId).FirstOrDefault();
            rating.RatingDate = DateTime.Now;
            if(rating.Rating1 < 0 || rating.Rating1 > 1)
            {
                ViewData["GameId"] = new SelectList(_context.Games, "Id", "Name", rating.GameId);
                ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Name", rating.PlayerId);
                return View(rating);
            }
            ModelState.Clear();
            TryValidateModel(rating);
            if (ModelState.IsValid)
            {
                _context.Add(rating);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Name", rating.GameId);
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Name", rating.PlayerId);
            return View(rating);
        }

        // GET: Ratings/Edit/5
        public async Task<IActionResult> Edit(int? id, int GameId, int PlayerId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
            {
                return NotFound();
            }
            ViewData["GameId"] = GameId;
            ViewData["PlayerId"] = PlayerId;
            return View(rating);
        }

        // POST: Ratings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int GameId, int PlayerId, [Bind("PlayerId,GameId,Rating1,RatingDate,Id")] Rating rating)
        {
            if (id != rating.Id)
            {
                return NotFound();
            }

            rating.GameId = GameId;
            rating.PlayerId = PlayerId;
            rating.Game = _context.Games.Include(g => g.Developer).Where(g => g.Id == rating.GameId).FirstOrDefault();
            rating.Player = _context.Players.Where(p => p.Id == rating.PlayerId).FirstOrDefault();
            rating.RatingDate = DateTime.Now;
            if (rating.Rating1 < 0 || rating.Rating1 > 1)
            {
                ViewData["GameId"] = new SelectList(_context.Games, "Id", "Name", rating.GameId);
                ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Name", rating.PlayerId);
                return View(rating);
            }
            ModelState.Clear();
            TryValidateModel(rating);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rating);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RatingExists(rating.Id))
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
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Name", rating.GameId);
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Name", rating.PlayerId);
            return View(rating);
        }

        // GET: Ratings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rating = await _context.Ratings
                .Include(r => r.Game)
                .Include(r => r.Player)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rating == null)
            {
                return NotFound();
            }

            return View(rating);
        }

        // POST: Ratings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating != null)
            {
                _context.Ratings.Remove(rating);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RatingExists(int id)
        {
            return _context.Ratings.Any(e => e.Id == id);
        }
    }
}
