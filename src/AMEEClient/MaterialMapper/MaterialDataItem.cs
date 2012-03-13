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



        public string DiscoverLink()
        {
            string link = "http://discover.amee.com/categories" + Path + "/data";
            return Drills.Aggregate(link, (current, x) => current + ("/" + HttpUtility.UrlEncode(x[1])));
        }

        public object CalculateCO2eByMass(int massInKg)
        {
            var response = _ameeClient.Calculate(Path, UID, new ValueItem("mass", massInKg.ToString()));
            return response.Amounts.Amount.First(a => a.Type == "CO2e").Value;
        }
    }
}
