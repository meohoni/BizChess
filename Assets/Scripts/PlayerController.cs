using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Class declaration
[System.Serializable]
public class PassStartEvent : UnityEvent<Player> { }

public class PlayerController : MonoBehaviour
{
    [SerializeField] private AnimationCurve moveCurve;
    [SerializeField] private Sprite[] avatars;
    [SerializeField] private PassStartEvent passStartEvent;
    [SerializeField] private UnityEvent onStartEvent;
    [SerializeField] private UnityEvent askUseFreeCardEvent;
    [SerializeField] private UnityEvent pay50Event;
    [SerializeField] private UnityEvent forceGetOutPrisonEvent;

    public const int PASS_START_AMOUNT = 200;
    public const float MOVE_PARABOL_TIME = 0.5f;
    public const float MOVE_LINEAR_TIME = 0.1f;

    private List<Player> players;
    private Board board;
    private CardController cardController;
    private UIPanelController uiPanelController;
    private PlayerConfigManager playerConfigManager;
    private int currentPlayer = 0;
    private int duoDiceCounter = 0;

    private static bool isProcessing = false;
    private bool passStart = false;
    private bool isOnStart = false;

    private Vector3 offsetTopLeft;
    private Vector3 offsetTopRight;
    private Vector3 offsetBottomLeft;
    private Vector3 offsetBottomRight;
 
    private void Start()
    {
        board = GetComponent<Board>();
        cardController = GetComponent<CardController>();
        uiPanelController = GetComponent<UIPanelController>();
        playerConfigManager = FindObjectOfType<PlayerConfigManager>();
        offsetTopLeft = new Vector3(-0.2f, 0.2f, 0);
        offsetTopRight = new Vector3(0.2f, 0.2f, 0);
        offsetBottomLeft = new Vector3(-0.2f, -0.2f, 0);
        offsetBottomRight = new Vector3(0.2f, -0.2f, 0);
        
        InitPlayers();
    }

    public List<Player> Players { get => players; }
    public int CurrentPlayer { get => currentPlayer; set => currentPlayer = value; }
    public static bool IsProcessing { get => isProcessing; }
    public int DuoDiceCounter { get => duoDiceCounter; set => duoDiceCounter = value; }

    private void InitPlayers()
    {
        List<PlayerConfig> playerConfigs = playerConfigManager.GetPlayerConfigs();
        UnityEngine.Object playerPrefab = Resources.Load("Prefabs/Player");
        UnityEngine.Object tokenPrefab = Resources.Load("Prefabs/Token");
        Material mat = (Material)Resources.Load("Materials/token/Material.007");

        players = new List<Player>();

        for (int i = 0; i < playerConfigs.Count; i++)
        {
            GameObject obj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            Player player = obj.GetComponent<Player>();
            
            // Init token. 1 is the card index in the board for the card position 0 in the list.
            GameObject token = Instantiate(tokenPrefab, GetPositionForToken(playerConfigs[i].ColorName, 1, cardController.Cards[0].gameObject), Quaternion.identity) as GameObject;
            token.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
            token.transform.localScale = new Vector3(30.0f, 30.0f, 20.0f);
            token.transform.parent = transform;
            token.GetComponent<Renderer>().material = mat;
            token.gameObject.GetComponent<Renderer>().material.color = ColorHelper.GetColorValue(playerConfigs[i].ColorName);

            // load avatar
            player.Init(playerConfigs[i].Id, playerConfigs[i].Name, playerConfigs[i].ColorName, avatars[playerConfigs[i].AvatarIdx], playerConfigs[i].IsAIPlayer, token);

            players.Add(player);
        }

        currentPlayer = 0;
        duoDiceCounter = 0;
    }

    public void ProcessTurn(int dieANum, int dieBNum)
    {
        if (isProcessing) return;
        isProcessing = true;
        StartCoroutine(ProcessTurnCoroutine(dieANum, dieBNum));
    }

    private IEnumerator ProcessTurnCoroutine(int dieANum, int dieBNum)
    {
        Card card;

        if (dieANum == dieBNum)
        {
           duoDiceCounter++;
        }

        // keep isInPrison status before process
        bool isInPrison = players[currentPlayer].IsInPrison;
        if (isInPrison)
        {
            yield return StartCoroutine(ProcessPrison(players[currentPlayer], dieANum, dieBNum));
        }

        CheckPassStart(dieANum + dieBNum);
        // MOVE
        yield return StartCoroutine(MoveAlongPath(dieANum + dieBNum));

        // if isOnStart, can not receive money
        if (isOnStart)
        {
            DoOnStart();
            while (UIPanelController.IsDisplayMessageProcessing)
            {
                yield return null;
            }
        }
        // if pass start card, give 200$
        else if (passStart)
        {
            DoPassStart();
            while (UIPanelController.IsPayFeeProcessing)
            {
                yield return null;
            }
        }

        // PROCEED CARD
        card = cardController.Cards[players[currentPlayer].CurrentPosition];
        switch (card.GetCardType())
        {
            case CardType.PRISON: // go to prison
                {
                    players[currentPlayer].IsInPrison = true;
                    yield return StartCoroutine(GoToPrison());
                    break;
                }
            case CardType.TITLE_DEED:
                {
                    cardController.ProcessTitleDeedCard(players[currentPlayer], players);
                    yield return new WaitWhile(() => CardController.IsCardProcessing);
                    break;
                }
            case CardType.UTILITY:
                {
                    cardController.ProcessUtilityCard(players[currentPlayer], players);
                    yield return new WaitWhile(() => CardController.IsCardProcessing);
                    break;
                }
            case CardType.STATION:
                {
                    cardController.ProcessStationCard(players[currentPlayer], players);
                    yield return new WaitWhile(() => CardController.IsCardProcessing);
                    break;
                }
            case CardType.COMMUNITY_CHEST:
                {
                    cardController.ProcessCommunityChestCard(players[currentPlayer], players);
                    break;
                }
            case CardType.CHANCE:
                {
                    cardController.ProcessChanceCard(players[currentPlayer], players);
                    break;
                }
            case CardType.PARKING:
            case CardType.VISIT_PRISON:
            case CardType.START:
                {
                    break;
                }
            default: { break; }
        }

        // go to prison if duo dice 3 times
        if (duoDiceCounter >= 3)
        {
            duoDiceCounter = 0;
            players[currentPlayer].IsInPrison = true;
            yield return StartCoroutine(GoToPrison());
            MoveToNextPlayer();
        }  
        // move to next player
        else if (dieANum != dieBNum)
        {
            duoDiceCounter = 0;
            MoveToNextPlayer();
        }

        isProcessing = false;
    }

    private void MoveToNextPlayer()
    {
        if (currentPlayer < players.Count - 1)
        {
            currentPlayer++;
        }
        else
        {
            currentPlayer = 0;
        }
    }

    private IEnumerator ProcessPrison(Player player, int dieANum, int dieBNum)
    {
        player.NumRollDiceSinceInPrison++;

        // if duo dice
        if(dieANum == dieBNum)
        {
            // get out prison
            ResetFree();
            yield return new WaitForSeconds(2.0f);
            yield break;
        }

        // if having free card, ask use free card
        if (player.Props[PropsType.FREE_CARD] > 0)
        {
            SessionData.UpdateSessionData(ActionType.ASK_USE_FREE_CARD, null, player, null);
            uiPanelController.AskUseFreeCard();

            while (UIPanelController.IsUseFreeCardProcessing)
            {
                yield return null;
            }
        }

        // ask to get out prison by paying $50
        if (player.IsInPrison && player.NumRollDiceSinceInPrison < 3)
        {
            SessionData.UpdateSessionData(ActionType.ASK_PAY_50, null, player, null);
            uiPanelController.AskPay50();

            while (UIPanelController.IsAskPay50Processing)
            {
                yield return null;
            }
        }

        // force get out prison
        if (player.IsInPrison && player.NumRollDiceSinceInPrison >= 3)
        {
            uiPanelController.SetMessage(LocaleHelper.GetLocalizationValue(LocaleHelper.FORCE_GET_OUT_PRISON_KEY));
            yield return new WaitForSeconds(2.0f);
            uiPanelController.PayFeeWithNoOpponent(player, 50, null);

            while (UIPanelController.IsPayFeeProcessing)
            {
                yield return null;
            }
            ResetFree();
        }
    }

    public void OnYesButton()
    {
        if (UIPanelController.HasClicked) return;

        UIPanelController.HasClicked = true;
        if (SessionData.ActionType == ActionType.ASK_USE_FREE_CARD)
        {
            AskUseFreeCard();
        }
        else if (SessionData.ActionType == ActionType.ASK_PAY_50)
        {
            StartCoroutine(AskPay50());
        }
    }

    public void OnNoButton()
    {
        if (UIPanelController.HasClicked) return;

        UIPanelController.HasClicked = true;
        if (SessionData.ActionType == ActionType.ASK_USE_FREE_CARD)
        {
        }
        else if (SessionData.ActionType == ActionType.ASK_PAY_50)
        {
        }
        else if (SessionData.ActionType == ActionType.FORCE_GET_OUT_PRISON)
        {
        }
    }

    private void AskUseFreeCard()
    {
        ResetFree();
        players[currentPlayer].Props[PropsType.FREE_CARD]--;
        UIPanelController.IsUseFreeCardProcessing = false;
    }

    private IEnumerator AskPay50()
    {
        uiPanelController.PayFeeWithNoOpponent(players[currentPlayer], 50, null);
        while (UIPanelController.IsPayFeeProcessing)
        {
            yield return null;
        }
        ResetFree();
        UIPanelController.IsAskPay50Processing = false;
    }

    private void ResetFree()
    {
        players[currentPlayer].IsInPrison = false;
        players[currentPlayer].NumRollDiceSinceInPrison = 0;
        uiPanelController.DisplayMessage(LocaleHelper.GetLocalizationValue(LocaleHelper.FREE_SUCCESS_KEY));
    }

    private void DoOnStart()
    {
        onStartEvent.Invoke();
    }

    private void DoPassStart()
    {
        // invoke event
        passStartEvent.Invoke(players[currentPlayer]);
    }

    private void CheckPassStart(int step)
    {
        // calculate passStart and isOnStart
        passStart = false;
        isOnStart = false;
        int pos = players[currentPlayer].CurrentPosition + step;
        if (pos == Board.NUM_CARDS)
        {
            isOnStart = true;
        }
        else if (pos > Board.NUM_CARDS)
        {
            passStart = true;
        }
    }
  
    private IEnumerator MoveAlongPath(int step)
    {
        int nextPosition = 0;

        for (int i = 0; i < step; i++)
        {
            nextPosition = players[currentPlayer].CurrentPosition + 1;
            if (nextPosition > Board.NUM_CARDS - 1)
            {
                nextPosition = 0;
            }

            // if last step, moving to the child position
            if(i == step - 1)
            {
           //     players[currentPlayer].Move(GetPositionForToken(players[currentPlayer].ColorName, nextPosition + 1, cardController.Cards[nextPosition].gameObject), true, moveCurve, MOVE_PARABOL_TIME);
                players[currentPlayer].Move(GetPositionForToken(players[currentPlayer].ColorName, nextPosition + 1, cardController.Cards[nextPosition].gameObject), true, null, MOVE_LINEAR_TIME);
            }
            else
            {
            //    players[currentPlayer].Move(cardController.Cards[nextPosition].gameObject.transform.position, true, moveCurve, MOVE_PARABOL_TIME);
                players[currentPlayer].Move(cardController.Cards[nextPosition].gameObject.transform.position, true, null, MOVE_LINEAR_TIME);
            }

            while (players[currentPlayer].IsMoving)
            {
                yield return null;
            }
        }
     }


    private Vector3 GetPositionForToken(ColorName color, int cardIndex, GameObject card)
    {
        // offset top bottom left right is relative to the center of the card.
        // we offset again a bit relative to the 4 sides of the board.
        Vector3 offset = Vector3.zero;
        if (cardIndex > 1 && cardIndex < 8)
        {
            offset = new Vector3(0, -0.2f, 0);
        }
        else if (cardIndex > 8 && cardIndex < 19)
        {
            offset = new Vector3(-0.2f, 0, 0);
        }
        else if (cardIndex > 19 && cardIndex < 26)
        {
            offset = new Vector3(0, 0.2f, 0);
        }
        else if (cardIndex > 26 && cardIndex <= 32)
        {
            offset = new Vector3(0.2f, 0, 0);
        }

        switch (color)
        {
            case ColorName.BLUE:
                return card.transform.position + offsetTopRight + offset;
            case ColorName.RED:
                return card.transform.position + offsetTopLeft + offset;
            case ColorName.GREEN:
                return card.transform.position + offsetBottomRight + offset;
            case ColorName.YELLOW:
                return card.transform.position + offsetBottomLeft + offset;
        }
        return new Vector3(5, 6, 0);
    }

    private IEnumerator GoToPrison()
    {
        int nextPosition = 0;
        if(players[currentPlayer].CurrentPosition > 7) // 7 is the position of the prison
        {
            // move backward
            for(int i = players[currentPlayer].CurrentPosition; players[currentPlayer].CurrentPosition > 7; i--)
            {
                nextPosition = i - 1;

                // if last step, moving to the child position
                if (i == 8)
                {
                    players[currentPlayer].Move(GetPositionForToken(players[currentPlayer].ColorName, nextPosition + 1, cardController.Cards[nextPosition].gameObject), false, null, MOVE_LINEAR_TIME);
                }
                else
                {
                    players[currentPlayer].Move(cardController.Cards[nextPosition].gameObject.transform.position, false, null, MOVE_LINEAR_TIME);
                }

                while (players[currentPlayer].IsMoving)
                {
                    yield return null;
                }
            }
        }
        else if (players[currentPlayer].CurrentPosition < 7)
        {
            // move forward
            for (int i = players[currentPlayer].CurrentPosition; players[currentPlayer].CurrentPosition < 7; i++)
            {
                nextPosition = i + 1;

                // if last step, moving to the child position
                if (i == 6)
                {
                    players[currentPlayer].Move(GetPositionForToken(players[currentPlayer].ColorName, nextPosition + 1, cardController.Cards[nextPosition].gameObject), true, null, MOVE_LINEAR_TIME);
                }
                else
                {
                    players[currentPlayer].Move(cardController.Cards[nextPosition].gameObject.transform.position, true, null, MOVE_LINEAR_TIME);
                }

                while (players[currentPlayer].IsMoving)
                {
                    yield return null;
                }
            }
        }
    }

}
