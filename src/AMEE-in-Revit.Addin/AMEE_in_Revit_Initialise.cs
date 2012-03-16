using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;

namespace AMEE_in_Revit.Addin
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class AMEE_in_Revit_Initialise : IExternalApplication
    {
        static string AddInPath = typeof(AMEE_in_Revit_Initialise).Assembly.Location;
        static string ButtonIconsFolder = Path.Combine(Path.GetDirectoryName(AddInPath), "Icons");

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                var panel = application.CreateRibbonPanel(Tab.Analyze, "AMEE");
                AddRecalculateCO2eButton(panel);
                AddUpdateCO2eVisualization(panel);
                panel.AddSeparator();
                AddAMEEConnectButton(panel);

                CO2eParameter.CO2eFieldUpdater.CreateAndRegister(application.ActiveAddInId);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to initialize AMEE-in-Revit addin: " + ex, "AMEE-in-Revit error");

                return Result.Failed;
            }
        }

        private void AddRecalculateCO2eButton(RibbonPanel panel)
        {
            var pushButton = panel.AddItem(new PushButtonData("recalculateCO2e", "Recalculate CO2e\nfor all elements", AddInPath, "AMEE_in_Revit.Addin.Commands.RecalculateCO2eCommand")) as PushButton;
            pushButton.Image = pushButton.LargeImage = new BitmapImage(new Uri(ButtonIconsFolder + @"\calculate.png"));
        }

        private void AddUpdateCO2eVisualization(RibbonPanel panel)
        {
            var pushButton = panel.AddItem(new PushButtonData("updateCO2eVisualization", "Update CO2e\nvisualization", AddInPath, "AMEE_in_Revit.Addin.Commands.UpdateCO2eVisualizationCommand")) as PushButton;
            pushButton.Image = pushButton.LargeImage = new BitmapImage(new Uri(ButtonIconsFolder + @"\visualize.png"));
        }

        private void AddAMEEConnectButton(RibbonPanel panel)
        {
            var pushButton = panel.AddItem(new PushButtonData("launchAMEEConnect", "AMEE Connect", AddInPath, "AMEE_in_Revit.Addin.Commands.LaunchAMEEConnectCommand")) as PushButton;
            pushButton.ToolTip = "Say Hello World";
            pushButton.Image = pushButton.LargeImage = new BitmapImage(new Uri(ButtonIconsFolder + @"\search_amee.png"));
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
