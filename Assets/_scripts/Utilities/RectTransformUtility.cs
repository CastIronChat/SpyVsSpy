using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class RectTransformUtility : MonoBehaviour
{
    [Description(@"
        Visualize a RectTransform with a gizmo so you can see its size in the scene view.

        Also has helpful getter to retrieve the size and corners of the rectangle,
        combining transform scale and size.
    ")]
    #region Fields
    private Color selectedColor = new Color(0, 0, 1, 0.5f);
    private Color normalColor = new Color(0, 0, 1, 0.2f);
    public bool showWhenSelected = true;
    public bool showWhenDeselected = true;
    private RectTransform rectTransform;
    #endregion
    void bind() {
        if ( rectTransform == null ) rectTransform = GetComponent<RectTransform>();
    }
    void Awake() {
        bind();
    }
    void OnDrawGizmosSelected()
    {
        bind();
        draw(true);
    }
    void OnDrawGizmos()
    {
        bind();
        draw(false);
    }
    void draw(bool selected) {
        if(selected ? showWhenSelected : showWhenDeselected) {
            Gizmos.color = selected ? selectedColor : normalColor;
            Gizmos.matrix = matrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
    }
    public Vector2 size {
        get => rectTransform.sizeDelta;
    }
    public Matrix4x4 matrix {
        get => transform.localToWorldMatrix * Matrix4x4.Scale(size);
    }
    private Matrix4x4 cornerMatrix {
        get => transform.localToWorldMatrix * Matrix4x4.Scale(size / 2);
    }
    public Vector3 upperRightCorner {
        get => cornerMatrix.MultiplyPoint(Vector2.up + Vector2.right);
    }
    public Vector3 upperLeftCorner {
        get => cornerMatrix.MultiplyPoint(Vector2.up + Vector2.left);
    }
    public Vector3 lowerLeftCorner {
        get => cornerMatrix.MultiplyPoint(Vector2.down + Vector2.left);
    }
    public Vector3 lowerRightCorner {
        get => cornerMatrix.MultiplyPoint(Vector2.down + Vector2.right);
    }
    public Vector2 globalSize {
        get => new Vector2(transform.lossyScale.x * size.x, transform.lossyScale.y * size.y);
    }
    public float width {
        get => transform.lossyScale.x * size.x;
    }
    public float height {
        get => transform.lossyScale.y * size.y;
    }
}
