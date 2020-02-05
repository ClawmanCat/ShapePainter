using ShapePainter.Shapes;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShapePainter {
    public partial class MainWindow : Window {
        private Dictionary<Group, List<Shape>> shapes = new Dictionary<Group, List<Shape>>();


        public MainWindow() {
            InitializeComponent();

            Add(Shapes.Shapes.CloneShape(Shapes.Shapes.Ellipse), Group.Global);
        }


        public void Add(Shape shape, Group group) {
            if (!shapes.ContainsKey(group)) shapes.Add(group, new List<Shape>());

            shapes[group].Add(shape);
            Canvas.Children.Add(shape);
        }


        public void Remove(Shape shape, Group group) {
            shapes[group].Remove(shape);
            Canvas.Children.Remove(shape);
        }


        public void Clear(object sender, EventArgs e) {
            shapes.Clear();

            foreach (var group in shapes) {
                foreach (var shape in group.Value) Remove(shape, group.Key);
            }
        }
        public void New(object sender, EventArgs e)
        {

        }
        public void Save(object sender, EventArgs e)
        {

        }
        public void Open(object sender, EventArgs e)
        {

        }
        public void AddEllipse(object sender, EventArgs e)
        {

        }
            public void AddRectangle(object sender, EventArgs e)
            {

            }
        public void AddOrnament(object sender, EventArgs e)
        {

        }

        protected override void OnRender(DrawingContext ctx) {

        }
    }
}
