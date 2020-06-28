using UnityEngine;
using UnityEngine.UI;

/// A row of icons, optionally with a selection box around one of them so the user can pick items.
/// Only implements rendering logic; meant to be puppeteered by something else like the `Player` or `Inventory`.
public class IconRowHUD : MonoBehaviour {
    public int getIconCount() {
        return transform.childCount - 1;
    }
    private RawImage getRawImage(int index) {
        return transform.GetChild(index + 1).GetComponent<RawImage>();
    }
    private RawImage getCursor() {
        return transform.GetChild(0).GetComponent<RawImage>();
    }
    public void setIcon(int index, Sprite icon) {
        getRawImage(index).texture = icon.texture;
    }
    public void setIconVisibility(int index, bool visible) {
        getRawImage(index).enabled = visible;
    }
    public void setCursorPosition(int index) {
        getCursor().transform.position = getRawImage(index).transform.position;
    }
    public void setCursorVisibility(bool visible) {
        getCursor().enabled = visible;
    }
}
