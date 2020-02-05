using Microsoft.Win32;
using ShapePainter.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;

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

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)rect.Right, (int)rect.Bottom, 96d, 96d, PixelFormats.Pbgra32);

            Canvas.Measure(new Size((int)Canvas.Width, (int)Canvas.Height));
            Canvas.Arrange(new Rect(new Size((int)Canvas.Width, (int)Canvas.Height)));

            renderTargetBitmap.Render(Canvas);

            string fileName = saveFileDialog.FileName;
            var filextension = Path.GetExtension(saveFileDialog.FileName);
            switch (filextension.ToLower())
            {
                case ".jpg":
            JpegBitmapEncoder jpgEncoder = new JpegBitmapEncoder();
                        jpgEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

                        using (FileStream file = File.Create(fileName))
                        {
                            jpgEncoder.Save(file);
                        }
                        break;
                case ".png":
                    PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
                        pngEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

                    using (FileStream file = File.Create(fileName))
                    {
                            pngEncoder.Save(file);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(filextension);
            }       
        }
    }
    public void Open(object sender, EventArgs e)
    {

            OpenFileDialog openfileDialog = new OpenFileDialog();
            string directory = System.IO.Path.GetDirectoryName(openfileDialog.FileName);

            openfileDialog.Title = "Select a file";
            openfileDialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (openfileDialog.ShowDialog() == true)
            {
                Canvas.Background = new ImageBrush(new Uri(directory, openfileDialog.FileName));
            }
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
