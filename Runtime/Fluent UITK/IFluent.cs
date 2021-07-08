using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace AV.UITK
{
    public interface IFluent<T> where T : VisualElement
    {
        T x { get; }
    }
}