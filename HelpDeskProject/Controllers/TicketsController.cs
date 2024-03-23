using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HelpDeskProject.Data;
using HelpDeskProject.Models;
using System.Security.Claims;
using HelpDeskProject.Services;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Xml.Linq;

namespace HelpDeskProject.Controllers
{

    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            var tickets = await _context.Tickets.Include(t => t.CreatedBy).ToListAsync();

            return View(tickets);
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.CreatedBy)
                .FirstOrDefaultAsync(m => m.TicketID == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FulllName");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ticket ticket)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {


                ticket.CreatedOn = DateTime.Now;
                ticket.CreatedById = userId;
                _context.Add(ticket);
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

                TempData["MESSAGE"] = "Ticket successfully created";

                return RedirectToAction(nameof(Index));
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
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FulllName", ticket.CreatedById);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FulllName", ticket.CreatedById);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ticket ticket)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id != ticket.TicketID)
            {
                return NotFound();
            }

            try
            {
                ticket.CreatedOn = DateTime.Now;
                ticket.CreatedById = userId;
                _context.Update(ticket);
                await _context.SaveChangesAsync();

                TempData["MESSAGE"] = "Ticket successfully Updated";

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
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FulllName", ticket.CreatedById);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.CreatedBy)
                .FirstOrDefaultAsync(m => m.TicketID == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                // Remove all the comments
                var comments = await _context.Comments.Where(x=>x.TicketID==id).ToListAsync();
                _context.Comments.RemoveRange(comments);
                await _context.SaveChangesAsync();


                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();

                TempData["MESSAGE"] = "Ticket successfully Deleted";
            }

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


            return RedirectToAction(nameof(Index));

        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.TicketID == id);
        }
    }
}
