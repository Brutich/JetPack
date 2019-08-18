using System;
using System.Collections.Generic;
using Dynamo.Graph.Nodes;
using JetBlue;
using ProtoCore.AST.AssociativeAST;

namespace JetBlueUI
{
    /// <summary>
    /// Sample NodeModel 
    /// In order to execute AstFactory.BuildFunctionCall 
    /// the methods have to be in a separate assembly and be loaded by Dynamo separately
    /// File pkg.json defines which dll are loaded
    /// </summary>
    [NodeName("JetNodeModel")]
    [NodeDescription("Example Node Model, multiplies AxB")]
    [NodeCategory("JetPack")]
    [InPortNames("A", "B")]
    [InPortTypes("double", "double")]
    [InPortDescriptions("Number A", "Numnber B")]
    [OutPortNames("C")]
    [OutPortTypes("double")]
    [OutPortDescriptions("Product of AxB")]
    [IsDesignScriptCompatible]
    public class JetNodeModel : NodeModel
    {
        public JetNodeModel()
        {
            RegisterAllPorts();
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            if (!InPorts[0].IsConnected || !InPorts[1].IsConnected) // old (!HasConnectedInput(0) || !HasConnectedInput(1))
            {
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
            }

            var functionCall =
              AstFactory.BuildFunctionCall(
                new Func<double, double, double>(Functions.MultiplyTwoNumbers),  //new Func<double, double, double>(SampleFunctions.MultiplyTwoNumbers),
                new List<AssociativeNode> { inputAstNodes[0], inputAstNodes[1] });

            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functionCall) };
        }
    }
}