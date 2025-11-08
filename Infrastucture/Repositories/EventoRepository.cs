using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repositories
{
    public class EventoRepository : GenericRepository<Evento>, IEventoRepository
    {
        public EventoRepository(VirticketDbContext context) : base(context) { }

        public Evento? GetByIdWithCreator(int id)
        {
            return _context.Eventos
                .AsNoTracking()
                .Include(e => e.UsuarioCreador)
                .FirstOrDefault(e => e.Id == id);
        }

        public List<Evento> GetAllWithCreators()
        {
            return _context.Eventos
                .AsNoTracking()
                .Include(e => e.UsuarioCreador)
                .ToList();
        }
    }
}