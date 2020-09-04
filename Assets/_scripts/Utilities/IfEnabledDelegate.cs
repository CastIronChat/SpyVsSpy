using UnityEngine;

public class IfEnabledDelegate
{
    public delegate void DelVoid0();
    public delegate void DelVoid1<P1>(P1 p1);
    public delegate void DelVoid2<P1, P2>(P1 p1, P2 p2);
    public delegate void DelVoid3<P1, P2, P3>(P1 p1, P2 p2, P3 p3);

    public static DelVoid0 create(Behaviour self, DelVoid0 wrappedDelegate)
    {
        return delegate()
        {
            if ( self.isActiveAndEnabled ) wrappedDelegate( );
        };
    }
    public static DelVoid1<P1> create<P1>(Behaviour self, DelVoid1<P1> wrappedDelegate)
    {
        return delegate(P1 p1)
        {
            if ( self.isActiveAndEnabled ) wrappedDelegate( p1 );
        };
    }
    public static DelVoid2<P1, P2> create<P1, P2>(Behaviour self, DelVoid2<P1, P2> wrappedDelegate)
    {
        return delegate(P1 p1, P2 p2)
        {
            if ( self.isActiveAndEnabled ) wrappedDelegate( p1, p2 );
        };
    }
    public static DelVoid3<P1, P2, P3> create<P1, P2, P3>(Behaviour self, DelVoid3<P1, P2, P3> wrappedDelegate)
    {
        return delegate(P1 p1, P2 p2, P3 p3)
        {
            if ( self.isActiveAndEnabled ) wrappedDelegate( p1, p2, p3 );
        };
    }
}
