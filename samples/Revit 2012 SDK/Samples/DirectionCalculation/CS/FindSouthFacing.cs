using System;
using System.IO;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
namespace Revit.Samples.DirectionCalculation
{
    /// <summary>
    /// Base class for "Find South Facing..." utilities.
    /// </summary>
    public class FindSouthFacingBase
    {
        #region Helper properties
       protected Autodesk.Revit.ApplicationServices.Application Application
        {
            get
            {
                return m_app;
            }
            set
            {
                m_app = value;
            }
        }

        protected Document Document
        {
            get
            {
                return m_doc;
            }
            set
            {
                m_doc = value;
            }
        }
        #endregion

        /// <summary>
        /// Identifies if a particular direction is "south-facing".  This means within a range of -45 degrees to 45 degrees 
        /// to the south vector (the negative Y axis).
        /// </summary>
        /// <param name="direction">The normalized direction to test.</param>
        /// <returns>True if the vector is in the designated range.</returns>
        protected bool IsSouthFacing(XYZ direction)
        {
            double angleToSouth = direction.AngleTo(-XYZ.BasisY);

            return Math.Abs(angleToSouth) < Math.PI / 4;
        }

        /// <summary>
        /// Transform a direction vector by the rotation angle of the ActiveProjectLocation.
        /// </summary>
        /// <param name="direction">The normalized direction.</param>
        /// <returns>The transformed location.</returns>
        protected XYZ TransformByProjectLocation(XYZ direction)
        {
            // Obtain the active project location's position.
            ProjectPosition position = Document.ActiveProjectLocation.get_ProjectPosition(XYZ.Zero);
            // Construct a rotation transform from the position angle.
            /* If I cared about transforming points as well as vectors, 
             I would need to concatenate two different transformations:
                //Obtain a rotation transform for the angle about true north
                Transform rotationTransform = Transform.get_Rotation(
                  XYZ.Zero, XYZ.BasisZ, pna );

                //Obtain a translation vector for the offsets
                XYZ translationVector = new XYZ(projectPosition.EastWest, projectPosition.NorthSouth, projectPosition.Elevation);

                Transform translationTransform = Transform.get_Translation(translationVector);

                //Combine the transforms into one.
                Transform finalTransform = translationTransform.Multiply(rotationTransform);
            */ 
            Transform transform = Transform.get_Rotation(XYZ.Zero, XYZ.BasisZ, position.Angle);
            // Rotate the input direction by the transform
            XYZ rotatedDirection = transform.OfVector(direction);
            return rotatedDirection;
        }

        #region Debugging Aids
        /// <summary>
        /// Debugging aid.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="curve"></param>
        protected void Write(String label, Curve curve)
        {
            if (m_writer == null)
                m_writer = new StreamWriter(@"c:\Directions.txt");
            XYZ start = curve.get_EndPoint(0);
            XYZ end = curve.get_EndPoint(1);

            m_writer.WriteLine(String.Format(label + " {0} {1}", XYZToString(start), XYZToString(end)));
        }

        /// <summary>
        /// Debugging aid.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private String XYZToString(XYZ point)
        {
            return "( " + point.X + ", " + point.Y + ", " + point.Z + ")";
        }

        /// <summary>
        /// Debugging aid.
        /// </summary>
        protected void CloseFile()
        {
            if (m_writer != null)
                m_writer.Close();
        }
        #endregion

        private Autodesk.Revit.ApplicationServices.Application m_app;
        private Document m_doc;
        private System.IO.TextWriter m_writer;
    }
}
