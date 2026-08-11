using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Models.Dtos.Responses;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ApiVersionNeutral]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ICategoriaRepository _categoriaRepository;

        public ProductoController(IProductoRepository productoRepository, ICategoriaRepository categoriaRepository)
        {
            _productoRepository = productoRepository;
            _categoriaRepository = categoriaRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProductos()
        {
            var productos = _productoRepository.GetProductos();
            var productosDto = productos.Adapt<List<ProductoDTO>>();
            return Ok(productosDto);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}", Name = "GetProducto")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProducto(int id)
        {
            var producto = _productoRepository.GetProducto(id);
            if (producto == null)
            {
                return NotFound($"Producto no encontrado : {id}");
            }
            var productoDto = producto.Adapt<ProductoDTO>();
            return Ok(productoDto);
        }

        [AllowAnonymous]
        [HttpGet("Paginado", Name = "GetProductoEnPagina")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProductoEnPagina([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("El número de página y la cantidad de registros deben ser mayores que cero.");
            }

            var totalProductos = _productoRepository.GetTotalProductos();
            var totalPaginas = (int)Math.Ceiling((double)totalProductos / pageSize);
            if (pageNumber > totalPaginas)
            {
                return NotFound("La página solicitada no existe.");
            }
            var producto = _productoRepository.GetProductosEnPaginas(pageNumber, pageSize);
            var productoDto = producto.Adapt<List<ProductoDTO>>();
            var paginacionResponse = new PaginacionResponse<ProductoDTO>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPaginas = totalPaginas,
                Items = productoDto
            };
            return Ok(paginacionResponse);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CrearProducto([FromForm] CrearProductoDTO crearProductoDTO)
        {
            if (crearProductoDTO == null)
            {
                return BadRequest(ModelState);
            }
            if (_productoRepository.ProductoExiste(crearProductoDTO.Nombre))
            {
                ModelState.AddModelError("CustomError", "Producto ya existe");
                return BadRequest(ModelState);
            }
            if (!_categoriaRepository.CategoriaExiste(crearProductoDTO.CategoriaId))
            {
                ModelState.AddModelError("CustomError", "categoria no existe");
                return BadRequest(ModelState);
            }
            var producto = crearProductoDTO.Adapt<Producto>();
            //Agregando imagen
            if (crearProductoDTO.Image != null)
            {
                ActualizarImagen(crearProductoDTO, producto);
            }
            else
            {
                producto.ImgUrl = "https://placehold.co/300x300";
            }
            if (!_productoRepository.CrearProducto(producto))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal guardando el registro {producto.Nombre}");
                return StatusCode(500, ModelState);
            }

            var crearProducto = _productoRepository.GetProducto(producto.ProductoId);
            var productoDTO = crearProducto.Adapt<ProductoDTO>();
            return CreatedAtRoute("GetProducto", new { id = producto.ProductoId }, productoDTO);
        }

        [HttpGet("buscarPorCategoria/{categoriaId:int}", Name = "GetProductoPorCategoria")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProductoPorCategoria(int categoriaId)
        {
            var productosPorCategoria = _productoRepository.GetProductosPorCategoria(categoriaId);
            if (productosPorCategoria.Count == 0)
            {
                return NotFound($"Categoria no encontrado : {categoriaId}");
            }
            var productosDto = productosPorCategoria.Adapt<List<ProductoDTO>>();
            return Ok(productosDto);
        }

        [HttpGet("buscarPorNombreDescrripcion/{nombre}", Name = "BuscarProductos")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult BuscarProductos(string nombre)
        {
            var productos = _productoRepository.BuscarProductos(nombre);
            if (productos.Count == 0)
            {
                return NotFound($"Productos no encontrado : {nombre}");
            }
            var productosDto = productos.Adapt<List<ProductoDTO>>();
            return Ok(productosDto);
        }

        [HttpPatch("comprarProducto/{nombre}/{cantidad:int}", Name = "VentaProducto")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult VentaProducto(string nombre, int cantidad)
        {
            if (string.IsNullOrWhiteSpace(nombre) || cantidad <= 0)
            {
                return BadRequest("El nombre o cantidad no son validos");
            }
            var productoExiste = _productoRepository.ProductoExiste(nombre);
            if (!productoExiste)
            {
                return BadRequest("el producto no existe");
            }
            if (!_productoRepository.VentaExitosa(nombre, cantidad))
            {
                ModelState.AddModelError("CustomError","No se completo la venta");
                return BadRequest(ModelState);
            }
            return Ok("Venta exitosa");
        }

        [HttpPut("{productoId:int}", Name = "ActualizarProducto")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarProducto(int productoId, [FromForm] UpdateProductoDTO updateProductoDTO)
        {
            if (updateProductoDTO == null)
            {
                return BadRequest(ModelState);
            }
            if (!_productoRepository.ProductoExiste(productoId))
            {
                ModelState.AddModelError("CustomError", "Producto no existe");
                return BadRequest(ModelState);
            }
            if (!_categoriaRepository.CategoriaExiste(updateProductoDTO.CategoriaId))
            {
                ModelState.AddModelError("CustomError", "categoria no existe");
                return BadRequest(ModelState);
            }
            var producto = updateProductoDTO.Adapt<Producto>();
            producto.ProductoId = productoId;
            //Agregando imagen
            if (updateProductoDTO.Image != null)
            {
                ActualizarImagen(updateProductoDTO, producto);
            }
            else
            {
                producto.ImgUrl = "https://placehold.co/300x300";
            }
            if (!_productoRepository.ActualizarProducto(producto))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal actualizando el registro {producto.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        private void ActualizarImagen(dynamic productoDTO, Producto producto)
        {
            string fileName = producto.ProductoId + Guid.NewGuid().ToString() + Path.GetExtension(productoDTO.Image.FileName);
            var imagenesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductosImagenes");
            if (!Directory.Exists(imagenesFolder))
            {
                Directory.CreateDirectory(imagenesFolder);
            }
            var filePath = Path.Combine(imagenesFolder, fileName);
            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                file.Delete();
            }
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                productoDTO.Image.CopyTo(stream);
            }

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
            producto.ImgUrl = $"{baseUrl}/ProductosImagenes/{fileName}";
            producto.ImgUrlLocal = filePath;
        }

        [HttpDelete("{productoId:int}", Name = "EliminarProducto")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult EliminarProducto(int productoId)
        {
            if (productoId == 0)
            {
                return BadRequest(ModelState);
            }
            var producto = _productoRepository.GetProducto(productoId);
            if (producto == null)
            {
                return NotFound($"Producto no encontrado : {productoId}");
            }
            if (!_productoRepository.BorrarProducto(producto))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal al eliminar el registro {producto.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
