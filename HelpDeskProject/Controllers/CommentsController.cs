using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HelpDeskProject.Data;
using HelpDeskProject.Models;
using System.Net.Sockets;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations.Schema;
using NuGet.Protocol;

namespace HelpDeskProject.Controllers
{

    [Authorize]
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            var comments = await _context.Comments.Include(c => c.Ticket).Include(c => c.User).ToListAsync();
            return View(comments);
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Ticket)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CommentID == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comments/Create
        public IActionResult Create()
        {
            ViewData["TicketID"] = new SelectList(_context.Tickets, "TicketID", "Title");
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "FullName");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Comment comment)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {

                comment.CreatedAt = DateTime.Now;
                comment.UserID = userId;

                _context.Add(comment);
                await _context.SaveChangesAsync();

                //Activity Logs
                var activity = new ActivityLog
                {
                    Action = "Create",
                    Timestamp = DateTime.Now,
                    BrowserInfo = Request.Headers["User-Agent"].ToString(),
                    IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserID = userId
                };
                _context.Add(activity);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));


                TempData["MESSAGE"] = "Ticket successfully Created";

            }
            catch (Exception ex)
            {
                //Error Logs
                var logs = new ErrorLog
                {
                    UserID = userId,
                    ErrorDescription = ex.Message,
                    LineNumber = 1,
                    IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Traceback = ex.InnerException.ToString(),
                    Timestamp = DateTime.Now,
                    BrowserInfo = Request.Headers["User-Agent"].ToString(),
                };
                _context.Add(logs);
                await _context.SaveChangesAsync();


            }

            ViewData["TicketID"] = new SelectList(_context.Tickets, "TicketID", "Title", comment.TicketID);
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "FullName", comment.UserID);

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
            ViewData["TicketID"] = new SelectList(_context.Tickets, "TicketID", "Priority", comment.TicketID);
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "FullName", comment.UserID);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Comment comment)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id != comment.CommentID)
            {
                return NotFound();
            }

            try
            {

                comment.CreatedAt = DateTime.Now;
                comment.UserID = userId;

                _context.Update(comment);
                await _context.SaveChangesAsync();

                //Activity Logs
                var activity = new ActivityLog
                {
                    Action = "Update",
                    Timestamp = DateTime.Now,
                    BrowserInfo = Request.Headers["User-Agent"].ToString(),
                    IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserID = userId
                };
                _context.Add(activity);
                await _context.SaveChangesAsync();

                TempData["MESSAGE"] = "Ticket successfully Created";

            }
            catch (Exception ex)
            {
                //Error Logs
                var logs = new ErrorLog
                {
                    UserID = userId,
                    ErrorDescription = ex.Message,
                    LineNumber = 1,
                    IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Traceback = ex.InnerException.ToString(),
                    Timestamp = DateTime.Now,
                    BrowserInfo = Request.Headers["User-Agent"].ToString(),
                };
                _context.Add(logs);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));

            ViewData["TicketID"] = new SelectList(_context.Tickets, "TicketID", "Priority", comment.TicketID);
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", comment.UserID);
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
                .Include(c => c.Ticket)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CommentID == id);
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
            }

            await _context.SaveChangesAsync();
            //Activity Logs
            var activity = new ActivityLog
            {
                Action = "Delete",
                Timestamp = DateTime.Now,
                BrowserInfo = Request.Headers["User-Agent"].ToString(),
                IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserID = userId
            };
            _context.Add(activity);
            await _context.SaveChangesAsync();

            TempData["MESSAGE"] = "Ticket successfully Deleted";

            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.CommentID == id);
        }
    }
}
