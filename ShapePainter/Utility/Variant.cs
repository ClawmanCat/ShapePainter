using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapePainter.Utility {
    public class Variant<T1, T2> {
        public delegate void T1Delegate(T1 x);
        public delegate void T2Delegate(T2 x);

        public object contained { get; set; }


        public Variant(T1 obj) { contained = obj; }
        public Variant(T2 obj) { contained = obj; }


        public static implicit operator T1(Variant<T1, T2> v) {
            return (T1) v.contained;
        }

        public static implicit operator T2(Variant<T1, T2> v) {
            return (T2) v.contained;
        }
    }
}
