using System.Collections.Generic;
using System.Linq;
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

//        private void EnsureMaterialDataItemIsLoaded(string materialName)
//        {
//
//                            if (_uid == null) {
//             Do drill and get UID
//                                RestRequest request = new RestRequest(Method.GET);
//                                string drillString = "";
//                                foreach (List<string> x in drills)
//                                {
//                                    if (drillString.Length != 0)
//                                        drillString += '&';
//                                    drillString += HttpUtility.UrlEncode(x[0]);
//                                    drillString += '=';
//                                    drillString += HttpUtility.UrlEncode(x[1]);
//                                }
//                                request.Resource = "/data" + _path + "/drill?" + drillString;
//                                RestResponse response = Connection.Instance().DoRequest(request);
//                                XmlNodeList nodes = Connection.Xpath(response.Content, "//amee:Choices/amee:Choice/amee:Name/text()");
//                                if (nodes.Count > 0)
//                                {
//                                    _uid = nodes.Item(0).Value;
//                                }
//                                else
//                                {
//                                    throw new Exception("Bad drill: " + _path + " " + drillString);
//                                }
//                            }
//                            return _uid;
//                        }
//        }

        public string DiscoverLink()
        {
            string link = "http://discover.amee.com/categories" + Path + "/data";
            return Drills.Aggregate(link, (current, x) => current + ("/" + HttpUtility.UrlEncode(x[1])));
        }

    }
}
