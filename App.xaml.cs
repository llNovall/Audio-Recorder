using Audio_Recorder.View;
using Audio_Recorder.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Audio_Recorder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow wnd = new MainWindow();
            wnd.DataContext = new RecorderViewModel();

            if (e.Args.Length == 1)
                MessageBox.Show("Now opening file : " + e.Args[0]);

            wnd.Show();
        }
    }
}
