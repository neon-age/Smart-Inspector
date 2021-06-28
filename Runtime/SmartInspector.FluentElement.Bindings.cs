

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace AV.Inspector.Runtime
{
    public static partial class SmartInspector
    {
        public partial class FluentElement<T>
        {
            #if UNITY_EDITOR
            public void Bind(SerializedObject serializedObject) => x.Bind(serializedObject);
            #else
            public void Bind(object serializedObject) {}
            #endif
            
            #if UNITY_EDITOR
            public void Unbind() => x.Unbind();
            #endif
            
            public FluentElement<PropertyField> NewField(SerializedProperty property, string label = null)
            {
                #if UNITY_EDITOR
                return new PropertyField(property) { label = label };
                #else
                return null;
                #endif
            }

            public FluentElement<PropertyField> NewField(string bindingPath, string label = null)
            {
                #if UNITY_EDITOR
                return new PropertyField { bindingPath = bindingPath, label = label };
                #else
                return null;
                #endif
            }
            
            public FluentElement<PropertyField> NewField<TSource>(Expression<Func<TSource, object>> bindingExp, string label = null)
            {
                #if UNITY_EDITOR
                var bindingPath = GetMemberPath(bindingExp);
                return new PropertyField { bindingPath = bindingPath, label = label };
                #else
                return null;
                #endif
            }
            
            
            static string GetMemberPath<TSource>(Expression<Func<TSource, object>> expression)
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
}