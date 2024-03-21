using Data;
using Data.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.DTOs;
using Models.Entidades;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class UsuarioController : BaseApiController
    {
        private readonly ApplicationDbContext _db;
        private readonly ITokenServicio _tokenService;

        public UsuarioController(ApplicationDbContext db, ITokenServicio tokenService)
        {
            _db = db;
            _tokenService = tokenService;
        }

        [Authorize]
        [HttpGet] //    api/usuario
        public async Task<ActionResult<IEnumerable<ClsUsuario>>> GetUsuarios()
        {
            var usuarios = await _db.clsUsuarios.ToListAsync();
            return Ok(usuarios);
        }

        [Authorize]
        [HttpGet("{id}")]   //  api/usuario/1
        public async Task<ActionResult<ClsUsuario>> GetUsuario(int id) 
        {
            var usuario = _db.clsUsuarios.FindAsync(id);
            return Ok(usuario);
        }

        [HttpPost("registro")]  // POST: api/usuario/registro
        public async Task<ActionResult<ClsUsuarioDTO>> Registro(ClsRegistroDTO clsRegistroDTO)
        {
            if(await UsuarioExistente(clsRegistroDTO.Username)) return BadRequest("UserName ya tiene Registrado");

            using var hmac = new HMACSHA512();
            var ClsUsuario = new ClsUsuario
            {
                Username = clsRegistroDTO.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(clsRegistroDTO.Password)),
                PasswordSalt = hmac.Key
            };
            _db.clsUsuarios.Add(ClsUsuario);
            await _db.SaveChangesAsync();
            return new ClsUsuarioDTO
            {
                Username = ClsUsuario.Username,
                Token = _tokenService.CrearToken(ClsUsuario)
            };
        }

        [HttpPost("login")] //  POST: api/usuario/login
        public async Task<ActionResult<ClsUsuarioDTO>> Login(ClsLoginDTO loginDTO)
        {
            var usuario = await _db.clsUsuarios.SingleOrDefaultAsync(x => x.Username == loginDTO.Username);
            if(usuario == null) return Unauthorized("Usuario no Valido");
            using var hmac = new HMACSHA512(usuario.PasswordSalt);
            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
            for (int i = 0; i < computeHash.Length; i++)
            {
                if (computeHash[i] != usuario.PasswordHash[i]) return Unauthorized("Password no Valido");
            }
            return new ClsUsuarioDTO
            {
                Username = usuario.Username,
                Token = _tokenService.CrearToken(usuario)
            };

        }

        private async Task<bool> UsuarioExistente(string username)
        {
            return await _db.clsUsuarios.AnyAsync(x => x.Username.ToLower() == username.ToLower());
        }
    }
}
