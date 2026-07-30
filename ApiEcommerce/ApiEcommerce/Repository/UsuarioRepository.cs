using System;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;

namespace ApiEcommerce.Repository;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ApplicationDbContext _db;
    public UsuarioRepository(ApplicationDbContext db)
    {
        _db = db;
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

    public Task<UsuarioLoginResponseDTO> Login(UsuarioLoginDTO usuarioLoginDTO)
    {
        throw new NotImplementedException();
    }

    public Task<Usuario> Register(CrearUsuarioDTO crearUsuarioDTO)
    {
        throw new NotImplementedException();
    }
}
