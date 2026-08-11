using System;
using ApiEcommerce.Models;
using ApiEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ApiEcommerce.Repository;

public class ProductoRepository : IProductoRepository
{
    private readonly ApplicationDbContext _db;
    public ProductoRepository(ApplicationDbContext db)
    {
        _db = db;
    }
    public bool ActualizarProducto(Producto producto)
    {
        if(producto == null)
        {
            return false;
        }
        producto.FechaActualizacion = DateTime.Now;
        _db.Productos.Update(producto);
        return Guardar();
    }

    public bool BorrarProducto(Producto producto)
    {
        if(producto == null)
        {
            return false;
        }
        _db.Productos.Remove(producto);
        return Guardar();
    }

    public ICollection<Producto> BuscarProductos(string nombreProducto)
    {
        IQueryable<Producto> query = _db.Productos;
        if (!string.IsNullOrEmpty(nombreProducto))
        {
            query = query.Include(p => p.Categoria).Where(
                p => p.Nombre.ToLower().Trim().Contains(nombreProducto.ToLower().Trim()) || 
                p.Descripcion.ToLower().Trim().Contains(nombreProducto.ToLower().Trim()));
        }
        return query.OrderBy(p => p.Nombre).ToList();
    }

    public bool CrearProducto(Producto producto)
    {
        if(producto == null)
        {
            return false;
        }
        producto.FechaCreacion = DateTime.Now;
        producto.FechaActualizacion = DateTime.Now;
        _db.Productos.Add(producto);
        return Guardar();
    }

    public Producto? GetProducto(int id)
    {
        if(id <= 0)
        {
            return null;
        }
        return _db.Productos.Include(p => p.Categoria).FirstOrDefault(p => p.ProductoId == id);
    }

    public ICollection<Producto> GetProductos()
    {
        return _db.Productos.Include(p => p.Categoria).OrderBy(p => p.Nombre).ToList();
    }

    public ICollection<Producto> GetProductosEnPaginas(int numeroPagina, int cantidadRegistros)
    {
        return _db.Productos.OrderBy(p => p.ProductoId)
        .Skip((numeroPagina - 1) * cantidadRegistros)
        .Take(cantidadRegistros).ToList();
    }

    public ICollection<Producto> GetProductosPorCategoria(int categoriaId)
    {
        if(categoriaId <= 0)
        {
            return new List<Producto>();
        }
        return _db.Productos.Include(p => p.Categoria).Where(p => p.CategoriaId == categoriaId).OrderBy(p => p.Nombre).ToList();
    }

    public int GetTotalProductos()
    {
        return _db.Productos.Count();
    }

    public bool Guardar()
    {
        return _db.SaveChanges() >= 0;
    }

    public bool ProductoExiste(int id)
    {
        if (id <= 0)
        {
            return false;
        }
        return _db.Productos.Any(p => p.ProductoId == id);
    }

    public bool ProductoExiste(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return false;
        }
        return _db.Productos.Any(p => p.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
    }

    public bool VentaExitosa(string nombreProducto, int cantidad)
    {
        if(string.IsNullOrWhiteSpace(nombreProducto) || cantidad <= 0)
        {
            return false;
        }

        var producto = _db.Productos.FirstOrDefault(p => p.Nombre.ToLower().Trim() == nombreProducto.ToLower().Trim());
        if(producto == null || producto.Stock < cantidad)
        {
            return false;
        } 
        producto.Stock -= cantidad;
        _db.Productos.Update(producto);
        return Guardar();
    }
}
