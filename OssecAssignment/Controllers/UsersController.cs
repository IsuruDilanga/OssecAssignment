using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OssecAssignment.Data;
using OssecAssignment.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace OssecAssignment.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        
        /*[HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            var userInDb = _context.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);

            if (userInDb != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.UserName)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                TempData["Email"] = user.Email;
                TempData["Authenticated"] = true;

                if(TempData["Email"] != null)
                {


                    return RedirectToAction("Index", "Home");
                }

                //return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Authenticated"] = false;
                ViewData["LoginError"] = "Invalid email or password";
            }

            ViewBag.Users = _context.Users.ToList();
            return View("Create", user);
        }
        */
        
        
        [HttpPost]
        public IActionResult Login(User user)
        {
            var userInDb = _context.Users.FirstOrDefault(u => u.Email == user.Email);

            if (userInDb != null && VerifyPassword(user.Password, userInDb.Password))
            {
                // Password matches, proceed with login
                TempData["Email"] = user.Email;  // Store email in TempData
                TempData["Authenticated"] = true;  // Store authentication status in TempData

                ViewData["Authenticated"] = true;
                HttpContext.Session.SetString("Authenticated", "true");
                HttpContext.Session.SetString("Email", user.Email);  // Store email in session

                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Password does not match or user not found
                TempData["Authenticated"] = false;  // Store authentication status in TempData
                ViewData["Authenticated"] = false;
                ViewData["LoginError"] = "Invalid email or password";  // Set error message
            }

            ViewBag.Users = _context.Users.ToList();
            return View("Create", user);  // Return the "Create" view with the user object
        }

        private bool VerifyPassword(string enteredPassword, string hashedPassword)
        {
            // Split the stored hash into salt and hashed password parts
            string[] parts = hashedPassword.Split(':');
            if (parts.Length != 2)
            {
                return false; // Invalid format
            }

            byte[] salt = Convert.FromBase64String(parts[0]);
            string storedHash = parts[1];

            // Compute the hash of the entered password using the same salt and iteration count
            string enteredPasswordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: enteredPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // Compare the computed hash with the stored hash
            return storedHash.Equals(enteredPasswordHash);
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Users");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: Users
        /*public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }*/

        // GET: Users
        public async Task<IActionResult> Index()
        {
            ViewBag.Users = await _context.Users.ToListAsync();
            return View(new User());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,UserName,Email,Password,ConfirmPassword")] User user)
        {
            if (ModelState.IsValid)
            {
                // Check if the email already exists in the database
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "User with this email already exists.");
                    return View(user);
                }

                // Hash the password before saving it to the database
                user.Password = HashPassword(user.Password);
                user.ConfirmPassword = user.Password;

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }


        private string HashPassword(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hash the password with PBKDF2
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // Combine salt and hashed password into a single string
            string hashedWithSalt = $"{Convert.ToBase64String(salt)}:{hashed}";

            return hashedWithSalt;
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,UserName,Email,Password,ConfirmPassword")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
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
            return View(user);
        }


        // GET: Users/Delete/5
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
