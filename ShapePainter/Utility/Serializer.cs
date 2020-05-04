using Microsoft.Win32;
using Newtonsoft.Json;
using ShapePainter.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ShapePainter.Utility {
    using WPFShape = System.Windows.Shapes.Shape;

    public static class Serializer {
        public static void SaveBitmap() {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JPEG Image (*.jpg, *.jpeg)|*.jpg;*.jpeg| PNG Image (*.png)|*.png";

            if (dialog.ShowDialog() ?? false) {
                BitmapEncoder encoder = Path.GetExtension(dialog.FileName) == ".png" 
                    ? (BitmapEncoder) new PngBitmapEncoder() 
                    : (BitmapEncoder) new JpegBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create(MainWindow.instance.ToBitmap()));
                
                using (FileStream fs = File.Create(dialog.FileName)) {
                    encoder.Save(fs);
                }
            }
        }


        public static void SaveJSON() {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JSON-Formatted Data (*json)|*.json";

            if (dialog.ShowDialog() ?? false) {
                string json = JsonConvert.SerializeObject(
                    MainWindow.instance.base_node, 
                    Formatting.Indented,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }
                );

                File.WriteAllText(dialog.FileName, json, Encoding.UTF8);
            }
        }


        public static void LoadJSON() {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JSON-Formatted Data (*json)|*.json";

            if (dialog.ShowDialog() ?? false) {
                MainWindow.instance.Reset();

                MainWindow.instance.base_node = JsonConvert.DeserializeObject<Group>(
                    File.ReadAllText(dialog.FileName),
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }
                );

                MainWindow.instance.base_node.revalidate();
                MainWindow.instance.AddCanvasObject(MainWindow.instance.base_node);
            }
        }


        public static Dictionary<string, WPFShape> ShapeDictionary = typeof(PlatonicForms).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(x => x.FieldType.IsSubclassOf(typeof(WPFShape)))
            .ToDictionary(x => x.FieldType.Name, x => (WPFShape) x.GetValue(null));
    }
}
