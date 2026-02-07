namespace tfd
{
    using tfd.Properties;
    using System;
    using System.Windows.Forms;
    using System.Threading.Tasks;
    using System.IO;

    public static class Program
    {
        private static NotifyIcon trayIcon;

        [STAThread]
        public static void Main()
        {
            EnvConfig.LoadVariables();

            if (!string.IsNullOrEmpty(EnvConfig.tfd_ExceptionLogFilePath))
            {
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += (s, e) => Program.LogUnhandledExceptions(e.Exception);
                AppDomain.CurrentDomain.UnhandledException += (s, e) => Program.LogUnhandledExceptions(e.ExceptionObject as Exception);
                TaskScheduler.UnobservedTaskException += (sender, e) => Program.LogUnhandledExceptions(e.Exception);
            }

            if (EnvConfig.tfd_IsProcessDPIAware) win32.SetProcessDPIAware();

            Program.trayIcon = new NotifyIcon()
            {
                Visible = true,
                Icon = Resources.tray_icon,
                ContextMenu = new ContextMenu(new MenuItem[]
                {
                    new MenuItem(EnvConfig.tfd_TrayMenuClearLogsText, (s,e) => (Logger.Instance as Logger).Clear()),
                    new MenuItem(EnvConfig.tfd_TrayMenuCopyLogsText, (s,e) => Clipboard.SetText((Logger.Instance as Logger).GetLogsAsString())),
                    new MenuItem(EnvConfig.tfd_TrayMenuExitText, (s,e) => Application.Exit()),
                }),
            };

            Application.Run(new AppForm());
            Program.trayIcon.Dispose();
        }

        private static void LogUnhandledExceptions(Exception exception)
        {
            if (exception != null) Logger.Instance.Error(exception.ToString());

            string allLogs = (Logger.Instance as Logger).GetLogsAsString();
            string exceptionStr =
                $"{DateTime.Now:u} ---------------{Environment.NewLine}"
                + $"{allLogs}{Environment.NewLine}{Environment.NewLine}";

            File.AppendAllText(EnvConfig.tfd_ExceptionLogFilePath, exceptionStr);
        }
    }
}