using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using static MiniTimeLogger.Support.ExceptionHandling;

namespace MiniTimeLogger.Controls
{
    /// <summary>
    /// Interaktionslogik für CategoryContentControl.xaml
    /// </summary>
    public partial class CategoryContentControl : Grid
    {
        public static List<CategoryContentControl> CategoryContentControls { get; } = new List<CategoryContentControl>();
        
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.RegisterAttached("FontSize", typeof(double), typeof(Grid), new PropertyMetadata(12d));
        public static readonly DependencyProperty FontWeightProperty =
            DependencyProperty.RegisterAttached("FontWeight", typeof(FontWeight), typeof(Grid), new PropertyMetadata(FontWeights.Normal));
        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.RegisterAttached("FontFamily", typeof(FontFamily), typeof(Grid), new PropertyMetadata(new FontFamily("Segoe UI")));
        public static readonly DependencyProperty FontStretchProperty =
            DependencyProperty.RegisterAttached("FontStretch", typeof(FontStretch), typeof(Grid), new PropertyMetadata(FontStretches.Normal));
        public static readonly DependencyProperty FontStyleProperty =
            DependencyProperty.RegisterAttached("FontStyle", typeof(FontStyle), typeof(Grid), new PropertyMetadata(FontStyles.Normal));
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.RegisterAttached("BorderThickness", typeof(Thickness), typeof(Grid), new PropertyMetadata(new Thickness(1)));
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.RegisterAttached("BorderBrush", typeof(Brush), typeof(Grid), new PropertyMetadata(Brushes.Black));
        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.RegisterAttached("LabelText", typeof(string), typeof(Grid), new PropertyMetadata("<LabelText>"));
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.RegisterAttached("Foreground", typeof(Brush), typeof(Grid), new PropertyMetadata(Brushes.Black));
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.RegisterAttached("IsSelected", typeof(bool), typeof(Grid), new PropertyMetadata(false));
        public static readonly DependencyProperty InnerMarginProperty =
            DependencyProperty.RegisterAttached("InnerMargin", typeof(Thickness), typeof(Grid), new PropertyMetadata(new Thickness(0)));

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }
        public FontWeight FontWeight
        {
            get => (FontWeight)GetValue(FontWeightProperty);
            set => SetValue(FontWeightProperty, value);
        }
        public FontFamily FontFamily
        {
            get => (FontFamily)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }
        public FontStretch FontStretch
        {
            get => (FontStretch)GetValue(FontStretchProperty);
            set => SetValue(FontStretchProperty, value);
        }
        public FontStyle FontStyle
        {
            get => (FontStyle)GetValue(FontStyleProperty);
            set => SetValue(FontStyleProperty, value);
        }
        public Thickness BorderThickness
        {
            get => (Thickness)GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }
        public Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }
        public string LabelText
        {
            get => (string)GetValue(LabelTextProperty);
            set => SetValue(LabelTextProperty, value);
        }
        public Brush Foreground
        {
            get => (Brush)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }
        public Thickness InnerMargin
        {
            get => (Thickness)GetValue(InnerMarginProperty);
            set => SetValue(InnerMarginProperty, value);
        }

        public CategoryContentControl()
        {
            InitializeComponent();
            CategoryContentControls.Add(this);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            IsSelected = true;

            CategoryContentControls.ForEach(control =>
            {
                if (control != this && Keyboard.IsKeyUp(Key.LeftCtrl) && Keyboard.IsKeyUp(Key.LeftShift) && Keyboard.IsKeyUp(Key.RightCtrl) && Keyboard.IsKeyUp(Key.RightShift))
                    control.IsSelected = false;
            });

            base.OnMouseLeftButtonDown(e);
        }
    }
}
