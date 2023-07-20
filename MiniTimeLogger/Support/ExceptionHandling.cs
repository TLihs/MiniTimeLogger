// WINI Tool
// Copyright (c) 2023 Toni Lihs
// Licensed under MIT License

using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using MiniTimeLogger.Windows;
using static WPFExceptionHandler.ExceptionManagement;

namespace MiniTimeLogger.Support
{
    public static class ExceptionHandling
    {
        public enum EXCEPTIONTYPES : uint
        {
            ERR_FUNCTION_NOTIMPLEMENTED = 0x_8000_0000,

            ERR_EH_FILELOGGINGDEACTIVATED = 0x_8000_0011,

            ERR_TYPING_CSTYPEINVALID = 0x_8000_0020,
            ERR_TYPING_INITYPEINVALID = 0x_8000_0021,
            ERR_TYPING_INITYPEINOTIMPLEMENTED = 0x_8000_0022,
        }

        public enum MessageSeverityTypes
        {
            MS_INFO,
            MS_WARNING,
            MS_ERROR
        }

        public static bool UseFileLogging => EHUseFileLogging;
        public static string LogFilePath
        {
            get
            {
                if (!EHUseFileLogging)
                    Log(EXCEPTIONTYPES.ERR_EH_FILELOGGINGDEACTIVATED, string.Empty);
                return EHUseFileLogging ? EHExceptionLogFilePath : string.Empty;
            }
            set
            {
                if (!EHUseFileLogging)
                    Log(EXCEPTIONTYPES.ERR_EH_FILELOGGINGDEACTIVATED, string.Empty);
                else
                    SetAlternativeLogFilePath(value);
            }
        }

        static ExceptionHandling()
        {
            ExceptionCaught += OnExceptionCaught;
        }

        private static void OnExceptionCaught(object sender, ExceptionCaughtEventArgs e)
        {
            if (e.IsHandled)
                return;

            if (e.IsCritical)
            {
                if (e.Exception != null)
                    CustomMsgBoxWindow.ShowCritical("Application encountered a critical error and will be closed\r\n\nSee log for further information.");
                else
                    CustomMsgBoxWindow.ShowError(e.Message + "\r\n\nClosing application.");
            }
            else
                CustomMsgBoxWindow.ShowError(e.Message);
        }

        public static void Create(bool useFileLogging, string logPath = "")
        {
            Debug.Print("Initializing ExceptionHandling...");
            CreateExceptionManagement(App.Current, AppDomain.CurrentDomain, true, useFileLogging);
            if (!string.IsNullOrEmpty(logPath))
                SetAlternativeLogFilePath(logPath);
            Debug.Print("ExceptionHandling initialized.");
        }

        public static string ExceptionToString(EXCEPTIONTYPES exceptionType)
        {
            switch (exceptionType)
            {
                case EXCEPTIONTYPES.ERR_FUNCTION_NOTIMPLEMENTED:
                    return "Function not implemented.";

                case EXCEPTIONTYPES.ERR_EH_FILELOGGINGDEACTIVATED:
                    return "Logging to file not active.";

                default:
                    return string.Format("<{0}>", Enum.GetName(typeof(EXCEPTIONTYPES), exceptionType));
            }
        }

        public static void Log(EXCEPTIONTYPES type, string message)
        {
            EHLogGenericError("{0} ({1})", ExceptionToString(type), message);
        }

        public static void Log(LogEntryType type, string message)
        {
            switch (type)
            {
                case LogEntryType.LE_WARNING:
                    EHLogWarning(message);
                    break;

                case LogEntryType.LE_ERROR_GENERIC:
                    EHLogGenericError(message);
                    break;

                case LogEntryType.LE_ERROR_CRITICAL:
                    EHLogCriticalError(message);
                    break;

                default:
                    EHLogDebug(message);
                    break;
            }
        }

        public static void LogDebug(string message, params string[] formatParameters) => EHLogDebug(message, formatParameters);
        public static void LogWarning(string message, params string[] formatParameters) => EHLogWarning(message, formatParameters);
        public static void LogGenericError(string message, params string[] formatParameters) => EHLogGenericError(message, formatParameters);
        public static void LogGenericError(Exception exception) => EHLogGenericError(exception);
        public static void LogCriticalError(Exception exception) => EHLogCriticalError(exception);

        public static void MsgBox(MessageSeverityTypes type, string message, params string[] formatParameters)
        {
            MsgBox(type, message, null, formatParameters);
        }
        public static void MsgBox(MessageSeverityTypes type, string message, Window owner, params string[] formatParameters)
        {
            LogEntryType logtype;

            switch (type)
            {
                case MessageSeverityTypes.MS_WARNING:
                    logtype = LogEntryType.LE_WARNING;
                    break;
                case MessageSeverityTypes.MS_ERROR:
                    logtype = LogEntryType.LE_ERROR_GENERIC;
                    break;
                default:
                    logtype = LogEntryType.LE_INFO;
                    break;
            }

            // TODO: Change to CustomMsgBoxControl
            EHMsgBox(logtype, message, owner, formatParameters);
        }
        public static MessageBoxResult MsgBoxYesNo(string message, params string[] formatParameters) => EHMsgBoxYesNo(message, null, formatParameters);
        public static MessageBoxResult MsgBoxYesNo(string message, Window owner, params string[] formatParameters) => EHMsgBoxYesNo(message, owner, formatParameters);
    }
}
