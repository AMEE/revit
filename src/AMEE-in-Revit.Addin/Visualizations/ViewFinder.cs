using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace AMEE_in_Revit.Addin
{
    public class ViewFinder
    {
        private readonly Document _document;

        public ViewFinder(Document document)
        {
            _document = document;
        }

        public View Get3DViewNamed(string viewName)
        {
            var viewCollector = new FilteredElementCollector(_document);
            ICollection<Element> views = viewCollector.OfClass(typeof(View3D)).ToElements(); //This has to be here, even though we don't seem to use it?!
            var viewElements = from element in viewCollector where element.Name == viewName select element;
            if (viewElements.Count() == 0)
            {
                return null;
            }
            return viewElements.Cast<View>().First();
        }
    }
}