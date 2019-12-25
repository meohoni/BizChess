using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static void SetEnglish()
    {
        Localize.SetCurrentLanguage(SystemLanguage.English);
        LocalizeImage.SetCurrentLanguage();
    }

    public static void SetVietnamese()
    {
        Localize.SetCurrentLanguage(SystemLanguage.Vietnamese);
        LocalizeImage.SetCurrentLanguage();
    }
}
