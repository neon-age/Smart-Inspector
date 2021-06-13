using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using static System.Linq.Expressions.Expression;

namespace AV.Inspector
{
    internal static class ExpressionUtils
    {
        public static void CreateDelegate<T>(MethodInfo method, out T lambda) where T : Delegate
        {
            lambda = (T)Delegate.CreateDelegate(typeof(T), method);
        }
    }
}