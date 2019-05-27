using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    // properties of the board
    public const int X_DIM = 9;
    public const int Y_DIM = 13;
    public const int NUM_CARDS = 36;

    private CardController cardController;
    private PlayerController playerController;
    private Dice dice;
    private bool isFinishTurn = true;
    private bool isGameOver = false;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        cardController = GetComponent<CardController>();
        playerController = GetComponent<PlayerController>();
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

        // DO ROLL HERE
        bool doRoll = false;
        // ask if player is auto rolling.
        if (playerController.Players[playerController.CurrentPlayer].IsAutoRoll)
        {
            doRoll = true;
        }
        else
        {
            // wait for tap.
            while (!Input.GetMouseButton(0))
            {
                yield return null;
            }
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
//                yield return playerController.StartCoroutine(playerController.ProcessTurn(Dice.DieANum + Dice.DieBNum));
                yield return playerController.StartCoroutine(playerController.ProcessTurn(25));
                print("End Moving.");
            }

            if (!PlayerController.IsProcessing)
            {
                isFinishTurn = true;
                print("End Turn!");
            }
        }
    }
}
