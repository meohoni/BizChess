using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Land Card includes Title Deed, Station, Utility Card.
public abstract class LandCardController : MonoBehaviour
{
    // UIPanelController
    private UIPanelController uiPanelController;
    protected CardController cardController;

    protected void Start()
    {
        uiPanelController = GetComponent<UIPanelController>();
        cardController = GetComponent<CardController>();
    }

    public void ProcessCard(Card card, Player player, List<Player> players)
    {
        if (CardController.IsCardProcessing) return;

        CardController.IsCardProcessing = true;
        // if no owner 
        if (card.Owner == null)
        {
            RequestBuyLand(card, player, players);
        }
        else if (card.Owner.Id == player.Id) // owner is this player
        { 
            if (card.GetCardType() == CardType.TITLE_DEED) // if a land
            {
                RequestBuild(card, player, players);
            }
            else
            {
                // return 
                CardController.IsCardProcessing = false;
            }
        }
        else if (card.Owner.Id != player.Id) //owner is another player
        {
            // if owner is not in jail
            // pay fee.
            if (!card.Owner.IsInPrison)
            {
                PayLandFee(player, card.Owner, GetFee(card));
            }
            else
            {
                // reject pay land fee
                StartCoroutine(DoRequest(false, null, null, null, ActionType.REJECT, null, LocaleHelper.GetLocalizationValue(LocaleHelper.REJECT_PAY_PRISON_KEY)));
            }
        }
    }
 
    private void RequestBuyLand(Card card, Player player, List<Player> players)
    {
        bool condition = false;
        // still have enough money, ask to buy or build
        if (player.Money > card.GetValue())
        {
            condition = true;
        }
           
        StartCoroutine(DoRequest(condition, card, player, players, ActionType.BUY_LAND, String.Format(LocaleHelper.GetLocalizationValue(LocaleHelper.BUY_LAND_KEY), card.GetValue()), LocaleHelper.GetLocalizationValue(LocaleHelper.BUY_IMPOSSIBLE_KEY)));
    }

    private void RequestBuild(Card card, Player player, List<Player> players)
    {
        string rejectMsg = "";
        string askMsg = "";
        bool condition = false;
        if (card.ConstructionLevel >= CardController.CONSTRUCTION_LEVEL)
        {
            condition = false;
            rejectMsg = LocaleHelper.GetLocalizationValue(LocaleHelper.BUILD_IMPOSSIBLE_MAX_LEVEL_KEY);
        }
        else {
            // build hotel
            if ((card.ConstructionLevel + 1) == CardController.CONSTRUCTION_LEVEL)
            {
                if (player.Money > card.GetFeeBuildHotel())
                {
                    condition = true;
                    askMsg = String.Format(LocaleHelper.GetLocalizationValue(LocaleHelper.BUILD_HOTEL_KEY), card.GetFeeBuildHotel());
                }
                else
                {
                    condition = false;
                    rejectMsg = LocaleHelper.GetLocalizationValue(LocaleHelper.BUILD_IMPOSSIBLE_LACK_MONEY_KEY);
                }
            }
            else // build house
            {
                if (player.Money > card.GetFeeBuildHotel())
                {
                    condition = true;
                    askMsg = String.Format(LocaleHelper.GetLocalizationValue(LocaleHelper.BUILD_HOUSE_KEY), card.GetFeeBuildHouse());;
                }
                else
                {
                    condition = false;
                    rejectMsg = LocaleHelper.GetLocalizationValue(LocaleHelper.BUILD_IMPOSSIBLE_LACK_MONEY_KEY);
                }
            }      
           
        }

        StartCoroutine(DoRequest(condition, card, player, players, ActionType.BUILD, askMsg, rejectMsg));
    }

    private IEnumerator DoRequest(bool condition, Card card, Player player, List<Player> players, ActionType actionType, string askMsg, string rejectMsg)
    {
        // meet condition, ask to buy or build
        if (condition)
        {
            uiPanelController.SetMessage(askMsg);
            // keep references in SessionData
            SessionData.UpdateSessionData(actionType, card, player, players);

            if (player.IsAIPlayer)
            {
                yield return new WaitForSeconds(1.0f);
                uiPanelController.NoBtn.interactable = false;
                // display 2 buttons Yes, No
                uiPanelController.DisplayYesButton();
                uiPanelController.DisplayNoButton();
                yield return new WaitForSeconds(1.0f);
                uiPanelController.YesBtn.OnPointerDown(new PointerEventData(EventSystem.current));
                yield return new WaitForSeconds(1.2f);
                SessionData.AutoClickHasClicked = true;
                uiPanelController.YesBtn.onClick.Invoke();
                SessionData.AutoClickHasClicked = false;
                uiPanelController.NoBtn.interactable = true;
            }
            else
            {
                // display 2 buttons Yes, No
                uiPanelController.DisplayYesButton();
                uiPanelController.DisplayNoButton();
            }
        }
        else
        {
            // print message reject buy or build
            uiPanelController.SetMessage(rejectMsg);
            yield return new WaitForSeconds(2.0f);
            CardController.IsCardProcessing = false;
       }
    }

    public void Build(Card card, Player player, List<Player> players)
    {
        StartCoroutine(BuildCoroutine(card, player, players));
    }

    private IEnumerator BuildCoroutine(Card card, Player player, List<Player> players)
    {
        card.ConstructionLevel++;

        if (card.ConstructionLevel < TitleDeedCardController.CONSTRUCTION_LEVEL_5)
        {
            // subtract player money
            uiPanelController.PayFeeWithNoOpponent(player, -1 * card.GetFeeBuildHouse(), LocaleHelper.GetLocalizationValue(LocaleHelper.BUILD_SUCCESS_KEY));
            // add props
            player.AddHouse();
        }
        else
        {
            // subtract player money
            uiPanelController.PayFeeWithNoOpponent(player, -1 * card.GetFeeBuildHotel(), LocaleHelper.GetLocalizationValue(LocaleHelper.BUILD_SUCCESS_KEY));
            // add props
            player.AddHotel();
        }

        uiPanelController.HideNoButton();
        uiPanelController.HideYesButton();
        uiPanelController.UpdateProps(player);

        // update top.
        DisplayBuilding(card);

        while (UIPanelController.IsPayFeeProcessing)
        {
            yield return null;
        }
        CardController.IsCardProcessing = false;
    }

    public void BuyLand(Card card, Player player, List<Player> players)
    {
        StartCoroutine(BuyLandCoroutine(card, player, players));
    }

    private IEnumerator BuyLandCoroutine(Card card, Player player, List<Player> players)
    {
        // subtract player money
        uiPanelController.PayFeeWithNoOpponent(player, -1 * card.GetValue(), LocaleHelper.GetLocalizationValue(LocaleHelper.BUY_SUCCESS_KEY));
        uiPanelController.HideNoButton();
        uiPanelController.HideYesButton();

        // update owner
        card.Owner = player;

        // update color for top.
        card.transform.Find("top").GetComponent<SpriteRenderer>().color = ColorHelper.GetColorValue(player.ColorName);

        // add props
        player.AddTitleDeed();
        uiPanelController.UpdateProps(player);

        while (UIPanelController.IsPayFeeProcessing)
        {
            yield return null;
        }
        CardController.IsCardProcessing = false;
    }

    private void PayLandFee(Player payer, Player payee, int fee)
    {
        if (UIPanelController.IsPayFeeProcessing) return;
        StartCoroutine(PayLandFeeCoroutine(payer, payee, fee));
    }

    private IEnumerator PayLandFeeCoroutine(Player payer, Player payee, int fee)
    {
        string msg = String.Format(LocaleHelper.GetLocalizationValue(LocaleHelper.PAY_LANDING_FEE_KEY), fee);
        uiPanelController.PayFeeWithOpponent(payer, payee, fee, msg);
        uiPanelController.HideNoButton();
        uiPanelController.HideYesButton();
        while (UIPanelController.IsPayFeeProcessing)
        {
            yield return null;
        }
        CardController.IsCardProcessing = false;
    }

    protected virtual void DisplayBuilding(Card card){}
    protected abstract int GetFee(Card card);
}
