using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.EfStuff.Model.Airport;
using WebApplication1.EfStuff.Repositoryies.Airport.Intrefaces;
using WebApplication1.EfStuff.Repositoryies.Interface;

namespace WebApplication1.Utils.MiniTimeline
{
    public class AirportUpdate
    {
        private int _landingStripCapacity = 10;
        private IServiceProvider _serviceProvider;
        public AirportUpdate(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void AirportStateUpdater()
        {
            AdmitPassengers();
            Task.WhenAll(
                Task.Run(() => LandFlights()),
                Task.Run(() => DepartPassengers()),
                Task.Run(() => ReturnFlights())
                );
        }

        private void LandFlights()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _citizenRepository = scope.ServiceProvider.GetRequiredService<ICitizenRepository>();
                var _flightsRepository = scope.ServiceProvider.GetRequiredService<IFlightsRepository>();
                var planesOnLandingStrip = _flightsRepository.GetLandedFlights().Count();
                var arrivingFlights = _flightsRepository.GetArrivingFlights();
                for (int i = 0; i < _landingStripCapacity - planesOnLandingStrip && i < arrivingFlights.Count(); i++)
                {
                    arrivingFlights[i].FlightStatus = FlightStatus.Landed;
                    _flightsRepository.Save(arrivingFlights[i]);
                    arrivingFlights.RemoveAt(i);
                }
                DelayFlights(arrivingFlights);
            }
        }

        private void AdmitPassengers()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var admittedFlights = 0;
                var _citizenRepository = scope.ServiceProvider.GetRequiredService<ICitizenRepository>();
                var _flightsRepository = scope.ServiceProvider.GetRequiredService<IFlightsRepository>();
                var arrivingFlights = _flightsRepository.GetLandedFlights();
                arrivingFlights.ForEach(f =>
                {
                    f.Passengers.ForEach(p =>
                    {
                        p.Flights.Remove(f);
                        p.Citizen.IsOutOfCity = false;
                        _citizenRepository.Save(p.Citizen);
                    });
                    admittedFlights++;
                });
                ConvertFlights(arrivingFlights, _flightsRepository);
            }
        }

        private void DepartPassengers()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _citizenRepository = scope.ServiceProvider.GetRequiredService<ICitizenRepository>();
                var _flightsRepository = scope.ServiceProvider.GetRequiredService<IFlightsRepository>();
                var departingFlights = _flightsRepository.GetDepartingFlights();
                var planesOnLandingStrip = _flightsRepository.GetLandedFlights();
                for (int i = 0; i < _landingStripCapacity - planesOnLandingStrip.Count() && i < departingFlights.Count(); i++)
                {
                    departingFlights[i].Passengers.ForEach(p =>
                    {
                        p.Citizen.IsOutOfCity = true;
                        _citizenRepository.Save(p.Citizen);
                    });
                    departingFlights[i].FlightStatus = FlightStatus.Departed;
                    _flightsRepository.Save(departingFlights[i]);
                    departingFlights.RemoveAt(i);
                }
                DelayFlights(departingFlights);
            }
        }

        private void ReturnFlights()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _citizenRepository = scope.ServiceProvider.GetRequiredService<ICitizenRepository>();
                var _flightsRepository = scope.ServiceProvider.GetRequiredService<IFlightsRepository>();
                var returningFlights = _flightsRepository.GetDepartedFlights();
                ConvertFlights(returningFlights, _flightsRepository);
            }
        }

        private void ConvertFlights(List<Flight> flights, IFlightsRepository _flightsRepository)
        {
            Random random = new Random();
            string[] places = new string[] { "Moscow", "New York", "Sydney", "Los Angeles", "Berlin", "Tokyo", "Paris", "Istanbul", "Rome", "Krakow", "Singapore" };
            foreach (var flight in flights)
            {
                if (flight.FlightStatus == FlightStatus.Landed)
                {
                    flight.Place = places[random.Next(places.Length)];
                    flight.FlightType = FlightType.DepartingFlight;
                    flight.FlightStatus = FlightStatus.OnTime;
                }
                else if (flight.FlightStatus == FlightStatus.Departed)
                {
                    flight.FlightType = FlightType.IncomingFlight;
                    flight.FlightStatus = FlightStatus.Expected;
                }
                flight.Date = DateTime.Now.AddDays(random.Next(5)).AddHours(random.Next(12)).AddMinutes(random.Next(30));
                _flightsRepository.Save(flight);
            }
        }

        private void DelayFlights(List<Flight> flights)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _flightsRepository = scope.ServiceProvider.GetRequiredService<IFlightsRepository>();
                flights.ForEach(f =>
                {
                    f.FlightStatus = FlightStatus.Delayed;
                    f.Date.AddMinutes(15);
                    _flightsRepository.Save(f);
                });
            }
        }
    }
}
