﻿using ShapePainter.Shapes;
using ShapePainter.Shapes.Canvas;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Path = System.IO.Path;
using System.Windows.Controls;
using System.Windows.Input;
using ShapePainter.Shapes.Canvas.Visitors;
using System.Linq;

namespace ShapePainter
{
    public partial class MainWindow : Window {
        private enum MouseState { NONE, SELECTING, MOVING, ADDING_SHAPE }

        public List<CanvasObject> objects { get; set; }
        public List<CanvasObject> selection { get; set; }

        private List<ICanvasCommand> history = new List<ICanvasCommand>();

        private MouseState mouseState;
        private Vector? mouseDownPos = null, mousePrevPos = null;
        private CanvasShape selectionRectangle = null;

        // While we update the screen every time the mouse is moved, we only want to record the begin- and endstates
        // in the history.
        // To achieve this we keep a temporary command to record the begin and endstates in.
        ICanvasCommand recording;

        private Dictionary<Key, bool> keyboard = new Dictionary<Key, bool>();

        private bool rectangleButton = false;
        private bool ellipseButton = false;
        public MainWindow() {
            InitializeComponent();

            this.objects = new List<CanvasObject>();
            this.selection = new List<CanvasObject>();

            List<CanvasObject> initial = new List<CanvasObject> {
                Group.Global
            };

            RunCommand(new CompoundCommand(initial.Select((CanvasObject o) => new AddRemoveCommand(o, AddRemoveCommand.Mode.ADD))));

        }


        public void RunCommand(ICanvasCommand command) {
            command.doCommand(this);
            history.Add(command);
        }


        public bool IsKeyDown(Key k) {
            return keyboard.ContainsKey(k) && keyboard[k];
        }


        private void SetKeyDown(Key k, bool down) {
            keyboard[k] = down;
        }


        public void AddObject(CanvasObject obj) {
            this.objects.Add(obj);

            var visitor = new CanvasObjectGenericVisitor(
                (Group group) => { },
                (CanvasShape shape) => {
                    Canvas.Children.Add(shape.shape);
                    InvalidateObject(shape);
                },
                false
            );

            obj.accept(visitor);
            Invalidate();
        }


        public void RemoveObject(CanvasObject obj) {
            this.objects.Remove(obj);
            this.selection.Remove(obj);

            var visitor = new CanvasObjectGenericVisitor(
                (Group group) => { },
                (CanvasShape shape) => {
                    Canvas.Children.Remove(shape.shape);
                },
                true
            );

            obj.accept(visitor);
            Invalidate();
        }




        public List<CanvasObject> GetSelectedObjects() {
            List<CanvasObject> result = new List<CanvasObject>();

            foreach (CanvasObject o in objects) {
                if (selection.Any((CanvasObject s) => o.ancestor(s, true))) result.Add(o);
            }

            return result;
        }
        private void Invalidate()
        {
            this.InvalidateVisual();
        }

        public void InvalidateObject(CanvasObject obj)
        {
            var visitor = new CanvasObjectGenericVisitor(
                (Group group) => { },
                (CanvasShape shape) => {
                    Canvas.SetLeft(shape.shape, shape.position.X);
                    Canvas.SetTop(shape.shape, shape.position.Y);
                },
                true
            );

            obj.accept(visitor);
            Invalidate();
        }
        public void SelectRegion(Vector a, Vector b, bool overwrite = true)
        {
            List<CanvasObject> result = new List<CanvasObject>();

            Vector min = new Vector(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
            Vector max = new Vector(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));

            foreach (CanvasObject o in objects)
            {
                if (
                    o.position.X >= min.X && o.position.X < max.X &&
                    o.position.Y >= min.Y && o.position.Y < max.Y
                ) result.Add(o);
            }

            SelectObjects(result, overwrite);
        }


        #region Selection
        public void SelectObjects(IEnumerable<CanvasObject> objects, bool overwrite = true)
        {
            selection.Remove(selectionRectangle);   // Can't select the selection rectangle.

            if (overwrite) ClearSelection();

            var visitor = new CanvasObjectGenericVisitor(
                (Group group) => { },
                (CanvasShape shape) => { shape.selected = true; },
                true
            );

            foreach (CanvasObject o in objects) o.accept(visitor);

            selection.AddRange(objects);
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
            MessageBox.Show(textToPrint);

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

                    long fileLength = CountLinesJSON(r, openfileDialog.FileName);
                    //while?
                    for (int index = 0; index < fileLength - 3; index++)
                    {

                        var searchShape = json.Split('[')[1];
                        var foundShape = searchShape.Split(',')[index];
                        string[] shapeThing = foundShape.Split(' ');/*gives shape string*/

                        int x = Convert.ToInt32(shapeThing[1]);
                        int y = Convert.ToInt32(shapeThing[2]);

                        string pattern = "[':]";
                        string replacement = "";
                        System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex(pattern);
                        string shapename = rgx.Replace(shapeThing[0], replacement);

                        if (shapename.Contains("Ellipse"))
                        {
                            AddObject(new CanvasShape(
                                CloneShape.Clone(PlatonicForms.Ellipse),
                                Group.Global,
                                new Vector(x, y)
                            ));
                        }
                        else if(shapename.Contains("Rectangle"))
                        {
                            AddObject(new CanvasShape(
                               CloneShape.Clone(PlatonicForms.Rectangle),
                               Group.Global,
                               new Vector(x,y)
                           ));
                        }
                    }
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


        public void ClearSelection() {
            var visitor = new CanvasObjectGenericVisitor(
                (Group group) => { },
                (CanvasShape shape) => { shape.selected = false; },
                true
            );

            foreach (CanvasObject o in selection) o.accept(visitor);

            this.selection.Clear();
        }
        public IReadOnlyList<CanvasObject> GetSelection() {
            return selection;
        }
        #endregion Selection
        #region PageHandlers
        public void ClearCanvas(object sender = null, EventArgs args = null) {
            foreach (var o in objects) RemoveObject(o);
            this.objects.Clear();
            
            this.ClearSelection();
        }
        public void Undo(object sender, EventArgs e)
        {
            //watch history list
            if (history.Count > 0)
            {
                var lastItem = history[history.Count - 1];
            }
        }
        public void Redo(object sender, EventArgs e)
        {
            if (history.Count > 0)
            {
                var lastItem = history[history.Count - 1];
            }
        }
        //add shape when clicked button
        //public void addShapesClick(object sender, MouseEventArgs e)
        //{
        //    if (rectangleButton == true)
        //    {
        //        MessageBox.Show("left button");

        //        AddRectangle(sender, e);
        //    }
        //    else if(ellipseButton == true)
        //    {
        //        AddEllipse(sender, e);
        //    }
        //}
        public void SelectRectangle(object sender, EventArgs e)
        {
            rectangleButton = true;
            ellipseButton = false;
            MessageBox.Show("clicked Rectangle");
        }
        public void SelectEllipse(object sender, EventArgs e)
        {
            ellipseButton = true;
            rectangleButton = false;
            MessageBox.Show("clicked Ellipse");
        }
        public void AddEllipse(object sender, MouseEventArgs e) {
            Point mousepos = Mouse.GetPosition(Canvas);

            AddObject(new CanvasShape(
            CloneShape.Clone(PlatonicForms.Ellipse),
            Group.Global,
            new Vector(mousepos.X, mousepos.Y)
            ));

        } 
        public void AddRectangle(object sender, MouseEventArgs e) {
                Point mousepos = Mouse.GetPosition(Canvas);

                AddObject(new CanvasShape(
                CloneShape.Clone(PlatonicForms.Rectangle),
                Group.Global,
                new Vector(mousepos.X, mousepos.Y)
                ));   
        }
        public void AddOrnament(object sender, EventArgs e) {}

        private void HandleMouseDown(object sender, MouseButtonEventArgs args) {
            this.mouseDownPos = (Vector) args.GetPosition(Canvas);
            this.mousePrevPos = mouseDownPos;

            // Drag to select, Shift + drag to move selection.
            mouseState = (IsKeyDown(Key.LeftShift) || IsKeyDown(Key.RightShift)) ? MouseState.MOVING : MouseState.SELECTING;

            // If selecting, create the selection rectangle.
            if (mouseState == MouseState.SELECTING) {
                this.selectionRectangle = new CanvasShape(
                    CloneShape.Clone(PlatonicForms.SelectionRectangle),
                    Group.Global,
                    mouseDownPos.Value
                );

                AddObject(this.selectionRectangle);

                recording = new SelectCommand();
                ((SelectCommand) recording).RecordStart(selection);
            } else if (mouseState == MouseState.MOVING) {
                recording = new MoveCommand();
                ((MoveCommand) recording).RecordStart(selection);
            }
            else if(mouseState == MouseState.ADDING_SHAPE)
            {
                if (rectangleButton == true)
                {
                    AddRectangle(sender, args);
                }
                else if (ellipseButton == true)
                {
                    AddEllipse(sender, args);
                }
            }
        }


        private void HandleMouseUp(object sender, MouseButtonEventArgs args) {
            if (mouseState == MouseState.SELECTING) {
                RemoveObject(selectionRectangle);
                this.selectionRectangle = null;

                ((SelectCommand) recording).RecordEnd(selection);
            } else if (mouseState == MouseState.MOVING) {
                ((MoveCommand) recording).RecordEnd(selection);
            }

            this.mouseState = MouseState.NONE;
            this.mouseDownPos = null;
            this.mousePrevPos = null;

            if (recording != null) {
                history.Add(recording);
                recording = null;
            }
        }


        private void HandleMouseMove(object sender, MouseEventArgs args) {
            if (mouseState == MouseState.NONE) return;

            Vector mousePos = (Vector) args.GetPosition(Canvas);
            Vector delta = mousePos - mousePrevPos.Value;

            if (mouseState == MouseState.SELECTING) {
                // Update selection rect & select everything inside.
                SelectRegion(mouseDownPos.Value, mousePos);

                var (min, max) = Utility.Utility.GetMinMax(mouseDownPos.Value, mousePos);
                selectionRectangle.position = min;
                selectionRectangle.size = max - min;

                InvalidateObject(selectionRectangle);
                Invalidate();
            } else {
                // Update position of all selected items.
                List<CanvasShape> shapes = new List<CanvasShape>();

                var visitor = new CanvasObjectGenericVisitor(
                    (Group group) => { },
                    (CanvasShape shape) => {
                        shapes.Add(shape);
                    },
                    true
                );

                foreach (var o in selection) o.accept(visitor);


                foreach (var shape in shapes) {
                    shape.position += delta;
                    InvalidateObject(shape);
                }
            }


            this.mousePrevPos = mousePos;
        }


        private void OnKeyDown(object sender, KeyEventArgs e) { SetKeyDown(e.Key, true);  }
        private void OnKeyUp(object sender, KeyEventArgs e)   { SetKeyDown(e.Key, false); }
        #endregion PageHandlers
    }
}
