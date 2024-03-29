﻿using System;
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
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Revit.Elements;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;
using Dynamo.Graph.Nodes;


namespace Elements
{
    /// <summary>
    /// The Element class.
    /// </summary>
    public class Element
    {
        private Element() { }

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
        /// Sets new name.
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

            Document document = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(document);
            elem.Name = name;
            TransactionManager.Instance.TransactionTaskDone();

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
        /// The node returns "true" if family instance is mirrored. (only one axis is flipped)
        /// </summary>
        /// <param name="familyInstance">Family instance for flipping test.</param>
        /// <returns>Is flipped</returns>
        /// <search>
        /// family, instance, test, flipped, mirrored
        /// </search> 
        [IsVisibleInDynamoLibrary(true)]
        [NodeCategory("Query")]
        public static bool IsMirrored(Revit.Elements.FamilyInstance familyInstance)
        {
            // Unwrap element
            var instance = (Autodesk.Revit.DB.FamilyInstance)familyInstance.InternalElement;

            return instance.Mirrored;
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
            if (familyInstance == null) return null;

            var instance = (Autodesk.Revit.DB.FamilyInstance)familyInstance.InternalElement;
            var toRoom = instance.ToRoom;

            return toRoom is null ? null : toRoom.ToDSType(true) as Revit.Elements.Room;
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
            if (familyInstance == null) return null;

            var instance = (Autodesk.Revit.DB.FamilyInstance)familyInstance.InternalElement;
            var fromRoom = instance.FromRoom;

            return fromRoom is null ? null : fromRoom.ToDSType(true) as Revit.Elements.Room;
        }


        /// <summary>
        /// Changes the type of family instance to another.
        /// </summary>
        /// <param name="familyInstance">Family instance for changing type.</param>
        /// <param name="familyType">Another family type.</param>
        /// <returns></returns>
        /// <search>
        /// change, type, family, instance
        /// </search>
        [IsVisibleInDynamoLibrary(true)]
        public static Revit.Elements.FamilyInstance ChangeType(
            Revit.Elements.FamilyInstance familyInstance,
            Revit.Elements.FamilyType familyType)
        {
            // Unwrap input parameters
            Autodesk.Revit.DB.FamilyInstance instance = familyInstance.InternalElement as Autodesk.Revit.DB.FamilyInstance;
            Autodesk.Revit.DB.Element anotherType = familyType.InternalElement;

            TransactionManager.Instance.EnsureInTransaction(DocumentManager.Instance.CurrentDBDocument);
            instance.ChangeTypeId(anotherType.Id);
            TransactionManager.Instance.TransactionTaskDone();

            return familyInstance;
        }


        /// <summary>
        /// Node to get the super component of current family instance.
        /// </summary>
        /// <param name="familyInstance">Family instance for getting the super component.</param>
        /// <returns></returns>
        /// <search>
        /// super component, family, instance
        /// </search>
        [IsVisibleInDynamoLibrary(true)]
        [NodeCategory("Query")]
        public static Revit.Elements.FamilyInstance SuperComponent(Revit.Elements.FamilyInstance familyInstance)
        {
            // Unwrap input parameters
            var instance = familyInstance.InternalElement as Autodesk.Revit.DB.FamilyInstance;
            var superComponent = instance.SuperComponent as Autodesk.Revit.DB.FamilyInstance;

            return superComponent is null ? null : superComponent.ToDSType(true) as Revit.Elements.FamilyInstance;
        }
    }


    /// <summary>
    /// The WallType class.
    /// </summary>
    public class WallType
    {    
        
        private WallType() { }


        /// <summary>
        /// Returns wall compound structure layers.
        /// </summary>
        /// <param name="wallType">Wall type</param>
        /// <returns></returns>
        /// <search>
        /// wall, compound, structure, layers
        /// </search>
        [IsVisibleInDynamoLibrary(true)]
        [NodeCategory("Query")]
        public static List<Layer> GetLayers(Revit.Elements.WallType wallType)
        {
            Document document = DocumentManager.Instance.CurrentDBDocument;

            var wt = wallType.InternalElement as Autodesk.Revit.DB.WallType;
            CompoundStructure structure = wt.GetCompoundStructure();
            int layerCount = structure.LayerCount;
            int strMaterialInd = structure.StructuralMaterialIndex;

            List<Layer> layers = new List<Layer>(); 

            for (int i = 0; i < layerCount; i++)
            {
                layers.Add(new Layer(structure.GetLayerFunction(i).ToString(),
                        document.GetElement(structure.GetMaterialId(i)) as Autodesk.Revit.DB.Material,
                        UnitUtils.ConvertFromInternalUnits(structure.GetLayerWidth(i), UnitTypeId.Millimeters),
                        i == strMaterialInd,
                        structure.IsCoreLayer(i) ));
            }

            return layers;
        }


        /// <summary>
        /// Returns wall type name. Basic, curtain or stacked wall.
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


    /// <summary>
    /// The Wall Compound Structure Layer class.
    /// </summary>
    public class Layer
    {
        /// <summary>
        /// The function of the layer.
        /// </summary>
        [NodeCategory("Query")]
        public string Function { get; }

        /// <summary>
        /// Material of the layer.
        /// </summary>
        [NodeCategory("Query")]
        public Autodesk.Revit.DB.Material Material { get; }

        /// <summary>
        /// Width of the layer.
        /// </summary>
        [NodeCategory("Query")]
        public double Width { get; }

        /// <summary>
        /// Is layer structural?
        /// </summary>
        [NodeCategory("Query")]
        public bool IsStructural { get; }

        /// <summary>
        /// Is layer core?
        /// </summary>
        [NodeCategory("Query")]
        public bool IsCore { get; }

        internal Layer(
            string layerFunction,
            Autodesk.Revit.DB.Material layerMaterial,
            double layerThickness, bool isStructuralLayer,
            bool isCoreLayer)
        {
            this.Function = layerFunction;
            this.Material = layerMaterial;
            this.Width = layerThickness;
            this.IsStructural = isStructuralLayer;
            this.IsCore = isCoreLayer;
        }
    }


    /// <summary>
    /// The Filter Element class.
    /// </summary>
    public class FilterElement
    {
        private FilterElement() { }

        /// <summary>
        /// Select a ParameterFilterElement from the current document by name
        /// </summary>
        /// <param name="name">Parameter filter name</param>
        /// <returns>Parameter filter</returns>
        /// <search>
        /// parameter, filter, by, rule, element
        /// </search>
        public static Autodesk.Revit.DB.ParameterFilterElement ByName(string name)
        {
            if (name == null) return null;

            var filterElement = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.ParameterFilterElement>()
                .FirstOrDefault(x => x.Name == name);

            if (filterElement == null)
                throw new Exception();

            return filterElement;
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
            Document document = DocumentManager.Instance.CurrentDBDocument;
            UIDocument uiDocument = DocumentManager.Instance.CurrentUIDocument;
            ICollection<ElementId> ids = uiDocument.Selection.GetElementIds();

            return ids.Select(x => document.GetElement(x).ToDSType(true));
        }


        /// <summary>
        /// The node returns all elements by rule filter from Revit document.
        /// </summary>
        /// <param name="parameterFilterElement">Parameter filter element.</param>
        /// <returns>Elements</returns>
        /// <search>
        /// elements, by, filter
        /// </search>
        [IsVisibleInDynamoLibrary(true)]
        [NodeName("All Elements by Filter")]
        public static IEnumerable<Revit.Elements.Element> AllElementsByFilter(ParameterFilterElement parameterFilterElement)
        {
            Document document = DocumentManager.Instance.CurrentDBDocument;

            var parameterFilter = parameterFilterElement as ParameterFilterElement;
            var categoriesIds = parameterFilter.GetCategories();
            var elementFilter = parameterFilter.GetElementFilter();

            var elements = new List<Autodesk.Revit.DB.Element>();
            foreach (var id in categoriesIds)
                elements.AddRange(new FilteredElementCollector(document).OfCategoryId(id).WherePasses(elementFilter));

            return elements.Select(x => x.ToDSType(true));
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

            var descriptions = new List<string>();
            var failureDefinitionId = new List<FailureDefinitionId>();
            var elements = new List<object>();

            foreach (Autodesk.Revit.DB.FailureMessage w in warnings)
            {
                descriptions.Add(w.GetDescriptionText());
                failureDefinitionId.Add(w.GetFailureDefinitionId());
                var elems = new List<Revit.Elements.Element>();
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

            IList<Autodesk.Revit.DB.Element> elements = new FilteredElementCollector(doc)
                .WherePasses(new ElementWorksetFilter(workset.Id))
                .ToElements();

            var outputData = new List<Revit.Elements.Element>();
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

            var worksets = new FilteredWorksetCollector(document);

            var wsKindes = new List<string>();
            var wsNames = new List<string>();
            var wsIds = new List<int>();

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
            var elementIds = new List<ElementId>();
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
            // Collect intersected elem IDs.
            var localCollector = new FilteredElementCollector(document, elementIds);
            Autodesk.Revit.DB.Element element = document.GetElement(element_id);
            List<ElementId> colectedElementIds = localCollector
                .WherePasses(new ElementIntersectsElementFilter(element))
                .ToElementIds() as List<ElementId>;

            // Remove initial Element ID from element_ids
            // and revome filtered element IDs from element_ids.
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

            var outputGroups = new List<IEnumerable<Revit.Elements.Element>>();

            // List of unwrapped dynamo element Ids
            var elementIds = new List<ElementId>();
            foreach (Revit.Elements.Element e in elements)
                elementIds.Add(e.InternalElement.Id);

            while (elementIds.Count > 0)
            {
                ElementId elemId = elementIds[0];

                var grouped = new List<ElementId>();
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
        public static IEnumerable<Revit.Elements.Element> FilterBySolidIntersection(Revit.Elements.Element[] elements, Autodesk.DesignScript.Geometry.Solid solid)
        {
            Document document = DocumentManager.Instance.CurrentDBDocument;

            // Convert Proto to Revit
            var unionSolid = DynamoToRevitBRep.ToRevitType(solid) as Autodesk.Revit.DB.Solid;

            // List of unwrapped Dynamo element Ids
            List<ElementId> elementIds = elements.Select(x => x.InternalElement.Id).ToList();

            // Collect intersected elements
            var сollector = new FilteredElementCollector(document, elementIds);
            сollector.WherePasses(new ElementIntersectsSolidFilter(unionSolid));
            var colectedElements = сollector.ToElements();

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

    /// <summary>
    /// The Room class.
    /// </summary>
    public class Room
    {
        private Room() { }

        private static readonly Document document = DocumentManager.Instance.CurrentDBDocument;

        /// <summary>
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        /// <search>
        /// rooms, from, to, doors
        /// </search> 
        [IsVisibleInDynamoLibrary(true)]
        [NodeCategory("Query")]
        [MultiReturn("FromDoors", "ToDoors")]
        public static Dictionary<string, object> Doors(Revit.Elements.Room room)
        {
            if (room == null) return null;

            Document document = DocumentManager.Instance.CurrentDBDocument;

            var fec = new FilteredElementCollector(document);
            fec.OfCategory(BuiltInCategory.OST_Doors);
            fec.OfClass(typeof(Autodesk.Revit.DB.FamilyInstance));
            var elements = fec.ToElements();

            if (!elements.Any())
                return new Dictionary<string, object>
                {
                    { "FromDoors", Enumerable.Empty<Revit.Elements.FamilyInstance>() },
                    { "ToDoors", Enumerable.Empty<Revit.Elements.FamilyInstance>() }
                };

            var fromDoors = new List<Revit.Elements.FamilyInstance>();
            var toDoors = new List<Revit.Elements.FamilyInstance>();
            var rm = room.InternalElement as Autodesk.Revit.DB.Architecture.Room;
            foreach (var element in elements)
            {
                var door = element as Autodesk.Revit.DB.FamilyInstance;

                if (door.ToRoom?.Id == rm.Id)
                {
                    toDoors.Add(element.ToDSType(true) as Revit.Elements.FamilyInstance);
                    continue;
                }

                if (door.FromRoom?.Id == rm.Id)
                {
                    fromDoors.Add(element.ToDSType(true) as Revit.Elements.FamilyInstance);
                }
            }

            return new Dictionary<string, object>
            {
                { "FromDoors", fromDoors},
                { "ToDoors", toDoors}
            }; 
        }
    }
}
    