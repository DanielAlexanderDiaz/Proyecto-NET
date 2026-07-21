using System;
using ApiEcommerce.Repository.IRepository;

namespace ApiEcommerce.Repository;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly ApplicationDbContext _db;

    public CategoriaRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public bool ActualizarCategoria(Categoria categoria)
    {
        categoria.Creacion = DateTime.Now;
        _db.Categorias.Update(categoria);
        return Guardar(); 
    }

    public bool BorrarCategoria(Categoria categoria)
    {
        _db.Categorias.Remove(categoria);
        return Guardar();
    }

    public bool CategoriaExiste(int id)
    {
        return _db.Categorias.Any(c => c.Id == id);
    }

    public bool CategoriaExiste(string nombre)
    {
        return _db.Categorias.Any(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
    }

    public bool CrearCategoria(Categoria categoria)
    {
        categoria.Creacion = DateTime.Now;
        _db.Categorias.Add(categoria);
        return Guardar();
    }

    public Categoria? GetCategoria(int id)
    {
        return _db.Categorias.FirstOrDefault(c => c.Id == id);
    }

    public ICollection<Categoria> GetCategorias()
    {
        return _db.Categorias.OrderBy(C => C.Nombre).ToList();
    }

    public bool Guardar()
    {
        return _db.SaveChanges() >= 0 ? true : false;
    }
}
