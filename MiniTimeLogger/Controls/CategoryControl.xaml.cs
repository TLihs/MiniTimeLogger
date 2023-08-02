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
    /// Interaktionslogik für CategoryControl.xaml
    /// </summary>
    public partial class CategoryControl : UserControl
    {
        private Category _category;
        private bool _editMode = false;
        
        public Category Item => _category;
        
        public CategoryControl()
        {
            InitializeComponent();
        }

        public void LoadCategoryData(Category item)
        {
            LogDebug($"{GetType()}::{GetCaller()}({item})");
            _category = item;
            TextBox_ItemText.Text = _category.Name;
            TextBox_ItemText.ToolTip = _category.Description;
            _category.PropertyChanged += OnItemPropertyChanged;
        }

        public void UnloadCategoryData()
        {
            LogDebug($"{GetType()}::{GetCaller()}()");
            _category.PropertyChanged -= OnItemPropertyChanged;
            TextBox_ItemText.Text = string.Empty;
            TextBox_ItemText.ToolTip = string.Empty;
            _category = null;
        }

        private void OnItemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Category.Name))
                TextBox_ItemText.Text = _category.Name;
            if (e.PropertyName != nameof(Category.Description))
                TextBox_ItemText.ToolTip = _category.Description;
        }

        private void Label_ItemText_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_editMode)
                return;

            TextBox_ItemText.IsReadOnly = false;
            TextBox_ItemText.Select(0, 0);
            _editMode = true;
        }

        private void TextBox_ItemText_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!_editMode)
                return;

            if (string.IsNullOrWhiteSpace(TextBox_ItemText.Text))
                TextBox_ItemText.Text = _category.Name;
            else
                _category.Name = TextBox_ItemText.Text;

            TextBox_ItemText.Select(0, 0);
            TextBox_ItemText.IsReadOnly = true;
            _editMode = false;
        }

        private void TextBox_ItemText_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                TextBox_ItemText_LostFocus(null, null);
        }
    }
}
