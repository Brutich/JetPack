#region Namespaces
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
using Revit.Elements;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Dynamo.Graph.Nodes;
#endregion // Namespaces

namespace Elements
{
    /// <summary>
    /// The Element class.
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
            List<ElementId> colectedElementIds = localCollector
                .WherePasses(new ElementIntersectsElementFilter(element))
                .ToElementIds() as List<ElementId>;

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
        [IsVisibleInDynamoLibrary(true)]
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
        /// The node returns true if element is hidden on view
        /// </summary>
        /// <param name="element">Elements for test.</param>
        /// <param name="view">View.</param>
        /// <returns>If True element hidden</returns>
        /// <search>
        /// element, hidden, on, view
        /// </search> 
        [IsVisibleInDynamoLibrary(true)]
        [NodeCategory("Query")]
        public static bool IsHiddenOnView(Revit.Elements.Element element, Revit.Elements.Views.View view)
        {
            // Unwrap elements
            Autodesk.Revit.DB.Element elem = element.InternalElement;
            Autodesk.Revit.DB.View inputView = (Autodesk.Revit.DB.View)view.InternalElement;

            return elem.IsHidden(inputView);
        }

    }


    /// <summary>
    /// The FamilyInstance class.
    /// </summary>
    public class FamilyInstance
    {

        private FamilyInstance() { }

        /// <summary>
        /// The node returns "true" if family instance is flipped.
        /// </summary>
        /// <param name="familyInstance">Family instance for flipping test.</param>
        /// <returns>Is flipped</returns>
        /// <search>
        /// family, instance, test, flipped
        /// </search> 
        [IsVisibleInDynamoLibrary(true)]
        [NodeCategory("Query")]
        public static bool IsFlipped(Revit.Elements.FamilyInstance familyInstance)
        {
            // Unwrap element
            Autodesk.Revit.DB.FamilyInstance instance = (Autodesk.Revit.DB.FamilyInstance)familyInstance.InternalElement;

            return instance.HandFlipped ^ instance.FacingFlipped;

        }


        /// <summary>
        /// Changes the type of family instance to another. It may be necessary to perform inside the transaction body.
        /// </summary>
        /// <param name="familyInstance">Family instance for changing type.</param>
        /// <param name="familyType">Another family type.</param>
        /// <returns></returns>
        /// <search>
        /// change, type, family, instance
        /// </search>
        [IsVisibleInDynamoLibrary(true)]
        [MultiReturn("Family Instance", "Report")]
        public static Dictionary<string, object> ChangeType(Revit.Elements.FamilyInstance familyInstance, Revit.Elements.FamilyType familyType)
        {

            // Unwrap input parameters
            Autodesk.Revit.DB.FamilyInstance instance = familyInstance.InternalElement as Autodesk.Revit.DB.FamilyInstance;
            Autodesk.Revit.DB.Element anotherType = familyType.InternalElement;

            string report = "";

            try
            {
                instance.ChangeTypeId(anotherType.Id);
                report = "Successfully";
            }
            catch (Exception e)
            {
                report = $"Error: {e}";
            }

            return new Dictionary<string, object>
            {
                { "Family Instance", instance},
                { "Report", report}
            };
        }
    }


    /// <summary>
    /// The WallType class.
    /// </summary>
    public class WallType
    {

        private WallType() { }

        /// <summary>
        /// Returns wall type name.
        /// </summary>
        /// <param name="wallType">Wall type</param>
        /// <returns></returns>
        /// <search>
        /// wall, type, name
        /// </search>
        [IsVisibleInDynamoLibrary(true)]
        [NodeCategory("Query")]
        public static string FamilyName(Revit.Elements.WallType wallType)
        {

            // Unwrap input parameter
            Autodesk.Revit.DB.WallType type = (Autodesk.Revit.DB.WallType)wallType.InternalElement;

            return type.FamilyName;

        }
    }
}


namespace Selection
{
    /// <summary>
    /// The Get Selection class.
    /// </summary>
    public class Get
    {

        private Get() { }


        /// <summary>
        /// The node returns all elements from active selection in Revit document.
        /// </summary>
        /// <param name="toggle">Switch for update selection.</param>
        /// <returns>Elements</returns>
        /// <search>
        /// elements, from, active, selection
        /// </search>
        [IsVisibleInDynamoLibrary(true)]
        [NodeName("All Elements From Active Selection")]
        public static IEnumerable<Revit.Elements.Element> AllElementsFromActiveSelection(bool toggle)
        {

            UIDocument uiDocument = DocumentManager.Instance.CurrentUIDocument;
            ICollection<ElementId> ids = uiDocument.Selection.GetElementIds();

            return from id in ids select uiDocument.Document.GetElement(id).ToDSType(true);

        }
        

        /// <summary>
        /// The node returns all Failure Messages from active Revit document.
        /// </summary>
        /// <param name="toggle">Switch for update.</param>
        /// <returns></returns>
        /// <search>
        /// warnings, elements, from, failure, messages
        /// </search>
        [IsVisibleInDynamoLibrary(true)]
        [MultiReturn("Descriptions", "Elements", "Failure Messages")]
        public static Dictionary<string, object> Warnings(bool toggle)
        {

            Document document = DocumentManager.Instance.CurrentDBDocument;
            IList<Autodesk.Revit.DB.FailureMessage> warnings = document.GetWarnings();

            List<string> descriptions = new List<string>();
            List<FailureDefinitionId> failureDefinitionId = new List<FailureDefinitionId>();
            List<object> elements = new List<object>();

            foreach (Autodesk.Revit.DB.FailureMessage w in warnings)
            {
                descriptions.Add(w.GetDescriptionText());
                failureDefinitionId.Add(w.GetFailureDefinitionId());
                List<Revit.Elements.Element> elems = new List<Revit.Elements.Element>();
                foreach (ElementId id in w.GetFailingElements())
                    elems.Add(document.GetElement(id).ToDSType(true));

                elements.Add(elems);
            }

            return new Dictionary<string, object>
            {
                { "Descriptions", descriptions},
                { "Elements", elements},
                { "Failure Messages", warnings}
            };
        }
    }


    /// <summary>
    /// The Set Selection class.
    /// </summary>
    public class Set
    {

        private Set() { }


        /// <summary>
        /// Set selection in Revit application by input elements
        /// </summary>
        /// <param name="elements">Elements for setting selection.</param>
        /// <returns></returns>
        /// <search>
        /// elements, set, selection
        /// </search>
        [IsVisibleInDynamoLibrary(true)]
        public static void Selection(Revit.Elements.Element[] elements)
        {
            // List of unwrapped Dynamo element Ids
            List<ElementId> elementIds = new List<ElementId>();
            foreach (Revit.Elements.Element e in elements)
                elementIds.Add(e.InternalElement.Id);

            UIDocument uiDocument = DocumentManager.Instance.CurrentUIDocument;
            uiDocument.Selection.SetElementIds(elementIds);

        }
    }
}


namespace Utilities
{
    /// <summary>
    /// The Filter class.
    /// </summary>
    public class Filter
    {

        private Filter() { }

        private static Document document = DocumentManager.Instance.CurrentDBDocument;

        /// <summary>
        /// The node returns "true" if the point inside the solid or on the surface and the distance to the geometry does not exceed the "tolerance" value.
        /// </summary>
        /// <param name="solid"></param>
        /// <param name="elements"></param>
        /// <returns>Inside or not Inside</returns>
        /// <search>
        /// inside, point, test, geometry
        /// </search> 
        [IsVisibleInDynamoLibrary(true)]
        [NodeCategory("Query")]
        public static object BySolidIntersection(Autodesk.DesignScript.Geometry.Solid solid, Revit.Elements.Element[] elements)
        {
            // Convert Proto to Revit
            Autodesk.Revit.DB.Solid unionSolid = DynamoToRevitBRep.ToRevitType(solid) as Autodesk.Revit.DB.Solid;

            /*
            TessellatedShapeBuilderTarget target = TessellatedShapeBuilderTarget.Solid;
            TessellatedShapeBuilderFallback fallback = TessellatedShapeBuilderFallback.Salvage;
            Autodesk.Revit.DB.Solid unionSolid = solid.ToRevitType(target, fallback, null, true).First() as Autodesk.Revit.DB.Solid;
            */

            /*
            IList<GeometryObject> geometries = solid.ToRevitType(TessellatedShapeBuilderTarget.Solid);
            
            Autodesk.Revit.DB.Solid unionSolid = null;
            foreach (GeometryObject obj in geometries)
            {
                Autodesk.Revit.DB.Solid _solid = obj as Autodesk.Revit.DB.Solid;

                if (null != _solid
                  && 0 < _solid.Faces.Size)
                {
                    if (null == unionSolid)
                    {
                        unionSolid = _solid;
                    }
                    else
                    {
                        unionSolid = BooleanOperationsUtils.ExecuteBooleanOperation(
                            unionSolid,
                            _solid,
                            BooleanOperationsType.Union);
                    }
                }
            }
            */
            
            // List of unwrapped Dynamo element Ids
            List<ElementId> elementIds = new List<ElementId>();
            foreach (Revit.Elements.Element e in elements)
                elementIds.Add(e.InternalElement.Id);

            // Collect intersected elements
            FilteredElementCollector сollector = new FilteredElementCollector(document, elementIds);
            List<Autodesk.Revit.DB.Element> colectedElements = сollector
                .WherePasses(new ElementIntersectsSolidFilter(unionSolid))
                .ToElements() as List<Autodesk.Revit.DB.Element>;

            colectedElements.ForEach(c => c.ToDSType(true));

            return colectedElements;

        }
    }
}


namespace Geometry
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
        [IsVisibleInDynamoLibrary(true)]
        [NodeCategory("Query")]
        public static bool IsInsideGeometry(Autodesk.DesignScript.Geometry.Point point, Autodesk.DesignScript.Geometry.Geometry geometry, double tolerance = 0.00)
        {
            bool isIn = geometry.DistanceTo(point) <= Math.Abs(tolerance);
            return isIn;

        }
    }
}
