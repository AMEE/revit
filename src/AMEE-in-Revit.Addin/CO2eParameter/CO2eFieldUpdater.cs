using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AMEE_in_Revit.Addin.CO2eParameter
{
    public class CO2eFieldUpdater : IUpdater
    {
        AddInId addinID;
        UpdaterId updaterID;

        public string GetAdditionalInformation() { return "Calculate the CO2e values of elements whenever the element changes"; }
        public ChangePriority GetChangePriority() { return ChangePriority.FloorsRoofsStructuralWalls; }
        public UpdaterId GetUpdaterId() { return updaterID; }
        public string GetUpdaterName() { return "AMEE CO2e calculator"; }

        public CO2eFieldUpdater(AddInId id)
        {
            addinID = id;
            updaterID = new UpdaterId(addinID, new Guid("010275A1-6560-48DA-8F60-71472270A984"));
        }

        public void Execute(UpdaterData data)
        {
            var doc = data.GetDocument();

            var elementIds = new List<ElementId>();
            elementIds.AddRange(data.GetAddedElementIds());
            elementIds.AddRange(data.GetModifiedElementIds());

            Settings.GetCO2eCalculator().UpdateElementCO2eParameters(
                elementIds.Select(doc.get_Element).ToList());
        }

        public static void CreateAndRegister(AddInId addinId)
        {
            var updater = new CO2eFieldUpdater(addinId);
            
            if (UpdaterRegistry.IsUpdaterRegistered(updater.GetUpdaterId()))
            {
                UpdaterRegistry.UnregisterUpdater(updater.GetUpdaterId());
            }
            UpdaterRegistry.RegisterUpdater(updater);

            var filter = Settings.CreateFilterForElementsWithCO2eParameter();

            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), filter, Element.GetChangeTypeGeometry());
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), filter, Element.GetChangeTypeElementDeletion());

        }
    }
}