//
// (C) Copyright 2003-2011 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//


using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit;
using Autodesk.Revit.ApplicationServices;

namespace Revit.SDK.Samples.Openings.CS
{
    /// <summary>
    /// This class which inherit from Autodesk.Revit.DB.BoundingBoxXYZ
    /// store the information about Max (Min) coordinate of object
    /// can get all the corner point coordinate and create X model line
    /// </summary>
    public class BoundingBox : BoundingBoxXYZ
    {
        /// <summary>
        /// define whether we have created Model Line on this BoundingBox
        /// </summary>
        private bool m_isCreated;  
        
        /// <summary>
        /// store all the corner points in BoundingBox
        /// </summary>
        private readonly List<Autodesk.Revit.DB.XYZ > m_points = new List<Autodesk.Revit.DB.XYZ >();  
        
        /// <summary>
        /// property to get all the points
        /// </summary>
        public List<Autodesk.Revit.DB.XYZ > Points
        {
            get
            {
                return m_points;
            }
        }

        /// <summary>
        /// property to get width of BoundingBox (short side)
        /// </summary>
        public double Width
        {
            get
            {
                double yDistance = 0;
                double xDistance = 0;
                yDistance = m_points[2].Y - m_points[1].Y;
                xDistance = m_points[5].X - m_points[2].X;
                return xDistance < yDistance ? xDistance : yDistance;
            }
        }

        /// <summary>
        /// property to get Length of BoundingBox (long side)
        /// </summary>
        public double Length
        {
            get
            {
                double yDistance = 0;
                double xDistance = 0;
                yDistance = m_points[2].Y - m_points[1].Y;
                xDistance = m_points[5].X - m_points[2].X;
                return xDistance > yDistance ? xDistance : yDistance;
            }
        }

        /// <summary>
        /// The default constructor
        /// </summary>
        /// <param name="boundBoxXYZ">The reference of the application in revit</param>
        public BoundingBox(BoundingBoxXYZ boundBoxXYZ)
        {
            this.Min = boundBoxXYZ.Min;
            this.Max = boundBoxXYZ.Max;

            GetCorners();
        }

        /// <summary>
        /// Create X model line with the BoundBox
        /// Create 12 lines to makeup an cube
        /// </summary>
        /// <param name="app">Application get from RevitAPI</param>
        public void CreateLines(UIApplication app)
        {
            if (m_isCreated)
            {
                return;
            }

            //create 12 lines
            for (int i = 0; i < 7; i++)
            {
                NewModelLine(app, i, i+1);
            }

            for (int i = 0; i < 5; i = i + 2)
            {
                NewModelLine(app, i, i + 3);
            }

            NewModelLine(app, 1, 6);
            NewModelLine(app, 0, 7);

            m_isCreated = true;
        }

        /// <summary>
        /// get all the Corner points of Cube Box via Min and Max
        /// </summary>
        private void GetCorners()
        {
            m_points.Add(this.Min);

            Autodesk.Revit.DB.XYZ point = new Autodesk.Revit.DB.XYZ(
                this.Min.X,
                this.Min.Y,
                this.Max.Z);
            m_points.Add(point);

            Autodesk.Revit.DB.XYZ point2 = new Autodesk.Revit.DB.XYZ(
                this.Min.X,
                this.Max.Y,
                this.Max.Z);
            m_points.Add(point2);

            Autodesk.Revit.DB.XYZ point3 = new Autodesk.Revit.DB.XYZ (
                this.Min.X,
                this.Max.Y,
                this.Min.Z);
            m_points.Add(point3);

            Autodesk.Revit.DB.XYZ point4 = new Autodesk.Revit.DB.XYZ(
                this.Max.X,
                this.Max.Y,
                this.Min.Z);
            m_points.Add(point4);

            m_points.Add(this.Max);

            Autodesk.Revit.DB.XYZ point5 = new Autodesk.Revit.DB.XYZ (
                this.Max.X,
                this.Min.Y,
                this.Max.Z);
            m_points.Add(point5);

            Autodesk.Revit.DB.XYZ point6 = new Autodesk.Revit.DB.XYZ(
                this.Max.X,
                this.Min.Y,
                this.Min.Z);
            m_points.Add(point6);
        }

        /// <summary>
        /// Create a Sketch Plane which pass the defined line
        /// the defined line must be one of BoundingBox Profile
        /// </summary>
        /// <param name="app">Application get from RevitAPI</param>
        /// <param name="aline">a line which sketch plane pass</param>
        private SketchPlane NewSketchPlanePassLine(Line aline, UIApplication app)
        {
            //in a cube only
            Autodesk.Revit.DB.XYZ norm;
            if (aline.get_EndPoint(0).X == aline.get_EndPoint(1).X)
            {
                norm = new Autodesk.Revit.DB.XYZ (1, 0, 0);
            }
            else if (aline.get_EndPoint(0).Y == aline.get_EndPoint(1).Y)
            {
                norm = new Autodesk.Revit.DB.XYZ (0, 1, 0);
            }
            else
            {
                norm = new Autodesk.Revit.DB.XYZ (0, 0, 1);
            }

            Autodesk.Revit.DB.XYZ point = aline.get_EndPoint(0);
            Plane plane = app.Application.Create.NewPlane(norm, point);
            SketchPlane sketchPlane = app.ActiveUIDocument.Document.Create.NewSketchPlane(plane);
            return sketchPlane;
        }

        /// <summary>
        /// new ModelLine in BoundingBox
        /// </summary>
        /// <param name="app">Application get from RevitAPI</param>
        /// <param name="pointIndex1">index of point in BoundingBox acme</param>
        /// <param name="pointIndex2">index of another point in BoundingBox acme</param>
        private void NewModelLine(UIApplication app, int pointIndex1, int pointIndex2)
        {
            Autodesk.Revit.DB.XYZ startP2 = m_points[pointIndex1];
            Autodesk.Revit.DB.XYZ endP2 = m_points[pointIndex2];

            try
            {
                Line line = Line.get_Bound(startP2, endP2);
                SketchPlane sketchPlane = NewSketchPlanePassLine(line, app);
                Line line2 = app.Application.Create.NewLineBound(startP2, endP2);
                app.ActiveUIDocument.Document.Create.NewModelCurve(line2, sketchPlane);
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
