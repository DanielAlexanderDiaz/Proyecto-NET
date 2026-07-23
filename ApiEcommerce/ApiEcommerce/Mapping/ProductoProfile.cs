using System;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using AutoMapper;

namespace ApiEcommerce.Mapping;

public class ProductoProfile: Profile
{
    public ProductoProfile()
    {
        CreateMap<Producto, ProductoDTO>().ReverseMap();
        CreateMap<Producto, CrearProductoDTO>().ReverseMap();
        CreateMap<Producto, UpdateProductoDTO>().ReverseMap();
    }
}
