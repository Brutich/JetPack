using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.DesignScript.Runtime;
using Revit.Elements;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;
using Dynamo.Graph.Nodes;


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
            // Collect intersected elem IDs.
            var localCollector = new FilteredElementCollector(document, elementIds);
            Autodesk.Revit.DB.Element element = document.GetElement(element_id);
            ICollection<ElementId> colectedElementIds = localCollector
                .WherePasses(new ElementIntersectsElementFilter(element))
                .ToElementIds();

            // Remove initial Element ID from element_ids
            // and revome filtered element IDs from element_ids.
            elementIds.Remove(element_id);
            foreach (ElementId id in colectedElementIds)
                elementIds.Remove(id);

            return colectedElementIds.ToList();
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

            var outputGroups = new List<IEnumerable<Revit.Elements.Element>>();

            // List of unwrapped dynamo element Ids
            var elementIds = new List<ElementId>();
            foreach (Revit.Elements.Element e in elements)
                elementIds.Add(e.InternalElement.Id);

            while (elementIds.Count > 0)
            {
                ElementId elemId = elementIds[0];

                var grouped = new List<ElementId> { elemId };

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
        public static IEnumerable<Revit.Elements.Element> FilterBySolidIntersection(Revit.Elements.Element[] elements, Autodesk.DesignScript.Geometry.Solid solid)
        {
            Document document = DocumentManager.Instance.CurrentDBDocument;

            // Convert Proto to Revit
            var unionSolid = DynamoToRevitBRep.ToRevitType(solid) as Autodesk.Revit.DB.Solid;

            // List of unwrapped Dynamo element Ids
            List<ElementId> elementIds = elements.Select(x => x.InternalElement.Id).ToList();

            // Collect intersected elements
            var fec = new FilteredElementCollector(document, elementIds);
            fec.WherePasses(new ElementIntersectsSolidFilter(unionSolid));
            var colectedElements = fec.ToElements();

            return colectedElements.Select(x => x.ToDSType(true));
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
        public static IEnumerable<Revit.Elements.Element> FilterByElementIntersection(Revit.Elements.Element[] elements, Revit.Elements.Element element)
        {
            Document document = DocumentManager.Instance.CurrentDBDocument;

            // List of unwrapped Dynamo element Ids
            List<ElementId> elementIds = elements.Select(x => x.InternalElement.Id).ToList();

            // Collect intersected elements
            var сollector = new FilteredElementCollector(document, elementIds);
            сollector.WherePasses(new ElementIntersectsElementFilter(element.InternalElement));
            var colectedElements = сollector.ToElements();

            return colectedElements.Select(x => x.ToDSType(true));
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
            List<ElementId> elementIds = elements.Select(e => e.InternalElement.Id).ToList();

            // Transaction Start
            TransactionManager.Instance.EnsureInTransaction(document);
            document.Delete(elementIds);
            //Transaction End
            TransactionManager.Instance.TransactionTaskDone();
        }
    }
}
    