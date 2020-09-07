using ComponentReferenceAttributes;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI showing player objectives.
/// Does not decide what to show based on game state; is puppeteered by other code.
/// </summary>
public class ObjectiveUI : MonoBehaviour
{
    private GameManager gm
    {
        get => GameManager.instance;
    }
    private PlayerManager pm
    {
        get => gm.playerManager;
    }
    private CollectibleManager cm
    {
        get => gm.collectibleManager;
    }

    [ChildComponent]
    public Text textComponent;

    /// <summary>
    /// Set the objective shown to the user.
    /// If null, the UI is hidden.
    /// </summary>
    public void setObjective([CanBeNull] string objective)
    {
        if ( objective == null )
        {
            gameObject.SetActive( false );
        }
        else
        {
            gameObject.SetActive( false );
            textComponent.text = objective;
        }
    }
}
