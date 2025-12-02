using BookBazaarApi.DAL;
using BookBazaarApi.DTOs;
using BookBazaarApi.Helpers;
using BookBazaarApi.Models;
using BookBazaarApi.Services.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
namespace BookBazaarApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasherService _passwordHasher;

        public AuthController(AppDbContext context, 
                                PasswordHasherService passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }


        [HttpPost("login")]
        public async Task<ActionResult<ResponseModel<LoginResponse>>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Username and password are required.");
            }
            var user = await _context.Users
                                    .Include(u => u.UserRoles)
                                        .ThenInclude(ur => ur.Role)
                                    .FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return Unauthorized("Invalid username or password");


            if (!_passwordHasher.VerifyPassword(user, request.Password))
                return Unauthorized("Invalid username or password");

            var loginresponse = new LoginResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            };
            var response = new ResponseModel<LoginResponse>
            {
                Success = true,
                Result = loginresponse,
                Message = "Login successfull."
            };

            return Ok(response);

        }

        [HttpPost("register")]
        public async Task<ActionResult<ResponseModel<RegisterDTO>>> Register([FromBody] RegisterDTO request)
        {


            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Username, email and password are required.");
            }

            // Check if username or email already exists
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return Conflict("Username:Username not available.");

            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return Conflict("Email:Email already registered.");

            // Create new user
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            // Hash password
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            // Save user to DB
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            //asigning user default role
            var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (defaultRole != null)
            {
                var userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = defaultRole.Id
                };

                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync(); 
            }
            var response = new ResponseModel<RegisterDTO>
            {
                Success = true,
                Result = request,
                Message = "User registered successfully."
            };

            return Ok(response);
        }

    }
}
