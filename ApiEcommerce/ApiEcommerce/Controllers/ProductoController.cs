using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public ProductoController(IProductoRepository productoRepository, ICategoriaRepository categoriaRepository, IMapper mapper)
        {
            _productoRepository = productoRepository;
            _categoriaRepository = categoriaRepository;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProductos()
        {
            var productos = _productoRepository.GetProductos();
            var productosDto = _mapper.Map<List<ProductoDTO>>(productos);
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
            var productoDto = _mapper.Map<ProductoDTO>(producto);
            return Ok(productoDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CrearProducto([FromBody] CrearProductoDTO crearProductoDTO)
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
            var producto = _mapper.Map<Producto>(crearProductoDTO);
            if (!_productoRepository.CrearProducto(producto))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal guardando el registro {producto.Nombre}");
                return StatusCode(500, ModelState);
            }

            var crearProducto = _productoRepository.GetProducto(producto.ProductoId);
            var productoDTO = _mapper.Map<ProductoDTO>(crearProducto);
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
            var productosDto = _mapper.Map<List<ProductoDTO>>(productosPorCategoria);
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
            var productosDto = _mapper.Map<List<ProductoDTO>>(productos);
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
        public IActionResult ActualizarProducto(int productoId, [FromBody] UpdateProductoDTO updateProductoDTO)
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
            var producto = _mapper.Map<Producto>(updateProductoDTO);
            producto.ProductoId = productoId;
            if (!_productoRepository.ActualizarProducto(producto))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal actualizando el registro {producto.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
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
