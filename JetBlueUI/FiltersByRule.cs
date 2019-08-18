using System;
using System.Collections.Generic;
using Dynamo.Graph.Nodes;
using JetBlue;
using ProtoCore.AST.AssociativeAST;

namespace JetBlueUI
{
    /// <summary>
    /// Sample NodeModel with custom Gui
    /// </summary>
    [NodeName("FiltersByRule")]
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
        private double _sliderValue;


        public double SliderValue
        {
            get { return _sliderValue; }
            set
            {
                _sliderValue = value;
                RaisePropertyChanged("SliderValue");
                OnNodeModified(false);
            }
        }

        public FiltersByRule()
        {
            RegisterAllPorts();
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {

            if (!InPorts[0].IsConnected) //(!HasConnectedInput(0))
            {
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
            }
            var sliderValue = AstFactory.BuildDoubleNode(SliderValue);
            var functionCall =
              AstFactory.BuildFunctionCall(
                new Func<double, double, double>(Functions.MultiplyTwoNumbers),
                new List<AssociativeNode> { inputAstNodes[0], sliderValue });

            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functionCall) };
        }
    }
}