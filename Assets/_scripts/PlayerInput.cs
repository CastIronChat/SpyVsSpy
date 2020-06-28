using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Players use this for keyboard input.
/// Avoids sprinkling keyboard-specific logic through the player's game logic.
/// TODO I don't actually know how unity's input handling works; maybe there's a built-in way to do reconfigurable controls?
///
/// `Down` means the key was pressed this frame
/// `Up` means released this frame
/// `Pressed` means check the key's current state
public class PlayerInput
{
    public bool GetDirectionDown(CardinalDirection direction) {
        return Input.GetKeyDown(getKeyCodeForDirection(direction));
    }
    public bool GetDirectionUp(CardinalDirection direction) {
        return Input.GetKeyUp(getKeyCodeForDirection(direction));
    }
    public bool GetDirectionPressed(CardinalDirection direction) {
        return Input.GetKey(getKeyCodeForDirection(direction));
    }
    private KeyCode getKeyCodeForDirection(CardinalDirection direction) {
        switch(direction) {
            case CardinalDirection.Up:
                return KeyCode.W;
            case CardinalDirection.Right:
                return KeyCode.D;
            case CardinalDirection.Left:
                return KeyCode.A;
            case CardinalDirection.Down:
                return KeyCode.S;
            default:
                throw new Exception("should never happen");
        }
    }
    public bool GetInteractDown() {
        return Input.GetKeyDown(KeyCode.Space);
    }
    public bool GetUseTrapDown() {
        return Input.GetKeyDown(KeyCode.T);
    }
    public bool GetChooseNextTrapDown() {
        return Input.GetKeyDown(KeyCode.Period);
    }
    public bool GetChoosePreviousTrapDown() {
        return Input.GetKeyDown(KeyCode.Comma);
    }
    public int? getChooseTrapByIndexDown() {
        if(Input.GetKeyDown(KeyCode.Alpha1)) return 0;
        if(Input.GetKeyDown(KeyCode.Alpha2)) return 1;
        if(Input.GetKeyDown(KeyCode.Alpha3)) return 2;
        if(Input.GetKeyDown(KeyCode.Alpha4)) return 3;
        return null;
    }

    bool __debug = true;
    // Methods with __debug prefix are for testing in development.
    // Should be disabled or removed to avoid cheating.
    public bool __debugInventoryResetDown() {
        return __debug && Input.GetKeyDown(KeyCode.Alpha5);
    }
}
