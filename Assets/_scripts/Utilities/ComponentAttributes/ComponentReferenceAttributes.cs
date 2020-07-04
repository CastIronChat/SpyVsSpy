
using UnityEngine;

namespace ComponentReferenceAttributes
{
    public abstract class ComponentReferenceAttribute : PropertyAttribute { }
    ///<summary>
    /// Inspector can auto-set reference to component on same GameObject
    ///</summary>
    public class OwnComponentAttribute : ComponentReferenceAttribute { }
    ///<summary>
    /// Inspector can auto-set reference to component on sibling gameObject (siblings have same parent as this gameObject)
    ///</summary>
    public class SiblingComponentAttribute : ComponentReferenceAttribute { }
    ///<summary>
    /// Inspector can auto-set reference to component on direct parent gameobject
    ///</summary>
    public class ParentComponentAttribute : ComponentReferenceAttribute { }
    ///<summary>
    /// Inspector can auto-set reference to component on direct child gameObject
    ///</summary>
    public class ChildComponentAttribute : ComponentReferenceAttribute { }
    ///<summary>
    /// Inspector can auto-set reference to component on parent, grandparent, etc.
    ///</summary>
    public class AncestorComponentAttribute : ComponentReferenceAttribute { }
    public class DescendantComponentAttribute : ComponentReferenceAttribute { }

}
