using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using userscan.Data;
using userscan.Models;

namespace UserScan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly ContactsDbContext _context;
        private readonly HttpClient _httpClient;

        public ContactsController(ContactsDbContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
        }

        // GET: api/contacts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContacts()
        {
            return await _context.Contacts.ToListAsync();
        }

        // GET: api/contacts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContact(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
                return NotFound();

            return contact;
        }

        // POST: api/contacts
        [HttpPost]
        public async Task<ActionResult<Contact>> CreateContact(Contact contact)
        {
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, contact);
        }

        // PUT: api/contacts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(int id, Contact updated)
        {
            if (id != updated.Id)
                return BadRequest();

            _context.Entry(updated).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Contacts.Any(e => e.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/contacts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
                return NotFound();

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/contacts/random
        [HttpPost("random")]
        public async Task<IActionResult> AddRandomContacts()
        {
            try
            {
                var response = await _httpClient.GetStringAsync("https://randomuser.me/api/?results=10");
                Console.WriteLine("=== RandomUser JSON Response ===");
                Console.WriteLine(response);

                var apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponse>(
    response,
    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
);

                if (apiResponse?.Results == null)
                    return BadRequest("Failed to fetch users");

                var contacts = apiResponse.Results.Select(user => new Contact
                {
                    Name = $"{user.Name?.First} {user.Name?.Last}",
                    FullAddress = $"{user.Location?.Street?.Number} {user.Location?.Street?.Name}, {user.Location?.City}, {user.Location?.State}, {user.Location?.Country}",
                    Email = user.Email,
                    Phone = user.Phone,
                    Cell = user.Cell,
                    RegistrationDate = user.Registered.Date,
                    Age = user.Dob.Age,
                    ImageUrl = user.Picture?.Large
                }).ToList();

                _context.Contacts.AddRange(contacts);
                await _context.SaveChangesAsync();

                return Ok(contacts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
