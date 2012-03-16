using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;

namespace AMEE_in_Revit.Addin.Visualizations
{
    public class CO2eVisualisationCreator
    {
        public static void UpdateCO2eVisualization(SpatialFieldManager sfm, Element element)
        {
            try
            {
                Debug.WriteLine(string.Format("Adding CO2e Analysis for element: {0}", element.Name));
                double CO2eForElement = 0;
                if (element.ParametersMap.Contains("CO2e"))
                {
                    CO2eForElement = element.ParametersMap.get_Item("CO2e").AsDouble();
                }

                int count = 0;
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

                    Debug.WriteLine(string.Format("elementId: {0}, face: {1}, spf idx: {2}, bounding box: {3},{4},{5},{6}", element.Id.IntegerValue, count, idx, bb.Min.U, bb.Min.V, bb.Max.U, bb.Max.V));

                    var pnts = new FieldDomainPointsByUV(uvPts);
                    var vals = new FieldValues(valList);

                    var resultSchema1 = new AnalysisResultSchema("CO2e schema", "AMEE CO2e schema");

                    sfm.UpdateSpatialFieldPrimitive(idx, pnts, vals, GetFirstRegisteredResult(sfm, resultSchema1));
                    count++;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        // Add the CO2e measuement for this point
        private static void AddMeasurement(double measurement, double u, double v, IList<UV> uvPts, IList<ValueAtPoint> valList)
        {
            uvPts.Add(new UV(u, v));
            valList.Add(new ValueAtPoint(new List<double> { measurement }));
        }

        private static int GetFirstRegisteredResult(SpatialFieldManager sfm, AnalysisResultSchema analysisResultSchema)
        {
            IList<int> registeredResults = new List<int>();
            registeredResults = sfm.GetRegisteredResults();
            return registeredResults.Count == 0 ? sfm.RegisterResult(analysisResultSchema) : registeredResults.First();
        }

        private static FaceArray GetFaces(Element element)
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
    }
}