using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.DB;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using JetBlue;
using Newtonsoft.Json;
using ProtoCore.AST.AssociativeAST;
using RevitServices.Persistence;

namespace JetBlueUI
{

    /// <summary>
    /// Sample NodeModel with custom Gui.
    /// </summary>
    [NodeName("Parameter Filter Elements")]
    [NodeDescription("Represents collection of Parameter Filter elements")]
    [NodeCategory("JetPack")]
    [OutPortNames("Parameter Filter Element")]
    [OutPortTypes("FilteredElementCollector")]
    [OutPortDescriptions("Product of AxSlider")]
    [IsDesignScriptCompatible]
    public class FiltersByRule : NodeModel
    {

        private ObservableCollection<string> filters;
        private string selectedFilter;

        public ObservableCollection<string> Filters
        {
            get { return filters; }
            set
            {
                filters = value;
                RaisePropertyChanged("Filters");
                //OnNodeModified(false);
            }
        }

        public string SelectedFilter
        {
            get { return selectedFilter; }
            set
            {
                selectedFilter = value;
                RaisePropertyChanged("SelectedFilter");
                //OnNodeModified(false);
            }
        }


        [JsonConstructor]
        private FiltersByRule(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
        {
        }

        public FiltersByRule()
        {
            RegisterAllPorts();

            var fec = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
            fec.OfClass(typeof(ParameterFilterElement));
            var elements = fec.ToElements();

            Filters = elements.Select(x => x.Name).ToObservableCollection();            
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            if (selectedFilter == null)
            {
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
            }

            var filterName = AstFactory.BuildStringNode(SelectedFilter);
            var functionCall = AstFactory.BuildFunctionCall(
                new Func<string, ParameterFilterElement>(Functions.ByName),
                new List<AssociativeNode> { filterName });

            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functionCall) };
        }
    }
}