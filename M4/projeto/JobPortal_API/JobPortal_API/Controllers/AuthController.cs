using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JobPortal_API.Data;
using JobPortal_API.DTOs;
using JobPortal_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace JobPortal_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterationRequestDTO model)
        {
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role); // "Admin", "Candidato", "Empresa"

                
                if (model.Role == "Candidato")
                {
                    var candidato = new Candidato
                    {
                        UserId = user.Id,
                        Nome = model.UserName, 
                        Email = model.Email
                    };
                    _context.Candidato.Add(candidato);
                }
                else if (model.Role == "Empresa")
                {
                    var empresa = new Empresa
                    {
                        UserId = user.Id,
                        Nome = model.UserName,
                        Email = model.Email
                    };
                    _context.Empresa.Add(empresa);
                }

                await _context.SaveChangesAsync();

                return Ok("User created successfully");
            }


            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null) return Unauthorized("Invalid credentials");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return Unauthorized("Invalid credentials");

            var roles = await _userManager.GetRolesAsync(user);

            string idCandidato = null;
            string idEmpresa = null;

            if (roles.Contains("Candidato"))
            {
                var candidato = await _context.Candidato.FirstOrDefaultAsync(c => c.UserId == user.Id);
                idCandidato = candidato?.IdCandidato.ToString();
            }

            if (roles.Contains("Empresa"))
            {
                var empresa = await _context.Empresa.FirstOrDefaultAsync(c => c.UserId == user.Id);
                idEmpresa = empresa?.IdEmpresa.ToString();
            }

            var jwtToken = GenerateJwtToken(user, roles, idCandidato, idEmpresa);

            return Ok(new APIResponse
            {
                IsSuccess = true,
                Result = new LoginResponseDTO
                {
                    User = new UserDTO
                    {
                        UserName = user.UserName,
                        Role = roles.FirstOrDefault()
                    },
                    Token = jwtToken
                }
            });
        }


        private string GenerateJwtToken(ApplicationUser user, IList<string> roles, string? idCandidato, string? idEmpresa)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            if (idCandidato != null)
            {
                claims.Add(new Claim("IdCandidato", idCandidato));
            }

            if (idEmpresa != null)
            {
                claims.Add(new Claim("IdEmpresa", idEmpresa));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("minha-chave-jwt-supersecreta"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "JobPortalAPI",
                audience: "JobPortalAPI",
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
