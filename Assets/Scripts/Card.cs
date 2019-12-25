using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum CardType
{
    TITLE_DEED,
    COMMUNITY_CHEST,
    CHANCE,
    START,
    VISIT_PRISON,
    PARKING,
    PRISON,
    UTILITY,
    STATION
}

public class Card : MonoBehaviour
{
    private CardInfo cardInfo;
    private Player owner;
    private int constructionLevel = 0;

    public Player Owner { get => owner; set => owner = value; }
    public int ConstructionLevel { get => constructionLevel; set => constructionLevel = value; }

    public void Init(CardInfo _cardInfo)
    {
        cardInfo = _cardInfo;
    }

    public CardType GetCardType()
    {
        return cardInfo.Type;
    }

    public string GetTitleKey()
    {
        return cardInfo.TitleKey;
    }

    public CardInfo GetCardData()
    {
        return cardInfo;
    }

    public int GetValue()
    {
        return cardInfo.Value;
    }

    public int[] GetLandingFees()
    {
        return cardInfo.LandingFees;
    }

    public int GetMorgate()
    {
        return cardInfo.Morgate;
    }

    public int GetFeeBuildHouse()
    {
        return cardInfo.FeeBuildHouse;
    }

    public int GetFeeBuildHotel()
    {
        return cardInfo.FeeBuildHotel;
    }

    public void OnClick()
    {
        Board board = FindObjectOfType<Board>();
        CardController cc = board.GetComponent<CardController>();
        cc.DisplayCardInfo(this);
    }

}
