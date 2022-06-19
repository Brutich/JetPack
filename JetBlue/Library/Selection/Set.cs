using System.Linq;
using Autodesk.Revit.UI;
using Autodesk.DesignScript.Runtime;
using RevitServices.Persistence;


namespace Selection
{
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
            var elementIds = elements.Select(elem => elem.InternalElement.Id).ToList();

            UIDocument uiDocument = DocumentManager.Instance.CurrentUIDocument;
            uiDocument.Selection.SetElementIds(elementIds);

        }
    }
}
    