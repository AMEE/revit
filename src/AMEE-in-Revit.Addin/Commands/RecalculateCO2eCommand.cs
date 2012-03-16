using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AMEE_in_Revit.Addin.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class RecalculateCO2eCommand : CommandBase, IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            SetStatusText("Recalculating CO2e for each element...");
            var sw = new Stopwatch();
            sw.Start();

            var collector = new FilteredElementCollector(commandData.Application.ActiveUIDocument.Document);
            ICollection<Element> ameeElements = collector.WherePasses(Settings.CreateFilterForElementsWithCO2eParameter()).WhereElementIsNotElementType().ToElements();

            var transaction = new Transaction(commandData.Application.ActiveUIDocument.Document, "RecalculateCO2eCommand");
            transaction.Start();
            Settings.GetCO2eCalculator().UpdateElementCO2eParameters(ameeElements);
            transaction.Commit();

            sw.Stop();
            SetStatusText("Recalculated all COe2 values in {0}", sw.Elapsed);
            return Result.Succeeded;
        }
    }
}