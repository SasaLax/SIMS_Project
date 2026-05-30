using System;
using System.Collections.Generic;
using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;

namespace CommunityHub.Ui.Helpers
{
    public static class BuildingSearchParser
    {
        private static readonly BuildingDbRepository _buildingRepository = new BuildingDbRepository();

        public static List<Building> ParseAndSearch(string unosiText)
        {
            int sobe = 0;
            int stanari = 0;
            string op = "";

            if (unosiText.Contains("&"))
            {
                string[] djelovi = unosiText.Split('&');
                int.TryParse(djelovi[0].Trim(), out sobe);
                int.TryParse(djelovi[1].Trim(), out stanari);
                op = "&";
            }

            else if (unosiText.Contains("|"))
            {
                string[] djelovi = unosiText.Split('|');
                int.TryParse(djelovi[0].Trim(), out sobe);
                int.TryParse(djelovi[1].Trim(), out stanari);
                op = "|";
            }

            else
            {
                int.TryParse(unosiText, out sobe);
                op = "";
            }

            return _buildingRepository.SearchByApartmentsAdvanced(sobe, stanari, op);
        }
    }
}