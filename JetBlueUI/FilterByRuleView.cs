using Dynamo.Controls;
using Dynamo.Wpf;

namespace JetBlueUI
{
    /// <summary>
    /// View customizer for FilterByRuleView Node Model.
    /// </summary>
    public class FilterByRuleView : INodeViewCustomization<FiltersByRule>
    {
        public void CustomizeView(FiltersByRule model, NodeView nodeView)
        {
            var combobox = new Combobox();
            nodeView.inputGrid.Children.Add(combobox);

            combobox.DataContext = model;
        }

        public void Dispose()
        {
        }
    }
}