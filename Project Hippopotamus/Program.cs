﻿using System;

namespace Hippopotamus.Engine.Core
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            GameEngine.Launch<Game>();
        }
    }
}
