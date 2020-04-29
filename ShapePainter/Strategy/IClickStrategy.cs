using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ShapePainter.Strategy {
    public interface IClickStrategy {
        void OnMouseDown(Vector downpos);
        void OnMouseUp(Vector downpos, Vector uppos);
        void OnMouseMoved(Vector downpos, Vector currpos);
    }
}
