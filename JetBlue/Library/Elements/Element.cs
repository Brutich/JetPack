using Autodesk.Revit.DB;
using Autodesk.DesignScript.Runtime;
using Revit.Elements;
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
}
    