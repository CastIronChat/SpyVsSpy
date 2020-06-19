using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    public GameManager gameManager;
    public int localPlayer;
    public GameObject roundUI;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void DisableUi()
    {
        roundUI.active = false;
    }
    public void EnableUi()
    {
        roundUI.active = true;
    }

    public void SelectRound(int rndtype)
    {
        gameManager.VoteForNewRoundType( rndtype, localPlayer );
    }
    public void SetRound(int rndtype)
    {

    }
    public int GetStartingLives()
    { return 3; }
    public int GetStartingScore()
    { return 0; }
    public int GetStartingMoney()
    { return 0; }


}
