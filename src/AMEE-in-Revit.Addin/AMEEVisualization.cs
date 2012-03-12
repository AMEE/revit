using System;
using System.Linq;
using System.Collections.Generic;
using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.UI;

namespace AMEE_in_Revit.Addin
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Automatic)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class AMEEVisualization : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication uiControlledApplication)
        {
            uiControlledApplication.ControlledApplication.DocumentOpened += OnDocumentOpen;
            return Result.Succeeded;
        }

        private void OnDocumentOpen(object sender, DocumentOpenedEventArgs e)
        {
            var app = sender as Autodesk.Revit.ApplicationServices.Application;
            var uiApp = new UIApplication(app);
            var doc = uiApp.ActiveUIDocument.Document;

            var collector = new FilteredElementCollector(doc);
            collector.WherePasses(new ElementClassFilter(typeof(FamilyInstance)));
            var sphereElements = from element in collector where element.Name == "sphere" select element;
            if (sphereElements.Count() == 0)
            {
                TaskDialog.Show("Error", "Sphere family must be loaded");
                return;
            }
            var sphere = sphereElements.Cast<FamilyInstance>().First();
            var viewCollector = new FilteredElementCollector(doc);
            ICollection<Element> views = viewCollector.OfClass(typeof(View3D)).ToElements(); //This has to be here, even though we don't seem to use it?!
            var viewElements = from element in viewCollector where element.Name == "AVF" select element;
            if (viewElements.Count() == 0)
            {
                TaskDialog.Show("Error", "A 3D view named 'AVF' must exist to run this application.");
                return;
            }
            var view = viewElements.Cast<View>().First();

            var updater = new CO2eFieldUpdater(uiApp.ActiveAddInId, sphere.Id, view.Id);
            if (!UpdaterRegistry.IsUpdaterRegistered(updater.GetUpdaterId())) UpdaterRegistry.RegisterUpdater(updater);
            var wallFilter = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            var familyFilter = new ElementClassFilter(typeof(FamilyInstance));
            var massFilter = new ElementCategoryFilter(BuiltInCategory.OST_Mass);
            IList<ElementFilter> filterList = new List<ElementFilter> {wallFilter, familyFilter, massFilter};
            var filter = new LogicalOrFilter(filterList);

            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), filter, Element.GetChangeTypeGeometry());
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), filter, Element.GetChangeTypeElementDeletion());
        }
        public Result OnShutdown(UIControlledApplication application) { return Result.Succeeded; }
    }

    public class CO2eFieldUpdater : IUpdater
    {
        AddInId addinID;
        UpdaterId updaterID;
        ElementId sphereID;
        ElementId viewID;
        public CO2eFieldUpdater(AddInId id, ElementId sphere, ElementId view)
        {
            addinID = id;
            sphereID = sphere;
            viewID = view;
            updaterID = new UpdaterId(addinID, new Guid("010275A1-6560-48DA-8F60-71472270A984"));
        }
        public void Execute(UpdaterData data)
        {
            var doc = data.GetDocument();
            var app = doc.Application;

            var view = doc.get_Element(viewID) as View;
            var sphere = doc.get_Element(sphereID) as FamilyInstance;
            var sphereLP = sphere.Location as LocationPoint;
            var sphereXYZ = sphereLP.Point;

            var sfm = SpatialFieldManager.GetSpatialFieldManager(view);
            if (sfm == null) sfm = SpatialFieldManager.CreateSpatialFieldManager(view, 3); // Three measurement values for each point
            sfm.Clear();

            var collector = new FilteredElementCollector(doc, view.Id);
            var wallFilter = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            var massFilter = new ElementCategoryFilter(BuiltInCategory.OST_Mass);
            var filter = new LogicalOrFilter(wallFilter, massFilter);
            ICollection<Element> elements = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();

            foreach (Face face in GetFaces(elements))
            {
                var idx = sfm.AddSpatialFieldPrimitive(face.Reference);
                
                var doubleList = new List<double>();
                IList<UV> uvPts = new List<UV>();
                IList<ValueAtPoint> valList = new List<ValueAtPoint>();
                var bb = face.GetBoundingBox();
                for (var u = bb.Min.U; u < bb.Max.U; u = u + (bb.Max.U - bb.Min.U) / 15)
                {
                    for (var v = bb.Min.V; v < bb.Max.V; v = v + (bb.Max.V - bb.Min.V) / 15)
                    {
                        UV uvPnt = new UV(u, v);
                        uvPts.Add(uvPnt);
                        XYZ faceXYZ = face.Evaluate(uvPnt);
                        // Specify three values for each point
                        doubleList.Add(100);
                        doubleList.Add(-100);
                        doubleList.Add(1000);
                        valList.Add(new ValueAtPoint(doubleList));
                        doubleList.Clear();
                    }
                }
                var pnts = new FieldDomainPointsByUV(uvPts);
                var vals = new FieldValues(valList);

                var resultSchema1 = new AnalysisResultSchema("Schema 1", "Schema 1 Description");

                sfm.UpdateSpatialFieldPrimitive(idx, pnts, vals, GetFirstRegisteredResult(sfm, resultSchema1));
            }
        }

        private int GetFirstRegisteredResult(SpatialFieldManager sfm, AnalysisResultSchema analysisResultSchema)
        {
            IList<int> registeredResults = new List<int>();
            registeredResults = sfm.GetRegisteredResults();
            int idx1 = 0;
            if (registeredResults.Count == 0)
            {
                idx1 = sfm.RegisterResult(analysisResultSchema);
            }
            else
            {
                idx1 = registeredResults.First();
            }
            return idx1;
        }

        public string GetAdditionalInformation() { return "Calculate distance from sphere to walls and display results"; }
        public ChangePriority GetChangePriority() { return ChangePriority.FloorsRoofsStructuralWalls; }
        public UpdaterId GetUpdaterId() { return updaterID; }
        public string GetUpdaterName() { return "Distance to Surfaces"; }

        private FaceArray GetFaces(ICollection<Element> elements)
        {
            FaceArray faceArray = new FaceArray();
            Options options = new Options();
            options.ComputeReferences = true;
            foreach (Element element in elements)
            {
                GeometryElement geomElem = element.get_Geometry(options);
                if (geomElem != null)
                {
                    foreach (GeometryObject geomObj in geomElem.Objects)
                    {
                        Solid solid = geomObj as Solid;
                        if (solid != null)
                        {
                            foreach (Face f in solid.Faces)
                            {
                                faceArray.Append(f);
                            }
                        }
                        GeometryInstance inst = geomObj as GeometryInstance;
                        if (inst != null) // in-place family walls
                        {
                            foreach (Object o in inst.SymbolGeometry.Objects)
                            {
                                Solid s = o as Solid;
                                if (s != null)
                                {
                                    foreach (Face f in s.Faces)
                                    {
                                        faceArray.Append(f);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return faceArray;
        }
    }
}
