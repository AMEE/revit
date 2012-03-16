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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Element = Autodesk.Revit.DB.Element;

namespace Revit.SDK.Samples.ManipulateForm.CS
{
   /// <summary>
   /// Implements the Revit add-in interface IExternalCommand
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
   public class Command : IExternalCommand
   {
      /// <summary>
      /// Revit document
      /// </summary>
      Document m_revitDoc;
      /// <summary>
      /// Revit application
      /// </summary>
      Application m_revitApp;
      /// <summary>
      /// Rectangle length of bottom profile
      /// </summary>
      double m_bottomLength = 200;
      /// <summary>
      /// Rectangle width of bottom profile
      /// </summary>
      double m_bottomWidth = 120;
      /// <summary>
      /// Height of bottom profile
      /// </summary>
      double m_bottomHeight = 0;
      /// <summary>
      /// Rectangle length of top profile
      /// </summary>
      double m_topLength = 140;
      /// <summary>
      /// Rectangle width of top profile
      /// </summary>
      double m_topWidth = 60;
      /// <summary>
      /// Height of top profile
      /// </summary>
      double m_topHeight = 40;
      /// <summary>
      /// offset of profile
      /// </summary>
      double m_profileOffset = 10;
      /// <summary>
      /// offset of vertex on bottom profile
      /// </summary>
      double m_vertexOffsetOnBottomProfile = 20;
      /// <summary>
      /// offset of vertex on middle profile
      /// </summary>
      double m_vertexOffsetOnMiddleProfile = 10;
      /// <summary>
      /// Used for double compare
      /// </summary>
      const double Epsilon = 0.000001;

      /// <summary>
      /// Implement this method as an external command for Revit.
      /// </summary>
      /// <param name="commandData">An object that is passed to the external application 
      /// which contains data related to the command, 
      /// such as the application object and active view.</param>
      /// <param name="message">A message that can be set by the external application 
      /// which will be displayed if a failure or cancellation is returned by 
      /// the external command.</param>
      /// <param name="elements">A set of elements to which the external application 
      /// can add elements that are to be highlighted in case of failure or cancellation.</param>
      /// <returns>Return the status of the external command. 
      /// A result of Succeeded means that the API external method functioned as expected. 
      /// Cancelled can be used to signify that the user cancelled the external operation 
      /// at some point. Failure should be returned if the application is unable to proceed with 
      /// the operation.</returns>
      public virtual Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData
          , ref string message, Autodesk.Revit.DB.ElementSet elements)
      {
         m_revitApp = commandData.Application.Application;
         m_revitDoc = commandData.Application.ActiveUIDocument.Document;
         Transaction transaction = new Transaction(m_revitDoc, "ManipulateForm");

         try
         {
            transaction.Start();

            // Create a loft form
            Form form = CreateLoft();
            m_revitDoc.Regenerate();
            // Add profile to the loft form
            int profileIndex = AddProfile(form);
            m_revitDoc.Regenerate();
            // Move the edges on added profile
            MoveEdgesOnProfile(form, profileIndex);
            m_revitDoc.Regenerate();
            // Move the added profile
            MoveProfile(form, profileIndex);
            m_revitDoc.Regenerate();
            // Move the vertex on bottom profile
            MoveVertexesOnBottomProfile(form);
            m_revitDoc.Regenerate();
            // Add edge to the loft form
            Reference edgeReference = AddEdge(form);
            m_revitDoc.Regenerate();
            // Move the added edge
            Autodesk.Revit.DB.XYZ offset = new Autodesk.Revit.DB.XYZ (0, -40, 0);
            MoveSubElement(form, edgeReference, offset);
            m_revitDoc.Regenerate();
            // Move the vertex on added profile
            MoveVertexesOnAddedProfile(form, profileIndex);
            m_revitDoc.Regenerate();

            transaction.Commit();
         }
         catch (Exception ex)
         {
            message = ex.Message;
            transaction.RollBack();
            return Autodesk.Revit.UI.Result.Failed;
         }

         return Autodesk.Revit.UI.Result.Succeeded;
      }

      /// <summary>
      /// Create a loft form
      /// </summary>
      /// <returns>Created loft form</returns>
      private Form CreateLoft()
      {
         // Prepare profiles for loft creation
         ReferenceArrayArray profiles = new ReferenceArrayArray();
         ReferenceArray bottomProfile = new ReferenceArray();
         bottomProfile = CreateProfile(m_bottomLength, m_bottomWidth, m_bottomHeight);
         profiles.Append(bottomProfile);
         ReferenceArray topProfile = new ReferenceArray();
         topProfile = CreateProfile(m_topLength, m_topWidth, m_topHeight);
         profiles.Append(topProfile);

         // return the created loft form
         return m_revitDoc.FamilyCreate.NewLoftForm(true, profiles);
      }

      /// <summary>
      /// Create a rectangle profile with provided length, width and height
      /// </summary>
      /// <param name="length">Length of the rectangle</param>
      /// <param name="width">Width of the rectangle</param>
      /// <param name="height">Height of the profile</param>
      /// <returns>The created profile</returns>
      private ReferenceArray CreateProfile(double length, double width, double height)
      {
         ReferenceArray profile = new ReferenceArray();
         // Prepare points to create lines
         List<XYZ> points = new List<XYZ>();
         points.Add(new Autodesk.Revit.DB.XYZ (-1 * length / 2, -1 * width / 2, height));
         points.Add(new Autodesk.Revit.DB.XYZ(length / 2, -1 * width / 2, height));
         points.Add(new Autodesk.Revit.DB.XYZ(length / 2, width / 2, height));
         points.Add(new Autodesk.Revit.DB.XYZ(-1 * length / 2, width / 2, height));

         // Prepare sketch plane to create model line
         Autodesk.Revit.DB.XYZ normal = new Autodesk.Revit.DB.XYZ (0, 0, 1);
         Autodesk.Revit.DB.XYZ origin = new Autodesk.Revit.DB.XYZ (0, 0, height);
         Plane geometryPlane = m_revitApp.Create.NewPlane(normal, origin);
         SketchPlane sketchPlane = m_revitDoc.FamilyCreate.NewSketchPlane(geometryPlane);

         // Create model lines and get their references as the profile
         for (int i = 0; i < 4; i++)
         {
            Autodesk.Revit.DB.XYZ startPoint = points[i];
            Autodesk.Revit.DB.XYZ endPoint = (i == 3 ? points[0] : points[i + 1]);
            Line line = m_revitApp.Create.NewLineBound(startPoint, endPoint);
            ModelCurve modelLine = m_revitDoc.FamilyCreate.NewModelCurve(line, sketchPlane);
            profile.Append(modelLine.GeometryCurve.Reference);
         }

         return profile;
      }

      /// <summary>
      /// Add profile to the loft form
      /// </summary>
      /// <param name="form">The loft form to be added edge</param>
      /// <returns>Index of the added profile</returns>
      private int AddProfile(Form form)
      {
         // Get a connecting edge from the form
         Autodesk.Revit.DB.XYZ startOfTop = new Autodesk.Revit.DB.XYZ (-1 * m_topLength / 2, -1 * m_topWidth / 2, m_topHeight);
         Autodesk.Revit.DB.XYZ startOfBottom = new Autodesk.Revit.DB.XYZ (-1 * m_bottomLength / 2, -1 * m_bottomWidth / 2, m_bottomHeight);
         Edge connectingEdge = GetEdgeByEndPoints(form, startOfTop, startOfBottom);

         // Add an profile with specific parameters
         double param = 0.5;
         return form.AddProfile(connectingEdge.Reference, param);
      }

      /// <summary>
      /// Move the profile
      /// </summary>
      /// <param name="form">The form contains the edge</param>
      /// <param name="profileIndex">Index of the profile to be moved</param>
      private void MoveProfile(Form form, int profileIndex)
      {
         Autodesk.Revit.DB.XYZ offset = new Autodesk.Revit.DB.XYZ (0, 0, 5);
         if (form.CanManipulateProfile(profileIndex))
         {
            form.MoveProfile(profileIndex, offset);
         }
      }

      /// <summary>
      /// Move the edges on profile
      /// </summary>
      /// <param name="form">The form contains the edge</param>
      /// <param name="profileIndex">Index of the profile to be moved</param>
      private void MoveEdgesOnProfile(Form form, int profileIndex)
      {
         Autodesk.Revit.DB.XYZ startOfTop = new Autodesk.Revit.DB.XYZ (-1 * m_topLength / 2, -1 * m_topWidth / 2, m_topHeight);
         Autodesk.Revit.DB.XYZ offset1 = new Autodesk.Revit.DB.XYZ (m_profileOffset, 0, 0);
         Autodesk.Revit.DB.XYZ offset2 = new Autodesk.Revit.DB.XYZ (-m_profileOffset, 0, 0);
         Reference r1 = null;
         Reference r2 = null;
         ReferenceArray ra = form.get_CurveLoopReferencesOnProfile(profileIndex, 0);
         foreach (Reference r in ra)
         {
            Line line = form.GetGeometryObjectFromReference(r) as Line;
            if (line == null)
            {
               throw new Exception("Get curve reference on profile as line error.");
            }
            Autodesk.Revit.DB.XYZ pnt1 = line.Evaluate(0, false);
            Autodesk.Revit.DB.XYZ pnt2 = line.Evaluate(1, false);
            if (Math.Abs(pnt1.X - pnt2.X) < Epsilon)
            {
               if (pnt1.X < startOfTop.X)
               {
                  r1 = r;
               }
               else
               {
                  r2 = r;
               }
            }
         }
         if ((r1 == null) || (r2 == null))
         {
            throw new Exception("Get line on profile error.");
         }
         MoveSubElement(form, r1, offset1);
         MoveSubElement(form, r2, offset2);
      }

      /// <summary>
      /// Move the form vertexes
      /// </summary>
      /// <param name="form">The form contains the vertexes</param>
      private void MoveVertexesOnBottomProfile(Form form)
      {
         Autodesk.Revit.DB.XYZ offset1 = new Autodesk.Revit.DB.XYZ (-m_vertexOffsetOnBottomProfile, -m_vertexOffsetOnBottomProfile, 0);
         Autodesk.Revit.DB.XYZ offset2 = new Autodesk.Revit.DB.XYZ (m_vertexOffsetOnBottomProfile, -m_vertexOffsetOnBottomProfile, 0);

         Autodesk.Revit.DB.XYZ startOfBottom = new Autodesk.Revit.DB.XYZ (-1 * m_bottomLength / 2, -1 * m_bottomWidth / 2, m_bottomHeight);
         Autodesk.Revit.DB.XYZ endOfBottom = new Autodesk.Revit.DB.XYZ (m_bottomLength / 2, -1 * m_bottomWidth / 2, m_bottomHeight);
         Edge bottomEdge = GetEdgeByEndPoints(form, startOfBottom, endOfBottom);
         ReferenceArray pntsRef = form.GetControlPoints(bottomEdge.Reference);
         Reference r1 = null;
         Reference r2 = null;
         foreach (Reference r in pntsRef)
         {
            Point pnt = form.GetGeometryObjectFromReference(r) as Point;
            if (pnt.Coord.IsAlmostEqualTo(startOfBottom))
            {
               r1 = r;
            }
            else
            {
               r2 = r;
            }
         }
         MoveSubElement(form, r1, offset1);
         MoveSubElement(form, r2, offset2);        
      }

      /// <summary>
      /// Move the form vertexes on added profile
      /// </summary>
      /// <param name="form">The form contains the vertexes</param>
      /// <param name="profileIndex">Index of added profile</param>
      private void MoveVertexesOnAddedProfile(Form form, int profileIndex)
      {
         Autodesk.Revit.DB.XYZ offset = new Autodesk.Revit.DB.XYZ (0, m_vertexOffsetOnMiddleProfile, 0);

         ReferenceArray ra = form.get_CurveLoopReferencesOnProfile(profileIndex, 0);
         foreach (Reference r in ra)
         {
            ReferenceArray ra2 = form.GetControlPoints(r);
            foreach (Reference r2 in ra2)
            {
               Point vertex = form.GetGeometryObjectFromReference(r2) as Point;
               if (Math.Abs(vertex.Coord.X) < Epsilon)
               {
                  MoveSubElement(form, r2, offset);
                  break;
               }
            }
         }
      }

      /// <summary>
      /// Add edge to the loft form
      /// </summary>
      /// <param name="form">The loft form to be added edge</param>
      /// <returns>Reference of the added edge</returns>
      private Reference AddEdge(Form form)
      {
         // Get two specific edges from the form
         Autodesk.Revit.DB.XYZ startOfTop = new Autodesk.Revit.DB.XYZ (-1 * m_topLength / 2, -1 * m_topWidth / 2, m_topHeight);
         Autodesk.Revit.DB.XYZ endOfTop = new Autodesk.Revit.DB.XYZ (m_topLength / 2, -1 * m_topWidth / 2, m_topHeight);
         Edge topEdge = GetEdgeByEndPoints(form, startOfTop, endOfTop);
         Autodesk.Revit.DB.XYZ startOfBottom = new Autodesk.Revit.DB.XYZ (-1 * (m_bottomLength / 2 + m_vertexOffsetOnBottomProfile), -1 * (m_bottomWidth / 2 + m_vertexOffsetOnBottomProfile), m_bottomHeight);
         Autodesk.Revit.DB.XYZ endOfBottom = new Autodesk.Revit.DB.XYZ ((m_bottomLength / 2 + m_vertexOffsetOnBottomProfile), -1 * (m_bottomWidth / 2 + m_vertexOffsetOnBottomProfile), m_bottomHeight);
         Edge bottomEdge = GetEdgeByEndPoints(form, startOfBottom, endOfBottom);

         // Add an edge between the two edges with specific parameters
         double topParam = 0.5;
         double bottomParam = 0.5;
         form.AddEdge(topEdge.Reference, topParam, bottomEdge.Reference, bottomParam);
         m_revitDoc.Regenerate();

         // Get the added edge and return its reference
         Autodesk.Revit.DB.XYZ startOfAddedEdge = startOfTop.Add(endOfTop.Subtract(startOfTop).Multiply(topParam));
         Autodesk.Revit.DB.XYZ endOfAddedEdge = startOfBottom.Add(endOfBottom.Subtract(startOfBottom).Multiply(bottomParam));
         return GetEdgeByEndPoints(form, startOfAddedEdge, endOfAddedEdge).Reference;
      }

      /// <summary>
      /// Get an edge from the form by its endpoints
      /// </summary>
      /// <param name="form">The form contains the edge</param>
      /// <param name="startPoint">Start point of the edge</param>
      /// <param name="endPoint">End point of the edge</param>
      /// <returns>The edge found</returns>
      private Edge GetEdgeByEndPoints(Form form, Autodesk.Revit.DB.XYZ startPoint, Autodesk.Revit.DB.XYZ endPoint)
      {
         Edge edge = null;

         // Get all edges of the form
         EdgeArray edges = null;
         Options geoOptions = m_revitApp.Create.NewGeometryOptions();
         geoOptions.ComputeReferences = true;
         Autodesk.Revit.DB.GeometryElement geoElement = form.get_Geometry(geoOptions);
         foreach (GeometryObject geoObject in geoElement.Objects)
         {
            Solid solid = geoObject as Solid;
            if (null == solid)
               continue;
            edges = solid.Edges;
         }

         // Traverse the edges and look for the edge with the right endpoints
         foreach (Edge ed in edges)
         {
            Autodesk.Revit.DB.XYZ rpPos1 = ed.Evaluate(0);
            Autodesk.Revit.DB.XYZ rpPos2 = ed.Evaluate(1);
            if ((startPoint.IsAlmostEqualTo(rpPos1) && endPoint.IsAlmostEqualTo(rpPos2)) ||
                (startPoint.IsAlmostEqualTo(rpPos2) && endPoint.IsAlmostEqualTo(rpPos1)))
            {
               edge = ed;
               break;
            }
         }

         return edge;
      }

      /// <summary>
      /// Move the sub element
      /// </summary>
      /// <param name="form">The form contains the sub element</param>
      /// <param name="subElemReference">Reference of the sub element to be moved</param>
      /// <param name="offset">offset to be moved</param>
      private void MoveSubElement(Form form, Reference subElemReference, Autodesk.Revit.DB.XYZ offset)
      {
         if (form.CanManipulateSubElement(subElemReference))
         {
            form.MoveSubElement(subElemReference, offset);
         }
      }
   }
}

