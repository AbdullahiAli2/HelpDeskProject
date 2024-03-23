using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HelpDeskProject.Data;
using HelpDeskProject.Models;
using Microsoft.AspNetCore.Authorization;

namespace HelpDeskProject.Controllers
{

    [Authorize]
    public class ErrorLogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ErrorLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ErrorLogs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ErrorLogs.Include(e => e.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ErrorLogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var errorLog = await _context.ErrorLogs
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.ErrorID == id);
            if (errorLog == null)
            {
                return NotFound();
            }

            return View(errorLog);
        }

        // GET: ErrorLogs/Create
        public IActionResult Create()
        {
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: ErrorLogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ErrorID,UserID,ErrorDescription,LineNumber,Traceback,Timestamp,BrowserInfo,IPAddress")] ErrorLog errorLog)
        {
            if (ModelState.IsValid)
            {
                _context.Add(errorLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", errorLog.UserID);
            return View(errorLog);
        }

        // GET: ErrorLogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var errorLog = await _context.ErrorLogs.FindAsync(id);
            if (errorLog == null)
            {
                return NotFound();
            }
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", errorLog.UserID);
            return View(errorLog);
        }

        // POST: ErrorLogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ErrorID,UserID,ErrorDescription,LineNumber,Traceback,Timestamp,BrowserInfo,IPAddress")] ErrorLog errorLog)
        {
            if (id != errorLog.ErrorID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(errorLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ErrorLogExists(errorLog.ErrorID))
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
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", errorLog.UserID);
            return View(errorLog);
        }

        // GET: ErrorLogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var errorLog = await _context.ErrorLogs
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.ErrorID == id);
            if (errorLog == null)
            {
                return NotFound();
            }

            return View(errorLog);
        }

        // POST: ErrorLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var errorLog = await _context.ErrorLogs.FindAsync(id);
            if (errorLog != null)
            {
                _context.ErrorLogs.Remove(errorLog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ErrorLogExists(int id)
        {
            return _context.ErrorLogs.Any(e => e.ErrorID == id);
        }
    }
}
