﻿//
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
using System.Linq;
using System.Text;

using Autodesk.Revit;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Revit.SDK.Samples.CompoundStructureCreation.CS
{
    #region A Class For Create CompoundStructure
    /// <summary>
    /// This command allows to create vertical CompoundStructure and applying to walls.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class WallCompoundStructure : Autodesk.Revit.UI.IExternalCommand
    {
        /// <summary>
        /// store the application
        /// </summary>
        UIApplication m_application;
        /// <summary>
        /// store the document
        /// </summary>
        UIDocument m_document;

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
        public Result Execute(Autodesk.Revit.UI.ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            m_application = commandData.Application;
            m_document = m_application.ActiveUIDocument;

            Transaction transaction = new Transaction(m_document.Document, "Create CompoundStructure for Wall");

            try
            {
                // Select at least a wall.
                SelElementSet selectedElements = m_document.Selection.Elements;
                if (selectedElements.IsEmpty)
                {
                    TaskDialog.Show("Error","Please select one wall at least.");
                    return Autodesk.Revit.UI.Result.Cancelled;
                }
                
                // Create the CompoundStructure for wall.
                transaction.Start();
                foreach (Element elem in selectedElements)
                {
                    Wall wall = elem as Wall;
                    if (wall != null)
                    {                      
                        CreateCSforWall(wall);
                    }
                }
                transaction.Commit();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                // If any error, give error information and return failed
                message = ex.Message;
                transaction.RollBack();
                return Result.Failed;
            }
        }

        /// <summary>
        /// Create vertical CompoundStructure for wall: new layers, split new region, new sweep and new reveal.
        /// </summary>
        /// <param name="wall">The wall applying the new CompoundStructure.</param>
        public void CreateCSforWall(Wall wall)
        {
            // Get CompoundStructure
            CompoundStructure wallCS = wall.WallType.GetCompoundStructure();

            // Get material for CompoundStructureLayer
            Material masonry_Brick = GetMaterial("Masonry - Brick");
            Material concrete = GetMaterial("Concrete");

            // Create CompoundStructureLayers
            List<CompoundStructureLayer> csLayers = new List<CompoundStructureLayer>();
            CompoundStructureLayer finish1Layer = new CompoundStructureLayer(0.2, MaterialFunctionAssignment.Finish1, masonry_Brick.Id);
            CompoundStructureLayer substrateLayer = new CompoundStructureLayer(0.1, MaterialFunctionAssignment.Substrate, ElementId.InvalidElementId);
            CompoundStructureLayer structureLayer = new CompoundStructureLayer(0.5, MaterialFunctionAssignment.Structure, concrete.Id);
            CompoundStructureLayer membraneLayer = new CompoundStructureLayer(0, MaterialFunctionAssignment.Membrane, ElementId.InvalidElementId);
            CompoundStructureLayer finish2Layer = new CompoundStructureLayer(0.2, MaterialFunctionAssignment.Finish2, masonry_Brick.Id);
            csLayers.Add(finish1Layer);
            csLayers.Add(substrateLayer);
            csLayers.Add(structureLayer);
            csLayers.Add(membraneLayer);
            csLayers.Add(finish2Layer);

            // Set the created layers to CompoundStructureLayer
            wallCS.SetLayers(csLayers);

            // Set shell layers and wrapping.
            wallCS.SetNumberOfShellLayers(ShellLayerType.Interior, 2);
            wallCS.SetNumberOfShellLayers(ShellLayerType.Exterior, 1);
            wallCS.SetParticipatesInWrapping(0, false);

            // Points for adding wall sweep and reveal.
            UV sweepPoint = UV.Zero;
            UV revealPoint = UV.Zero;

            // split the region containing segment 0.
            int segId = wallCS.GetSegmentIds()[0];
            foreach (int regionId in wallCS.GetAdjacentRegions(segId))
            {
                // Get the end points of segment 0.
                UV endPoint1 = UV.Zero;
                UV endPoint2 = UV.Zero;
                wallCS.GetSegmentEndPoints(segId, regionId, out endPoint1, out endPoint2);

                // Split a new region in splited point and orientation.
                RectangularGridSegmentOrientation splitOrientation = (RectangularGridSegmentOrientation)(((int)(wallCS.GetSegmentOrientation(segId)) + 1) % 2);
                UV splitUV = (endPoint1 + endPoint2) / 2.0;
                int newRegionId = wallCS.SplitRegion(splitUV, splitOrientation);
                bool isValidRegionId = wallCS.IsValidRegionId(newRegionId);

                // Find the enclosing region and the two segments intersected by a line through the split point
                int segId1;
                int segId2;
                int findRegionId = wallCS.FindEnclosingRegionAndSegments(splitUV, splitOrientation, out segId1, out segId2);

                // Get the end points of finding segment 1 and compute the wall sweep point.
                UV eP1 = UV.Zero;
                UV eP2 = UV.Zero;
                wallCS.GetSegmentEndPoints(segId1, findRegionId, out eP1, out eP2);
                sweepPoint = (eP1 + eP2) / 4.0;

                // Get the end points of finding segment 2 and compute the wall reveal point.
                UV ep3 = UV.Zero;
                UV ep4 = UV.Zero;
                wallCS.GetSegmentEndPoints(segId2, findRegionId, out ep3, out ep4);
                revealPoint = (ep3 + ep4) / 2.0;
            }

            // Create a WallSweepInfo for wall sweep
            WallSweepInfo sweepInfo = new WallSweepInfo(true, WallSweepType.Sweep);
            PrepareWallSweepInfo(sweepInfo, sweepPoint.V);
            // Set sweep profile: Sill-Precast : 8" Wide
            sweepInfo.ProfileId = GetProfile("8\" Wide").Id;  
            sweepInfo.Id = 101;
            wallCS.AddWallSweep(sweepInfo);

            // Create a WallSweepInfo for wall reveal
            WallSweepInfo revealInfo = new WallSweepInfo(true, WallSweepType.Reveal);
            PrepareWallSweepInfo(revealInfo, revealPoint.U);
            revealInfo.Id = 102;
            wallCS.AddWallSweep(revealInfo);

            // Set the new wall CompoundStructure to the type of wall.
            wall.WallType.SetCompoundStructure(wallCS);
        }

        /// <summary>
        /// Setting the WallSweepInfo for sweep and reveal.
        /// </summary>
        /// <param name="wallSweepInfo">The WallSweepInfo.</param>
        /// <param name="distance">The distance from either the top or base of the wall</param>
        private void PrepareWallSweepInfo(WallSweepInfo wallSweepInfo, double distance)
        {
            wallSweepInfo.DistanceMeasuredFrom = DistanceMeasuredFrom.Base;
            wallSweepInfo.Distance = distance;
            wallSweepInfo.WallSide = WallSide.Exterior;
            wallSweepInfo.Id = -1;
            wallSweepInfo.WallOffset = -0.1;
        }

        /// <summary>
        /// Getting the specific material by name.
        /// </summary>
        /// <param name="name">The name of specific material.</param>
        /// <returns>The specifi material</returns>
        private Material GetMaterial(string name)
        {
            FilteredElementCollector collector = new FilteredElementCollector(m_document.Document);
            collector.WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_Materials));
            var MaterialElement = from element in collector
                                where element.Name == name
                                select element;
            return MaterialElement.First<Element>() as Material;
        }

        /// <summary>
        /// Getting the profile for sweep or reveal.
        /// </summary>
        /// <param name="name">The name of specific profile.</param>
        /// <returns>The specific profile.</returns>
        private FamilySymbol GetProfile(string name)
        {
            FilteredElementCollector profiles = new FilteredElementCollector(m_document.Document);
            profiles.OfCategory(BuiltInCategory.OST_ProfileFamilies);
            var MaterialElement = from element in profiles
                                  where element.Name == name
                                  select element;
            return MaterialElement.First<Element>() as FamilySymbol;
        }
    }
    #endregion Create CompoundStructure
}
