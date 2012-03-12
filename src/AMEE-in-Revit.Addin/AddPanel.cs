using System;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;

namespace AMEE_in_Revit.Addin
{
    public class CsAddpanel : Autodesk.Revit.UI.IExternalApplication
    {
        public Autodesk.Revit.UI.Result OnStartup(UIControlledApplication application)
        {
            // add new ribbon panel
            var ribbonPanel = application.CreateRibbonPanel("NewRibbonPanel");
            //Create a push button in the ribbon panel “NewRibbonPanel”
            //the add-in application “HelloWorld” will be triggered when button is pushed
            var pushButton = ribbonPanel.AddItem(new PushButtonData("HelloWorld",
            "HelloWorld", @"D:\HelloWorld.dll", "HelloWorld.CsHelloWorld")) as PushButton;
            // Set the large image shown on button
            var uriImage = new Uri(@"D:\Sample\HelloWorld\bin\Debug\39-Globe_32x32.png");
            var largeImage = new BitmapImage(uriImage);
            pushButton.LargeImage = largeImage;
            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
