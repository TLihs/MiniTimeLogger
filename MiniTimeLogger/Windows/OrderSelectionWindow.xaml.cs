using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using MiniTimeLogger.Data;

namespace MiniTimeLogger.Windows
{
    /// <summary>
    /// Interaktionslogik für OrderSelectionWindow.xaml
    /// </summary>
    public partial class OrderSelectionWindow : Window
    {
        private ObservableCollection<OrderItemEntry> _orderItems = new ObservableCollection<OrderItemEntry>();
        
        public OrderSelectionWindow()
        {
            InitializeComponent();

            ListBox_Projects.ItemsSource = _orderItems;
        }
    }
}
