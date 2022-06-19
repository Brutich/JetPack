using Dynamo.Graph.Nodes;


namespace Elements
{
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
}
    