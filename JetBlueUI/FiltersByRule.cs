using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using RevitServices.Persistence;
using Autodesk.Revit.DB;

using JetBlue;


namespace JetBlueUI
{
    /// <summary>
    /// Sample NodeModel with custom Gui
    /// </summary>
    [NodeName("Filters By Rule")]
    [NodeDescription("Example Node Model, multiplies A x the value of the slider")]
    [NodeCategory("JetPack")]
    [InPortNames("A")]
    [InPortTypes("double")]
    [InPortDescriptions("Number A")]
    [OutPortNames("C")]
    [OutPortTypes("double")]
    [OutPortDescriptions("Product of AxSlider")]
    [IsDesignScriptCompatible]
    public class FiltersByRule : NodeModel
    {
        private ObservableCollection<string> _filters;
        private string _selectedFilter;

        public ObservableCollection<string> Filters
        {
            get { return _filters; }
            set
            {
                _filters = value;
                RaisePropertyChanged("Filters");
                OnNodeModified(false);
            }
        }

        public string SelectedFilter
        {
            get { return _selectedFilter; }
            set
            {
                _selectedFilter = value;
                RaisePropertyChanged("SelectedFilter");
                OnNodeModified(false);
            }
        }

        public FiltersByRule()
        {
            var fec = new FilteredElementCollector(DocumentManager.Instance.CurrentDBDocument);
            fec.OfClass(typeof(ParameterFilterElement));
            var elements = fec.ToElements();

            _filters = elements.Select(x => x.Name).ToObservableCollection();

            RegisterAllPorts();
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {

            if (!InPorts[0].IsConnected)
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