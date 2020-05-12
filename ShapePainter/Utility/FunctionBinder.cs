using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapePainter.Utility {
    // Imagine having variadic templates...
    public static class FunctionBinder {
        // Actions
        public static Action Bind<T1>(this Action<T1> action, T1 value) {
            return () => { action(value); };
        }


        public static Action<T1> Bind<T1, T2>(this Action<T1, T2> action, T2 value) {
            return (T1 a) => { action(a, value); };
        }


        public static Action<T1, T2> Bind<T1, T2, T3>(this Action<T1, T2, T3> action, T3 value) {
            return (T1 a, T2 b) => { action(a, b, value); };
        }


        public static Action<T1, T2, T3> Bind<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T4 value) {
            return (T1 a, T2 b, T3 c) => { action(a, b, c, value); };
        }


        // Functions
        public static Func<R> Bind<R, T1>(this Func<T1, R> func, T1 value) {
            return () => { return func(value); };
        }


        public static Func<T1, R> Bind<R, T1, T2>(this Func<T1, T2, R> func, T2 value) {
            return (T1 a) => { return func(a, value); };
        }


        public static Func<T1, T2, R> Bind<R, T1, T2, T3>(this Func<T1, T2, T3, R> func, T3 value) {
            return (T1 a, T2 b) => { return func(a, b, value); };
        }


        public static Func<T1, T2, T3, R> Bind<R, T1, T2, T3, T4>(this Func<T1, T2, T3, T4, R> func, T4 value) {
            return (T1 a, T2 b, T3 c) => { return func(a, b, c, value); };
        }
    }
}
