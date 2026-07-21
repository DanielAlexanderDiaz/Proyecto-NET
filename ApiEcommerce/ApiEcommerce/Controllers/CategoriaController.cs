using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IMapper _mapper;

        public CategoriaController(ICategoriaRepository categoriaRepository, IMapper mapper)
        {
            _categoriaRepository = categoriaRepository;
            _mapper = mapper;
        }

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
    }
}
