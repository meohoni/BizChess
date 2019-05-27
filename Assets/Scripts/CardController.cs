using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardController : MonoBehaviour
{
    public const string CARD_TITLE_PREFIX = "CARD_TITLE_";

    [SerializeField] private Sprite cornerCard;

    private List<Card> cards;
    private Dictionary<int, CardData> cardDataDict;

    private Dictionary<ColorName, List<GameObject>> cardGroups;
    private Board board;

    private DeedCardController deedCardController;
    private ChestCardController chestCardController;
    private ChanceCardController chanceCardController;

    public List<Card> Cards { get => cards; }

    void Start()
    {
        board = GetComponent<Board>();
        deedCardController = GetComponent<DeedCardController>();
        chestCardController = GetComponent<ChestCardController>();
        chanceCardController = GetComponent<ChanceCardController>();

        cardDataDict = new Dictionary<int, CardData>();
        UnityEngine.Object[] cardDataObjects = Resources.LoadAll("SO/Cards", typeof(CardData));

        for (int i = 0; i < cardDataObjects.Length; i++)
        {
            CardData cd = (CardData)cardDataObjects[i];
            if (!cardDataDict.ContainsKey(cd.Id))
            {
                cardDataDict.Add(cd.Id, cd);
            }
        }

        foreach(var cardData in cardDataDict)
        {
            print(cardData.Value.TitleKey);
            print(cardData.Value.Type);
            print(cardData.Value.ColorName);
        }

        InitCards();
    }

    private void InitCards()
    {
        int cardId;
        CardData cardData;

        cards = new List<Card>();
        cardGroups = new Dictionary<ColorName, List<GameObject>>();
        UnityEngine.Object cardPrefab = Resources.Load("Prefabs/Card");
        UnityEngine.Object cardTextParentPrefab = Resources.Load("Prefabs/CardTextParent");
        GameObject uiCanvas = GameObject.Find("UICanvas");

        Dictionary<int, Vector2> localPostionOfCards = GetLocalPositionOfCards();
        
        for (int i = 1; i <= Board.NUM_CARDS; i++)
        {
            if (cardDataDict.ContainsKey(i))
            {
                cardData = cardDataDict[i];
            }
            else // log error
            {
                Debug.LogError("Card Data id" + i + " not found.");
                break;
            }

            // create object card.
            Vector2 localPosition = localPostionOfCards[i];
            GameObject obj = Instantiate(cardPrefab, GetWorldPositionOfCard(board.gameObject.transform.position, localPosition), Quaternion.identity) as GameObject;
            obj.transform.SetParent(board.gameObject.transform);

            // assign color
            obj.GetComponent<SpriteRenderer>().color = ColorHelper.GetColorValue(cardData.ColorName);

            // assign color group
            if (cardGroups.ContainsKey(cardData.ColorName))
            {
                cardGroups[cardData.ColorName].Add(obj);
            }
            else
            {
                List<GameObject> group = new List<GameObject>();
                group.Add(obj);
                cardGroups.Add(cardData.ColorName, group);
            }

            // remove the top part if it is a corner card and assign the sprite cornerCard
            if (i == 1 || i == 8 || i == 19 || i == 26)
            {
                obj.GetComponent<SpriteRenderer>().sprite = cornerCard;
                GameObject topCorner = obj.transform.Find("top").gameObject;
                Destroy(topCorner);
            }

            // ROTATE CARD
            // rotate card if it belongs to the left.
            if (i > 8 && i < 19)
            {
                obj.transform.Rotate(new Vector3(0, 0, -90));
            }
            // corner top left
            else if(i == 19)
            {
                obj.transform.Rotate(new Vector3(0, 0, -90));
            }
            // rotate card if it belongs to the top
            else if (i > 19 && i < 26)
            {
                obj.transform.Rotate(new Vector3(0, 0, 180));
            }
            // corner top right
            else if (i == 26)
            {
                obj.transform.Rotate(new Vector3(0, 0, 90));
            }
            // rotate card if it belongs to the right
            else if (i > 26 && i <= 36)
            {
                obj.transform.Rotate(new Vector3(0, 0, 90));
            }
            // END ROTATE

            // assign card name
            GetCardText(obj, i, cardTextParentPrefab, uiCanvas);

            Card card = obj.GetComponent<Card>();
            card.Init(i);

            // add to the list of cards
            cards.Add(card);
        }
    }

    public void ProcessDeedCard(Player currentPlayer, List<Player> players)
    {
        Card card = cards[currentPlayer.CurrentPosition];

        // if deed card,
        //      if no owner, ask to buy
        //      if having owner
        //              if owner is this player, 
        //                      if can build building, ask build
        //              if owner is another player, pay fee.
    }

    public void ProcessChanceCard(Player player, List<Player> players)
    {
        throw new NotImplementedException();
    }

    public void ProcessChestCard(Player player, List<Player> players)
    {
        throw new NotImplementedException();
    }

    // if utility card, ask for buy
    public void ProcessUtilityCard(Player currentPlayer, List<Player> players)
    {
        Card card = cards[currentPlayer.CurrentPosition];

    }


    /**********************************************************************************
     * 
     * UTILITY METHODS
     * 
     *********************************************************************************/

    private void GetCardText(GameObject card, int index, UnityEngine.Object cardTextParentPrefab, GameObject uiCanvas)
    {
        if (index == 1 || index == 8 || index == 19 || index == 26)
        {
            return;
        }

        GameObject obj = Instantiate(cardTextParentPrefab) as GameObject;
        obj.transform.SetParent(uiCanvas.transform, false);
        obj.transform.position = card.transform.position;

        if (index > 1 && index < 8)
        {
            obj.transform.position += new Vector3(0.0f, -0.2f, 0.0f);
        }
        else if (index > 8 && index < 19)
        {
            obj.transform.position += new Vector3(-0.2f, 0.0f, 0.0f);
            obj.transform.Find("CardText").transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
        else if (index > 19 && index < 26)
        {
            obj.transform.position += new Vector3(0.0f, 0.2f, 0.0f);
        }
        else if (index > 26 && index <= 36)
        {
            obj.transform.position += new Vector3(0.2f, 0.0f, 0.0f);
            obj.transform.Find("CardText").transform.localRotation = Quaternion.Euler(Vector3.zero);
        }

        LocalizeTM localizedTMText = obj.transform.Find("CardText").GetComponent<LocalizeTM>();
        localizedTMText.localizationKey = CARD_TITLE_PREFIX + (index);

        localizedTMText.UpdateLocale();
    }

    // Convert the relative position of a child  card of the board to world position.
    private Vector2 GetWorldPositionOfCard(Vector2 parent, Vector2 localPosition)
    {
        return new Vector2(parent.x - Board.X_DIM / 2.0f + localPosition.x,
                            parent.y - Board.Y_DIM / 2.0f + localPosition.y - 0.65f);
    }

    private Dictionary<int, Vector2> GetLocalPositionOfCards()
    {
        Dictionary<int, Vector2> localPostionOfCards = new Dictionary<int, Vector2>();
        float x = 9.0f, y = 0.0f;
        float X, Y;
        for (int i = 1; i <= Board.NUM_CARDS; i++)
        {
            // calculate the local postion for each card.
            if (i == 1)
            {
                x -= 1.5f;
            }
            else if (i > 1 && i < 8)
            {
                x--;
            }
            else if (i == 8)
            {
                x -= 1.5f;
            }
            else if (i == 9)
            {
                y += 1.5f;
            }
            else if (i > 9 && i < 20)
            {
                y++;
            }
            else if (i == 20)
            {
                x += 1.5f;
            }
            else if (i > 20 && i <= 26)
            {
                x++;
            }
            else if (i > 26 && i <= 36)
            {
                y--;
            }

            // adjusting the offsets here
            X = x;
            Y = y;
            // offsetX and offsetY due to rotate card
            if (i > 8 && i < 19)
            {
                X += 0.25f;
                Y -= 0.25f;
            }

            if (i > 26 && i <= 36)
            {
                X += 0.25f;
                Y -= 0.25f;
            }

            // offsetX and offsetY due to move to center of card.
            if (i == 1 || i == 8 || i == 19 || i == 26)
            {
                X += 1.5f / 2.0f;
                Y += 1.5f / 2.0f;
            }
            else
            {
                X += 1.0f / 2.0f;
                Y += 1.5f / 2.0f;
            }

            localPostionOfCards.Add(i, new Vector2(X, Y));
        }

        return localPostionOfCards;
    }
}
