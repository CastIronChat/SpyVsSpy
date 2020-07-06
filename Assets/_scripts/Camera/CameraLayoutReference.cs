using UnityEngine;
using UnityEngine.Assertions;

public class CameraLayoutReference : MonoBehaviour
{
    public int maximumPlayers = 9;

    public RectTransformUtility getLocalPlayerCamera()
    {
        var ret = transform.GetChild( 0 ).GetComponent<RectTransformUtility>();
        Assert.IsNotNull( ret );
        return ret;
    }

    public RectTransformUtility getNonLocalPlayerCamera(int index)
    {
        var ret = transform.GetChild( index - 1 );
        Assert.IsNotNull( ret );
        var ret2 = ret.GetComponent<RectTransformUtility>();
        Assert.IsNotNull( ret2 );
        return ret2;
    }
}
