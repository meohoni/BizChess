using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardController : MonoBehaviour
{
    public const string CARD_TITLE_PREFIX = "CARD_TITLE_";
    public const int CONSTRUCTION_LEVEL = 5;

    [SerializeField] private Sprite cornerCard;

    private List<Card> cards;
    private List<Card> stationCards;
    private List<Card> utilityCards;
    private Dictionary<int, CardInfo> cardInfoDict;
    private Dictionary<ColorName, List<Card>> titleDeedGroups;
    private Board board;

    private TitleDeedCardController titleDeedCardController;
    private UtilityCardController utilityCardController;
    private StationCardController stationCardController;
    private CommunityChestCardController communityChestCardController;
    private ChanceCardController chanceCardController;

    public List<Card> Cards { get => cards; }

    // UI Popup CardInfoPanel
    [SerializeField] private GameObject cardInfoPanel;
    [SerializeField] private TextMeshProUGUI cardTitleTxt;

    private static bool isCardProcessing = false;

    public static bool IsCardProcessing { get => isCardProcessing; set => isCardProcessing = value; }


    void Start()
    {
        board = GetComponent<Board>();
        titleDeedCardController = GetComponent<TitleDeedCardController>();
        utilityCardController = GetComponent<UtilityCardController>();
        stationCardController = GetComponent<StationCardController>();
        communityChestCardController = GetComponent<CommunityChestCardController>();
        chanceCardController = GetComponent<ChanceCardController>();

        cardInfoDict = new Dictionary<int, CardInfo>();
        UnityEngine.Object[] cardDataObjects = Resources.LoadAll("SO/Cards", typeof(CardInfo));

        for (int i = 0; i < cardDataObjects.Length; i++)
        {
            CardInfo ci = (CardInfo)cardDataObjects[i];
            if (!cardInfoDict.ContainsKey(ci.Id))
            {
                cardInfoDict.Add(ci.Id, ci);
            }
        }

        InitCards();
    }

    private void InitCards()
    {
        CardInfo cardInfo;

        cards = new List<Card>();
        stationCards = new List<Card>();
        utilityCards = new List<Card>();
        titleDeedGroups = new Dictionary<ColorName, List<Card>>(new ColorNameComparer());
        UnityEngine.Object cardPrefab = Resources.Load("Prefabs/Card");
        UnityEngine.Object cardTextParentPrefab = Resources.Load("Prefabs/CardTextParent");
        GameObject uiCanvas = GameObject.Find("UICanvas");

        Dictionary<int, Vector2> localPostionOfCards = GetLocalPositionOfCards();

        for (int i = 1; i <= Board.NUM_CARDS; i++)
        {
            int cardId = i;
            if (cardInfoDict.ContainsKey(cardId))
            {
                cardInfo = cardInfoDict[cardId];
            }
            else // log error
            {
                Debug.LogError("Card Data id" + i + " not found.");
                break;
            }

            cardInfo.TitleKey = CARD_TITLE_PREFIX + cardId;

            // create object card.
            Vector2 localPosition = localPostionOfCards[i];
            GameObject obj = Instantiate(cardPrefab, GetWorldPositionOfCard(board.gameObject.transform.position, localPosition), Quaternion.identity) as GameObject;
            obj.transform.SetParent(board.gameObject.transform);

            // assign color
            obj.GetComponent<SpriteRenderer>().color = ColorHelper.GetColorValue(cardInfo.ColorName);

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
            else if (i == 19)
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
            GetCardText(obj, i, cardTextParentPrefab, cardInfo, uiCanvas);

            Card card = obj.GetComponent<Card>();
            card.Init(cardInfo);

            // add to the list of cards
            cards.Add(card);

            // assign to stationCards list
            if (card.GetCardType() == CardType.STATION)
            {
                stationCards.Add(card);
            }

            // assign to utilityCards list
            if (card.GetCardType() == CardType.UTILITY)
            {
                utilityCards.Add(card);
            }

            // assign color group
            if (titleDeedGroups.ContainsKey(cardInfo.ColorName))
            {
                titleDeedGroups[cardInfo.ColorName].Add(card);
            }
            else
            {
                List<Card> group = new List<Card>();
                group.Add(card);
                titleDeedGroups.Add(cardInfo.ColorName, group);
            }
        }
    }

    public void ProcessTitleDeedCard(Player player, List<Player> players)
    {
        Card card = cards[player.CurrentPosition];
        titleDeedCardController.ProcessCard(card, player, players);
    }

    // if utility card, ask for buy
    public void ProcessUtilityCard(Player player, List<Player> players)
    {
        Card card = cards[player.CurrentPosition];
        utilityCardController.ProcessCard(card, player, players);
    }

    public void ProcessStationCard(Player player, List<Player> players)
    {
        Card card = cards[player.CurrentPosition];
        stationCardController.ProcessCard(card, player, players);
    }

    public void ProcessChanceCard(Player player, List<Player> players)
    {

    }

    public void ProcessCommunityChestCard(Player player, List<Player> players)
    {

    }

    public void DisplayCardInfo(Card card)
    {
        //open the appropriated card info panel based on its type.
        // close the others panels.
        if (card.GetCardType() == CardType.TITLE_DEED)
        {
            titleDeedCardController.DisplayCardInfo(card);
            utilityCardController.HideCardInfo();
            stationCardController.HideCardInfo();
        }
        else if (card.GetCardType() == CardType.UTILITY)
        {
            utilityCardController.DisplayCardInfo(card);
            titleDeedCardController.HideCardInfo();
            stationCardController.HideCardInfo();
        }
        else if (card.GetCardType() == CardType.STATION)
        {
            stationCardController.DisplayCardInfo(card);
            titleDeedCardController.HideCardInfo();
            utilityCardController.HideCardInfo();
        } else
        {
            return; // if not above types then do nothing.
        }

        //update title
        LocalizeTM localizedTMText = cardTitleTxt.GetComponent<LocalizeTM>();
        localizedTMText.localizationKey = card.GetTitleKey();
        localizedTMText.UpdateLocale();

        cardInfoPanel.SetActive(true);
    }

    public void OnCloseCardInfo()
    {
        cardInfoPanel.SetActive(false);
    }

    public void OnYesButton()
    {
        if (UIPanelController.HasClicked) return;
        if (SessionData.CurrentPlayer.IsAIPlayer && !SessionData.AutoClickHasClicked) return;

        UIPanelController.HasClicked = true;
        if (SessionData.ActionType == ActionType.BUY_LAND)
        {
            if (SessionData.Card.GetCardType() == CardType.TITLE_DEED)
            {
                titleDeedCardController.BuyLand(SessionData.Card, SessionData.CurrentPlayer, SessionData.Players);
            }
            else if (SessionData.Card.GetCardType() == CardType.STATION)
            {
                stationCardController.BuyLand(SessionData.Card, SessionData.CurrentPlayer, SessionData.Players);
            }
            else if (SessionData.Card.GetCardType() == CardType.UTILITY)
            {
                utilityCardController.BuyLand(SessionData.Card, SessionData.CurrentPlayer, SessionData.Players);
            }
        }
        else if (SessionData.ActionType == ActionType.BUILD)
        {
            if (SessionData.Card.GetCardType() == CardType.TITLE_DEED)
            {
                titleDeedCardController.Build(SessionData.Card, SessionData.CurrentPlayer, SessionData.Players);
            }
        }
    }

    public void OnNoButton()
    {
        if (UIPanelController.HasClicked) return;
        if (SessionData.CurrentPlayer.IsAIPlayer && !SessionData.AutoClickHasClicked) return;

        UIPanelController.HasClicked = true;
        CardController.IsCardProcessing = false;
    }

    // check if a card belongs to a group that belongs to the same onwer
    public bool IsLandGroup(Card card)
    {
        int i = 0;
        if (titleDeedGroups.ContainsKey(card.GetCardData().ColorName))
        {
            foreach (Card c in titleDeedGroups[card.GetCardData().ColorName])
            {
                if(c.Owner != null && c.Owner.Id == card.Owner.Id)
                {
                    i++;
                }
                
            }
        }
        return (i == 3) ? true : false;
    }

    public int GetNumStationsBelongToPlayer(Player player)
    {
        return GetNumCardsBelongToPlayer(player, stationCards);
    }

    public int GetNumUtilitiesBelongToPlayer(Player player)
    {
        return GetNumCardsBelongToPlayer(player, utilityCards);
    }

    private int GetNumCardsBelongToPlayer(Player player, List<Card> _cards)
    {
        int i = 0;
        foreach (Card c in _cards)
        {
            if (c.Owner != null)
            {
                if (c.Owner.Id == player.Id)
                {
                    i++;
                }
            }
        }
        return i;
    }
    /**********************************************************************************
     * 
     * UTILITY METHODS
     * 
     *********************************************************************************/

    private void GetCardText(GameObject card, int index, UnityEngine.Object cardTextParentPrefab, CardInfo cardData, GameObject uiCanvas)
    {
        if (index == 1 || index == 8 || index == 19 || index == 26)
        {
            return;
        }

        GameObject obj = Instantiate(cardTextParentPrefab) as GameObject;
        obj.transform.SetParent(uiCanvas.transform, false);
        Vector3 pos = card.transform.position;
        pos.z = 0.0f;
        obj.transform.position = pos;

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

        TextMeshProUGUI localizedTMText = obj.transform.Find("CardText").GetComponent<TextMeshProUGUI>();
        if(cardData.Type == CardType.TITLE_DEED ||
           cardData.Type == CardType.UTILITY ||
           cardData.Type == CardType.STATION)
        {
            localizedTMText.SetText(String.Format("{0}{1}{2}", LocaleHelper.GetLocalizationValue(cardData.TitleKey), "\n", String.Format(LocaleHelper.GetMoneyValueString(), cardData.Value)));
        }
        else
        {
            localizedTMText.SetText(LocaleHelper.GetLocalizationValue(cardData.TitleKey));
        }
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
