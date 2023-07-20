// WINI Tool
// Copyright (c) 2023 Toni Lihs
// Licensed under MIT License

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MiniTimeLogger.Support
{
    public static class GUIHandling
    {
        public static void SetButtonState(Button button, bool enabled, bool keepVisible = false)
        {
            button.IsEnabled = enabled;
            button.Focusable = enabled;
            button.Visibility = enabled || keepVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }
    }
}
