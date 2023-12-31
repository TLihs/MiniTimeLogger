﻿using MiniTimeLogger.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using static MiniTimeLogger.Support.ExceptionHandling;
using static MiniTimeLogger.Input.MouseHandling;

namespace MiniTimeLogger
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Create(true);
            InitalizeMouseHandling();
            StartWatchingMouseState();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            StopWatchingMouseState();

            int count = CategoryItem.CategoryObjects.Count;
            for (int i = 0; i < count; i++)
                CategoryItem.CategoryObjects[i] = null;

            base.OnExit(e);
        }
    }
}
