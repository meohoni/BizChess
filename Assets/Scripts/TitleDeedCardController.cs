using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitleDeedCardController : LandCardController
{
    public const int CONSTRUCTION_LEVEL_1 = 1;
    public const int CONSTRUCTION_LEVEL_2 = 2;
    public const int CONSTRUCTION_LEVEL_3 = 3;
    public const int CONSTRUCTION_LEVEL_4 = 4;
    public const int CONSTRUCTION_LEVEL_5 = 5;

    // UI 
    [SerializeField] private GameObject         titleDeedCardInfoPanel;

    [SerializeField] private Sprite             building;
    [SerializeField] private TextMeshProUGUI    valueTxt;
    [SerializeField] private TextMeshProUGUI    morgateTxt;
    [SerializeField] private TextMeshProUGUI    feeBuildHouseTxt;
    [SerializeField] private TextMeshProUGUI    feeBuildHotelTxt;
    [SerializeField] private TextMeshProUGUI[]  landingfeesTxts;

    public void DisplayCardInfo(Card card)
    {
        valueTxt.SetText(String.Format(LocaleHelper.GetMoneyValueString(), card.GetValue()));
        morgateTxt.SetText(String.Format(LocaleHelper.GetMoneyValueString(), card.GetMorgate()));
        feeBuildHouseTxt.SetText(String.Format(LocaleHelper.GetMoneyValueString(), card.GetFeeBuildHouse()));
        feeBuildHotelTxt.SetText(String.Format(LocaleHelper.GetMoneyValueString(), card.GetFeeBuildHotel()));

        for (int i = 0; i < landingfeesTxts.Length; i++)
        {
            landingfeesTxts[i].SetText(String.Format(LocaleHelper.GetMoneyValueString(), card.GetLandingFees()[i]));
        }

        titleDeedCardInfoPanel.SetActive(true);
    }

    public void HideCardInfo()
    {
        titleDeedCardInfoPanel.SetActive(false);
    }

    protected override int GetFee(Card card)
    {
        // if card belongs to a group that belongs to one player, then fee will be doubled.
        return cardController.IsLandGroup(card) ? card.GetLandingFees()[card.ConstructionLevel] * 2 : card.GetLandingFees()[card.ConstructionLevel];
    }

    protected override void DisplayBuilding(Card card)
    {
        Transform parent = card.transform.Find("top");
        // delete old 4 houses if construct hotel
        if (card.ConstructionLevel == CONSTRUCTION_LEVEL_5)
        {
            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }
        }
        GameObject go = new GameObject("house") as GameObject;
        go.transform.parent = parent;
        go.transform.localScale = new Vector3(0.2f, 1.0f, 1.0f);
        go.transform.localRotation = parent.localRotation;

        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = building;
        renderer.sortingLayerName = "TopCard";
        renderer.sortingOrder = 1;
        renderer.color = ColorHelper.GetColorValue(ColorName.GREY);

        switch (card.ConstructionLevel)
        {
            case CONSTRUCTION_LEVEL_1: // first house
                {
                    go.transform.localPosition = new Vector3(-0.36f, 0f, 0f);
                }
                break;
            case CONSTRUCTION_LEVEL_2: // second house
                {
                    go.transform.localPosition = new Vector3(-0.12f, 0f, 0f);
                }
                break;
            case CONSTRUCTION_LEVEL_3: // third house
                {
                    go.transform.localPosition = new Vector3(0.12f, 0f, 0f);
                }
                break;
            case CONSTRUCTION_LEVEL_4: // fourth house
                {
                    go.transform.localPosition = new Vector3(0.36f, 0f, 0f);
                }
                break;
            case CONSTRUCTION_LEVEL_5: // hotel
                {
                    // create hotel
                    go.name = "hotel";
                    go.transform.localScale = new Vector3(0.9f, 0.8f, 1.0f);
                    go.transform.localPosition = new Vector3(0f, 0f, 0f);
                }
                break;
        }
    }
}
