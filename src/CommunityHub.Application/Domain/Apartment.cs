using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityHub.Application.Domain
{
    public class Apartment
    {
        public int Id { get; private set; }
        public string Description { get; private set; }

        public int numberOfRooms { get; private set; }

        public int maxNumberOfResidents { get; private set; }

        public string buildingId { get; private set; }

        public Apartment(int id, string description, int numberOfRooms, int maxNumberOfResidents, string buildingId)
        {
            this.Id = id;
            this.Description = description;
            this.numberOfRooms = numberOfRooms;
            this.maxNumberOfResidents = maxNumberOfResidents;
            this.buildingId = buildingId;
        }
    }
}
