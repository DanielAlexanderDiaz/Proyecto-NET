using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProductos()
        {
            var productos = _productoRepository.GetProductos();
            var productosDto = _mapper.Map<List<ProductoDTO>>(productos);
            return Ok(productosDto);
        }

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
            return CreatedAtRoute("GetProducto", new { id = producto.ProductoId }, producto);
        }
    }
}
