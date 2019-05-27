using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class managing UI art localization. Language specific art shall be saved following this folder structure:
///
///     Resources/Localization/UI/English 
///     Resources/Localization/UI/Italian 
///     Resources/Localization/UI/Japanese
///
/// ... and so on, where the folder name is the string version of the SystemLanguage enumeration.
/// </summary>

[RequireComponent(typeof(Image))]
public class LocalizeImage : MonoBehaviour
{
    public string localizationKey;
    private const string STR_LOCALIZATION_PREFIX = "Localization/UI/";
    private Image image;

    /// <summary>
    /// This is to set the Sprite according to the selected language by code. It update all the Image
    /// components in the scene. This MUST be called AFTER the language has been set in Localize class.
    /// </summary>
    public static void SetCurrentLanguage()
    {
        LocalizeImage[] allTexts = GameObject.FindObjectsOfType<LocalizeImage>();
        for (int i = 0; i < allTexts.Length; i++)
            allTexts[i].UpdateLocale();
    }

    /// <summary>
    /// Update the Sprite of the Image we are attached to. It has a 100ms delay to allow Start operations.
    /// </summary>
    public void UpdateLocale()
    {
        if (!image) return; // catching race condition
        Invoke("_updateLocale", 0.1f);
    }

    /// <summary>
    /// Update the Sprite of the Image we are attached to. If language has not been set yet try again
    /// in 100ms.
    /// </summary>
    private void _updateLocale()
    {
        if (Locale.CurrentLanguageHasBeenSet)
        {
            Sprite tmp = Resources.Load(STR_LOCALIZATION_PREFIX + Locale.PlayerLanguage.ToString() + "/" + localizationKey, typeof(Sprite)) as Sprite;
            if (tmp != null)
                image.sprite = tmp;
            return;
        }
        UpdateLocale();
    }

    private void Start()
    {
        image = GetComponent<Image>();
        UpdateLocale();
    }

}
