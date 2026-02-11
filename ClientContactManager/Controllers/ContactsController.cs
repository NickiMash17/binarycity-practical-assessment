using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClientContactManager.Data;
using ClientContactManager.Models;

namespace ClientContactManager.Controllers
{
    public class ContactsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ContactsController> _logger;

        public ContactsController(ApplicationDbContext context, ILogger<ContactsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Contacts
        public async Task<IActionResult> Index()
        {
            var contacts = await _context.Contacts
                .Include(c => c.ClientContacts)
                .OrderBy(c => c.Surname)
                .ThenBy(c => c.Name)
                .ToListAsync();

            return View(contacts);
        }

        // GET: Contacts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Contacts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Surname,Email")] Contact contact)
        {
            // Check email uniqueness
            if (await _context.Contacts.AnyAsync(c => c.Email == contact.Email))
            {
                ModelState.AddModelError("Email", "Email already exists");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(contact);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Contact created successfully";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating contact");
                    ModelState.AddModelError("", "An unexpected error occurred while creating the contact.");
                }
            }
            return View(contact);
        }

        // GET: Contacts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .Include(c => c.ClientContacts)
                    .ThenInclude(cc => cc.Client)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contact == null)
            {
                return NotFound();
            }

            // Get available clients for linking (clients not already linked)
            var linkedClientIds = contact.ClientContacts.Select(cc => cc.ClientId).ToList();
            ViewBag.AvailableClients = await _context.Clients
                .Where(c => !linkedClientIds.Contains(c.Id))
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(contact);
        }

        // POST: Contacts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Surname,Email")] Contact contact)
        {
            if (id != contact.Id)
            {
                return NotFound();
            }

            // Check email uniqueness (excluding current contact)
            if (await _context.Contacts.AnyAsync(c => c.Email == contact.Email && c.Id != contact.Id))
            {
                ModelState.AddModelError("Email", "Email already exists");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contact);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Contact updated successfully";
                    return RedirectToAction(nameof(Edit), new { id = contact.Id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating contact with id {ContactId}", id);
                    ModelState.AddModelError("", "An unexpected error occurred while updating the contact.");
                }
            }

            // Reload navigation properties
            var existingContact = await _context.Contacts
                .Include(c => c.ClientContacts)
                    .ThenInclude(cc => cc.Client)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingContact != null)
            {
                var linkedClientIds = existingContact.ClientContacts.Select(cc => cc.ClientId).ToList();
                ViewBag.AvailableClients = await _context.Clients
                    .Where(c => !linkedClientIds.Contains(c.Id))
                    .OrderBy(c => c.Name)
                    .ToListAsync();
            }

            return View(contact);
        }

        // POST: Contacts/LinkClient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkClient(int contactId, int clientId)
        {
            try
            {
                // Check if link already exists
                var existingLink = await _context.ClientContacts
                    .FirstOrDefaultAsync(cc => cc.ContactId == contactId && cc.ClientId == clientId);

                if (existingLink != null)
                {
                    TempData["ErrorMessage"] = "This client is already linked to the contact";
                    return RedirectToAction(nameof(Edit), new { id = contactId });
                }

                // Create new link
                var clientContact = new ClientContact
                {
                    ContactId = contactId,
                    ClientId = clientId
                };

                _context.ClientContacts.Add(clientContact);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Client linked successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error linking client {ClientId} to contact {ContactId}", clientId, contactId);
                TempData["ErrorMessage"] = "An unexpected error occurred while linking the client.";
            }

            return RedirectToAction(nameof(Edit), new { id = contactId });
        }

        // POST: Contacts/UnlinkClient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlinkClient(int contactId, int clientId)
        {
            try
            {
                var clientContact = await _context.ClientContacts
                    .FirstOrDefaultAsync(cc => cc.ContactId == contactId && cc.ClientId == clientId);

                if (clientContact != null)
                {
                    _context.ClientContacts.Remove(clientContact);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Client unlinked successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Link not found";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unlinking client {ClientId} from contact {ContactId}", clientId, contactId);
                TempData["ErrorMessage"] = "An unexpected error occurred while unlinking the client.";
            }

            return RedirectToAction(nameof(Edit), new { id = contactId });
        }

        // GET: Contacts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .Include(c => c.ClientContacts)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var contact = await _context.Contacts.FindAsync(id);
                if (contact != null)
                {
                    _context.Contacts.Remove(contact);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Contact deleted successfully";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting contact with id {ContactId}", id);
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the contact.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
            return _context.Contacts.Any(e => e.Id == id);
        }
    }
}
