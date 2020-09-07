using System;
using System.Collections;
using ComponentReferenceAttributes;
using ICSharpCode.NRefactory.Visitors;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using Random = System.Random;

public class GetawayAnimation : MonoBehaviour
{
    [Description( @"
        Puppeteers player's avatar and camera for duration of getaway animation.
        Announces when animation has completed so that external logic can show restart UI.
    " )]
    [SerializeField]
    [OwnComponent]
    private Animator animator;

    [SerializeField] private Transform cameraFollows;
    [SerializeField] private Transform playerFollows;

    public delegate void OnAnimationFinishedDelegate();

    public event OnAnimationFinishedDelegate OnAnimationFinished;

    private GameManager gm
    {
        get => GameManager.instance;
    }

    private CameraManager cm
    {
        get => gm.cameraManager;
    }

    private Player winner;
    private bool isPlayerFollowingAnimation = false;

    private void Update()
    {
        if ( isPlayerFollowingAnimation )
        {
            winner.transform.position = playerFollows.transform.position;
            winner.transform.rotation = playerFollows.transform.rotation;
        }
    }

    private void OnValidate()
    {
        Assert.IsNotNull( animator );
        Assert.IsNotNull( playerFollows );
        Assert.IsNotNull( cameraFollows );
    }

    private void Start()
    {
        animator.enabled = false;
        gm.OnSetWinner += StartAnimationIfEnabled;
        OnAnimationFinished += delegate() { gm.startButtonShownToMasterClient = true; };
    }

    public void StartAnimationIfEnabled(Player winner)
    {
        if ( !isActiveAndEnabled ) return;
        StartAnimation(winner);
    }

    private void StartAnimation(Player winner) {
        this.winner = winner;
        var cam = cm.getCameraForPlayer(this.winner);
        cam.alternativeFollowTarget = cameraFollows;
        cam.snapToRoomGrid = false;
        isPlayerFollowingAnimation = true;
        animator.playbackTime = 0;
        animator.enabled = true;
    }

    private void AnimationEvent_PlayerEntersGetawayVehicle()
    {
        winner.setAvatarVisibility( false );
    }

    private void AnimationEvent_AnimationEnds()
    {
        winner.setAvatarVisibility( true );
        isPlayerFollowingAnimation = false;
        var cam = cm.getCameraForPlayer(winner);
        cam.alternativeFollowTarget = null;
        cam.snapToRoomGrid = true;
        animator.enabled = false;
        winner = null;
        OnAnimationFinished();
    }
}
