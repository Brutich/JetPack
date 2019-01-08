using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using RevitServices.Persistence;
using Revit.Elements;
using Autodesk.DesignScript.Geometry;

namespace Utilities
{
    /// <summary>
    /// The Utilities class.
    /// </summary>
    public class Element
    {

        private Element() { }


        private static Document document = DocumentManager.Instance.CurrentDBDocument;


        private static List<ElementId> Intersect(ElementId element_id, ref List<ElementId> elementIds)
        {
            // Collect intersected elem IDs
            FilteredElementCollector localCollector = new FilteredElementCollector(document, elementIds);
            Autodesk.Revit.DB.Element element = document.GetElement(element_id);
            List<ElementId> colectedElementIds = (List<ElementId>)(localCollector.WherePasses(new ElementIntersectsElementFilter(element)).ToElementIds());

            // Remove initial Element ID from element_ids
            // and revome filtered element IDs from element_ids
            elementIds.Remove(element_id);
            foreach (ElementId id in colectedElementIds)
                elementIds.Remove(id);

            return colectedElementIds;

        }


        /// <summary>
        /// The node returns elements grouped by intersection
        /// </summary>
        /// <param name="elements">Elements for grouping.</param>
        /// <returns>Groups of elements</returns>
        /// <search>
        /// elements, groups, intersection
        /// </search> 
        [Autodesk.DesignScript.Runtime.IsVisibleInDynamoLibrary(true)]
        public static List<IEnumerable<Revit.Elements.Element>> GroupByIntersection(Revit.Elements.Element[] elements)
        {

            List<IEnumerable<Revit.Elements.Element>> outputGroups = new List<IEnumerable<Revit.Elements.Element>>();

            // List of unwrapped dynamo element Ids
            List<ElementId> elementIds = new List<ElementId>();
            foreach (Revit.Elements.Element e in elements)
                elementIds.Add(e.InternalElement.Id);

            while (elementIds.Count > 0)
            {
                ElementId elemId = elementIds[0];

                List<ElementId> grouped = new List<ElementId>();
                grouped.Add(elemId);

                List<ElementId> colectedElemIds = Intersect(elemId, ref elementIds);
                grouped.AddRange(colectedElemIds);

                while ((colectedElemIds.Count > 0) && (elementIds.Count > 0))
                {
                    List<ElementId> intersectionIds = Intersect(colectedElemIds[0], ref elementIds);
                    grouped.AddRange(intersectionIds);
                    colectedElemIds.AddRange(intersectionIds);
                    colectedElemIds.Remove(colectedElemIds[0]);
                }

                // Add list of wrapped to Dynamo type elements
                outputGroups.Add(from id in grouped select document.GetElement(id).ToDSType(true));
            }

            return outputGroups;

        }


        /// <summary>
        /// The node returns "true" if family instance is flipped.
        /// </summary>
        /// <param name="familyInstance">Family instance for flipping test.</param>
        /// <returns>Is flipped</returns>
        /// <search>
        /// family, instance, test, flipped
        /// </search> 
        [Autodesk.DesignScript.Runtime.IsVisibleInDynamoLibrary(true)]
        public static bool IsFlipped(Revit.Elements.FamilyInstance familyInstance)
        {
            // Unwrap element
            Autodesk.Revit.DB.FamilyInstance instance = (Autodesk.Revit.DB.FamilyInstance)familyInstance.InternalElement;

            return instance.HandFlipped ^ instance.FacingFlipped;

        }


        /// <summary>
        /// The node returns all elements from active selection in Revit application
        /// </summary>
        /// <param name="toggle">Switch for update selection.</param>
        /// <returns>Elements</returns>
        /// <search>
        /// elements, from, active, selection
        /// </search>
        [Autodesk.DesignScript.Runtime.IsVisibleInDynamoLibrary(true)]
        public static IEnumerable<Revit.Elements.Element> AllElementsFromActiveSelection(bool toggle)
        {

            UIDocument uiDocument = DocumentManager.Instance.CurrentUIDocument;
            ICollection<ElementId> ids = uiDocument.Selection.GetElementIds();

            return from id in ids select uiDocument.Document.GetElement(id).ToDSType(true);

        }

    }
}


namespace Geometry.Points
{
    /// <summary>
    /// The Point class.
    /// </summary>
    public class Point
    {
        private Point() { }

        /// <summary>
        /// The node returns "true" if the point inside the solid or on the surface and the distance to the geometry does not exceed the "tolerance" value.
        /// </summary>
        /// <param name="geometry">Geometry for search distance.</param>
        /// <param name="point">Point</param>
        /// <param name="tolerance">Extra search distance to geometry</param>
        /// <returns>Inside or not Inside</returns>
        /// <search>
        /// inside, point, test, geometry
        /// </search> 
        [Autodesk.DesignScript.Runtime.IsVisibleInDynamoLibrary(true)]
        public static Boolean IsInsideGeometry(Autodesk.DesignScript.Geometry.Point point, Autodesk.DesignScript.Geometry.Geometry geometry, Double tolerance = 0.00)
        {
            Boolean isIn = geometry.DistanceTo(point) <= Math.Abs(tolerance);
            return isIn;

        }
    }
}


