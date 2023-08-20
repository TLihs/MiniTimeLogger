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
        public enum ResizeModeType
        {
            Left = 0b0001,
            TopLeft = 0b0101,
            Top = 0b0100,
            TopRight = 0b0110,
            Right = 0b0010,
            BottomRight = 0b1010,
            Bottom = 0b1000,
            BottomLeft = 0b1001,
            None = 0b0000
        }

        public enum ResizeType
        {
            Horizontal,
            Vertical,
            Uniform,
            None
        }

        public enum CollapseType
        {
            CollapseHorizontal,
            CollapseVertical,
            CollapseNone
        }

        public static void SetButtonState(Button button, bool enabled, bool keepVisible = false)
        {
            button.IsEnabled = enabled;
            button.Focusable = enabled;
            button.Visibility = enabled || keepVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public static bool IsResizeModeLeft(ResizeModeType resizeModeType)
        {
            return (resizeModeType & ResizeModeType.Left) == ResizeModeType.Left;
        }

        public static bool IsResizeModeRight(ResizeModeType resizeModeType)
        {
            return (resizeModeType & ResizeModeType.Right) == ResizeModeType.Right;
        }

        public static bool IsResizeModeTop(ResizeModeType resizeModeType)
        {
            return (resizeModeType & ResizeModeType.Top) == ResizeModeType.Top;
        }

        public static bool IsResizeModeBottom(ResizeModeType resizeModeType)
        {
            return (resizeModeType & ResizeModeType.Bottom) == ResizeModeType.Bottom;
        }
    }
}
