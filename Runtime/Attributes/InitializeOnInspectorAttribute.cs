using System;

namespace AV.Inspector.Runtime
{
    [AttributeUsage(AttributeTargets.Method)]
    public class InitializeOnInspectorAttribute : Attribute
    {
    }
}