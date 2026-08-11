using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;

        public UsuarioController(IUsuarioRepository usuarioRepository, IMapper mapper)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUsuarios()
        {
            var usuarios = _usuarioRepository.GetUsuarios();
            var usuariosDTO = _mapper.Map<List<UsuarioDTO>>(usuarios);
            return Ok(usuariosDTO);
        }

        [HttpGet("{id}", Name = "GetUsuario")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUsuario(string id)
        {
            var usuario = _usuarioRepository.GetUsuario(id);
            if (usuario == null)
            {
                return NotFound($"Usuario no encontrado : {id}");
            }
            var usuarioDTO = _mapper.Map<UsuarioDTO>(usuario);
            return Ok(usuarioDTO);
        }

        [AllowAnonymous]
        [HttpPost(Name = "RegistrarUsuario")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegistrarUsuario([FromBody] CrearUsuarioDTO crearUsuarioDTO)
        {
            if (crearUsuarioDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (string.IsNullOrWhiteSpace(crearUsuarioDTO.Username))
            {
                return BadRequest("username es requerido");
            }
            if (!_usuarioRepository.EsUnicoElNombre(crearUsuarioDTO.Username))
            {
                return BadRequest("El usuario ya existe");
            }

            var resultado = await _usuarioRepository.Register(crearUsuarioDTO);
            if(resultado == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al registrar el usuario");
            }
            return CreatedAtRoute("GetUsuario", new { id = resultado.Id },  resultado);
        }

        [AllowAnonymous]
        [HttpPost("Login", Name = "LoginUsuario")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginUsuario([FromBody] UsuarioLoginDTO usuarioLoginDTO)
        {
            if (usuarioLoginDTO == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = await _usuarioRepository.Login(usuarioLoginDTO);
            if(usuario == null)
            {
                return Unauthorized();
            }
            return Ok(usuario);
        }
    }
}
