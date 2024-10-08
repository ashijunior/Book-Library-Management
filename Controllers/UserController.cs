using BookManagement.AppDBContext;
using AutoMapper;
using BookManagement.DTOs;
using BookManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BookManagement.Hasher;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly LibraryContext _libraryContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserController(LibraryContext libraryContext, IMapper mapper, IConfiguration configuration)
        {
            _libraryContext = libraryContext;
            _mapper = mapper;
            _configuration = configuration;
        }

        // POST: api/User/Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            if (registerDto == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid registration data.");
            }

            // Check if user already exists
            if (_libraryContext.Users.Any(u => u.Username == registerDto.Username || u.Email == registerDto.Email))
            {
                return BadRequest("User with the same username or email already exists.");
            }

            // Map DTO to User model
            var user = _mapper.Map<Users>(registerDto);

            // Hash the password before saving
            user.Password = PasswordHasher.HashPassword(registerDto.Password);
            //user.Role = "Admin"; // Assign default role

            // Add and save the user to the database
            await _libraryContext.Users.AddAsync(user);
            await _libraryContext.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        // POST: api/User/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (loginDto == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid login data.");
            }

            // Find the user by username
            var user = await _libraryContext.Users
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null || !PasswordHasher.VerifyPassword(loginDto.Password, user.Password))
            {
                return Unauthorized("Invalid username or password.");
            }

            // Generate JWT Token
            var token = GenerateJwtToken(user);

            // Update user token and save to the database
            user.Token = token;
            _libraryContext.Users.Update(user);
            await _libraryContext.SaveChangesAsync();

            return Ok(new { Token = token });
        }

        // GET: api/User/GetUserDetails
        [HttpGet("GetUserDetails")]
        [Authorize]
        public async Task<IActionResult> GetUserDetails()
        {
            // Retrieve the username from the claims (token)
            var username = User.FindFirstValue(ClaimTypes.Name);

            // Find the user in the database
            var user = await _libraryContext.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Map user to DTO for response
            var userDto = _mapper.Map<RegisterDTO>(user);

            return Ok(userDto);
        }

        // GET: api/User/GetAllUsers
        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "Admin")] // Restrict access to users with the Admin role
        public async Task<IActionResult> GetAllUsers()
        {
            // Retrieve all users from the database
            var users = await _libraryContext.Users.ToListAsync();

            // Map users to DTOs for response
            var userDtos = _mapper.Map<List<RegisterDTO>>(users);

            return Ok(userDtos);
        }

        // POST: api/User/CreateAdmin
        [HttpPost("CreateAdmin")]
        [Authorize(Roles = "Admin")] // Only allow existing admins to create new admins
        public async Task<IActionResult> CreateAdmin([FromBody] RegisterDTO registerDto)
        {
            if (registerDto == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid registration data.");
            }

            // Check if user already exists
            if (_libraryContext.Users.Any(u => u.Username == registerDto.Username || u.Email == registerDto.Email))
            {
                return BadRequest("User with the same username or email already exists.");
            }

            // Map DTO to User model
            var user = _mapper.Map<Users>(registerDto);

            // Hash the password before saving
            user.Password = PasswordHasher.HashPassword(registerDto.Password);
            user.Role = "Admin"; // Assign role as Admin

            // Add and save the user to the database
            await _libraryContext.Users.AddAsync(user);
            await _libraryContext.SaveChangesAsync();

            return Ok("Admin registered successfully.");
        }


        // Helper method to generate JWT token
        private string GenerateJwtToken(Users user)
        {
            // Retrieve the key from the configuration
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Set up the claims for the token
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)  // Add role claim if applicable
            };

            // Create the token with expiration
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(3),  // Token expiration
                signingCredentials: creds
            );

            // Write the token to a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
