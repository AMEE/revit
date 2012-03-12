using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;

namespace AMEE_in_Revit.Addin
{
    public class AMEEPanel : IExternalApplication
    {
        // ExternalCommands assembly path
        static string AddInPath = typeof(AMEEPanel).Assembly.Location;
        // Button icons directory
        static string ButtonIconsFolder = Path.GetDirectoryName(AddInPath);
        // uiApplication
        static UIApplication uiApplication = null;

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                var panel = application.CreateRibbonPanel(Tab.Analyze, "AMEE");
                AddAMEEConnectButton(panel);
                panel.AddSeparator();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "AMEE-in-Revit error");

                return Result.Failed;
            }
        }

        private void AddAMEEConnectButton(RibbonPanel panel)
        {
            var pushButton = panel.AddItem(new PushButtonData("launchAMEEConnect", "AMEE Connect", AddInPath, "AMEE_in_Revit.Addin.LaunchAMEEConnectCommand")) as PushButton;
            pushButton.ToolTip = "Say Hello World";
            // Set the large image shown on button
            pushButton.LargeImage = new BitmapImage(new Uri(ButtonIconsFolder+ @"\AMEE.ico"));
        }


        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
