using MiniTimeLogger.Controls.Base;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Windows.Input;

using static MiniTimeLogger.Support.ExceptionHandling;
using System.Windows;

namespace MiniTimeLogger.Input
{
    public static class MouseHandling
    {
        public enum MouseButtonStates
        {
            LeftButtonUp = 0x02,
            LeftButtonDown = 0x04,
            RightButtonUp = 0x08,
            RightButtonDown = 0x10,
            MiddleButtonUp = 0x20,
            MiddleButtonDown = 0x40
        }

        private static Timer _mouseStateWatcher;
        private static MouseButtonStates _lastMouseButtonState;
        private static Point _lastMousePosition;

        public static event MouseButtonEventHandler LeftMouseButtonChanged;
        public static event MouseButtonEventHandler RightMouseButtonChanged;
        public static event MouseButtonEventHandler MiddleMouseButtonChanged;
        public static event MouseEventHandler MousePositionChanged;

        public static Type ThisStaticType = typeof(MouseHandling);

        public static void InitalizeMouseHandling()
        {
            LogDebug($"{ThisStaticType}::{GetCaller()}()");

            _lastMouseButtonState = MouseButtonStates.LeftButtonUp | MouseButtonStates.RightButtonUp;

            LeftMouseButtonChanged += OnLeftMouseButtonChanged;
            RightMouseButtonChanged += OnRightMouseButtonChanged;
            MiddleMouseButtonChanged += OnMiddleMouseButtonChanged;
            MousePositionChanged += OnMousePositionChanged;
        }

        public static void SetLeftButton(bool down)
        {
            if (down)
            {
                _lastMouseButtonState |= MouseButtonStates.LeftButtonDown;
                _lastMouseButtonState &= ~MouseButtonStates.LeftButtonUp;
            }
            else
            {
                _lastMouseButtonState |= MouseButtonStates.LeftButtonUp;
                _lastMouseButtonState &= ~MouseButtonStates.LeftButtonDown;
            }
        }

        public static void SetRightButton(bool down)
        {
            if (down)
            {
                _lastMouseButtonState |= MouseButtonStates.RightButtonDown;
                _lastMouseButtonState &= ~MouseButtonStates.RightButtonUp;
            }
            else
            {
                _lastMouseButtonState |= MouseButtonStates.RightButtonUp;
                _lastMouseButtonState &= ~MouseButtonStates.RightButtonDown;
            }
        }

        public static void SetMiddleButton(bool down)
        {
            if (down)
            {
                _lastMouseButtonState |= MouseButtonStates.MiddleButtonDown;
                _lastMouseButtonState &= ~MouseButtonStates.MiddleButtonUp;
            }
            else
            {
                _lastMouseButtonState |= MouseButtonStates.MiddleButtonUp;
                _lastMouseButtonState &= ~MouseButtonStates.MiddleButtonDown;
            }
        }

        public static void SetMousePosition(Point position)
        {
            _lastMousePosition = position;
        }

        public static void StartWatchingMouseState()
        {
            LogDebug($"{ThisStaticType}::{GetCaller()}()");

            if (_mouseStateWatcher == null)
            {
                _mouseStateWatcher = new Timer()
                {
                    AutoReset = true,
                    Interval = 1000 / 60 // Refresh rate equal to render rate, which is 60 frames per second
                };
                _mouseStateWatcher.Elapsed += AsyncRefreshMouseState;
                _mouseStateWatcher.Start();
            }
            else if (!_mouseStateWatcher.Enabled)
                _mouseStateWatcher.Start();
        }

        public static void StopWatchingMouseState()
        {
            _mouseStateWatcher?.Stop();
        }

        private static void AsyncRefreshMouseState(object sender, ElapsedEventArgs e)
        {
            bool leftbuttondown = false;
            bool rightbuttondown = false;
            bool middlebuttondown = false;
            Point mouseposition = new Point();

            App.Current.Dispatcher.Invoke(() =>
            {
                leftbuttondown = Mouse.LeftButton == MouseButtonState.Pressed;
                rightbuttondown = Mouse.RightButton == MouseButtonState.Pressed;
                middlebuttondown = Mouse.MiddleButton == MouseButtonState.Pressed;
                mouseposition = Mouse.GetPosition(App.Current.MainWindow);
            }, System.Windows.Threading.DispatcherPriority.Normal);

            if (_lastMouseButtonState.HasFlag(MouseButtonStates.LeftButtonDown) != leftbuttondown)
            {
                SetLeftButton(leftbuttondown);
                App.Current.Dispatcher.Invoke(() =>
                    LeftMouseButtonChanged.Invoke(null, new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)),
                    System.Windows.Threading.DispatcherPriority.Normal);
            }
            if (_lastMouseButtonState.HasFlag(MouseButtonStates.RightButtonDown) != rightbuttondown)
            {
                SetRightButton(rightbuttondown);
                App.Current.Dispatcher.Invoke(() =>
                    RightMouseButtonChanged.Invoke(null, new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Right)),
                    System.Windows.Threading.DispatcherPriority.Normal);
            }
            if (_lastMouseButtonState.HasFlag(MouseButtonStates.MiddleButtonDown) != middlebuttondown)
            {
                SetMiddleButton(middlebuttondown);
                App.Current.Dispatcher.Invoke(() =>
                    MiddleMouseButtonChanged.Invoke(null, new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Middle)),
                    System.Windows.Threading.DispatcherPriority.Normal);
            }
            if (mouseposition != _lastMousePosition)
            {
                SetMousePosition(mouseposition);
                App.Current.Dispatcher.Invoke(() =>
                    MousePositionChanged.Invoke(null, new MouseEventArgs(Mouse.PrimaryDevice, 0)),
                    System.Windows.Threading.DispatcherPriority.Normal);
            }
        }

        public static void OnLeftMouseButtonChanged(object sender, MouseButtonEventArgs e)
        {
            LogDebug($"{ThisStaticType}::{GetCaller()} - State: {e.ButtonState}");

            if (e.ChangedButton == MouseButton.Left)
            {
                foreach (BaseCategoryItemControl item in BaseCategoryItemControl.CategoryObjectControls)
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                        item.TriggerMouseButtonLeftDown(e);
                    else
                        item.TriggerMouseButtonLeftUp(e);
                }

                foreach (BaseCategoryControl item in BaseCategoryControl.CategoryObjectControls)
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                        item.TriggerMouseButtonLeftDown(e);
                    else
                        item.TriggerMouseButtonLeftUp(e);
                }
            }
        }

        public static void OnRightMouseButtonChanged(object sender, MouseButtonEventArgs e)
        {
            LogDebug($"{ThisStaticType}::{GetCaller()} - State: {e.ButtonState}");

            if (e.ChangedButton == MouseButton.Right)
            {
                foreach (BaseCategoryItemControl item in BaseCategoryItemControl.CategoryObjectControls)
                {
                    if (e.RightButton == MouseButtonState.Pressed)
                        item.TriggerMouseButtonRightDown(e);
                    else
                        item.TriggerMouseButtonRightUp(e);
                }

                foreach (BaseCategoryControl item in BaseCategoryControl.CategoryObjectControls)
                {
                    if (e.RightButton == MouseButtonState.Pressed)
                        item.TriggerMouseButtonRightDown(e);
                    else
                        item.TriggerMouseButtonRightUp(e);
                }
            }
        }

        public static void OnMiddleMouseButtonChanged(object sender, MouseButtonEventArgs e)
        {
            LogDebug($"{ThisStaticType}::{GetCaller()} - State: {e.ButtonState}");

            if (e.ChangedButton == MouseButton.Middle)
            {
                foreach (BaseCategoryItemControl item in BaseCategoryItemControl.CategoryObjectControls)
                {
                    if (e.MiddleButton == MouseButtonState.Pressed)
                        item.TriggerMouseButtonMiddleDown(e);
                    else
                        item.TriggerMouseButtonMiddleUp(e);
                }

                foreach (BaseCategoryControl item in BaseCategoryControl.CategoryObjectControls)
                {
                    if (e.MiddleButton == MouseButtonState.Pressed)
                        item.TriggerMouseButtonMiddleDown(e);
                    else
                        item.TriggerMouseButtonMiddleUp(e);
                }
            }
        }

        public static void OnMousePositionChanged(object sender, MouseEventArgs e)
        {
            // LogDebug($"{ThisStaticType}::{GetCaller()} - Position: {e.GetPosition(App.Current.MainWindow)}");

            // TODO: Increase performance
            foreach (BaseCategoryItemControl item in BaseCategoryItemControl.CategoryObjectControls)
                item.TriggerMouseMove(e);

            foreach (BaseCategoryControl item in BaseCategoryControl.CategoryObjectControls)
                item.TriggerMouseMove(e);
        }
    }
}
