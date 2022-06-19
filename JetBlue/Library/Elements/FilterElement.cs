using System;
using System.Linq;
using Autodesk.Revit.DB;
using RevitServices.Persistence;


namespace Elements
{
    /// <summary>
    /// The Filter Element class.
    /// </summary>
    public class FilterElement
    {
        private FilterElement() { }

        /// <summary>
        /// Select a ParameterFilterElement from the current document by name
        /// </summary>
        /// <param name="name">Parameter filter name</param>
        /// <returns>Parameter filter</returns>
        /// <search>
        /// parameter, filter, by, rule, element
        /// </search>
        public static ParameterFilterElement ByName(string name)
        {
            if (name == null) return null;

            var filterElement = DocumentManager.Instance.ElementsOfType<ParameterFilterElement>()
                .FirstOrDefault(x => x.Name == name);

            if (filterElement == null)
                throw new Exception();

            return filterElement;
        }
    }
}
    