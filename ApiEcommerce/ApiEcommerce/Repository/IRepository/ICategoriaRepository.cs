using System;

namespace ApiEcommerce.Repository.IRepository;

public interface ICategoriaRepository
{
    ICollection<Categoria> GetCategorias();
    Categoria GetCategoria(int id);
    bool CategoriaExiste(int id);
    bool CategoriaExiste(string nombre);
    bool CrearCategoria(Categoria categoria);
    bool ActualizarCategoria(Categoria categoria);
    bool BorrarCategoria(Categoria categoria);
    bool Guardar();
}
