using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using Newtonsoft.Json;
using ProtoCore.AST.AssociativeAST;
using RevitServices.Persistence;

namespace JetBlueUI
{
    public abstract class JetPackDropDownBase : DSDropDownBase
    {
        protected JetPackDropDownBase(string value) : base(value) { }

        [JsonConstructor]
        public JetPackDropDownBase(string value, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(value, inPorts, outPorts) { }


        /// <summary>
        /// Whether it have valid Enumeration values to the output.
        /// </summary>
        /// <param name="itemValueToIgnore"></param>
        /// <param name="selectedValueToIgnore"></param>
        /// <returns>True is that there are valid values to output,false is that only a null value to output.</returns>
        public Boolean CanBuildOutputAst(string itemValueToIgnore = null, string selectedValueToIgnore = null)
        {
            if (Items.Count == 0 || SelectedIndex < 0)
                return false;
            if (!string.IsNullOrEmpty(itemValueToIgnore) && Items[0].Name == itemValueToIgnore)
                return false;
            if (!string.IsNullOrEmpty(selectedValueToIgnore) && Items[SelectedIndex].Name == selectedValueToIgnore)
                return false;
            return true;
        }
    }

    [NodeName("Parameter Filter Elements")]
    [NodeDescription("Represents collection of Parameter Filter elements")]
    [NodeCategory("JetPack.Selection.Get")]
    [IsDesignScriptCompatible]
    public class FiltersByRule : JetPackDropDownBase
    {
        private const string NO_PARAMETER_FILTERS = "No parameter filters available.";
        private const string outputName = "Parameter Filter Element";

        public FiltersByRule() : base(outputName) { }

        [JsonConstructor]
        public FiltersByRule(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(outputName, inPorts, outPorts) { }

        protected override SelectionState PopulateItemsCore(string currentSelection)
        {
            Items.Clear();

            var fec = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);

            fec.OfClass(typeof(ParameterFilterElement));
            var elements = fec.ToElements();
            if (!elements.Any())
            {
                Items.Add(new DynamoDropDownItem(NO_PARAMETER_FILTERS, null));
                SelectedIndex = 0;
                return SelectionState.Done;
            }

            foreach (Element element in elements)
                Items.Add(new DynamoDropDownItem(element.Name, element.Name));

            Items = Items.OrderBy(x => x.Name).ToObservableCollection();
            return SelectionState.Restore;
        }        

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            if (!CanBuildOutputAst(NO_PARAMETER_FILTERS))
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };

            // Build an AST node for the type of object contained in your Items collection.
            var filterName = AstFactory.BuildStringNode((string)Items[SelectedIndex].Item);
            var functionCall = AstFactory.BuildFunctionCall(
                new Func<string, ParameterFilterElement>(Elements.FilterElement.ByName),
                new List<AssociativeNode> { filterName });

            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functionCall) };
        }
    }
}