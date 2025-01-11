using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserScan.Models;
using System.Net.Http.Json;

using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace userscan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly HttpClient _httpClient;

        public UsersController(UserContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var apiUrl = "https://randomuser.me/api/?page=3&results=20&seed=abc";

            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(data, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }); ;
                Console.WriteLine("response" + apiResponse);

                if (apiResponse == null || apiResponse.Results == null || !apiResponse.Results.Any())
                {
                    return BadRequest("Failed to parse user data.");
                }
                var users = apiResponse.Results.Select(user => new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = user.Name?.First ?? "unknown",
                    LastName = user.Name?.Last ?? "Unknown",
                    Email = user.Email ?? "No Email",
                    DateOfBirth = user.Dob.Date,
                    Phone = user.Phone,
                    Address = user.Location != null
                ? $"{user.Location.Street?.Number} {user.Location.Street?.Name}, {user.Location.City}, {user.Location.Country}"
                : "Unknown Address",
                    ProfilePicture = user.Picture?.Large ?? "No Picture"

                }).ToList();
                // foreach (var user in users)
                // {
                //     _context.Add(new User
                //     {
                //         FirstName = user.Name.First,
                //         LastName = user.Name.Last,
                //         Email = user.Email,
                //         DateOfBirth = user.Dob.Date,
                //         Phone = user.Phone,
                //         Address = $"{user.Location.Street.Name}{user.Location.City}{user.Location.Country}",
                //         ProfilePicture = user.Picture.Large

                //     });

                // }
                //Console.WriteLine(users);
                _context.Users.AddRange(users);
                await _context.SaveChangesAsync();
                return Ok(users);
            }
            else
            {
                return StatusCode((int)response.StatusCode);
            }

            // return await _context.Users.ToListAsync();
        }



        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(Guid id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
