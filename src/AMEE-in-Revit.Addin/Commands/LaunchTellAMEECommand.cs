using AMEE_in_Revit.Addin.Mockups;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AMEE_in_Revit.Addin.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class LaunchTellAMEECommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var f = new TellAMEEForm();
            f.ShowDialog();
                
            return Result.Succeeded;
        }
    }
}
