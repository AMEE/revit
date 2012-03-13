using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;

namespace AMEE_in_Revit.Addin
{
    public class AnalysisDisplayStyles
    {
        public void SetCO2eAnalysisDisplayStyle(View view)
        {
            var t = new Transaction(view.Document);
            t.Start("SetCO2eAnalysisDisplayStyle");

            AnalysisDisplayStyle analysisDisplayStyle = null;
            // Look for an existing analysis display style with a specific name
            var collector1 = new FilteredElementCollector(view.Document);
            ICollection<Element> collection =
                collector1.OfClass(typeof(AnalysisDisplayStyle)).ToElements();
            var displayStyle = from element in collection
                               where element.Name == "CO2e Display Style 1"
                               select element;
            // If display style does not already exist in the document, create it
            if (displayStyle.Count() == 0)
            {
                var coloredSurfaceSettings = new AnalysisDisplayColoredSurfaceSettings { ShowGridLines = false };
                var colorSettings = new  AnalysisDisplayColorSettings();
                var deepRed = new Color(166, 0, 0);
                var red = new Color(255, 44, 01);
                var orange = new Color(255, 179, 0);
                var green = new Color(0, 253, 0);
                var lightGreen = new Color(128, 255, 12);
                colorSettings.MaxColor = deepRed;
                colorSettings.SetIntermediateColors(new List<AnalysisDisplayColorEntry>
                                                        { 
                                                          new AnalysisDisplayColorEntry(green),
                                                          new AnalysisDisplayColorEntry(orange),
                                                          new AnalysisDisplayColorEntry(red)
                                                        });
                colorSettings.MinColor = lightGreen;
                var legendSettings = new AnalysisDisplayLegendSettings
                                         {
                                             NumberOfSteps = 10,
                                             Rounding = 0.05,
                                             ShowDataDescription = false,
                                             ShowLegend = true
                                         };
                var collector2 = new FilteredElementCollector(view.Document);
                ICollection<Element> elementCollection = collector2.OfClass(typeof(TextNoteType)).ToElements();
                var textElements = from element in collector2
                                   where element.Name == "LegendText"
                                   select element;
          
                // if LegendText exists, use it for this Display Style
                if (textElements.Count() > 0)
                {
                    var textType =
                        textElements.Cast<TextNoteType>().ElementAt(0);
                    legendSettings.SetTextTypeId(textType.Id, view.Document);
                }
                analysisDisplayStyle = AnalysisDisplayStyle.CreateAnalysisDisplayStyle(view.Document,
                                                                                       "CO2e Display Style 1", coloredSurfaceSettings, colorSettings, legendSettings);
            }
            else
            {
                analysisDisplayStyle =
                    displayStyle.Cast<AnalysisDisplayStyle>().ElementAt(0);
            }
            // now assign the display style to the view
            view.AnalysisDisplayStyleId = analysisDisplayStyle.Id;

            t.Commit();
        }
    }
}