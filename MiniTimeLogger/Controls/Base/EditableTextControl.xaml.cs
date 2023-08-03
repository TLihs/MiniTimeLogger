using MiniTimeLogger.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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

namespace MiniTimeLogger.Controls.Base
{
    /// <summary>
    /// Interaktionslogik für EditableTextControl.xaml
    /// </summary>
    public partial class EditableTextControl : TextBox
    {
        private object _categoryObject = null;
        private Type _categoryType = null;
        private Timer _clickTimer = new Timer();
        private int _clickCount = 0;
        private MouseButtonEventArgs _mouseClickEventArgs;
        private bool _initiallyFocusable;

        public event MouseButtonEventHandler MouseSingleClick;
        public new event MouseButtonEventHandler MouseDoubleClick;
        
        public bool InitiallyFocusable
        {
            get => _initiallyFocusable;
            set => _initiallyFocusable = value;
        }

        public object CategoryObject
        {
            get => _categoryObject;
            set
            {
                if (value != null && (value.GetType() == typeof(Category) || value.GetType() == typeof(CategoryItem)))
                {
                    _categoryObject = value;
                    _categoryType = value.GetType();
                    if (_categoryType == typeof(Category))
                        ((Category)_categoryObject).PropertyChanged += OnItemPropertyChanged;
                    else if (_categoryType == typeof(CategoryItem))
                        ((CategoryItem)_categoryObject).PropertyChanged += OnItemPropertyChanged;
                }
            }
        }
        
        public EditableTextControl()
        {
            MouseDoubleClick += OnMouseDoubleClick;
            MouseSingleClick += OnMouseSingleClick;

            InitializeComponent();

            ResetClickTimer();
        }

        private void ResetClickTimer()
        {
            _clickTimer = new Timer(400);
            _clickTimer.Elapsed += OnMouseTimerExpire;
            _clickTimer.AutoReset = false;
        }

        private void OnItemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_categoryType == typeof(Category))
            {
                if (e.PropertyName == nameof(Category.Name))
                    Text = ((Category)_categoryObject).Name;
                if (e.PropertyName != nameof(Category.Description))
                    ToolTip = ((Category)_categoryObject).Description;
            }
            else if (_categoryType == typeof(CategoryItem))
            {
                if (e.PropertyName == nameof(CategoryItem.Name))
                    Text = ((CategoryItem)_categoryObject).Name;
                if (e.PropertyName != nameof(CategoryItem.Description))
                    ToolTip = ((CategoryItem)_categoryObject).Description;
            }
        }

        private void OnMouseTimerExpire(object sender, ElapsedEventArgs e)
        {
            if (_clickCount == 2)
                MouseDoubleClick.Invoke(sender, _mouseClickEventArgs);
            else if (_clickCount > 0)
                MouseSingleClick.Invoke(sender, _mouseClickEventArgs);
            else
                return;

            _clickCount = 0;
            _mouseClickEventArgs = null;
            ResetClickTimer();
        }

        private void TextBox_ItemText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LogDebug($"{GetType().FullName}::{GetCaller()}()");
            _clickTimer.Stop();
            _mouseClickEventArgs = e;
            _clickCount++;
            _clickTimer.Start();
        }

        public virtual void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                LogDebug($"{GetType().FullName}::{GetCaller()}()");
                if (!IsReadOnly && Focusable)
                    return;

                IsReadOnly = false;
                Focusable = true;
                Focus();
                Select(0, 0);
            }, System.Windows.Threading.DispatcherPriority.Input);
        }

        public virtual void OnMouseSingleClick(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                LogDebug($"{GetType().FullName}::{GetCaller()}()");
                if (InitiallyFocusable)
                {
                    Focusable = true;
                    Focus();
                }
            }, System.Windows.Threading.DispatcherPriority.Input);
        }

        private void TextBox_ItemText_LostFocus(object sender, RoutedEventArgs e)
        {
            LogDebug($"{GetType().FullName}::{GetCaller()}()");
            if (!Focusable || _categoryObject == null)
                return;

            if (!IsReadOnly)
            {
                if (string.IsNullOrWhiteSpace(Text))
                {
                    if (_categoryType == typeof(Category))
                        Text = ((Category)_categoryObject).Name;
                    else if (_categoryType == typeof(CategoryItem))
                        Text = ((CategoryItem)_categoryObject).Name;
                }
                else
                {
                    if (_categoryType == typeof(Category))
                        ((Category)_categoryObject).Name = Text;
                    else if (_categoryType == typeof(CategoryItem))
                        ((CategoryItem)_categoryObject).Name = Text;
                }

                IsReadOnly = true;
            }

            Focusable = false;
        }

        private void TextBox_ItemText_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                TextBox_ItemText_LostFocus(null, null);
        }
    }
}
