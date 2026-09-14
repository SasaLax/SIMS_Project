using System;

namespace CommunityHub.Application.Domain
{
    public enum BuildingStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class Building
    {
        public long Id { get; private set; }
        public string BuildingCode { get; private set; }
        public Address Address { get; private set; }
        public string Neighbourhood { get; private set; }
        public Location Location { get; private set; }
        public int NumberOfFloors { get; private set; }
        public string? ManagerJmbg { get; private set; }
        public BuildingStatus Status { get; private set; }

        public Building(long id, string buildingCode, Address address, string neighbourhood, Location location, int numberOfFloors, string? managerJmbg, BuildingStatus status = BuildingStatus.Pending)
        {
            Id = id;
            BuildingCode = buildingCode;
            Address = address;
            Neighbourhood = neighbourhood;
            Location = location;
            NumberOfFloors = numberOfFloors;
            ManagerJmbg = managerJmbg;
            Status = status;
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

        public void Approve() => Status = BuildingStatus.Approved;
        public void Reject() => Status = BuildingStatus.Rejected;
    }
}
