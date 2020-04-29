using ShapePainter.Shapes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ShapePainter {
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            this.MainWindow = ShapePainter.MainWindow.instance;
            ShapePainter.MainWindow.instance.Init();

            this.MainWindow.Show();
        }
    }
}
