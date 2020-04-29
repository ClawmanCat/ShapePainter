using Microsoft.Win32;
using ShapePainter.Shapes;
using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ShapePainter.Utility
{
    public static class Serializer {
        private static JavaScriptEncoder GetEncoder() {
            TextEncoderSettings settings = new TextEncoderSettings();
            settings.AllowRange(UnicodeRanges.All);

            return JavaScriptEncoder.Create(settings);
        }


        public class CanvasObjectSerializer : JsonConverter<ICanvasObject> {
            public override ICanvasObject Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options) {
                reader.Read();
                string s = reader.GetString();
                
                bool is_group = reader.TokenType == JsonTokenType.PropertyName && s == "name";

                if (is_group) return JsonSerializer.Deserialize<Group>(ref reader, options);
                else return JsonSerializer.Deserialize<Shape>(ref reader, options);
            }

            public override void Write(Utf8JsonWriter writer, ICanvasObject value, JsonSerializerOptions options) {
                if (value is Group) writer.WriteStringValue(JsonSerializer.Serialize((Group) value, options));
                else writer.WriteStringValue(JsonSerializer.Serialize((Shape) value, options));
            }
        }


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
                string json = JsonSerializer.Serialize<ICanvasObject>(
                    MainWindow.instance.base_node, 
                    new JsonSerializerOptions { WriteIndented = true, Encoder = GetEncoder() }
                );

                File.WriteAllText(dialog.FileName, json, Encoding.UTF8);
            }
        }


        public static void LoadJSON() {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JSON-Formatted Data (*json)|*.json";

            if (dialog.ShowDialog() ?? false) {
                MainWindow.instance.base_node = (Group) JsonSerializer.Deserialize(
                    File.ReadAllText(dialog.FileName, Encoding.UTF8), 
                    typeof(ICanvasObject), 
                    new JsonSerializerOptions { Encoder = GetEncoder() }
                );
            }
        }
    }
}
