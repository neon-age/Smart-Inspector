using System;
using System.Linq.Expressions;

namespace AV.UITK
{
    public static partial class FluentUITK
    {
        /// <summary> Retrieves full path of a source member that can be used for binding. </summary>
        public static string GetMemberPath<TSource>(Expression<Func<TSource, object>> expression)
        {
            var member = expression.Body as MemberExpression;
            if (member == null)
            {
                var unary = expression.Body as UnaryExpression;
                
                if (unary != null && unary.NodeType == ExpressionType.Convert)
                    member = unary.Operand as MemberExpression;
            }

            var result = member?.ToString();
            result = result?.Substring(result.IndexOf('.') + 1);

            return result;
        }
    }
}