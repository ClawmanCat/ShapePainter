using ShapePainter.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static ShapePainter.Utility.Serializer;

namespace ShapePainter.Shapes {
    [JsonConverter(typeof(CanvasObjectSerializer))] public abstract class ICanvasObject {
        [JsonIgnore] public ICanvasObject parent { get; set; }
        public List<ICanvasObject> children { get; set; }


        public ICanvasObject(ICanvasObject parent, params ICanvasObject[] children) {
            this.parent = parent;
            this.children = new List<ICanvasObject>(children);

            if (parent != null) parent.children.Add(this);
        }


        public abstract void accept(IVisitor visitor);
    }
}
