using ShapePainter.Shapes;
using ShapePainter.Utility.Command;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;
using System.Windows.Controls;
using System.Windows.Input;
using System.Reflection;
using System.Diagnostics;
using ShapePainter.Shapes.Canvas.Visitors;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace ShapePainter
{
    public partial class MainWindow : Window
    {
        private Dictionary<Group, List<CanvasShape>> objects = new Dictionary<Group, List<CanvasShape>>();
        private List<CanvasObject> selection = null;

        private Point? mouseDownPos = null;
        private CanvasShape selectionRect = null;

        private List<ICanvasCommand> history = new List<ICanvasCommand>();

        public MainWindow()
        {
            InitializeComponent();

            Add(Group.Global);

            //// Test shapes
            //Add(new CanvasShape(
            //    CloneShape.Clone(PlatonicForms.Ellipse),
            //    Group.Global,
            //    new Point(30, 30)
            //));

            //Add(new CanvasShape(
            //    CloneShape.Clone(PlatonicForms.Rectangle),
            //    Group.Global,
            //    new Point(500, 300)
            //));

            //Add(new CanvasShape(
            //    CloneShape.Clone(PlatonicForms.Ellipse),
            //    Group.Global,
            //    new Point(750, 700)
            //));
        }


        public void Add(CanvasObject obj)
        {
            // TODO: if selection contains ancestor, add this to selection.
            var visitor = new CanvasObjectGenericVisitor(
                (Group group) =>
                {
                    objects.Add(group, new List<CanvasShape>());
                },
                (CanvasShape shape) =>
                {
                    objects[(Group)shape.parent].Add(shape);
                    ((Group)shape.parent).add(shape);

                    Canvas.Children.Add(shape.shape);
                    Canvas.SetLeft(shape.shape, shape.position.X);
                    Canvas.SetTop(shape.shape, shape.position.Y);
                },
                true
            );

            obj.accept(visitor);


            this.InvalidateVisual();
            Canvas.InvalidateVisual();
        }


        public void Remove(CanvasObject obj)
        {
            // TODO: If selection contains this or contains ancestor remove this from selection.
            var visitor = new CanvasObjectGenericVisitor(
                (Group group) =>
                {
                    objects.Remove(group);
                },
                (CanvasShape shape) =>
                {
                    if (objects.ContainsKey((Group)shape.parent))
                    {
                        objects[(Group)shape.parent].Remove(shape);
                    }

                    Canvas.Children.Remove(shape.shape);
                },
                true
            );

            obj.accept(visitor);


            this.InvalidateVisual();
            Canvas.InvalidateVisual();
        }


        public void Clear(object sender = null, EventArgs e = null)
        {
            foreach (var pair in objects)
            {
                foreach (var shape in pair.Value)
                {
                    var visitor = new CanvasObjectGenericVisitor(
                        (Group g) => { },
                        (CanvasShape s) => { Canvas.Children.Remove(s.shape); },
                        false
                    );
                }
            }

            objects.Clear();
        }
        public void New(object sender, EventArgs e)
        {
        }
        private void Save(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Json file(*json)|*.json| JPG file (*.jpg)|*.jpg| PNG file(*.png)|*.png";
            if (saveFileDialog.ShowDialog() == true)
            {
                Rect rect = new Rect(Canvas.RenderSize);

                RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)rect.Right, (int)rect.Bottom, 96d, 96d, PixelFormats.Pbgra32);

                Canvas.Measure(new Size((int)Canvas.Width, (int)Canvas.Height));
                Canvas.Arrange(new Rect(new Size((int)Canvas.Width, (int)Canvas.Height)));

                renderTargetBitmap.Render(Canvas);

                string fileName = saveFileDialog.FileName;
                var fileExtension = Path.GetExtension(saveFileDialog.FileName);
                switch (fileExtension.ToLower())
                {
                    case ".json":
                        SaveToJSON(renderTargetBitmap, fileName);
                        break;
                    case ".jpg":
                        SaveToJPG(renderTargetBitmap, fileName);
                        break;
                    case ".png":  
                        SaveToPNG(renderTargetBitmap, fileName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(fileExtension);
                }
            }
        }
        public void SaveToJSON(RenderTargetBitmap renderTargetBitmap, string fileName)
        {
            CanvasObjectPrintVisitor v = new CanvasObjectPrintVisitor();
            Group.Global.accept(v);
            String textToPrint = v.getJSON();

            File.WriteAllText(fileName, textToPrint);
        }
        public void SaveToJPG(RenderTargetBitmap renderTargetBitmap, string fileName)
        {
            JpegBitmapEncoder jpgEncoder = new JpegBitmapEncoder();
            jpgEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            using (FileStream file = File.Create(fileName))
            {
                jpgEncoder.Save(file);
            }
        }
        public void SaveToPNG(RenderTargetBitmap renderTargetBitmap, string fileName)
        {
            PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            using (FileStream file = File.Create(fileName))
            {
                pngEncoder.Save(file);
            }
        }
        public void Open(object sender, EventArgs e)
        {

            OpenFileDialog openfileDialog = new OpenFileDialog();

            openfileDialog.Title = "Select a file";
            openfileDialog.Filter = "Json file(*json)|*.json| JPG file (*.jpg)|*.jpg| PNG file(*.png)|*.png";
            if (openfileDialog.ShowDialog() == true)
            {
                var filextension = Path.GetExtension(openfileDialog.FileName);

                switch (filextension.ToLower())
                {
                    case ".json":
                        OpenJSON(openfileDialog);
                        break;
                    case ".jpg":
                        OpenJPG(openfileDialog);
                        break;
                    case ".png":
                        OpenPNG(openfileDialog);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(filextension);
                }
            }
        }
        public static long CountLinesJSON(StreamReader r, string f)
        {
            long count = 0;
            using (r = new StreamReader(f))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    count++;
                }
            }
            return count;
        }
        public void OpenJSON(OpenFileDialog openfileDialog)
        {
            if (openfileDialog.FileName.Trim() != string.Empty)
            {
                using (StreamReader r = new StreamReader(openfileDialog.FileName))
                {
                    string json = r.ReadToEnd();
                    //var searchShape = json.Split('[')[1];

                    //var foundShape = searchShape.Split(',')[0];
                    //string[] shapeSplit = foundShape.Split(' ');/*gives ellipse shape string*/
                    //double posx = Convert.ToDouble(shapeSplit[1]);/*gives posx of ellipse*/
                    //double posy = Convert.ToDouble(shapeSplit[2]);

                    //MessageBox.Show("x = " + posx);
                    //MessageBox.Show("y = " + posy);

                    long fileLength = CountLinesJSON(r, openfileDialog.FileName);
                    //while?
                    for (int index = 1; index < fileLength -2; index++) {
                        int count =  1;
                        count += 1;
                        var searchShape = json.Split('[')[count -1];

                        var foundShape = searchShape.Split(',')[count];
                        string[] shapeSplit = foundShape.Split(' ');/*gives ellipse shape string*/

                        string shapename = shapeSplit[index - 1].Replace("':", "");
                            MessageBox.Show(shapename);

                            string[] shapeThing = foundShape.Split(' ');
                            double x = Convert.ToDouble(shapeThing[index]);
                            double y = Convert.ToDouble(shapeThing[index]);

                            Add(new CanvasShape(
                            CloneShape.Clone(PlatonicForms.Ellipse),
                            Group.Global,
                            new Point(x, y)
                         ));
                    }


                    //add to list / canvas

                }
            }
        }
        public void OpenPNG(OpenFileDialog openfileDialog)
        {
            var pngPath = openfileDialog.FileName;
            Canvas.Background = new ImageBrush(new BitmapImage(new Uri(pngPath)));
        }
        public void OpenJPG(OpenFileDialog openfileDialog)
        {
            var jpgPath = openfileDialog.FileName;
            Canvas.Background = new ImageBrush(new BitmapImage(new Uri(jpgPath)));
        }
        public void Select(Point min, Point max)
        {
            Deselect();

            List<CanvasObject> result = new List<CanvasObject>();

            foreach (var group in objects)
            {
                foreach (var shape in group.Value)
                {
                    // Select if the center is within the select area.
                    if (
                        shape.position.X >= min.X && shape.position.X <= max.X &&
                        shape.position.Y >= min.Y && shape.position.Y <= max.Y
                        ) result.Add(shape);
                }
            }
            selection = result;

            // Make selected elements red.
            foreach (var shape in result) ((CanvasShape)shape).shape.Stroke = Brushes.Red;
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

        public void Select(Group group)
        {
            Deselect();

            selection = new List<CanvasObject> { group };

            // Make selected elements red.
            foreach (var shape in objects[group]) shape.shape.Stroke = Brushes.Red;
        }


        public void Deselect()
        {
            if (selection == null) return;

            // Turn previously selected elements back to being black.
            foreach (var obj in selection)
            {
                if (obj is CanvasShape) ((CanvasShape)obj).shape.Stroke = Brushes.Black;
            }

            selection = null;
        }


        public List<CanvasShape> GetSelection()
        {
            List<CanvasShape> result = new List<CanvasShape>();
            if (selection == null) return result;

            var visitor = new CanvasObjectGenericVisitor(
                (Group group) => { },
                (CanvasShape shape) => { result.Add(shape); },
                true
            );

            return result;
        }


        private void HandleMouseDown(object sender, MouseButtonEventArgs args)
        {
            mouseDownPos = args.GetPosition(Canvas);

            selectionRect = new CanvasShape(
                CloneShape.Clone(PlatonicForms.SelectionRectangle),
                Group.Global,
                mouseDownPos.Value
            );

            Add(selectionRect);
        }


        private void HandleMouseUp(object sender, MouseButtonEventArgs args)
        {
            if (mouseDownPos == null) return;

            Point mouseUpPos = args.GetPosition(Canvas);
            (Point min, Point max) = Utility.Utility.GetMinMax(mouseDownPos.Value, mouseUpPos);

            Remove(selectionRect);

            Select(min, max);

            mouseDownPos = null;
        }


        private void HandleMouseMove(object sender, MouseEventArgs args)
        {
            if (selectionRect == null || mouseDownPos == null) return;

            Point mouseCurrentPos = Mouse.GetPosition(Canvas);
            (Point min, Point max) = Utility.Utility.GetMinMax(mouseDownPos.Value, mouseCurrentPos);

            Canvas.SetLeft(selectionRect.shape, min.X);
            Canvas.SetTop(selectionRect.shape, min.Y);

            selectionRect.shape.Width = max.X - min.X;
            selectionRect.shape.Height = max.Y - min.Y;
        }
    }
}
