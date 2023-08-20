using MiniTimeLogger.Controls.Base;
using MiniTimeLogger.Data;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace MiniTimeLogger.Controls
{
    /// <summary>
    /// Interaktionslogik für CategoryItemControl.xaml
    /// </summary>
    public partial class CategoryItemControl : BaseCategoryItemControl
    {
        private bool _buttonsVisible = false;
        
        public override string LabelText
        {
            get => Label_Content.LabelText;
            set => Label_Content.LabelText = value;
        }

        public CategoryItemControl()
        {
            InitializeComponent();
            Label_Content.Height = double.NaN;
        }

        public new Type GetType()
        {
            return typeof(CategoryItemControl);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            LogDebug($"{GetType()}::{GetCaller()}() - sizeInfo: {sizeInfo.PreviousSize} (old); {sizeInfo.NewSize} (new)");

            RefreshSize();

            base.OnRenderSizeChanged(sizeInfo);
        }

        public void HideAddButtons()
        {
            if (_buttonsVisible)
            {
                LogDebug($"{GetType()}::{GetCaller()}() - Name: {CategoryObject.Name}");

                Button_AddSubItem.Visibility = Visibility.Collapsed;
                Button_AddSiblingItemAbove.Visibility = Visibility.Collapsed;
                Button_AddSiblingItemBelow.Visibility = Visibility.Collapsed;
                _buttonsVisible = false;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            // TODO: Move to CategoryGridControl and handle it there.
            foreach (CategoryItem child in CategoryItem.CategoryObjects)
                if (child.Control != this)
                    child.Control.HideAddButtons();
            
            if (!Label_Content.IsSelected || !IsMouseOver)
            {
                HideAddButtons();
                return;
            }

            Point position = e.GetPosition(this);

            // TODO: When making add buttons visible, the item control should be resized.
            if (position.X > ActualWidth - Button_AddSubItem.Width && position.X <= ActualWidth)
            {
                if (Button_AddSubItem.Visibility != Visibility.Visible)
                {
                    Button_AddSubItem.Visibility = Visibility.Visible;
                    Button_AddSiblingItemAbove.Visibility = Visibility.Collapsed;
                    Button_AddSiblingItemBelow.Visibility = Visibility.Collapsed;
                    _buttonsVisible = true;
                }
            }
            else if (position.Y < Button_AddSiblingItemAbove.Height && position.Y >= 0)
            {
                if (Button_AddSiblingItemAbove.Visibility != Visibility.Visible)
                {
                    Button_AddSubItem.Visibility = Visibility.Collapsed;
                    Button_AddSiblingItemAbove.Visibility = Visibility.Visible;
                    Button_AddSiblingItemBelow.Visibility = Visibility.Collapsed;
                    _buttonsVisible = true;
                }
            }
            else if (position.Y > ActualHeight - Button_AddSiblingItemBelow.Height && position.Y <= ActualHeight)
            {
                if (Button_AddSiblingItemBelow.Visibility != Visibility.Visible)
                {
                    Button_AddSubItem.Visibility = Visibility.Collapsed;
                    Button_AddSiblingItemAbove.Visibility = Visibility.Collapsed;
                    Button_AddSiblingItemBelow.Visibility = Visibility.Visible;
                    _buttonsVisible = true;
                }
            }
            else
                HideAddButtons();

            base.OnMouseMove(this, e);
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
            OnMouseMove(e);
        }
    }
}
