using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Net.Sockets;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CommunityHub.Application.Domain
{
    public class Building
    {
        public string id { get; private set; }

        public Address Address { get; private set; }

        public string Neighbourhood { get; private set; }

        public Location Location { get; private set; }

        public int numberOfFloors { get; private set; }

        public string? Manager { get; private set; } 

        public Building(string id, Address address, string neighbourhood, Location location, int numberOfFloors, string? manager)
        {
            this.id = id;
            this.Address = address;
            this.Neighbourhood = neighbourhood;
            this.Location = location;
            this.numberOfFloors = numberOfFloors;
            this.Manager = manager;
        }

        public string FullAddress
        {
            get
            {
                if (Address == null) return "";
                return $"{Address.Street} {Address.Number}".Trim();
            }
        }

        public string FullLocation
        {
            get
            {
                if (Location == null) return Neighbourhood;
                return $"{Location.City}, {Location.Country}".Trim();
            }
        }

    }
}
