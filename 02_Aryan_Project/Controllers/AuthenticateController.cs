using _02_Aryan_Project.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace _02_Aryan_Project.Controllers
{
    [Route("api/[controller]")] // Defines the route for the controller as "api/authenticate"
    [ApiController] // Specifies that this controller handles API requests
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        // Constructor to initialize dependencies (UserManager, RoleManager, and Configuration)
        public AuthenticateController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Generates a JWT token based on the provided authentication claims.
        /// </summary>
        /// <param name="authClaims">List of claims (e.g., username, roles)</param>
        /// <returns>JWT security token</returns>
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            // Get the secret key from the configuration settings
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            // Generate the JWT token
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"], // Issuer of the token
                audience: _configuration["JWT:ValidAudience"], // Audience for the token
                expires: DateTime.Now.AddHours(3), // Token expiration time (3 hours)
                claims: authClaims, // User claims (e.g., username, roles)
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }

        /// <summary>
        /// API endpoint for user login. Validates user credentials and returns a JWT token.
        /// </summary>
        /// <param name="model">LoginModel containing username and password</param>
        /// <returns>JWT token if authentication is successful; Unauthorized response otherwise</returns>
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            // Find user by username
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                // Get user roles
                var userRoles = await _userManager.GetRolesAsync(user);

                // Create authentication claims (username and unique identifier)
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique token ID
                };

                // Add user roles as claims
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                // Generate the JWT token
                var token = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token), // Return token as string
                    expiration = token.ValidTo // Return token expiration time
                });
            }
            return Unauthorized(); // Return 401 Unauthorized if credentials are incorrect
        }

        /// <summary>
        /// API endpoint for user registration. Creates a new user with the "Member" role.
        /// </summary>
        /// <param name="model">RegisterModel containing username, email, and password</param>
        /// <returns>Success message if registration is successful; error message otherwise</returns>
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            // Check if the user already exists
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "User already exists!" });

            // Create new user instance
            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };

            // Create user in the database
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            // Ensure the "Member" role exists before assigning it
            if (!await _roleManager.RoleExistsAsync(UserRoles.Member))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Member));

            // Assign "Member" role to the newly created user
            if (await _roleManager.RoleExistsAsync(UserRoles.Member))
                await _userManager.AddToRoleAsync(user, UserRoles.Member);

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        /// <summary>
        /// API endpoint for admin registration. Creates a new user with the "Admin" role.
        /// </summary>
        /// <param name="model">RegisterModel containing username, email, and password</param>
        /// <returns>Success message if registration is successful; error message otherwise</returns>
        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            // Check if the user already exists
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "User already exists!" });

            // Create new user instance
            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };

            // Create user in the database
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            // Ensure the "Admin" role exists before assigning it
            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

            // Assign "Admin" role to the newly created user
            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }
    }
}
