using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("REGISTRO DE USUARIO")]
        public IActionResult Register([FromBody] RegisterDTO dto)
        {
            try
            {
                var resultado = _usuarioService.RegistrarUsuario(dto);

                if (resultado == "Usuario registrado con éxito.")
                {
                    return Ok(new { mensaje = resultado });
                }

                return BadRequest(new { error = resultado });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        //  Obtener todos los usuarios (solo Admin/SuperAdmin)
        [HttpGet("OBTENER TODOS LOS USUARIOS")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult ObtenerUsuarios()
        {
            try
            {
                var usuarios = _usuarioService.ObtenerUsuarios();

                // Mapear a DTO sin información sensible
                var usuariosDTO = usuarios.Select(u => new UsuarioInfoDTO
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    Email = u.Email,
                    Rol = u.Rol,
                    FechaCreacion = u.FechaCreacion
                }).ToList();

                return Ok(usuariosDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Obtener usuario por ID (Admin/SuperAdmin o el propio usuario)
        [HttpGet("OBTENER USUARIO POR ID")]
        [Authorize]
        public IActionResult ObtenerUsuarioPorId(int id)
        {
            try
            {
                var usuarioActualEmail = User.Identity?.Name;
                var usuarioActual = _usuarioService.ObtenerUsuarioPorEmail(usuarioActualEmail);
                var usuarioSolicitado = _usuarioService.ObtenerUsuarioPorId(id);

                if (usuarioSolicitado == null)
                    return NotFound(new { error = "Usuario no encontrado" });

                // Solo permitir si es admin, superadmin o el propio usuario
                if (usuarioActual?.Rol != "Admin" && usuarioActual?.Rol != "SuperAdmin" && usuarioActual?.Id != id)
                    return Forbid();

                // Retornar DTO sin información sensible
                var usuarioDTO = new UsuarioInfoDTO
                {
                    Id = usuarioSolicitado.Id,
                    Nombre = usuarioSolicitado.Nombre,
                    Apellido = usuarioSolicitado.Apellido,
                    Email = usuarioSolicitado.Email,
                    Rol = usuarioSolicitado.Rol,
                    FechaCreacion = usuarioSolicitado.FechaCreacion
                };

                return Ok(usuarioDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        //  Ver perfil del usuario logueado
        [HttpGet("VER PERFIL DE USUARIO LOGUEADO")]
        [Authorize]
        public IActionResult Perfil()
        {
            try
            {
                var email = User.Identity?.Name;
                var usuario = _usuarioService.ObtenerUsuarioPorEmail(email);

                if (usuario == null)
                    return NotFound(new { error = "Usuario no encontrado" });

                return Ok(new UsuarioInfoDTO
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Email = usuario.Email,
                    Rol = usuario.Rol,
                    FechaCreacion = usuario.FechaCreacion
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        //  Actualizar perfil del usuario logueado
        [HttpPut("ACTUALIZAR PERFIL DE USUARIO LOGUEADO")]
        [Authorize]
        public IActionResult ActualizarPerfil([FromBody] ActualizarPerfilDTO dto)
        {
            try
            {
                var email = User.Identity?.Name;
                var usuario = _usuarioService.ObtenerUsuarioPorEmail(email);

                if (usuario == null)
                    return NotFound(new { error = "Usuario no encontrado" });

                // Actualizar datos permitidos
                usuario.Nombre = dto.Nombre;
                usuario.Apellido = dto.Apellido;

                _usuarioService.ActualizarUsuario(usuario);

                return Ok(new { mensaje = "Perfil actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        //  Cambiar contraseña del usuario logueado
        [HttpPut("CAMBIAR CONTRASEÑA")]
        [Authorize]
        public IActionResult CambiarPassword([FromBody] CambiarPasswordDTO dto)
        {
            try
            {
                var email = User.Identity?.Name;
                var usuario = _usuarioService.ObtenerUsuarioPorEmail(email);

                if (usuario == null)
                    return NotFound(new { error = "Usuario no encontrado" });

                // Aquí deberías verificar la contraseña actual antes de cambiar
                // Por ahora asumimos que pasa directamente la nueva contraseña
                usuario.Password = dto.NuevaPassword; // Deberías hacer hash aquí

                _usuarioService.ActualizarUsuario(usuario);

                return Ok(new { mensaje = "Contraseña actualizada exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        //  Cambiar rol de usuario (solo SuperAdmin)
        [HttpPatch("CAMBIAR-ROL/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult CambiarRol([FromRoute] int id, [FromBody] CambiarRolDTO dto)
        {
            try
            {
                _usuarioService.CambiarRolUsuario(id, dto.NuevoRol);
                return Ok(new { mensaje = $"Rol del usuario {id} cambiado a {dto.NuevoRol}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        //  Eliminar usuario (solo SuperAdmin)
        [HttpDelete("eliminar/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult EliminarUsuario(int id)
        {
            try
            {
                _usuarioService.EliminarUsuario(id);
                return Ok(new { mensaje = $"Usuario {id} eliminado exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        //  Eliminar mi propia cuenta
        [HttpDelete("ELIMINAR CUENTA")]
        [Authorize]
        public IActionResult EliminarMiCuenta()
        {
            try
            {
                var email = User.Identity?.Name;
                var usuario = _usuarioService.ObtenerUsuarioPorEmail(email);

                if (usuario == null)
                    return NotFound(new { error = "Usuario no encontrado" });

                _usuarioService.EliminarUsuario(usuario.Id);
                return Ok(new { mensaje = "Cuenta eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}