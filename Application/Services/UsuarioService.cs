using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public string RegistrarUsuario(RegisterDTO dto)
        {
            var existe = _usuarioRepository.GetByEmail(dto.Email);
            if (existe != null)
                throw new UsuarioExceptions.EmailYaRegistradoException(dto.Email);

            var nuevoUsuario = new Usuario
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                Password = dto.Password, 
                FechaCreacion = DateTime.Now,
                Rol = dto.Rol ?? "User" 
            };

            _usuarioRepository.Add(nuevoUsuario);
            return "Usuario registrado con éxito.";
        }

        public List<Usuario> ObtenerUsuarios()
        {
            return _usuarioRepository.GetAll();
        }

        public Usuario? ObtenerUsuarioPorId(int id)
        {
            var usuario = _usuarioRepository.GetById(id);
            if (usuario == null)
                throw new UsuarioExceptions.UsuarioNoEncontradoException(id);

            return usuario;
        }

        public Usuario? ObtenerUsuarioPorEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            var usuario = _usuarioRepository.GetByEmail(email);
            if (usuario == null)
                throw new UsuarioExceptions.UsuarioNoEncontradoException(email);

            return usuario;
        }

        //  Nuevo método Login
        public Usuario? Login(LoginDTO dto)
        {
            var usuario = _usuarioRepository.GetByEmail(dto.Email);

            // Si no existe o la contraseña no coincide → lanza excepción
            if (usuario == null || usuario.Password != dto.Password)
                throw new UsuarioExceptions.CredencialesInvalidasException();

            // Si pasa la validación → retorna el usuario
            return usuario;
        }



        public void ActualizarUsuario(Usuario usuario)
        {
            var usuarioExistente = _usuarioRepository.GetById(usuario.Id);
            if (usuarioExistente == null)
                throw new UsuarioExceptions.UsuarioNoEncontradoException(usuario.Id);

            // Actualizar propiedades permitidas
            usuarioExistente.Nombre = usuario.Nombre;
            usuarioExistente.Apellido = usuario.Apellido;
            usuarioExistente.Email = usuario.Email;

            // Solo actualizar password si se proporciona uno nuevo
            if (!string.IsNullOrEmpty(usuario.Password))
                usuarioExistente.Password = usuario.Password;

            _usuarioRepository.Update(usuarioExistente);
        }

        public void EliminarUsuario(int id)
        {
            var usuario = _usuarioRepository.GetById(id);
            if (usuario == null)
                //throw new Exception("Usuario no encontrado");
                throw new UsuarioExceptions.UsuarioNoEncontradoException(id);

            _usuarioRepository.Delete(id);
        }

        public void CambiarRolUsuario(int id, string nuevoRol)
        {
            var usuario = _usuarioRepository.GetById(id);
            if (usuario == null)
                //throw new Exception("Usuario no encontrado");
                throw new UsuarioExceptions.UsuarioNoEncontradoException(id);

            // Validar que el rol sea válido
            var rolesValidos = new[] { "User", "Admin", "SuperAdmin" };
            if (!rolesValidos.Contains(nuevoRol))
                //throw new Exception("Rol no válido");
                throw new ValidationException("Rol", "Rol no válido");

            usuario.Rol = nuevoRol;
            _usuarioRepository.Update(usuario);
        }
        public bool ExisteUsuario(int id)
        {
            return _usuarioRepository.GetById(id) != null;
        }

    }
}