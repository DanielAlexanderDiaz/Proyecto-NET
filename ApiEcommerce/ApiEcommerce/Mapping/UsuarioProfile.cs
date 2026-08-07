using System;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using AutoMapper;

namespace ApiEcommerce.Mapping;

public class UsuarioProfile: Profile
{
   public UsuarioProfile()
    {
        CreateMap<Usuario, UsuarioDTO>().ReverseMap();
        CreateMap<Usuario, CrearUsuarioDTO>().ReverseMap();
        CreateMap<Usuario, UsuarioLoginDTO>().ReverseMap();
        CreateMap<Usuario, UsuarioLoginResponseDTO>().ReverseMap();
        CreateMap<Usuario, UsuarioProfile>().ReverseMap();
        CreateMap<Usuario, UsuarioRegisterDTO>().ReverseMap();
        CreateMap<ApplicationUser, UsuarioDataDTO>().ReverseMap();
    }
}
