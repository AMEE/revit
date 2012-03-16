using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using AMEEClient;
using AMEEClient.MaterialMapper;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AMEE_in_Revit.Addin.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class RecalculateCO2eCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var sw = new Stopwatch();
            sw.Start();

            var collector = new FilteredElementCollector(commandData.Application.ActiveUIDocument.Document);
            ICollection<Element> ameeElements = collector.WherePasses(Settings.CreateFilterForElementsWithCO2eParameter()).WhereElementIsNotElementType().ToElements();

            var transaction = new Transaction(commandData.Application.ActiveUIDocument.Document, "RecalculateCO2eCommand");
            transaction.Start();
            Settings.GetCO2eCalculator().UpdateElementCO2eParameters(ameeElements);
            transaction.Commit();

            sw.Stop();
            MessageBox.Show(string.Format("Recalculated all COe2 values in {0}", sw.Elapsed));
            return Result.Succeeded;
        }
    }
}