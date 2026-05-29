using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityHub.Application.Domain
{
    public class Address
    {
        public String Street { get; private set; }
        public int Number { get; private set; }

        public Address(string street, int number)
        {
            this.Street = street;
            this.Number = number;
        }
    }
}
