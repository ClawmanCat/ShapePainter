using Newtonsoft.Json;
using ShapePainter.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapePainter.Shapes {
    public abstract class ICanvasObject {
        [JsonIgnore] public ICanvasObject parent { get; set; }
        public List<ICanvasObject> children { get; set; }


        // For deserialization
        public ICanvasObject() : this(null) { }


        public ICanvasObject(ICanvasObject parent, params ICanvasObject[] children) {
            this.parent = parent;
            this.children = new List<ICanvasObject>(children);

            if (parent != null) parent.children.Add(this);
        }


        public void revalidate(ICanvasObject parent = null) {
            this.parent = parent;
            foreach (ICanvasObject obj in children) obj.revalidate(this);
        }


        public virtual bool is_a(Type type) {
            return this.GetType().IsAssignableFrom(type);
        }


        public abstract void accept(IVisitor visitor);
    }
}
