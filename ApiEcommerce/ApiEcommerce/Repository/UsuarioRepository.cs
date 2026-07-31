using System;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

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

        return null;
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
