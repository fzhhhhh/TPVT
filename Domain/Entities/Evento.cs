using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Domain.Entities
{
    public class Evento
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Lugar { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public bool Disponible { get; set; } = true;

        //  Relación con Usuario (Creador)
        public int UsuarioCreadorId { get; set; }
        public Usuario UsuarioCreador { get; set; } = null!;
    }
}