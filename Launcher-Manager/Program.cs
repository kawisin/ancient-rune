using DotNetEnv;
using Launcher_Manager.DB;

namespace Launcher_Manager
{
    
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var envPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"../../../.env"));
            Env.Load(envPath);
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            LauncherService.ListenToLaunchQueue();
            Application.Run(new Form1());
        }
    }
}