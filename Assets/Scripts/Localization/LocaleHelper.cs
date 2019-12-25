using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LocaleHelper
{
    public const string MONEY_VALUE_KEY = "MONEY_VALUE";
    public const string BUY_LAND_KEY = "BUY_LAND";
    public const string BUY_IMPOSSIBLE_KEY = "BUY_IMPOSSIBLE";
    public const string BUILD_HOUSE_KEY = "BUILD_HOUSE";
    public const string BUILD_HOTEL_KEY = "BUILD_HOTEL";
    public const string BUILD_IMPOSSIBLE_LACK_MONEY_KEY = "BUILD_IMPOSSIBLE_LACK_MONEY";
    public const string BUILD_IMPOSSIBLE_MAX_LEVEL_KEY = "BUILD_IMPOSSIBLE_MAX_LEVEL";
    public const string PROPS_KEY = "PROPS";
    public const string PASS_START_KEY = "PASS_START";
    public const string TAP_REQUEST_KEY = "TAP_REQUEST";
    public const string PAY_LANDING_FEE_KEY = "PAY_LANDING_FEE";
    public const string RECEIVE_LANDING_FEE_KEY = "RECEIVE_LANDING_FEE";
    public const string BUILD_SUCCESS_KEY = "BUILD_SUCCESS";
    public const string BUY_SUCCESS_KEY = "BUY_SUCCESS";
    public const string REJECT_PAY_PRISON_KEY = "REJECT_PAY_PRISON";
    public const string REJECT_PASS_START_KEY = "REJECT_PASS_START";
    public const string FREE_SUCCESS_KEY = "FREE_SUCCESS_KEY";
    public const string ASK_USE_FREE_CARD_KEY = "ASK_USE_FREE_CARD";
    public const string ASK_PAY_50_KEY = "ASK_PAY_50";
    public const string FORCE_GET_OUT_PRISON_KEY = "FORCE_GET_OUT_PRISON";

    private static string moneyValueString = "";
 
    public static string GetMoneyValueString()
    {
        if (!System.String.IsNullOrEmpty(moneyValueString))
            return moneyValueString;
        else if (Locale.CurrentLanguageStrings.ContainsKey(MONEY_VALUE_KEY))
        {
            moneyValueString = Locale.CurrentLanguageStrings[MONEY_VALUE_KEY].Replace(@"\n", "" + '\n');
        }
        else
        {
            moneyValueString = "${0}";
        }
        return moneyValueString;
    }

    public static string GetLocalizationValue(string localizationKey)
    {
        if (!System.String.IsNullOrEmpty(localizationKey) && Locale.CurrentLanguageStrings.ContainsKey(localizationKey))
            return Locale.CurrentLanguageStrings[localizationKey].Replace(@"\n", "" + '\n');
        return "";
    }
}
