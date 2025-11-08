using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Interfaces
{
    public interface IEventoRepository : IGenericRepository<Evento>
    {
        /*   List<Evento> GetAll();
           Evento? GetById(int id);
           void Add(Evento evento);
           void Update(Evento evento);
           void Delete(int id);*/
        Evento GetByIdWithCreator(int id); //  NUEVO MÉTODO
        List<Evento> GetAllWithCreators(); //  NUEVO MÉTODO

    }
}