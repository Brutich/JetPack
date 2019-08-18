using Dynamo.Controls;
using Dynamo.Wpf;

namespace JetBlueUI
{
    public class HelloGuiNodeView : INodeViewCustomization<FiltersByRule>
    {
        public void CustomizeView(FiltersByRule model, NodeView nodeView)
        {
            var slider = new Slider();
            nodeView.inputGrid.Children.Add(slider);
            slider.DataContext = model;
        }

        public void Dispose()
        {
        }
    }
}