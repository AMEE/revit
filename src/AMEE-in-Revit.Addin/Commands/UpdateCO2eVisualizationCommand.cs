using System.Collections.Generic;
using System.Diagnostics;
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
    public class UpdateCO2eVisualizationCommand : CommandBase, IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiApp = commandData.Application;
            var doc = uiApp.ActiveUIDocument.Document;
            
            SetStatusText("Updating CO2e visualisation...");

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
                if (count++ > 200)
                {
                    SetStatusText("Skipping CO2e visualisation update for element {0} since we have already updated more than 200", element.Name);
                    continue;
                }
                SetStatusText("Updating CO2e visualisation for element {0}...", element.Name);
                CO2eVisualisationCreator.UpdateCO2eVisualization(sfm, element);
            }

            sw.Stop();
            SetStatusText("Updated all CO2e visualisations in {0}", sw.Elapsed);

            return Result.Succeeded;
        }
    }
}