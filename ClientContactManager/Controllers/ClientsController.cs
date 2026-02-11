using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClientContactManager.Data;
using ClientContactManager.Models;
using ClientContactManager.Services;

namespace ClientContactManager.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ClientCodeGenerator _codeGenerator;

        public ClientsController(ApplicationDbContext context, ClientCodeGenerator codeGenerator)
        {
            _context = context;
            _codeGenerator = codeGenerator;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            var clients = await _context.Clients
                .Include(c => c.ClientContacts)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(clients);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Client client)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Generate unique client code
                    client.ClientCode = await _codeGenerator.GenerateClientCodeAsync(client.Name);

                    _context.Add(client);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Client created successfully with code: {client.ClientCode}";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating client: {ex.Message}");
                }
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .Include(c => c.ClientContacts)
                    .ThenInclude(cc => cc.Contact)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null)
            {
                return NotFound();
            }

            // Get available contacts for linking (contacts not already linked)
            var linkedContactIds = client.ClientContacts.Select(cc => cc.ContactId).ToList();
            ViewBag.AvailableContacts = await _context.Contacts
                .Where(c => !linkedContactIds.Contains(c.Id))
                .OrderBy(c => c.Surname)
                .ThenBy(c => c.Name)
                .ToListAsync();

            return View(client);
        }

        // POST: Clients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ClientCode")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Load existing client to preserve the client code
                    var existingClient = await _context.Clients.FindAsync(id);
                    if (existingClient == null)
                    {
                        return NotFound();
                    }

                    // Only update the name, keep the original client code
                    existingClient.Name = client.Name;
                    
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Client updated successfully";
                    return RedirectToAction(nameof(Edit), new { id = client.Id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
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
                    ModelState.AddModelError("", $"Error updating client: {ex.Message}");
                }
            }

            // Reload navigation properties
            var reloadedClient = await _context.Clients
                .Include(c => c.ClientContacts)
                    .ThenInclude(cc => cc.Contact)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (reloadedClient != null)
            {
                var linkedContactIds = reloadedClient.ClientContacts.Select(cc => cc.ContactId).ToList();
                ViewBag.AvailableContacts = await _context.Contacts
                    .Where(c => !linkedContactIds.Contains(c.Id))
                    .OrderBy(c => c.Surname)
                    .ThenBy(c => c.Name)
                    .ToListAsync();
            }

            return View(client);
        }

        // POST: Clients/LinkContact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkContact(int clientId, int contactId)
        {
            try
            {
                // Check if link already exists
                var existingLink = await _context.ClientContacts
                    .FirstOrDefaultAsync(cc => cc.ClientId == clientId && cc.ContactId == contactId);

                if (existingLink != null)
                {
                    TempData["ErrorMessage"] = "This contact is already linked to the client";
                    return RedirectToAction(nameof(Edit), new { id = clientId });
                }

                // Create new link
                var clientContact = new ClientContact
                {
                    ClientId = clientId,
                    ContactId = contactId
                };

                _context.ClientContacts.Add(clientContact);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Contact linked successfully";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error linking contact: {ex.Message}";
            }

            return RedirectToAction(nameof(Edit), new { id = clientId });
        }

        // GET: Clients/UnlinkContact
        public async Task<IActionResult> UnlinkContact(int clientId, int contactId)
        {
            try
            {
                var clientContact = await _context.ClientContacts
                    .FirstOrDefaultAsync(cc => cc.ClientId == clientId && cc.ContactId == contactId);

                if (clientContact != null)
                {
                    _context.ClientContacts.Remove(clientContact);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Contact unlinked successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Link not found";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error unlinking contact: {ex.Message}";
            }

            return RedirectToAction(nameof(Edit), new { id = clientId });
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .Include(c => c.ClientContacts)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var client = await _context.Clients.FindAsync(id);
                if (client != null)
                {
                    _context.Clients.Remove(client);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Client deleted successfully";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting client: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}
