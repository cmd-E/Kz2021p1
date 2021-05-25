using System.Collections.Generic;
using WebApplication1.EfStuff.Model.Airport;

namespace WebApplication1.EfStuff.Repositoryies.Airport.Intrefaces
{
    public interface IPassengersRepository : IBaseRepository<Passenger>
    {
        Passenger GetPassengerByCitizenId(long citizenId);
        public List<Flight> GetFlightsByPassengerId(long passengerId);
    }
}
