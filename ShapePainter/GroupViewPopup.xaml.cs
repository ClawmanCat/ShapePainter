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
        private DecoratedObject decorateobject;


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
        public string textTop
        {
            get
            {
                if (TextTop == null) return string.Empty;
                return TextTop.Text;
            }
            set
            {
                //knop die de selected ding omzet naar decoratedobject

                MessageBox.Show(TextTop.Text);
                var topText = TextTop.Text;
                decorateobject.ornamentShape(topText);

                //add text to shape
                MainWindow.instance.ForceRebuildGroupView();
            }
        }
        public bool textTop_locked
        {
            get { return !(obj is Shape); }
        }
    }
}