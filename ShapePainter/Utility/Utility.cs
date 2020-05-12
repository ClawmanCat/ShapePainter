using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using static System.Linq.Enumerable;


namespace ShapePainter.Utility {
    public static class Utility {
        public static (Vector, Vector) GetMinMax(Vector a, Vector b) {
            Vector min = new Vector(
                Math.Min(a.X, b.X),
                Math.Min(a.Y, b.Y)
            );

            Vector max = new Vector(
                Math.Max(a.X, b.X),
                Math.Max(a.Y, b.Y)
            );

            return (min, max);
        }


        public static IEnumerable<T> ReverseView<T>(this IList<T> list) {
            for (int i = list.Count - 1; i >= 0; --i) yield return list[i];
        }


        public static void SetValue(this MemberInfo info, object target, object value) {
            if (info.MemberType == MemberTypes.Field) ((FieldInfo) info).SetValue(target, value);
            else if (info.MemberType == MemberTypes.Property) ((PropertyInfo) info).SetValue(target, value);
            else throw new ArgumentException("Argument info must be a field or a property.");
        }


        public static object GetValue(this MemberInfo info, object target) {
            if (info.MemberType == MemberTypes.Field) return ((FieldInfo) info).GetValue(target);
            else if (info.MemberType == MemberTypes.Property) return ((PropertyInfo) info).GetValue(target);
            else throw new ArgumentException("Argument info must be a field or a property.");
        }


        public static bool InBox(this Vector v, Vector a, Vector b) {
            var (min, max) = GetMinMax(a, b);
            return v.X >= min.X && v.Y >= min.Y && v.X < max.X && v.Y < max.Y;
        }
    }
}