using System;
using ApiEcommerce.Models;
namespace ApiEcommerce.Repository.IRepository;

public interface IProductoRepository
{
    ICollection<Producto> GetProductos();
    ICollection<Producto> GetProductosEnPaginas(int PageNumber, int PageSize);
    int GetTotalProductos();
    ICollection<Producto> GetProductosPorCategoria(int categoriaId);
    ICollection<Producto> BuscarProductos(string nombreProducto);
    Producto? GetProducto(int id);
    bool VentaExitosa(string nombreProducto, int cantidad);
    bool ProductoExiste(int id);
    bool ProductoExiste(string nombre);
    bool CrearProducto(Producto producto);
    bool ActualizarProducto(Producto producto);
    bool BorrarProducto(Producto producto);
    bool Guardar();
}
