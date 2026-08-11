using System;
using ApiEcommerce.Models;
using Microsoft.AspNetCore.Identity;

namespace ApiEcommerce.Data;

public static class DataSeeder
{
    public static void SeedData(ApplicationDbContext appContext)
    {
        // Seeding de Roles
    if (!appContext.Roles.Any())
    {
      appContext.Roles.AddRange(
        new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
        new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER" }
      );
    }
    // Seeding de Categorías
    if (!appContext.Categorias.Any())
    {
      appContext.Categorias.AddRange(
        new Categoria { Nombre = "Ropa y accesorios", Creacion = DateTime.Now },
        new Categoria { Nombre = "Electrónicos", Creacion = DateTime.Now },
        new Categoria { Nombre = "Deportes", Creacion = DateTime.Now },
        new Categoria { Nombre = "Hogar", Creacion = DateTime.Now },
        new Categoria { Nombre = "Libros", Creacion = DateTime.Now }
      );
    }
    // Seeding de Usuario Administrador
    if (!appContext.ApplicationUser.Any())
    {
      var hasher = new PasswordHasher<ApplicationUser>();
      var adminUser = new ApplicationUser
      {
        Id = "admin-001",
        UserName = "admin@admin.com",
        NormalizedUserName = "ADMIN@ADMIN.COM",
        Email = "admin@admin.com",
        NormalizedEmail = "ADMIN@ADMIN.COM",
        EmailConfirmed = true,
        Nombre = "Administrador"
      };
      adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin123!");

      var regularUser = new ApplicationUser
      {
        Id = "user-001",
        UserName = "user@user.com",
        NormalizedUserName = "USER@USER.COM",
        Email = "user@user.com",
        NormalizedEmail = "USER@USER.COM",
        EmailConfirmed = true,
        Nombre = "Usuario Regular"
      };
      regularUser.PasswordHash = hasher.HashPassword(regularUser, "User123!");

      appContext.ApplicationUser.AddRange(adminUser, regularUser);
    }
    // Seeding de UserRoles
    if (!appContext.UserRoles.Any())
    {
      appContext.UserRoles.AddRange(
        new IdentityUserRole<string> { UserId = "admin-001", RoleId = "1" }, // Admin
        new IdentityUserRole<string> { UserId = "user-001", RoleId = "2" }   // User
      );
    }

    // Seeding de Productos
    if (!appContext.Productos.Any())
    {
      appContext.Productos.AddRange(
        new Producto
        {
          Nombre = "Camiseta Básica",
          Descripcion = "Camiseta de algodón 100%",
          Precio = 25.99m,
          SKU = "PROD-001-CAM-M",
          Stock = 50,
          CategoriaId = 1,
          Categoria = appContext.Categorias.Find(1)!,
          ImgUrl = "https://via.placeholder.com/300x300/FF0000/FFFFFF?text=Camiseta",
          FechaCreacion = DateTime.Now
        },
        new Producto
        {
          Nombre = "Smartphone Galaxy",
          Descripcion = "Teléfono inteligente con 128GB",
          Precio = 599.99m,
          SKU = "PROD-002-PHO-BLK",
          Stock = 25,
          CategoriaId = 2,
          Categoria = appContext.Categorias.Find(2)!,
          ImgUrl = "https://via.placeholder.com/300x300/0000FF/FFFFFF?text=Smartphone",
          FechaCreacion = DateTime.Now
        },
        new Producto
        {
          Nombre = "Pelota de Fútbol",
          Descripcion = "Pelota oficial FIFA",
          Precio = 45.00m,
          SKU = "PROD-003-BAL-WHT",
          Stock = 30,
          CategoriaId = 3,
          Categoria = appContext.Categorias.Find(3)!,
          ImgUrl = "https://via.placeholder.com/300x300/00FF00/FFFFFF?text=Pelota",
          FechaCreacion = DateTime.Now
        },
        new Producto  
        {
          Nombre = "Lámpara de Mesa",
          Descripcion = "Lámpara LED regulable",
          Precio = 89.99m,
          SKU = "PROD-004-LAM-WHT",
          Stock = 15,
          CategoriaId = 4,
          Categoria = appContext.Categorias.Find(4)!,
          ImgUrl = "https://via.placeholder.com/300x300/FFFF00/000000?text=Lampara",
          FechaCreacion = DateTime.Now
        },
        new Producto
        {
          Nombre = "El Quijote",
          Descripcion = "Novela clásica de Cervantes",
          Precio = 19.99m,
          SKU = "PROD-005-LIB-ESP",
          Stock = 100,
          CategoriaId = 5,
          Categoria = appContext.Categorias.Find(5)!,
          ImgUrl = "https://via.placeholder.com/300x300/800080/FFFFFF?text=Libro",
          FechaCreacion = DateTime.Now
        },
        new Producto
        {
          Nombre = "Jeans Clásicos",
          Descripcion = "Pantalones vaqueros azules",
          Precio = 79.99m,
          SKU = "PROD-006-PAN-BLU",
          Stock = 40,
          CategoriaId = 1,
          Categoria = appContext.Categorias.Find(1)!,
          ImgUrl = "https://via.placeholder.com/300x300/4169E1/FFFFFF?text=Jeans",
          FechaCreacion = DateTime.Now
        },
        new Producto
        {
          Nombre = "Tablet Pro",
          Descripcion = "Tablet 10.5 pulgadas con stylus incluido",
          Precio = 459.99m,
          SKU = "PROD-007-TAB-SIL",
          Stock = 20,
          CategoriaId = 2,
          Categoria = appContext.Categorias.Find(2)!,
          ImgUrl = "https://via.placeholder.com/300x300/C0C0C0/000000?text=Tablet",
          FechaCreacion = DateTime.Now
        },
        new Producto
        {
          Nombre = "Zapatillas Running",
          Descripcion = "Zapatillas deportivas para correr",
          Precio = 129.99m,
          SKU = "PROD-008-ZAP-BLK",
          Stock = 35,
          CategoriaId = 3,
          Categoria = appContext.Categorias.Find(3)!,
          ImgUrl = "https://via.placeholder.com/300x300/000000/FFFFFF?text=Zapatillas",
          FechaCreacion = DateTime.Now
        },
        new Producto  
        {
          Nombre = "Cafetera Express",
          Descripcion = "Cafetera automática con molinillo integrado",
          Precio = 299.99m,
          SKU = "PROD-009-CAF-BLK",
          Stock = 12,
          CategoriaId = 4,
          Categoria = appContext.Categorias.Find(4)!,
          ImgUrl = "https://via.placeholder.com/300x300/2F4F4F/FFFFFF?text=Cafetera",
          FechaCreacion = DateTime.Now
        },
        new Producto
        {
          Nombre = "Programación en C#",
          Descripcion = "Guía completa de programación en C# y .NET",
          Precio = 49.99m,
          SKU = "PROD-010-LIB-ESP",
          Stock = 80,
          CategoriaId = 5,
          Categoria = appContext.Categorias.Find(5)!,
          ImgUrl = "https://via.placeholder.com/300x300/008B8B/FFFFFF?text=C%23+Book",
          FechaCreacion = DateTime.Now
        },
        new Producto  
        {
          Nombre = "Chaqueta Deportiva",
          Descripcion = "Chaqueta impermeable para actividades al aire libre",
          Precio = 149.99m,
          SKU = "PROD-011-CHA-NAV",
          Stock = 28,
          CategoriaId = 1,
          Categoria = appContext.Categorias.Find(1)!,
          ImgUrl = "https://via.placeholder.com/300x300/000080/FFFFFF?text=Chaqueta",
          FechaCreacion = DateTime.Now
        },
        new Producto
        {
          Nombre = "Auriculares Bluetooth",
          Descripcion = "Auriculares inalámbricos con cancelación de ruido",
          Precio = 189.99m,
          SKU = "PROD-012-AUR-BLK",
          Stock = 45,
          CategoriaId = 2,
          Categoria = appContext.Categorias.Find(2)!,
          ImgUrl = "https://via.placeholder.com/300x300/1C1C1C/FFFFFF?text=Auriculares",
          FechaCreacion = DateTime.Now
        }
      );
    }
    appContext.SaveChanges();
    }
}
