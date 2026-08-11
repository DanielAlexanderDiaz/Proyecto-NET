using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ApiEcommerce.Repository;

public class UsuarioRepository : IUsuarioRepository
{
    public readonly ApplicationDbContext _db;
    private string? secretKey;

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    public UsuarioRepository(ApplicationDbContext db, IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _db = db;
        secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public bool EsUnicoElNombre(string nombre)
    {
        return !_db.Usuarios.Any(u => u.NombreUsuario.ToLower().Trim() == nombre.ToLower().Trim());
    }

    public ApplicationUser? GetUsuario(string id)
    {
        return _db.ApplicationUser.FirstOrDefault(u => u.Id == id);
    }

    public ICollection<ApplicationUser> GetUsuarios()
    {
        return _db.ApplicationUser.OrderBy(u => u.UserName).ToList();
    }

    public async Task<UsuarioLoginResponseDTO> Login(UsuarioLoginDTO usuarioLoginDTO)
    {
        if (string.IsNullOrEmpty(usuarioLoginDTO.Name))
        {
            return new UsuarioLoginResponseDTO()
            {
                Token= "",
                Usuario = null,
                Mensaje = "El nombre es requerido"
            };
        }

        var usuario = await _db.ApplicationUser.FirstOrDefaultAsync<ApplicationUser>(u => u.UserName != null && u.UserName.ToLower().Trim() == usuarioLoginDTO.Name.ToLower().Trim());
        if (usuario == null)
        {
            return new UsuarioLoginResponseDTO()
            {
                Token= "",
                Usuario = null,
                Mensaje = "El nombre no encontrado"
            };
        }
        if (usuarioLoginDTO.Password == null)
        {
            return new UsuarioLoginResponseDTO()
            {
                Token= "",
                Usuario = null,
                Mensaje = "La contraseña es requerida"
            };
        }
        bool isPasswordValid = await _userManager.CheckPasswordAsync(usuario, usuarioLoginDTO.Password);
        if (!isPasswordValid)
        {
            return new UsuarioLoginResponseDTO()
            {
                Token= "",
                Usuario = null,
                Mensaje = "Credenciales incorrectas"
            };
        }
        var handlerToken = new JwtSecurityTokenHandler();
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException("secretKey no esta configurada");
        }
        var roles = await _userManager.GetRolesAsync(usuario);
        var key = Encoding.UTF8.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", usuario.Id.ToString()),
                new Claim("username", usuario.UserName ?? string.Empty),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? string.Empty)
            }
            ),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = handlerToken.CreateToken(tokenDescriptor);
        return new UsuarioLoginResponseDTO()
        {
            Token = handlerToken.WriteToken(token),
            Usuario = usuario.Adapt<UsuarioDataDTO>(),
            Mensaje = "Usuario logeado correctamente"
        };
    }

    public async Task<UsuarioDataDTO> Register(CrearUsuarioDTO crearUsuarioDTO)
    {
        if (string.IsNullOrEmpty(crearUsuarioDTO.Username))
        {
            throw new ArgumentNullException("El nombre de usuario es requerido");
        }
        if (string.IsNullOrEmpty(crearUsuarioDTO.Password))
        {
            throw new ArgumentNullException("La contraseña es requerida");
        }

        var usuario = new ApplicationUser()
        {
            UserName = crearUsuarioDTO.Username,
            Email = crearUsuarioDTO.Username,
            NormalizedEmail = crearUsuarioDTO.Username.ToUpper(),
            Nombre = crearUsuarioDTO.Name
        };

        var resultado = await _userManager.CreateAsync(usuario, crearUsuarioDTO.Password);
        if (resultado.Succeeded)
        {
            var usuarioRol = crearUsuarioDTO.Role ?? "User";
            var roleExists = await _roleManager.RoleExistsAsync(usuarioRol);
            if (!roleExists)
            {
                var identityRole = new IdentityRole(usuarioRol);
                await _roleManager.CreateAsync(identityRole);
            }
            await _userManager.AddToRoleAsync(usuario, usuarioRol);
            var crearUsuario = _db.ApplicationUser.FirstOrDefault(u => u.UserName == crearUsuarioDTO.Username);
            return crearUsuario.Adapt<UsuarioDataDTO>();
        }
        var errores = string.Join(", ", resultado.Errors.Select(e => e.Description));
        throw new ApplicationException($"No se pudo crear el usuario: {errores}");
        throw new ApplicationException("No se pudo realizar el registro");
    }
}
