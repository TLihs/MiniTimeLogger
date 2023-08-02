using MiniTimeLogger.Data;
using MiniTimeLogger.Windows;
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

namespace MiniTimeLogger
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Category maincategory = Category.CreateCategory(null, "Test1", "Category for test 1");
            Category secondarycategory = Category.CreateCategory(maincategory, "Test2", "Category for test 2");
            Category tertiarycategory = Category.CreateCategory(secondarycategory, "Test3", "Category for test 3");
            Category.CreateCategory(tertiarycategory, "Blumblums Category");

            CategoryItem.CreateCategoryItem(null, "TestItem1", "No. 1");
            CategoryItem.CreateCategoryItem(null, "TestItem2", "No. 2");
            CategoryItem.CreateCategoryItem(CategoryItem.CategoryObjects[0], "TestItem3", "No. 3");
            CategoryItem.CreateCategoryItem(CategoryItem.CategoryObjects[2], "TestItem4", "No. 4");
            CategoryItem.CreateCategoryItem(CategoryItem.CategoryObjects[1], "TestItem5", "No. 5");
            CategoryItem.CreateCategoryItem(CategoryItem.CategoryObjects[1], "TestItem6", "No. 6");
        }
    }
}
