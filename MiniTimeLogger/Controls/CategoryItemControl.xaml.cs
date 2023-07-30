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

namespace MiniTimeLogger.Controls
{
    /// <summary>
    /// Interaktionslogik für CategoryItemControl.xaml
    /// </summary>
    public partial class CategoryItemControl : UserControl
    {
        private CategoryItem _categoryItem;

        public CategoryItem Item => _categoryItem;
        
        public CategoryItemControl()
        {
            InitializeComponent();
        }

        public void LoadCategoryItemData(CategoryItem item)
        {
            _categoryItem = item;
            Label_ItemText.Content = _categoryItem.Name;
            Label_ItemText.ToolTip = _categoryItem.Description;
            _categoryItem.PropertyChanged += OnItemPropertyChanged;
        }

        public void UnloadCategoryItemData()
        {
            _categoryItem.PropertyChanged -= OnItemPropertyChanged;
            Label_ItemText.Content = string.Empty;
            Label_ItemText.ToolTip = string.Empty;
            _categoryItem = null;
        }

        private void OnItemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CategoryItem.Name))
                Label_ItemText.Content = _categoryItem.Name;
            if (e.PropertyName != nameof(CategoryItem.Description))
                Label_ItemText.ToolTip = _categoryItem.Description;
        }
    }
}
