using Microsoft.Win32;
using ShapePainter.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
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
        private void Save(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JPG file (*.jpg)|*.jpg| PNG file(*.png)|*.png";
            if (saveFileDialog.ShowDialog() == true)
            {
                Rect rect = new Rect(Canvas.RenderSize);

                RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)rect.Right, (int)rect.Bottom, 96d, 96d, PixelFormats.Pbgra32);

                Canvas.Measure(new Size((int)Canvas.Width, (int)Canvas.Height));
                Canvas.Arrange(new Rect(new Size((int)Canvas.Width, (int)Canvas.Height)));

                renderBitmap.Render(Canvas);

                string filename = saveFileDialog.FileName;
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                using (FileStream file = File.Create(filename))
                {
                    encoder.Save(file);
                }
            }
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
