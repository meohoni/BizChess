using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StationCardController : LandCardController
{
    [SerializeField] private GameObject stationCardInfoPanel;

    [SerializeField] private TextMeshProUGUI valueTxt;
    [SerializeField] private TextMeshProUGUI morgateTxt;
    [SerializeField] private TextMeshProUGUI[] landingfeesTxts;

    public void DisplayCardInfo(Card card)
    {
        valueTxt.SetText(String.Format(LocaleHelper.GetMoneyValueString(), card.GetValue()));
        morgateTxt.SetText(String.Format(LocaleHelper.GetMoneyValueString(), card.GetMorgate()));

        for (int i = 0; i < landingfeesTxts.Length; i++)
        {
            landingfeesTxts[i].SetText(String.Format(LocaleHelper.GetMoneyValueString(), card.GetLandingFees()[i]));
        }

        stationCardInfoPanel.SetActive(true);
    }

    public void HideCardInfo()
    {
        stationCardInfoPanel.SetActive(false);
    }

    protected override int GetFee(Card card)
    {
        int numStations = cardController.GetNumStationsBelongToPlayer(card.Owner);
        return (numStations > 0) ? card.GetLandingFees()[numStations - 1] : 0 ;
    }
}
