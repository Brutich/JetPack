using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using JetBlue;
using Newtonsoft.Json;
using ProtoCore.AST.AssociativeAST;
using RevitServices.Persistence;

namespace JetBlueUI
{
    [NodeName("Parameter Filter Elements")]
    [NodeDescription("Represents collection of Parameter Filter elements")]
    [NodeCategory("JetPack")]
    [IsDesignScriptCompatible]
    public class FiltersByRule : DSDropDownBase
    {
        public FiltersByRule() : base("Parameter Filter Element") { }


        [JsonConstructor]
        public FiltersByRule(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base("Parameter Filter Element", inPorts, outPorts) { }

        protected override SelectionState PopulateItemsCore(string currentSelection)
        {

            Items.Clear();

            var fec = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
            fec.OfClass(typeof(ParameterFilterElement));
            var elements = fec.ToElements();

            List<DynamoDropDownItem> newItems = elements.Select(x => new DynamoDropDownItem(x.Name, x.Name)).ToList();

            Items.AddRange(newItems);

            return SelectionState.Done;
        }
        
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            if(Items.Count == 0 || SelectedIndex < 0)
            {
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
            }

            // Build an AST node for the type of object contained in your Items collection.
            var filterName = AstFactory.BuildStringNode((string)Items[SelectedIndex].Item);
            var functionCall = AstFactory.BuildFunctionCall(
                new Func<string, ParameterFilterElement>(Functions.ByName),
                new List<AssociativeNode> { filterName });

            //var stringNode = AstFactory.BuildStringNode((string)Items[SelectedIndex].Item);
            var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functionCall);

            return new List<AssociativeNode> { assign };
        }
    }
}