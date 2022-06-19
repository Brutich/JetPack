using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.DesignScript.Runtime;
using RevitServices.Persistence;
using Dynamo.Graph.Nodes;


namespace Elements
{
    /// <summary>
    /// The WallType class.
    /// </summary>
    public class WallType
    {    
        
        private WallType() { }


        /// <summary>
        /// Returns wall compound structure layers.
        /// </summary>
        /// <param name="wallType">Wall type</param>
        /// <returns></returns>
        /// <search>
        /// wall, compound, structure, layers
        /// </search>
        [IsVisibleInDynamoLibrary(true)]
        [NodeCategory("Query")]
        public static List<Layer> GetLayers(Revit.Elements.WallType wallType)
        {
            Document document = DocumentManager.Instance.CurrentDBDocument;

            var wt = wallType.InternalElement as Autodesk.Revit.DB.WallType;
            CompoundStructure structure = wt.GetCompoundStructure();
            int layerCount = structure.LayerCount;
            int strMaterialInd = structure.StructuralMaterialIndex;

            List<Layer> layers = new List<Layer>(); 

            for (int i = 0; i < layerCount; i++)
            {
                layers.Add(new Layer(structure.GetLayerFunction(i).ToString(),
                        document.GetElement(structure.GetMaterialId(i)) as Autodesk.Revit.DB.Material,
                        UnitUtils.ConvertFromInternalUnits(structure.GetLayerWidth(i), UnitTypeId.Millimeters),
                        i == strMaterialInd,
                        structure.IsCoreLayer(i) ));
            }

            return layers;
        }


        /// <summary>
        /// Returns wall type name. Basic, curtain or stacked wall.
        /// </summary>
        /// <param name="wallType">Wall type</param>
        /// <returns></returns>
        /// <search>
        /// wall, type, name
        /// </search>
        [IsVisibleInDynamoLibrary(true)]
        [NodeCategory("Query")]
        public static string FamilyName(Revit.Elements.WallType wallType)
        {
            return (wallType.InternalElement as Autodesk.Revit.DB.WallType).FamilyName;
        }

    }
}
    