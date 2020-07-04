using UnityEngine;


/// Used to create a box in the editor with a given size, with helpers to get coordinates of the corners at runtime.
/// Can be used to make layouts.
/// Made for 2D, so the Z coordinates are meaningless.
public class SizeReference : MonoBehaviour
{
    [Description(@"
        Renders a box in the scene but doesn't do anything at runtime.
        Useful for visual layout in the editor, and then code can query
        for the coordinates of corners of the box.
    ")]
    public Vector2 size = Vector2.one;
    private Color selectedColor = new Color(0, 0, 1, 0.5f);
    private Color normalColor = new Color(0, 0, 1, 0.2f);
    public Matrix4x4 matrix {
        get => transform.localToWorldMatrix * Matrix4x4.Scale(size);
    }
    private Matrix4x4 cornerMatrix {
        get => transform.localToWorldMatrix * Matrix4x4.Scale(size / 2);
    }
    Vector3 upperRightCorner {
        get => cornerMatrix.MultiplyPoint(Vector2.up + Vector2.right);
    }
    Vector3 upperLeftCorner {
        get => cornerMatrix.MultiplyPoint(Vector2.up + Vector2.left);
    }
    Vector3 lowerLeftCorner {
        get => cornerMatrix.MultiplyPoint(Vector2.down + Vector2.left);
    }
    Vector3 lowerRightCorner {
        get => cornerMatrix.MultiplyPoint(Vector2.down + Vector2.right);
    }
    Vector2 globalSize {
        get => new Vector2(transform.lossyScale.x * size.x, transform.lossyScale.y * size.y);
    }
    Vector3 height {
        get => cornerMatrix.MultiplyPoint(Vector2.right);
    }
    void OnDrawGizmosSelected()
    {
        draw(true);
    }
    void OnDrawGizmos()
    {
        draw(false);
    }
    void draw(bool selected) {
        Gizmos.DrawCube(lowerLeftCorner, Vector3.one);
        Gizmos.color = selected ? selectedColor : normalColor;
        Gizmos.matrix = matrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}
