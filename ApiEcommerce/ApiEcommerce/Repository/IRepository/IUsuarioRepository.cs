using System;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;

namespace ApiEcommerce.Repository.IRepository;

public interface IUsuarioRepository
{
    ICollection<ApplicationUser> GetUsuarios();
    ApplicationUser? GetUsuario(string id);
    bool EsUnicoElNombre(string nombre);
    Task<UsuarioLoginResponseDTO> Login(UsuarioLoginDTO usuarioLoginDTO);
    Task<UsuarioDataDTO> Register(CrearUsuarioDTO crearUsuarioDTO);
}
