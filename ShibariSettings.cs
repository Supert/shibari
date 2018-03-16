using UnityEngine;
using TypeReferences;

namespace Shibari
{
    public class ShibariSettings : MonoBehaviour
    {
        [ClassExtends(typeof(BindableData), AllowAbstract = false, Grouping = ClassGrouping.ByNamespaceFlat)]
        public ClassTypeReference RootNodeType; 
    }
}