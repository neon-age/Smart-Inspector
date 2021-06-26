
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

            public FluentElement<PropertyField> NewProperty(string bindingPath, string label = null)
            {
                #if UNITY_EDITOR
                return new PropertyField { bindingPath = bindingPath, label = label };
                #else
                return null;
                #endif
            }
        }
    }
}