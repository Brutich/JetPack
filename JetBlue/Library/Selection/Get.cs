using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.DesignScript.Runtime;
using Revit.Elements;
using RevitServices.Persistence;
using Dynamo.Graph.Nodes;


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

            var categoriesIds = parameterFilterElement.GetCategories();
            var elementFilter = parameterFilterElement.GetElementFilter();

            var elements = new List<Autodesk.Revit.DB.Element>();
            foreach (var id in categoriesIds)
            {
                using (var fec = new FilteredElementCollector(document))
                {
                    fec.OfCategoryId(id).WherePasses(elementFilter);
                    elements.AddRange(fec.ToElements());
                }
            }

            return elements.Select(x => x.ToDSType(true)).ToList();
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
            var elements = new List<List<Revit.Elements.Element>>();

            foreach (Autodesk.Revit.DB.FailureMessage w in warnings)
            {
                descriptions.Add(w.GetDescriptionText());
                failureDefinitionId.Add(w.GetFailureDefinitionId());

                List<Revit.Elements.Element> elems = w.GetFailingElements()
                    .Select(id => document.GetElement(id)
                    .ToDSType(true))
                    .ToList();

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

            var fec = new FilteredElementCollector(doc);
            fec.WherePasses(new ElementWorksetFilter(workset.Id));
            var elements = fec.ToElements();

            return elements.Select(x => x.ToDSType(true)).ToList();
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
}
    