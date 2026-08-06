using ApiEcommerce.Constants;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    // [EnableCors(PolicyNames.AllowSpecificOrigin)]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IMapper _mapper;

        public CategoriaController(ICategoriaRepository categoriaRepository, IMapper mapper)
        {
            _categoriaRepository = categoriaRepository;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategorias()
        {
            var categorias = _categoriaRepository.GetCategorias();
            var categoriasDto = new List<CategoriaDTO>();
            foreach (var categoria in categorias)
            {
                categoriasDto.Add(_mapper.Map<CategoriaDTO>(categoria));
            }
            return Ok(categoriasDto);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}", Name = "GetCategoria")]
        // [ResponseCache(Duration = 10)]
        [ResponseCache(CacheProfileName = "Default20")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategoria(int id)
        {
            Console.WriteLine($"Categoria con el ID {id} a las {DateTime.Now}");
            var categoria = _categoriaRepository.GetCategoria(id);
            Console.WriteLine($"Respuesta con el ID {id}");
            if (categoria == null)
            {
                return NotFound($"Categoría no encontrada : {id}");
            }
            var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);
            return Ok(categoriaDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CrearCategoria([FromBody] CrearCategoriaDTO crearCategoriaDTO)
        {
            if (crearCategoriaDTO == null)
            {
                return BadRequest(ModelState);
            }
            if (_categoriaRepository.CategoriaExiste(crearCategoriaDTO.Nombre))
            {
                ModelState.AddModelError("CustomError", "La categoría ya existe");
                return BadRequest(ModelState);
            }
            var categoria = _mapper.Map<Categoria>(crearCategoriaDTO);
            if (!_categoriaRepository.CrearCategoria(categoria))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal guardando el registro {categoria.Nombre}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetCategoria", new { id = categoria.Id }, categoria);
        }

        [HttpPatch("{id:int}", Name = "UpdateCategoria")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateCategoria(int id, [FromBody] CrearCategoriaDTO UpdateCategoriaDTO)
        {
            if (!_categoriaRepository.CategoriaExiste(id))
            {
                return NotFound($"Categoría no encontrada : {id}");
            }
            if (UpdateCategoriaDTO == null)
            {
                return BadRequest(ModelState);
            }
            if (_categoriaRepository.CategoriaExiste(UpdateCategoriaDTO.Nombre))
            {
                ModelState.AddModelError("CustomError", "La categoría ya existe");
                return BadRequest(ModelState);
            }
            var categoria = _mapper.Map<Categoria>(UpdateCategoriaDTO);
            categoria.Id = id;
            if (!_categoriaRepository.ActualizarCategoria(categoria))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal actualizando el registro {categoria.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteCategoria")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult BorrarCategoria(int id)
        {
            if (!_categoriaRepository.CategoriaExiste(id))
            {
                return NotFound($"Categoría no encontrada : {id}");
            }
            var categoria = _categoriaRepository.GetCategoria(id);
            if (categoria == null)
            {
                return NotFound($"Categoría no encontrada : {id}");
            }
            if (!_categoriaRepository.BorrarCategoria(categoria))
            {
                ModelState.AddModelError("CustomError", $"Algo salió mal borrando el registro {categoria.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
