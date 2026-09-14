using System;

namespace CommunityHub.Application.Domain
{
    public class Apartment
    {
        public long Id { get; set; }
        public int ApartmentNumber { get; private set; }
        public string Description { get; private set; }
        public int NumberOfRooms { get; private set; }
        public int MaxNumberOfResidents { get; private set; }
        public long BuildingId { get; private set; }

        public Apartment(long id, int apartmentNumber, string description, int numberOfRooms, int maxNumberOfResidents, long buildingId)
        {
            Id = id;
            ApartmentNumber = apartmentNumber;
            Description = description;
            NumberOfRooms = numberOfRooms;
            MaxNumberOfResidents = maxNumberOfResidents;
            BuildingId = buildingId;
        }
    }
}
