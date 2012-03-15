using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.UI;

namespace AMEE_in_Revit.Addin.Visualizations
{
    public class CO2eFieldUpdater : IUpdater
    {
        AddInId addinID;
        UpdaterId updaterID;
        ElementId _viewIdId;

        public string GetAdditionalInformation() { return "Visualize the CO2e values of elements"; }
        public ChangePriority GetChangePriority() { return ChangePriority.FloorsRoofsStructuralWalls; }
        public UpdaterId GetUpdaterId() { return updaterID; }
        public string GetUpdaterName() { return "AMEE CO2e visualizer"; }

        public CO2eFieldUpdater(AddInId id, ElementId viewId)
        {
            addinID = id;
            _viewIdId = viewId;
            updaterID = new UpdaterId(addinID, new Guid("010275A1-6560-48DA-8F60-71472270A984"));
        }

        public void Execute(UpdaterData data)
        {
            var doc = data.GetDocument();

            var view = doc.get_Element(_viewIdId) as View;

            var sfm = SpatialFieldManager.GetSpatialFieldManager(view);
            if (sfm == null) sfm = SpatialFieldManager.CreateSpatialFieldManager(view, 1); // One measurement value for each point
            sfm.Clear();

            var collector = new FilteredElementCollector(doc, view.Id);
            ICollection<Element> elements = collector.WherePasses(Settings.CreateFilterForElementsWithCO2eParameter()).WhereElementIsNotElementType().ToElements();
            
            foreach (var element in elements)
            {
                double CO2eForElement = 0;
                if (element.ParametersMap.Contains("CO2e"))
                {
                    CO2eForElement = element.ParametersMap.get_Item("CO2e").AsDouble();
                }
                
                foreach (Face face in GetFaces(element))
                {
                    var idx = sfm.AddSpatialFieldPrimitive(face.Reference);

                    IList<UV> uvPts = new List<UV>();
                    IList<ValueAtPoint> valList = new List<ValueAtPoint>();
                    var bb = face.GetBoundingBox();
                    AddMeasurement(CO2eForElement, bb.Min.U, bb.Min.V, uvPts, valList);
                    AddMeasurement(CO2eForElement, bb.Min.U, bb.Max.V, uvPts, valList);
                    AddMeasurement(CO2eForElement, bb.Max.U, bb.Max.V, uvPts, valList);
                    AddMeasurement(CO2eForElement, bb.Max.U, bb.Min.V, uvPts, valList);

                    var pnts = new FieldDomainPointsByUV(uvPts);
                    var vals = new FieldValues(valList);

                    var resultSchema1 = new AnalysisResultSchema("CO2e schema", "AMEE CO2e schema");

                    sfm.UpdateSpatialFieldPrimitive(idx, pnts, vals, GetFirstRegisteredResult(sfm, resultSchema1));
                }
            }
            
        }

        // Add the CO2e measuement for this point
        private void AddMeasurement(double measurement, double u, double v, IList<UV> uvPts, IList<ValueAtPoint> valList)
        {
            uvPts.Add(new UV(u, v));
            valList.Add(new ValueAtPoint(new List<double> { measurement }));
        }

        private int GetFirstRegisteredResult(SpatialFieldManager sfm, AnalysisResultSchema analysisResultSchema)
        {
            IList<int> registeredResults = new List<int>();
            registeredResults = sfm.GetRegisteredResults();
            return registeredResults.Count == 0 ? sfm.RegisterResult(analysisResultSchema) : registeredResults.First();
        }

        private FaceArray GetFaces(Element element)
        {
            var faceArray = new FaceArray();
            var options = new Options();
            options.ComputeReferences = true;
            var geomElem = element.get_Geometry(options);
            if (geomElem != null)
            {
                foreach (GeometryObject geomObj in geomElem.Objects)
                {
                    var solid = geomObj as Solid;
                    if (solid != null)
                    {
                        foreach (Face f in solid.Faces)
                        {
                            faceArray.Append(f);
                        }
                    }
                    var inst = geomObj as GeometryInstance;
                    if (inst != null) // in-place family walls
                    {
                        foreach (Object o in inst.SymbolGeometry.Objects)
                        {
                            var s = o as Solid;
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
            return faceArray;
        }

        public static void CreateAndRegister(UIApplication uiApp, View view)
        {
            var updater = new CO2eFieldUpdater(uiApp.ActiveAddInId, view.Id);
            if (!UpdaterRegistry.IsUpdaterRegistered(updater.GetUpdaterId())) UpdaterRegistry.RegisterUpdater(updater);

            var filter = Settings.CreateFilterForElementsWithCO2eParameter();

            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), filter, Element.GetChangeTypeGeometry());
            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), filter, Element.GetChangeTypeElementDeletion());
        }
    }
}