using AMEE_in_Revit.Addin.Visualizations;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AMEE_in_Revit.Addin.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class RunCO2eFieldUpdaterCommand : IExternalCommand
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

            CO2eFieldUpdater.CreateAndRegister(uiApp, view);

            return Result.Succeeded;
        }
    }
}