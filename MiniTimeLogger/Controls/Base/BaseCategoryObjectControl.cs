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

using static MiniTimeLogger.Support.GUIHandling;
using static MiniTimeLogger.Support.ExceptionHandling;
using System.Diagnostics.Eventing.Reader;

namespace MiniTimeLogger.Controls.Base
{
    /// <summary>
    /// Interaktionslogik für BaseCategoryObjectControl.xaml
    /// </summary>
    public abstract class BaseCategoryObjectControl<T1, T2> : UserControl
        where T1 : BaseCategoryObject<T1, T2>
        where T2 : BaseCategoryObjectControl<T1, T2>
    {
        private Vector _mouseDragOffset = new Vector(0d, 0d);
        private bool _collapsed = false;
        private double _lastWidth = 120;
        private double _lastHeight = 120;
        private ResizeModeType _resizeMode = ResizeModeType.None;

        public static string ThisStaticType => typeof(T1).Name;
        public static List<T2> CategoryObjectControls { get; } = new List<T2>();

        public T1 CategoryObject { get; set; }
        public T2 NextControl { get; set; }
        public T2 PreviousControl { get; set; }
        public double CollapsedWidth { get; set; } = 5d;
        public double CollapsedHeight { get; set; } = 5d;
        public double UncollapsedMinWidth { get; set; } = 10d;
        public double UncollapsedMinHeight { get; set; } = 10d;
        public abstract string LabelText { get; set; }

        public static readonly DependencyProperty CollapseTypeProperty =
            DependencyProperty.RegisterAttached("CollapseType", typeof(CollapseType), typeof(T2), new PropertyMetadata(CollapseType.CollapseNone));
        public static readonly DependencyProperty ResizeTypeProperty =
            DependencyProperty.RegisterAttached("ResizeType", typeof(ResizeType), typeof(T2), new PropertyMetadata(ResizeType.None));
        public static readonly DependencyProperty ResizeEdgeSizeProperty =
            DependencyProperty.RegisterAttached("ResizeEdgeSize", typeof(Thickness), typeof(T2), new PropertyMetadata(new Thickness(5d)));

        public CollapseType CollapseType
        {
            get => (CollapseType)GetValue(CollapseTypeProperty);
            set => SetValue(CollapseTypeProperty, value);
        }
        public ResizeType ResizeType
        {
            get => (ResizeType)GetValue(ResizeTypeProperty);
            set => SetValue(ResizeTypeProperty, value);
        }
        public Thickness ResizeEdgeSize
        {
            get => (Thickness)GetValue(ResizeEdgeSizeProperty);
            set => SetValue(ResizeEdgeSizeProperty, value);
        }

        public BaseCategoryObjectControl()
        {
            MouseMove += OnMouseMove;
            DragEnter += OnDragEnter;
            DragLeave += OnDragLeave;

            CategoryObjectControls.Add((T2)this);
        }

        public virtual void AddItem(CategoryItem item)
        {
            LogDebug($"{GetType()}::{GetCaller()}()");

            double top = 0d;

            if (item.NextItem != null)
                top = item.NextItem.Control.GetTop();
            else if (item.PreviousItem != null)
                top = item.PreviousItem.Control.GetTop() + item.PreviousItem.Control.ActualHeight;
            else if (item.Parent != null)
                top = item.Parent.Control.GetTop();

            LogDebug($"{GetType()}::{GetCaller()}() - top: {top}; RenderSize: {item.Control.RenderSize}");
            item.Control.SetTop(top);

            CategoryItem nextitem = item.NextItem;
            CategoryItem currentitem = item;

            while (nextitem != null)
            {
                nextitem.Control.SetTop(currentitem.Control.GetTop() + currentitem.Control.ActualHeight);
                currentitem = nextitem;
                nextitem = currentitem.NextItem;
            }
        }

        public virtual void RemoveItem(CategoryItem item)
        {
            if (typeof(T2) == typeof(CategoryControl))
            {
                T2 subcategory = NextControl;

                while (subcategory != null)
                {
                    foreach (CategoryItem subitem in item.CategoryItems)
                        subcategory.RemoveItem(subitem);

                    subcategory = subcategory.NextControl;
                }
            }
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(this);

            if (_resizeMode == ResizeModeType.None)
                return;

            if (IsResizeModeLeft(_resizeMode))
                CategoryObject?.Parent?.Control.OnMouseMove(sender, e);
            else if (IsResizeModeTop(_resizeMode))
                PreviousControl?.OnMouseMove(sender, e);
            else
            {
                Point newsize = position + _mouseDragOffset;
                Resize(newsize.X, newsize.Y);
            }
        }

        public void OnDragEnter(object sender, DragEventArgs e)
        {
            LogDebug($"{GetType()}::{GetCaller()}");
            
            // if ResizeModeType for this control is set to none, then resizing is not allowed
            if (ResizeType == ResizeType.None)
                return;

            if (e.KeyStates == DragDropKeyStates.LeftMouseButton)
            {
                _resizeMode = CheckResizeStart(e.GetPosition(this));

                if (_resizeMode == ResizeModeType.None)
                    return;
                
                if (IsResizeModeLeft(_resizeMode))
                    CategoryObject?.Parent?.Control?.OnDragEnter(sender, e);
                else if (IsResizeModeTop(_resizeMode))
                    PreviousControl?.OnDragEnter(sender, e);
                else
                {
                    Point position = e.GetPosition(this);

                    if (IsResizeModeRight(_resizeMode))
                        _mouseDragOffset.X = ActualWidth - position.X;
                    if (IsResizeModeBottom(_resizeMode))
                        _mouseDragOffset.Y = ActualHeight - position.Y;
                }
            }
        }

        public void OnDragLeave(object sender, DragEventArgs e)
        {
            LogDebug($"{GetType()}::{GetCaller()}");

            if (_resizeMode == ResizeModeType.None)
                return;

            if (IsResizeModeLeft(_resizeMode))
                CategoryObject?.Parent?.Control.OnDragLeave(sender, e);
            else if (IsResizeModeTop(_resizeMode))
                PreviousControl?.OnDragLeave(sender, e);

            ResetResizeMode();
        }

        private ResizeModeType CheckResizeStart(Point position)
        {
            if (position.X < 0 || position.Y < 0 || position.X > ActualWidth || position.Y > ActualHeight)
                return ResizeModeType.None;

            if (position.X <= ResizeEdgeSize.Left)
            {
                if (position.Y <= ResizeEdgeSize.Top)
                    return ResizeModeType.TopLeft;
                else if (position.Y >= ActualHeight - ResizeEdgeSize.Bottom)
                    return ResizeModeType.BottomLeft;
                else
                    return ResizeModeType.Left;
            }
            else if (position.X >= ActualWidth - ResizeEdgeSize.Right)
            {
                if (position.Y <= ResizeEdgeSize.Top)
                    return ResizeModeType.TopRight;
                else if (position.Y >= ActualHeight - ResizeEdgeSize.Bottom)
                    return ResizeModeType.BottomRight;
                else
                    return ResizeModeType.Right;
            }
            else if (position.Y <= ResizeEdgeSize.Top)
                return ResizeModeType.Top;
            else if (position.Y >= ResizeEdgeSize.Bottom)
                return ResizeModeType.Bottom;
            else
                return ResizeModeType.None;
        }

        public void ResetResizeMode()
        {
            LogDebug($"{GetType()}::{GetCaller()}");

            _resizeMode = ResizeModeType.None;
            _mouseDragOffset = new Vector(0d, 0d);
        }

        // TODO: RefreshControlSize should be in the Control class
        // TODO: Parent controls should set top position for their child controls
        // TODO: Child controls should set the height for the parent control
        public void Resize(double width, double height)
        {
            if (width > UncollapsedMinWidth)
                Width = width;
            else
                Width = UncollapsedMinWidth;

            if (height > UncollapsedMinHeight)
                Height = height;
            else
                Height = UncollapsedMinHeight;
        }

        public virtual bool ToggleCollapsed()
        {
            if (CollapseType == CollapseType.CollapseNone)
                return false;
            
            if (_collapsed)
            {
                if (CollapseType == CollapseType.CollapseHorizontal)
                    Width = _lastWidth;
                else if (CollapseType == CollapseType.CollapseVertical)
                    Height = _lastHeight;

                _collapsed = false;
            }
            else
            {
                if (CollapseType == CollapseType.CollapseHorizontal)
                {
                    _lastWidth = Width;
                    Width = 5;
                }
                else if (CollapseType == CollapseType.CollapseVertical)
                {
                    _lastHeight = Height;
                    Height = 5;
                }

                _collapsed = true;
            }

            return _collapsed;
        }

        public double GetTop()
        {
            return Canvas.GetTop(this);
        }

        public double GetBottom()
        {
            return Canvas.GetBottom(this);
        }

        public double GetLeft()
        {
            return Canvas.GetLeft(this);
        }

        public double GetRight()
        {
            return Canvas.GetRight(this);
        }

        public void SetTop(double top)
        {
            Canvas.SetTop(this, top);
        }

        public void SetBottom(double bottom)
        {
            Canvas.SetBottom(this, bottom);
        }

        public void SetLeft(double left)
        {
            Canvas.SetLeft(this, left);
        }

        public void SetRight(double right)
        {
            Canvas.SetRight(this, right);
        }

        public void OffsetTop(double offset)
        {
            SetTop(GetTop() + offset);
        }

        public void OffsetBottom(double offset)
        {
            SetBottom(GetBottom() + offset);
        }

        public void OffsetLeft(double offset)
        {
            SetLeft(GetLeft() + offset);
        }

        public void OffsetRight(double offset)
        {
            SetRight(GetRight() + offset);
        }

        public abstract void TriggerMouseButtonLeftDown(MouseButtonEventArgs e);
        public abstract void TriggerMouseButtonRightDown(MouseButtonEventArgs e);
        public abstract void TriggerMouseButtonMiddleDown(MouseButtonEventArgs e);
        public abstract void TriggerMouseButtonLeftUp(MouseButtonEventArgs e);
        public abstract void TriggerMouseButtonRightUp(MouseButtonEventArgs e);
        public abstract void TriggerMouseButtonMiddleUp(MouseButtonEventArgs e);
        public abstract void TriggerMouseMove(MouseEventArgs e);
    }
}
