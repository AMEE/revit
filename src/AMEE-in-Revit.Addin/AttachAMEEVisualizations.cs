using AMEE_in_Revit.Addin.Visualizations;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace AMEE_in_Revit.Addin
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class AttachAMEEVisualizations : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication uiControlledApplication)
        {
            uiControlledApplication.ControlledApplication.DocumentOpened += OnDocumentOpen;
            return Result.Succeeded;
        }

        private static void OnDocumentOpen(object sender, DocumentOpenedEventArgs e)
        {
            var app = sender as Autodesk.Revit.ApplicationServices.Application;
            var uiApp = new UIApplication(app);
            var doc = uiApp.ActiveUIDocument.Document;

            var view = new ViewFinder(doc).Get3DViewNamed("CO2e");
            if (view == null)
            {
                TaskDialog.Show("Error", "A 3D view named 'CO2e' must exist to view the AMEE CO2e visualization.");
                return;
            }

            new AnalysisDisplayStyles().SetCO2eAnalysisDisplayStyle(view);

            CO2eFieldUpdater.CreateAndRegister(uiApp, view);

        }
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
