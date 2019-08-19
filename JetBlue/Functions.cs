using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using RevitServices.Persistence;
using System;
using System.Linq;

namespace JetBlue
{
    /// <summary>
    /// In order to be executed by the NodeModel AstFactory.BuildFunctionCall 
    /// these methods have to be in a separate assembly and be loaded by Dynamo separately
    /// File pkg.json defines which dll are loaded
    /// </summary>
    [IsVisibleInDynamoLibrary(false)]
    public class Functions
    {
        /// <summary>
        /// Multiply two numbers.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double MultiplyTwoNumbers(double a, double b)
        {
            return a * b;
        }
        

        /// <summary>
        /// Filter Name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string FilterName(string name)
        {
            return name;
        }


        /// <summary>
        /// Select a ParameterFilterElement from the current document by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ParameterFilterElement ByName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            var type = DocumentManager.Instance.ElementsOfType<Autodesk.Revit.DB.ParameterFilterElement>()
                .FirstOrDefault(x => x.Name == name);

            if (type == null)
            {
                throw new Exception();
            }

            return type;
        }


    }
}
 