using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UtilityCardController : LandCardController
{
    [SerializeField] private GameObject utilityCardInfoPanel;
    [SerializeField] private TextMeshProUGUI valueTxt;
    [SerializeField] private TextMeshProUGUI morgateTxt;

    public const int ONE_UTILITY_FACTOR = 4;
    public const int TWO_UTILITY_FACTOR = 10;

    public void DisplayCardInfo(Card card)
    {
        valueTxt.SetText(String.Format(LocaleHelper.GetMoneyValueString(), card.GetValue()));
        morgateTxt.SetText(String.Format(LocaleHelper.GetMoneyValueString(), card.GetMorgate()));

        utilityCardInfoPanel.SetActive(true);
    }

    public void HideCardInfo()
    {
        utilityCardInfoPanel.SetActive(false);
    }

    protected override int GetFee(Card card)
    {
        int numUtilities = cardController.GetNumUtilitiesBelongToPlayer(card.Owner);
        switch (numUtilities)
        {
            case 1:
                {
                    return (Dice.DieANum + Dice.DieBNum) * ONE_UTILITY_FACTOR;
                }
            case 2:
                {
                    return (Dice.DieANum + Dice.DieBNum) * TWO_UTILITY_FACTOR;
                }
        }
        return 0;
    }
}

