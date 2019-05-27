using UnityEngine;

public class MenuManager : MonoBehaviour
{

    public void SetEnglish()
    {
        Localize.SetCurrentLanguage(SystemLanguage.English);
        LocalizeImage.SetCurrentLanguage();
    }

    public void SetItalian()
    {
        Localize.SetCurrentLanguage(SystemLanguage.Italian);
        LocalizeImage.SetCurrentLanguage();
    }

    public void SetJapanese()
    {
        Localize.SetCurrentLanguage(SystemLanguage.Japanese);
        LocalizeImage.SetCurrentLanguage();
    }

    public static void SetVietnamese()
    {
        Localize.SetCurrentLanguage(SystemLanguage.Vietnamese);
        LocalizeImage.SetCurrentLanguage();
    }
}
