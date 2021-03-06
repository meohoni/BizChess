﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class managing UI text localization. Language specific strings shall be saved following this
/// folder structure:
///
///     Resources/Localization/English.txt 
///     Resources/Localization/Italian.txt 
///     Resources/Localization/Japanese.txt
///
/// ... and so on, where the file name (and consequently the resource name) is the string version of
/// the SystemLanguage enumeration.
///
/// The file format is as follows:
///
///     key=value
///
/// A TAB character is also accepted as key/value separator. 
/// In the value you can use the standard /// notation for newline: \n
/// </summary>

public abstract class LocalizeBase : MonoBehaviour
{
    public string localizationKey;


    /// <summary>
    /// Update the value of the Text we are attached to.
    /// </summary>
    public abstract void UpdateLocale();

    protected virtual void Start()
    {
        // The first Text object getting here inits the CultureInfo data and loads the language file,
        // if any
        //if (!Locale.currentLanguageHasBeenSet)
        //{
        //    SetCurrentLanguage(Locale.PlayerLanguage);
        //}
        UpdateLocale();
    }

    /// <summary>
    /// Returns the localized string for a given key
    /// </summary>
    /// <param name="key">the key to lookup</param>
    /// <returns></returns>
    public static string GetLocalizedString(string key)
    {
        if (Locale.CurrentLanguageStrings.ContainsKey(key))
            return Locale.CurrentLanguageStrings[key];
        else
            return string.Empty;
    }

    /// <summary>
    /// This is to set the language by code. It also update all the Text components in the scene.
    /// </summary>
    /// <param name="language">The new language</param>
    public static void SetCurrentLanguage(SystemLanguage language)
    {
        Locale.CurrentLanguage = language.ToString();
        Locale.PlayerLanguage = language;
        Locale.currentLanguageHasBeenSet = true;
        //Localize[] allTexts = GameObject.FindObjectsOfType<Localize>();
        //LocalizeTM[] allTextsTM = GameObject.FindObjectsOfType<LocalizeTM>();
        //for (int i = 0; i < allTexts.Length; i++)
        //    allTexts[i].UpdateLocale();
        //for (int i = 0; i < allTextsTM.Length; i++)
        //    allTextsTM[i].UpdateLocale();
    }

}
