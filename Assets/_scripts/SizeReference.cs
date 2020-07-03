using UnityEngine;

public class SizeReference : MonoBehaviour
{
    public Vector3 size;
    void OnDrawGizmosSelected()
    {
        // Draw a semitransparent blue cube at the transforms position
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawWireCube(transform.position, size);
    }
}
