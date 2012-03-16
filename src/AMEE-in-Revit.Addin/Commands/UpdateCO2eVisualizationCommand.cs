using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using AMEE_in_Revit.Addin.Visualizations;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Analysis;

namespace AMEE_in_Revit.Addin.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class UpdateCO2eVisualizationCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiApp = commandData.Application;
            var doc = uiApp.ActiveUIDocument.Document;
           
            var view = new ViewFinder(doc).Get3DViewNamed("CO2e");
            if (view == null)
            {
                TaskDialog.Show("Error", "A 3D view named 'CO2e' must exist to view the AMEE CO2e visualization.");
                return Result.Failed;
            }

            new AnalysisDisplayStyles().SetCO2eAnalysisDisplayStyle(view);

            var sfm = SpatialFieldManager.GetSpatialFieldManager(view);
            if (sfm == null) sfm = SpatialFieldManager.CreateSpatialFieldManager(view, 1); // One measurement value for each point
            sfm.Clear();

            var sw = new Stopwatch();
            sw.Start();
            var collector = new FilteredElementCollector(doc, view.Id);
            ICollection<Element> co2eElements = collector.WherePasses(Settings.CreateFilterForElementsWithCO2eParameter()).WhereElementIsNotElementType().ToElements();
            var count = 0;
            foreach (var element in co2eElements)
            {
                if (count++ > 200) continue;
                CO2eFieldUpdater.UpdateCO2eMeasurements(sfm, element);
            }
            sw.Stop();
            MessageBox.Show(string.Format("Update all CO2e Measurements in {0}", sw.Elapsed));

           // CO2eFieldUpdater.CreateAndRegister(uiApp, view);

            return Result.Succeeded;
        }
    }
}