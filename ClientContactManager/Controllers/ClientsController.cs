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
        private readonly ILogger<ClientsController> _logger;

        public ClientsController(
            ApplicationDbContext context,
            ClientCodeGenerator codeGenerator,
            ILogger<ClientsController> logger)
        {
            _context = context;
            _codeGenerator = codeGenerator;
            _logger = logger;
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
            // ClientCode is generated server-side, so remove it from model validation.
            ModelState.Remove(nameof(Client.ClientCode));

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
                    _logger.LogError(ex, "Error creating client");
                    ModelState.AddModelError("", "An unexpected error occurred while creating the client.");
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
                    _logger.LogError(ex, "Error updating client with id {ClientId}", id);
                    ModelState.AddModelError("", "An unexpected error occurred while updating the client.");
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
                _logger.LogError(ex, "Error linking contact {ContactId} to client {ClientId}", contactId, clientId);
                TempData["ErrorMessage"] = "An unexpected error occurred while linking the contact.";
            }

            return RedirectToAction(nameof(Edit), new { id = clientId });
        }

        // POST: Clients/UnlinkContact
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                _logger.LogError(ex, "Error unlinking contact {ContactId} from client {ClientId}", contactId, clientId);
                TempData["ErrorMessage"] = "An unexpected error occurred while unlinking the contact.";
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
                _logger.LogError(ex, "Error deleting client with id {ClientId}", id);
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the client.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}
