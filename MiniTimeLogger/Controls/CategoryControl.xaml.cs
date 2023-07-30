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
    /// Interaktionslogik für CategoryControl.xaml
    /// </summary>
    public partial class CategoryControl : UserControl
    {
        private Category _category;
        
        public Category Item => _category;
        
        public CategoryControl()
        {
            InitializeComponent();
        }

        public void LoadCategoryData(Category item)
        {
            _category = item;
            Label_ItemText.Content = _category.Name;
            Label_ItemText.ToolTip = _category.Description;
            _category.PropertyChanged += OnItemPropertyChanged;
        }

        public void UnloadCategoryData()
        {
            _category.PropertyChanged -= OnItemPropertyChanged;
            Label_ItemText.Content = string.Empty;
            Label_ItemText.ToolTip = string.Empty;
            _category = null;
        }

        private void OnItemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Category.Name))
                Label_ItemText.Content = _category.Name;
            if (e.PropertyName != nameof(Category.Description))
                Label_ItemText.ToolTip = _category.Description;
        }
    }
}
