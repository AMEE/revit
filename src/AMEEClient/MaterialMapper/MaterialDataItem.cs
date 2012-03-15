using System;
using System.Collections.Generic;
using System.Linq;
using AMEEClient.Model;
using CityIndex.JsonClient;

namespace AMEEClient.MaterialMapper
{
    public class MaterialDataItem
    {
        private readonly Client _ameeClient;

        public MaterialDataItem(Client ameeClient, string materialName, string path, List<List<string>> drills)
        {
            _ameeClient = ameeClient;
            MaterialName = materialName;
            Path = path;
            Drills = drills;
        }

        public string MaterialName { get; private set; }
        public string Path { get; private set; }
        public List<List<string>> Drills { get; private set; }

        private string _uid;
        public string UID
        {
            get
            {
                if (string.IsNullOrEmpty(_uid))
                {
                    var response = _ameeClient.GetDrillDown(Path, Drills);
                    _uid = response.Choices.Choices[0].Value;
                }
                return _uid;
            }
        }

        public double DensityKgPerM3
        {
            get
            {
                //TODO - where can we get the actual density from?
                return 2;
            }
        }

        public string DiscoverLink()
        {
            var link = "http://discover.amee.com/categories" + Path + "/data";
            return Drills.Aggregate(link, (current, x) => current + ("/" + HttpUtility.UrlEncode(x[1])));
        }

        public double CalculateCO2eByMass(double massInKg)
        {
            var response = _ameeClient.Calculate(Path, UID, new ValueItem("mass", massInKg.ToString()));
            return Convert.ToDouble(response.Amounts.Amount.First(a => a.Type == "CO2e").Value);
        }

        public double CalculateCO2eByVolume(double volumeInM3)
        {
            var massInKg = volumeInM3*DensityKgPerM3;
            var response = _ameeClient.Calculate(Path, UID, new ValueItem("mass", massInKg.ToString()));
            return Convert.ToDouble(response.Amounts.Amount.First(a => a.Type == "CO2e").Value);
        }

    }
}
