using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Board : MonoBehaviour
{
    [SerializeField] UnityEvent tapEvent;
    // properties of the board
    public const int X_DIM = 9;
    public const int Y_DIM = 13;
    public const int NUM_CARDS = 36;

    private CardController cardController;
    private PlayerController playerController;
    private UIPanelController uiPanelController;
    private Dice dice;
    private static bool isFinishTurn = true;
    private bool isGameOver = false;

    public static bool IsFinishTurn { get => isFinishTurn; }

    void Start()
    {
        cardController = GetComponent<CardController>();
        playerController = GetComponent<PlayerController>();
        uiPanelController = GetComponent<UIPanelController>();

        dice = GetComponent<Dice>();
        ColorHelper.BuildColorDict();
    }

    void Update()
    {
        if (isFinishTurn)
            {
                StartCoroutine(ProcessGameLogic());
            }                   
    }

    private IEnumerator ProcessGameLogic()
    { 
        isFinishTurn = false;
 
        UpdateUIPanel();

        // DO ROLL HERE
        bool doRoll = false;
        // ask if player is auto rolling.
        if (playerController.Players[playerController.CurrentPlayer].IsAutoRoll)
        {
            doRoll = true;
        }
        else
        {
            // display message requesting tap
            uiPanelController.DisplayTapMsg();
            // wait for tap.
            while (!Input.GetMouseButton(0))
            {
                yield return null;
            }
            tapEvent.Invoke();
            uiPanelController.ClearMsg();

            // roll
            doRoll = true;
        }

        if (doRoll)
        {
            // roll die
            if (!Dice.IsDiceRunning)
            {
                print("Start Turn.");
                yield return dice.StartCoroutine(dice.RollDice());
            }

            if (!Dice.IsDiceRunning && !PlayerController.IsProcessing)
            {
                print("Start Moving.");
                playerController.ProcessTurn(Dice.DieANum, Dice.DieBNum);
                while (PlayerController.IsProcessing)
                {
                    yield return null;
                }
                print("End Moving.");
            }

            if (!PlayerController.IsProcessing)
            {
                isFinishTurn = true;
                print("End Turn!");
            }
        }
    }

    private void UpdateUIPanel()
    {
        // update player
        uiPanelController.UpdatePlayer(playerController.Players[playerController.CurrentPlayer], true);
        uiPanelController.ResetMsgBox();
        UIPanelController.HasClicked = false;
    }
}
