using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication1.EfStuff.Model.Airport;
using WebApplication1.EfStuff.Repositoryies.Airport.Intrefaces;

namespace WebApplication1.EfStuff.Repositoryies.Airport
{
    public class PassengersRepository : BaseRepository<Passenger>, IPassengersRepository
    {
        public PassengersRepository(KzDbContext kzDbContext) : base(kzDbContext)
        {
        }

        public Passenger GetPassengerByCitizenId(long citizenId)
        {
            return _dbSet.SingleOrDefault(p => p.CitizenId == citizenId);
        }

        public List<Flight> GetFlightsByPassengerId(long passengerId)
        {
            return _dbSet.SingleOrDefault(p => p.Id == passengerId)?.Flights;
        }
    }
}
