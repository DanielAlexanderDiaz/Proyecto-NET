using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ApiEcommerce.Repository;

public class UsuarioRepository : IUsuarioRepository
{
    public readonly ApplicationDbContext _db;
    private string? secretKey;
    public UsuarioRepository(ApplicationDbContext db, IConfiguration configuration)
    {
        _db = db;
        secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
    }

    public bool EsUnicoElNombre(string nombre)
    {
        return !_db.Usuarios.Any(u => u.NombreUsuario.ToLower().Trim() == nombre.ToLower().Trim());
    }

    public Usuario? GetUsuario(int id)
    {
        return _db.Usuarios.FirstOrDefault(u => u.Id == id);
    }

    public ICollection<Usuario> GetUsuarios()
    {
        return _db.Usuarios.OrderBy(u => u.NombreUsuario).ToList();
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

        var usuario = await _db.Usuarios.FirstOrDefaultAsync<Usuario>(u => u.NombreUsuario.ToLower().Trim() == usuarioLoginDTO.Name.ToLower().Trim());
        if (usuario == null)
        {
            return new UsuarioLoginResponseDTO()
            {
                Token= "",
                Usuario = null,
                Mensaje = "El nombre no encontrado"
            };
        }
        if (!BCrypt.Net.BCrypt.Verify(usuarioLoginDTO.Password, usuario.Password))
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

        var key = Encoding.UTF8.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", usuario.Id.ToString()),
                new Claim("username", usuario.NombreUsuario),
                new Claim(ClaimTypes.Role, usuario.Role ?? string.Empty)
            }
            ),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = handlerToken.CreateToken(tokenDescriptor);
        return new UsuarioLoginResponseDTO()
        {
            Token = handlerToken.WriteToken(token),
            Usuario = new UsuarioRegisterDTO()
            {
                NombreUsuario = usuario.NombreUsuario,
                Nombre = usuario.Nombre,
                Role = usuario.Role,
                Password = usuario.Password ?? ""
            },
            Mensaje = "Usuario logeado correctamente"
        };
    }

    public async Task<Usuario> Register(CrearUsuarioDTO crearUsuarioDTO)
    {
        var contraseñaEncriptada = BCrypt.Net.BCrypt.HashPassword(crearUsuarioDTO.Password);
        var usuario = new Usuario()
        {
            NombreUsuario = crearUsuarioDTO.Username ?? "No nombre Usuario",
            Nombre = crearUsuarioDTO.Name,
            Role = crearUsuarioDTO.Role,
            Password = contraseñaEncriptada
        };
        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();
        return usuario;
    }
}
