using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly AuthService _authService;

        public AuthController(IUsuarioService usuarioService, AuthService authService)
        {
            _usuarioService = usuarioService;
            _authService = authService;
        }

       

        [HttpPost("INICIO DE SESION")]
        public IActionResult Login([FromBody] LoginDTO dto)
        {
            try
            {
                var usuario = _usuarioService.Login(dto);
                if (usuario == null)
                    return Unauthorized(new { error = "Email o contraseña incorrectos." });

                var token = _authService.GenerarToken(usuario);

                return Ok(new
                {
                    token = token,
                    usuario = new
                    {
                        id = usuario.Id,
                        email = usuario.Email,
                        nombre = usuario.Nombre,
                        apellido = usuario.Apellido,
                        rol = usuario.Rol
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        //  Endpoint para verificar token (opcional)
        [HttpGet("VERIFICAR TOKEN")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult VerificarToken()
        {
            var email = User.Identity?.Name;
            var rol = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            return Ok(new
            {
                mensaje = "Token válido",
                email = email,
                rol = rol
            });
        }
    }
}