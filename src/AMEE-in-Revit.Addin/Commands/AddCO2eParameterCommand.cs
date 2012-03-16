using System.Windows.Forms;
using AMEE_in_Revit.Addin.CO2eParameter;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace AMEE_in_Revit.Addin.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class AddCO2eParameterCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            new SharedParameterManipulator(commandData.Application)
                .ApplyParameterToAllElements(Settings.CO2eParam);
            
            MessageBox.Show("Added CO2eParam to all built in categories");
            return Result.Succeeded;
        }
    }
}