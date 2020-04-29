using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapePainter.Utility {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class Resetable : Attribute { }
}
