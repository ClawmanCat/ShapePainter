using ShapePainter.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapePainter.Command {
    public class ClearCommand : CompoundCommand {
        public ClearCommand(List<ICanvasObject> objects) : base(objects.Select((ICanvasObject o) => new AddRemoveCommand(o, false)).ToArray()) { }
    }
}
