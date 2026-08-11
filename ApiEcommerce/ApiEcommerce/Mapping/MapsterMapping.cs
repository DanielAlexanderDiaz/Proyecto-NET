using Mapster;
using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;

namespace ApiEcommerce.Mapping;

public static class MapsterMapping
{
    // Register explicit mappings that can't be handled by convention
    public static void RegisterMappings()
    {
        var config = TypeAdapterConfig.GlobalSettings;

        // Producto -> ProductoDTO: map CategoriaNombre from Producto.Categoria.Nombre
        config.ForType<Producto, ProductoDTO>()
              .Map(dest => dest.CategoriaNombre, src => src.Categoria != null ? src.Categoria.Nombre : null);

        // Additional mappings can be added here if needed.
    }
}
