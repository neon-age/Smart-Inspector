using UnityEngine.UIElements;
using static AV.Inspector.Runtime.SmartInspector;

namespace AV.Inspector
{
    internal partial class SmartInspector
    {
        internal static StyleSheet tooltipStyles => FindStyleSheet(guid: "080805133721dc943a83773c25479977");
        internal static StyleSheet scrollViewStyles => FindStyleSheet(guid: "46cdd7b95ede0f14390e6a26d6913911");
        internal static StyleSheet headerStyles => FindStyleSheet(guid: "d200692a7d7191c4f8832b720dbe739e");
        
        internal static StyleSheet tabsBarStyles => FindStyleSheet(guid: "84161fa001b1f7d43b2bd51691ba6906");
        internal static StyleSheet tabsBarStylesLight => FindStyleSheet(guid: "e43b3eff03f716348a15f62c39efe6c0");
    }
}