using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Journal
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string PATH = Directory.GetCurrentDirectory() + "\\journal.db";

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            DB._DBconnectionString = "Data Source=" + PATH + ";Version=3";
            DB.CreateDbIfNotExists(PATH);

            MainWindow main = new MainWindow();
            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            Current.MainWindow = main;
            main.Show();
        }
    }
}
