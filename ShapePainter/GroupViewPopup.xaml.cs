using ShapePainter.Shapes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShapePainter
{
    public partial class GroupViewPopup : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ICanvasObject obj;


        public string name {
            get {
                return obj is Group ? ((Group) obj).name : ((Shape) obj).shape.GetType().Name;
            }
            set
            {
                ((Group)obj).name = value;
                OnPropertyChanged("name");
                MainWindow.instance.ForceRebuildGroupView();
            }
        }


        public bool name_locked
        {
            get { return !(obj is Group); }
        }


        public GroupViewPopup(ICanvasObject obj)
        {
            this.obj = obj;

            InitializeComponent();
        }


        private void OnPropertyChanged(string name) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}