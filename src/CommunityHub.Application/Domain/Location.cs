using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityHub.Application.Domain
{
    public class Location
    {
        public string City { get; private set; }

        public string Country { get; private set; }
        public Location(string city, string country)
        {
            this.City = city;
            this.Country = country;
        }
    }
}
