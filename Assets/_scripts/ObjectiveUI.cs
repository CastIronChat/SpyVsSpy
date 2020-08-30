/// <summary>
/// TODO Doesn't appear to be used anywhere; make CollectibleManager use this class?
/// </summary>
public class ObjectiveUI
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


    private void Update()
    {
        var localPlayerState = gm.collectibleManager.getState( pm.localPlayer );
        if ( localPlayerState.hasAllCollectibles )
        {

        }
    }
}
