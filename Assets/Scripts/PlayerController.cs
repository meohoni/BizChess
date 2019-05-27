using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private AnimationCurve moveCurve;

    public const int PASS_START_AMOUNT = 200;
    public const float MOVE_PARABOL_TIME = 0.5f;
    public const float MOVE_LINEAR_TIME = 0.2f;

    private List<Player> players;
    private Board board;
    private CardController cardController;
    private PlayerConfigManager playerConfigManager;
    private int currentPlayer;
    private static bool isProcessing = false;
    private bool passStart = false;

    private Vector3 offsetTopLeft;
    private Vector3 offsetTopRight;
    private Vector3 offsetBottomLeft;
    private Vector3 offsetBottomRight;
 
    private void Start()
    {
        board = GetComponent<Board>();
        cardController = GetComponent<CardController>();
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
            
            // 1 is the card index in the board for the card position 0 in the list.
            GameObject token = Instantiate(tokenPrefab, GetPositionForToken(playerConfigs[i].ColorName, 1, cardController.Cards[0].gameObject), Quaternion.identity) as GameObject;
            token.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
            token.transform.localScale = new Vector3(30.0f, 30.0f, 20.0f);
            token.transform.parent = transform;
            token.GetComponent<Renderer>().material = mat;
            token.gameObject.GetComponent<Renderer>().material.color = ColorHelper.GetColorValue(playerConfigs[i].ColorName);
            player.Init(playerConfigs[i].Id, playerConfigs[i].Name, playerConfigs[i].ColorName, playerConfigs[i].IsAIPlayer, token);

            players.Add(player);
        }

        currentPlayer = 0;
    }

    public IEnumerator ProcessTurn(int step)
    {
        Card card;
        isProcessing = true;

        // CALCULATE passStart
        passStart = false;
        if(players[currentPlayer].CurrentPosition + step > Board.NUM_CARDS - 1)
        {
            passStart = true;
        }

        // MOVE
        yield return StartCoroutine(MoveAlongPath(step));

 
        // if pass start card, give 200$
        if (passStart)
        {
            players[currentPlayer].Money += PASS_START_AMOUNT;
            // play animation.
        }

        // PROCEED CARD
        card = cardController.Cards[players[currentPlayer].CurrentPosition];
        switch (card.Type)
        {
            case CardType.PRISON: // go to prison
                {
                    yield return StartCoroutine(GoToPrison());
                    break;
                }
            case CardType.DEED: // if deed card, process deed card
                {
                    cardController.ProcessDeedCard(players[currentPlayer], players);
                    break;
                }
            case CardType.UTILITY:
                {
                    cardController.ProcessUtilityCard(players[currentPlayer], players);
                    break;
                }
            case CardType.COMMUNITY_CHEST:
                {
                    cardController.ProcessChestCard(players[currentPlayer], players);
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
       

        // MOVE POINTER TO NEXT PLAYER.
        if (currentPlayer < players.Count - 1)
        {
            currentPlayer++;
        }
        else
        {
            currentPlayer = 0;
        }

        isProcessing = false;
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
                players[currentPlayer].Move(GetPositionForToken(players[currentPlayer].ColorName, nextPosition + 1, cardController.Cards[nextPosition].gameObject), true, moveCurve, MOVE_PARABOL_TIME);
            }
            else
            {
                players[currentPlayer].Move(cardController.Cards[nextPosition].gameObject.transform.position, true, moveCurve, MOVE_PARABOL_TIME);
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
