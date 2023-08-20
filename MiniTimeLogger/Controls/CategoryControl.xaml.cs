using MiniTimeLogger.Controls.Base;
using MiniTimeLogger.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using static MiniTimeLogger.Support.ExceptionHandling;
using static MiniTimeLogger.Support.GUIHandling;

namespace MiniTimeLogger.Controls
{
    /// <summary>
    /// Interaktionslogik für CategoryControl.xaml
    /// </summary>
    public partial class CategoryControl : BaseCategoryControl
    {
        public override string LabelText
        {
            get => Label_Content.LabelText;
            set => Label_Content.LabelText = value;
        }

        public CategoryControl()
        {
            InitializeComponent();
            Label_Content.Height = double.NaN;
            Label_Content.DragEnter += OnDragEnter;
            Label_Content.DragLeave += OnDragLeave;
            Label_Content.MouseMove += OnMouseMove;
        }

        public new Type GetType()
        {
            return typeof(CategoryControl);
        }

        public override void AddItem(CategoryItem item)
        {
            base.AddItem(item);

            Canvas_CategoryItems.Children.Add(item.Control);
        }

        public override void RemoveItem(CategoryItem item)
        {
            Canvas_CategoryItems.Children.Remove(item.Control);

            base.RemoveItem(item);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            foreach (CategoryItemControl control in Canvas_CategoryItems.Children)
                control.Width = ActualWidth;
        }

        private void OnMouseSingleClick(object sender, MouseEventArgs e)
        {
            SortItemsByName();
        }

        private void SortItemsByName()
        {
            
        }

        public new void ToggleCollapsed() => ContextItem_Collapse.Header = base.ToggleCollapsed() ? "Hide" : "Show";

        private void ContextItem_Collapse_Click(object sender, RoutedEventArgs e)
        {
            ToggleCollapsed();
        }

        public override void TriggerMouseButtonLeftDown(MouseButtonEventArgs e)
        {

        }

        public override void TriggerMouseButtonRightDown(MouseButtonEventArgs e)
        {

        }

        public override void TriggerMouseButtonMiddleDown(MouseButtonEventArgs e)
        {

        }

        public override void TriggerMouseButtonLeftUp(MouseButtonEventArgs e)
        {

        }

        public override void TriggerMouseButtonRightUp(MouseButtonEventArgs e)
        {

        }

        public override void TriggerMouseButtonMiddleUp(MouseButtonEventArgs e)
        {

        }

        public override void TriggerMouseMove(MouseEventArgs e)
        {

        }
    }
}
