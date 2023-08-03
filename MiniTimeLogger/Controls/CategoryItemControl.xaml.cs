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

namespace MiniTimeLogger.Controls
{
    /// <summary>
    /// Interaktionslogik für CategoryItemControl.xaml
    /// </summary>
    public partial class CategoryItemControl : UserControl
    {
        public CategoryItem CategoryObject
        {
            get => (CategoryItem)EditableTextControl_Content.CategoryObject;
            set => EditableTextControl_Content.CategoryObject = value;
        }

        public CategoryItemControl()
        {
            InitializeComponent();
            EditableTextControl_Content.Height = double.NaN;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
        }
    }
}
