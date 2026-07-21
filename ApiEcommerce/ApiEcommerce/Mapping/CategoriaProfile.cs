using System;
using ApiEcommerce.Models.Dtos;
using AutoMapper;

namespace ApiEcommerce.Mapping;

public class CategoriaProfile: Profile
{
    public CategoriaProfile()
    {
        CreateMap<Categoria, CategoriaDTO>().ReverseMap();
        CreateMap<Categoria, CrearCategoriaDTO>().ReverseMap();
    }
}
