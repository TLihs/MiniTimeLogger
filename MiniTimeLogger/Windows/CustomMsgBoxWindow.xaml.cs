// WINI Tool
// Copyright (c) 2023 Toni Lihs
// Licensed under MIT License

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using static MiniTimeLogger.Support.GUIHandling;
using static MiniTimeLogger.Support.ExceptionHandling;
using static WPFExceptionHandler.ExceptionManagement;

namespace MiniTimeLogger.Windows
{
    /// <summary>
    /// Interaktionslogik für CustomMsgBoxWindow.xaml
    /// </summary>
    public partial class CustomMsgBoxWindow : Window
    {
        public enum CustomMsgBoxResult
        {
            CMBR_Ok,
            CMBR_Cancel,
            CMBR_Yes,
            CMBR_No,
            CMBR_Retry
        }

        public enum CustomMsgBoxButtons
        {
            CMBB_Ok,
            CMBB_OkCancel,
            CMBB_YesNo,
            CMBB_YesNoCancel,
            CMBB_RetryCancel
        }

        private const string BUTTON_OK_TEXT = "OK";
        private const string BUTTON_CANCEL_TEXT = "Cancel";
        private const string BUTTON_YES_TEXT = "Yes";
        private const string BUTTON_NO_TEXT = "No";
        private const string BUTTON_RETRY_TEXT = "Retry";

        private CustomMsgBoxButtons _availableButtons;

        public new CustomMsgBoxResult DialogResult { get; private set; }
        
        private CustomMsgBoxWindow(CustomMsgBoxButtons buttons, string title, string message)
        {
            DialogResult = CustomMsgBoxResult.CMBR_Cancel;
            _availableButtons = buttons;
            
            InitializeComponent();

            Title = title;
            TextBlock_Message.Text = message;
            SetupButtons();
        }

        public static CustomMsgBoxResult Show(string message, string title = null, CustomMsgBoxButtons buttons = CustomMsgBoxButtons.CMBB_Ok)
        {
            CustomMsgBoxWindow window = new CustomMsgBoxWindow(buttons, string.IsNullOrWhiteSpace(title) ? "Info" : title, message);
            return window.ShowDialog();
        }

        public static void ShowCritical(Exception exception)
        {
            CustomMsgBoxWindow window = new CustomMsgBoxWindow(CustomMsgBoxButtons.CMBB_Ok, GetHRToMessage(exception.HResult),
                exception.ToString() + "\r\n\nApplication will be closed.");
            // We ignore any result, since the ShowCritical just shows an information, after which we close the application.
            window.ShowDialog();
        }

        public static void ShowCritical(string message)
        {
            CustomMsgBoxWindow window = new CustomMsgBoxWindow(CustomMsgBoxButtons.CMBB_Ok, "Critical Error",
                message + "\r\n\nApplication will be closed.");
            // We ignore any result, since the ShowCritical just shows an information, after which we close the application.
            window.ShowDialog();
        }

        public static void ShowError(string message)
        {
            CustomMsgBoxWindow window = new CustomMsgBoxWindow(CustomMsgBoxButtons.CMBB_Ok, "Error", message);
            // We ignore any result, since the ShowError just shows an information, after which we just continue.
            window.ShowDialog();
        }

        private new CustomMsgBoxResult ShowDialog()
        {
            base.ShowDialog();
            return DialogResult;
        }

        private void SetupButtons()
        {
            switch (_availableButtons)
            {
                case CustomMsgBoxButtons.CMBB_OkCancel:
                    Button_OkNoCancel.Content = BUTTON_CANCEL_TEXT;
                    SetButtonState(Button_OkYesNoRetry, true);

                    Button_OkYesNoRetry.Content = BUTTON_OK_TEXT;
                    SetButtonState(Button_OkYesNoRetry, true);

                    Button_Yes.Content = string.Empty;
                    SetButtonState(Button_Yes, false);

                    break;

                case CustomMsgBoxButtons.CMBB_YesNo:
                    Button_OkNoCancel.Content = BUTTON_NO_TEXT;
                    SetButtonState(Button_OkYesNoRetry, true);

                    Button_OkYesNoRetry.Content = BUTTON_YES_TEXT;
                    SetButtonState(Button_OkYesNoRetry, true);

                    Button_Yes.Content = string.Empty;
                    SetButtonState(Button_Yes, false);

                    break;

                case CustomMsgBoxButtons.CMBB_YesNoCancel:
                    Button_OkNoCancel.Content = BUTTON_CANCEL_TEXT;
                    SetButtonState(Button_OkYesNoRetry, true);

                    Button_OkYesNoRetry.Content = BUTTON_NO_TEXT;
                    SetButtonState(Button_OkYesNoRetry, true);

                    Button_Yes.Content = BUTTON_YES_TEXT;
                    SetButtonState(Button_Yes, true);

                    break;

                case CustomMsgBoxButtons.CMBB_RetryCancel:
                    Button_OkNoCancel.Content = BUTTON_CANCEL_TEXT;
                    SetButtonState(Button_OkYesNoRetry, true);

                    Button_OkYesNoRetry.Content = BUTTON_RETRY_TEXT;
                    SetButtonState(Button_OkYesNoRetry, true);

                    Button_Yes.Content = string.Empty;
                    SetButtonState(Button_Yes, false);

                    break;

                default:
                    Button_OkNoCancel.Content = BUTTON_OK_TEXT;
                    SetButtonState(Button_OkYesNoRetry, true);

                    Button_OkYesNoRetry.Content = string.Empty;
                    SetButtonState(Button_OkYesNoRetry, false);

                    Button_Yes.Content = string.Empty;
                    SetButtonState(Button_Yes, false);

                    break;
            }
        }

        private void Button_Yes_Click(object sender, RoutedEventArgs e)
        {
            switch (_availableButtons)
            {
                case CustomMsgBoxButtons.CMBB_YesNoCancel:
                    DialogResult = CustomMsgBoxResult.CMBR_Yes;
                    Close();
                    break;

                default:
                    LogGenericError("CustomMsgBoxWindow::Button_Yes_Click([object], [RoutedEventArgs]) - Invalid button clicked.");
                    break;
            }
        }

        private void Button_OkYesNoRetry_Click(object sender, RoutedEventArgs e)
        {
            switch (_availableButtons)
            {
                case CustomMsgBoxButtons.CMBB_OkCancel:
                    DialogResult = CustomMsgBoxResult.CMBR_Ok;
                    Close();
                    break;

                case CustomMsgBoxButtons.CMBB_YesNo:
                    DialogResult = CustomMsgBoxResult.CMBR_Yes;
                    Close();
                    break;

                case CustomMsgBoxButtons.CMBB_YesNoCancel:
                    DialogResult = CustomMsgBoxResult.CMBR_No;
                    Close();
                    break;

                case CustomMsgBoxButtons.CMBB_RetryCancel:
                    DialogResult = CustomMsgBoxResult.CMBR_Retry;
                    Close();
                    break;

                default:
                    LogGenericError("CustomMsgBoxWindow::Button_OkYesNoRetry_Click([object], [RoutedEventArgs]) - Invalid button clicked.");
                    break;
            }
        }

        private void Button_OkNoCancel_Click(object sender, RoutedEventArgs e)
        {
            switch (_availableButtons)
            {
                case CustomMsgBoxButtons.CMBB_OkCancel | CustomMsgBoxButtons.CMBB_YesNoCancel | CustomMsgBoxButtons.CMBB_RetryCancel:
                    DialogResult = CustomMsgBoxResult.CMBR_Cancel;
                    Close();
                    break;

                case CustomMsgBoxButtons.CMBB_YesNo:
                    DialogResult = CustomMsgBoxResult.CMBR_No;
                    Close();
                    break;

                default:
                    DialogResult = CustomMsgBoxResult.CMBR_Ok;
                    Close();
                    break;
            }
        }
    }
}
