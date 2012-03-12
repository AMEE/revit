using System;
using System.Linq;
using AMEEClient.Model;
using NUnit.Framework;

namespace AMEEClient.Tests
{
    [TestFixture]
    public class DefraFixture
    {
        public const string AmeeUrl = "https://stage.amee.com";
        public const string AmeeUserName = "AMEE_in_Revit";
        public const string AmeePassword = "ghmuasqx";

        [Test]
        public void CanGrabValue()
        {

            var client = new Client(new Uri(AmeeUrl), AmeeUserName, AmeePassword);
            var dataItemResponse = client.GetDataItem("transport/defra/fuel", "9DE1D9435784");
            var dataItemValue = dataItemResponse.DataItem.ItemValues.First(v => v.ItemValueDefinition.Path == "massTotalCO2ePerVolume").Value;

            Assert.AreEqual("0.54362", dataItemValue);
        }

        [Test]
        public void CanCalculate()
        {
            var client = new Client(new Uri(AmeeUrl), AmeeUserName, AmeePassword);
            var value = client.Calculate("transport/defra/fuel", "9DE1D9435784", new ValueItem("volume", "10"));

            Assert.AreEqual(value.Amounts.Amount[0].Value, "4.7385");
        }

        [Test]
        public void CanGetTransportDefraPassenger()
        {
            var client = new Client(new Uri(AmeeUrl), AmeeUserName, AmeePassword);

            // path is pretty robust, leading and trailing whacks and blanks will be trimmed
            string path = "/transport/defra/passenger / ";
            DrillDownResponse r = client.GetDrillDown(path);
            Assert.AreEqual("type", r.Choices.Name);
            var choice = r.Choices.Choices.FirstOrDefault(c => c.Name == "taxi");
            Assert.IsNotNull(choice);

            r = client.GetDrillDown(path, new ValueItem("type", "taxi"));
            Assert.AreEqual("subtype", r.Choices.Name);
            choice = r.Choices.Choices.FirstOrDefault(c => c.Name == "regular");
            Assert.IsNotNull(choice);

            r = client.GetDrillDown(path, new ValueItem("type", "taxi"), new ValueItem("subtype", "regular"));

            Assert.AreEqual(1, r.Choices.Choices.Count);
            Assert.AreEqual("uid", r.Choices.Name);

            choice = r.Choices.Choices.First();
            var item = client.GetDataItem(path, choice.Name);

        }


        /// <summary>
        /// http://www.amee.com/developer/docs/ch03.php
        /// http://www.amee.com/developer/docs/ch04.php
        /// 
        /// </summary>
        [Test]
        public void Home_Developer_Documentation_GetEmissionFactors()
        {

            var client = new Client(new Uri(AmeeUrl), AmeeUserName, AmeePassword);


            // Performing Drilldowns 

            const string path = "transport/defra/fuel";

            // get choices for transport/defra/fuel
            DrillDownResponse r = client.GetDrillDown(path);

            // drill down to petrol
            var choice = r.Choices.Choices.FirstOrDefault(c => c.Name == "petrol");

            Assert.IsNotNull(choice);
            var selectionName = r.Choices.Name;
            var selection = new ValueItem(selectionName, choice.Name);

            // get the uid for petrol
            r = client.GetDrillDown(path, selection);

            // this is the end of this drilldown hierarchy. if more levels existed we would repeat previous step
            // but in this case, the choices will be 'uid' with a single value that we use to get a data item
            Assert.AreEqual("uid", r.Choices.Name);
            Assert.AreEqual(1, r.Choices.Choices.Count);

            choice = r.Choices.Choices[0];



            // Fetching Data Items 

            // use the uid to fetch the data item
            // (appears that the choice 'name' is the value to use and, when present, 'value' echoes 'name')

            DataItemResponse item = client.GetDataItem(path, choice.Name);

            // go ahead and hover item and explore. have yet to determine different response formats for different paths but DataItemResponse 
            // seems to cover this path

            // Do Calculation
            //            var profile = client.CreateProfile();
            //
            //            var calc = client.Calculate(profile.Profile.Uid, path, new ValueItem("dataItemUid", item.DataItem.Uid), new ValueItem("volume", "500"), new ValueItem("representation", "full"));
            //            
            //            Assert.AreEqual("1155.8500000000001", calc.totalAmount.Value);
            //
            //            var defaultAmount = calc.profileItems[0].Amounts.Amount.FirstOrDefault(a => a.Default);
            //            Assert.IsNotNull(defaultAmount);
            //            
            // rounding error between total and defaultAmount
            // Expected string length 18 but was 7. Strings differ at index 7.
            //  Expected: "1155.8500000000001"                                
            //  But was:  "1155.85"                                           
            // Assert.AreEqual(calc.totalAmount.Value, defaultAmount.Value);
            //
            //            var relatedAmount = calc.profileItems[0].Amounts.Amount.FirstOrDefault(a => a.Type == "lifeCycleCO2e");
            //            Assert.IsNotNull(relatedAmount);
            //            Assert.AreEqual("1361.35", relatedAmount.Value);

        }
        [Test, Ignore("i think they are reworking the meaning of profiles")]
        public void CanUseProfiles()
        {

            var client = new Client(new Uri(AmeeUrl), AmeeUserName, AmeePassword);

            var profile = client.CreateProfile();

            var profiles = client.GetProfiles();
            Assert.Greater(profiles.Profiles.Length, 0);
            foreach (var p in profiles.Profiles)
            {
                client.DeleteProfile(p.Uid);
            }
            profiles = client.GetProfiles();
            Assert.AreEqual(0, profiles.Profiles.Length);
            client.Dispose();

        }
    }
}
