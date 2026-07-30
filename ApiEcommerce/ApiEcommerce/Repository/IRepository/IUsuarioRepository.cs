using System;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;

namespace ApiEcommerce.Repository.IRepository;

public interface IUsuarioRepository
{
    ICollection<Usuario> GetUsuarios();
    Usuario? GetUsuario(int id);
    bool EsUnicoElNombre(string nombre);
    Task<UsuarioLoginResponseDTO> Login(UsuarioLoginDTO usuarioLoginDTO);
    Task<Usuario> Register(CrearUsuarioDTO crearUsuarioDTO);
}
