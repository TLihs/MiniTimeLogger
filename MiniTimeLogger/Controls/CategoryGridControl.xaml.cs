﻿using MiniTimeLogger.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaktionslogik für CategoryGridControl.xaml
    /// </summary>
    public partial class CategoryGridControl : UserControl
    {
        public static ObservableCollection<CategoryControl> Categories { get; set; } = new ObservableCollection<CategoryControl>();

        public CategoryGridControl()
        {
            InitializeComponent();
            StackPanel_Categories.ItemsSource = Categories;
            Button_AddCategory.Content = "+";
        }

        private void Button_AddCategory_Click(object sender, RoutedEventArgs e)
        {
            Category category = Category.CreateCategory(Category.CategoryObjects.Last(), "New Category", "");
            //category.Control.EditableTextControl_Content.TextBox_ItemText.Focusable = true;
            //category.Control.EditableTextControl_Content.TextBox_ItemText.IsReadOnly = false;
            //category.Control.EditableTextControl_Content.TextBox_ItemText.Focus();
            //category.Control.EditableTextControl_Content.TextBox_ItemText.Select(0, 0);
        }
    }
}
