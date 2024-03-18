using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GamesDomain.Model;
using GamesInfrastructure;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace GamesInfrastructure.Controllers
{
    public class CommentsController : Controller
    {
        private readonly DbgamesContext _context;

        public CommentsController(DbgamesContext context)
        {
            _context = context;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            var dbgamesContext = _context.Comments.Include(c => c.Game).Include(c => c.Player);
            return View(await dbgamesContext.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Game)
                .Include(c => c.Player)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comments/Create
        public IActionResult Create()
        {
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Name");
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Name");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Comment comment)
        {
            comment.CommentDate = DateTime.Now;
            comment.Player = _context.Players.Where(p => p.Id == comment.PlayerId).FirstOrDefault();
            comment.Game = _context.Games.Include(g => g.Developer).Where(p => p.Id == comment.GameId).FirstOrDefault();
            ModelState.Clear();
            TryValidateModel(comment);

            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Name", comment.GameId);
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Name", comment.PlayerId);
            return View(comment);
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Name", comment.GameId);
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Name", comment.PlayerId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlayerId,GameId,Text,CommentDate,Id")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
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
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Name", comment.GameId);
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Name", comment.PlayerId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Game)
                .Include(c => c.Player)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = _context.Comments
                .Include(c => c.Game)
                .Include(c => c.Player)
                .FirstOrDefault(m => m.Id == id);
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
            if (comment != null)
            {
                _context.Comments.Remove(comment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
