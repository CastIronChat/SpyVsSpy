using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Players use this for keyboard input.
/// Avoids sprinkling keyboard-specific logic through the player's game logic.
/// TODO I don't actually know how unity's input handling works; maybe there's a built-in way to do reconfigurable controls?
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

}
