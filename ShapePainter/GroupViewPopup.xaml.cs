using ShapePainter.Shapes;
using ShapePainter.Utility;
using ShapePainter.Visitor;
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

namespace ShapePainter {
    public partial class GroupViewPopup : Window, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        
        private ICanvasObject obj;


        public string name {
            get {
                string name = "";

                var visitor = new GenericVisitor(
                    (Group group) => {
                        name = group.name;
                    },
                    (Shape shape) => {
                        name = shape.shape.GetType().Name;
                    },
                    false
                );

                obj.accept(visitor);
                return name;
            }
            set {
                var visitor = new GenericVisitor(
                    (Group group) => {
                        group.name = value;
                    },
                    (Shape shape) => {},
                    false
                );

                obj.accept(visitor);

                OnPropertyChanged("name");
                MainWindow.instance.ForceRebuildGroupView();
            }
        }


        public string ornament {
            get {
                DecoratedShape.Side side = GetSideForSelectedPicker(OrnamentPicker.SelectedItem);

                DecoratedShape decorated = obj as DecoratedShape;
                return decorated?.ornaments[Enum.GetValues(typeof(DecoratedShape.Side)).IndexOf(side)] ?? "";
            }
            set {
                DecoratedShape.Side side = GetSideForSelectedPicker(OrnamentPicker.SelectedItem);

                DecoratedShape decorated = obj as DecoratedShape;
                if (decorated == null) {
                    MainWindow.instance.RemoveCanvasObject(obj);

                    decorated = new DecoratedShape(obj);
                    obj = decorated;

                    MainWindow.instance.AddCanvasObject(decorated);
                }

                decorated.ornaments[Enum.GetValues(typeof(DecoratedShape.Side)).IndexOf(side)] = value;

                OnPropertyChanged("ornament");

                MainWindow.instance.ForceRebuildGroupView();
                MainWindow.instance.Invalidate();
            }
        }


        private DecoratedShape.Side GetSideForSelectedPicker(object picked) {
            DecoratedShape.Side side = DecoratedShape.Side.LEFT;

            if (picked == PickerLeft)   side = DecoratedShape.Side.LEFT;
            if (picked == PickerRight)  side = DecoratedShape.Side.RIGHT;
            if (picked == PickerTop)    side = DecoratedShape.Side.TOP;
            if (picked == PickerBottom) side = DecoratedShape.Side.BOTTOM;

            return side;
        }


        public bool name_locked {
            get { return !(obj is Group); }
        }


        public GroupViewPopup(ICanvasObject obj) {
            this.obj = obj;

            InitializeComponent();
        }


        private void OnPropertyChanged(string name) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        private void OnSelectedOrnamentChanged(object sender, SelectionChangedEventArgs e) {
            OnPropertyChanged("ornament");

            MainWindow.instance.ForceRebuildGroupView();
            MainWindow.instance.Invalidate();
        }
    }
}
