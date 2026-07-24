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
        private readonly IMapper _mapper;

        public ProductoController(IProductoRepository productoRepository, IMapper mapper)
        {
            _productoRepository = productoRepository;
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
    }
}
