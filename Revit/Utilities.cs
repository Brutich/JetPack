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
using RevitServices.Transactions;
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



        /// <summary>
        /// Sets new name. It may be necessary to perform inside the transaction.
        /// </summary>
        /// <param name="element">Element for renaming.</param>
        /// <param name="name">New name.</param>
        /// <returns>Renamed element</returns>
        /// <search>
        /// element, set, name, change
        /// </search> 
        [IsVisibleInDynamoLibrary(true)]
        public static Revit.Elements.Element SetName(Revit.Elements.Element element, string name)
        {
            // Unwrap elements
            Autodesk.Revit.DB.Element elem = element.InternalElement;

            elem.Name = name;

            return elem.ToDSType(true);
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
        /// The "To Room" set for the door or window in the last phase of the project.
        /// </summary>
        /// <param name="familyInstance">Door or window family instance.</param>
        /// <returns>Room</returns>
        /// <search>
        /// to room, instance, door, window
        /// </search> 
        [IsVisibleInDynamoLibrary(true)]
        [NodeCategory("Query")]
        public static Revit.Elements.Room ToRoom(Revit.Elements.FamilyInstance familyInstance)
        {
            // Unwrap element
            Autodesk.Revit.DB.FamilyInstance instance = (Autodesk.Revit.DB.FamilyInstance)familyInstance.InternalElement;

            return instance.ToRoom.ToDSType(true) as Revit.Elements.Room;
        }


        /// <summary>
        /// The "From Room" set for the door or window in the last phase of the project.
        /// </summary>
        /// <param name="familyInstance">Door or window family instance.</param>
        /// <returns>Room</returns>
        /// <search>
        /// from room, instance, door, window
        /// </search> 
        [IsVisibleInDynamoLibrary(true)]
        [NodeCategory("Query")]
        public static Revit.Elements.Room FromRoom(Revit.Elements.FamilyInstance familyInstance)
        {
            // Unwrap element
            Autodesk.Revit.DB.FamilyInstance instance = (Autodesk.Revit.DB.FamilyInstance)familyInstance.InternalElement;

            return instance.FromRoom.ToDSType(true) as Revit.Elements.Room;
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
        public static Dictionary<string, object> ChangeType(
            Revit.Elements.FamilyInstance familyInstance,
            Revit.Elements.FamilyType familyType)
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

            return (wallType.InternalElement as Autodesk.Revit.DB.WallType).FamilyName;

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


        // All Elements by Workset
        /// <summary>
        /// The node returns all elements by workset.
        /// </summary>
        /// <param name="toggle">Switch for update selection.</param>
        /// <param name="workset"></param>
        /// <returns>Elements</returns>
        /// <search>
        /// elements, from, workset, selection
        /// </search>
        [IsVisibleInDynamoLibrary(true)]
        [NodeName("All Elements From Workset")]
        public static IEnumerable<Revit.Elements.Element> AllElementsByWorkset(Workset workset)
        {

            Document doc = DocumentManager.Instance.CurrentDBDocument;

            FilteredElementCollector elementCollector = new FilteredElementCollector(doc);

            IList<Autodesk.Revit.DB.Element> elements = elementCollector
                .WherePasses(new ElementWorksetFilter(workset.Id))
                .ToElements();

            List<Revit.Elements.Element> outputData = new List<Revit.Elements.Element>();
            foreach (Autodesk.Revit.DB.Element elem in elements)
                outputData.Add(elem.ToDSType(true));

            return outputData;

        }


        /// <summary>
        /// The node returns all worksets from active Revit document.
        /// </summary>
        /// <param name="toggle">Switch for update.</param>
        /// <returns></returns>
        /// <search>
        /// worksets, elements
        /// </search>
        [IsVisibleInDynamoLibrary(true)]
        [MultiReturn("Kinds", "Names", "IDs", "Worksets")]
        public static Dictionary<string, object> Worksets(bool toggle)
        {

            Document document = DocumentManager.Instance.CurrentDBDocument;

            FilteredWorksetCollector worksets = new FilteredWorksetCollector(document);

            List<string> wsKindes = new List<string>();
            List<string> wsNames = new List<string>();
            List<int> wsIds = new List<int>();

            foreach (Autodesk.Revit.DB.Workset ws in worksets)
            {
                wsKindes.Add(ws.Kind.ToString());
                wsNames.Add(ws.Name);
                wsIds.Add(ws.Id.IntegerValue);
            }

            return new Dictionary<string, object>
            {
                { "Kinds", wsKindes},
                { "Names", wsNames},
                { "IDs", wsIds},
                { "Worksets", worksets}
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
    /// The Elements class.
    /// </summary>
    public class Elements
    {

        private Elements() { }

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
        /// A filter to find elements that intersect the given solid geometry.
        /// The input solid used for this filter can be obtained from an existing element.
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="solid">Input solid</param>
        /// <returns>Finded elements</returns>
        /// <search>
        /// filter, find, elements, intersect, solid, geometry
        /// </search> 
        [IsVisibleInDynamoLibrary(true)]
        [NodeCategory("Query")]
        public static List<Revit.Elements.Element> FilterBySolidIntersection(Revit.Elements.Element[] elements, Autodesk.DesignScript.Geometry.Solid solid)
        {
            // Convert Proto to Revit
            Autodesk.Revit.DB.Solid unionSolid = DynamoToRevitBRep.ToRevitType(solid) as Autodesk.Revit.DB.Solid;
            
            // List of unwrapped Dynamo element Ids
            List<ElementId> elementIds = new List<ElementId>();
            foreach (Revit.Elements.Element e in elements)
                elementIds.Add(e.InternalElement.Id);

            // Collect intersected elements
            FilteredElementCollector сollector = new FilteredElementCollector(document, elementIds);
            List<Autodesk.Revit.DB.Element> colectedElements = сollector
                .WherePasses(new ElementIntersectsSolidFilter(unionSolid))
                .ToElements() as List<Autodesk.Revit.DB.Element>;

            List<Revit.Elements.Element> outputElements = new List<Revit.Elements.Element>();

            foreach (Autodesk.Revit.DB.Element c in colectedElements)
            {
                outputElements.Add(c.ToDSType(true));
            }

            return outputElements;

        }


        /// <summary>
        /// A filter to find elements that intersect the given element.
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="element"></param>
        /// <returns>Finded elements</returns>
        /// <search>
        /// filter, find, elements, intersect, element
        /// </search> 
        [IsVisibleInDynamoLibrary(true)]
        [NodeCategory("Query")]
        public static List<Revit.Elements.Element> FilterByElementIntersection(Revit.Elements.Element[] elements, Revit.Elements.Element element)
        {
            // Convert Proto to Revit
            Autodesk.Revit.DB.Element operatorElement = element.InternalElement;

            // List of unwrapped Dynamo element Ids
            List<ElementId> elementIds = new List<ElementId>();
            foreach (Revit.Elements.Element e in elements)
                elementIds.Add(e.InternalElement.Id);

            // Collect intersected elements
            FilteredElementCollector сollector = new FilteredElementCollector(document, elementIds);
            List<Autodesk.Revit.DB.Element> colectedElements = сollector
                .WherePasses(new ElementIntersectsElementFilter(operatorElement))
                .ToElements() as List<Autodesk.Revit.DB.Element>;


            List<Revit.Elements.Element> outputElements = new List<Revit.Elements.Element>();

            foreach (Autodesk.Revit.DB.Element c in colectedElements)
            {
                outputElements.Add(c.ToDSType(true));
            }

            return outputElements;

        }


        /// <summary>
        /// Delete elements from the document with their dependencies
        /// </summary>
        /// <param name="elements">List of elements for deleting.</param>
        /// <returns></returns>
        /// <search>
        /// delete, elements.
        /// </search> 
        [IsVisibleInDynamoLibrary(true)]
        public static void Delete(Revit.Elements.Element[] elements)
        {
            // List of unwrapped Dynamo element Ids
            List<ElementId> elementIds = new List<ElementId>();
            foreach (Revit.Elements.Element e in elements)
                elementIds.Add(e.InternalElement.Id);

            // Transaction Start
            TransactionManager.Instance.EnsureInTransaction(document);

            document.Delete(elementIds);

            //Transaction End
            TransactionManager.Instance.TransactionTaskDone();
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
            
            return geometry.DistanceTo(point) <= Math.Abs(tolerance);

        }
    }
}
