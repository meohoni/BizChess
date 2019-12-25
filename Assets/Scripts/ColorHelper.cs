using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// reference to https://htmlcolorcodes.com/color-names/
public enum ColorName
{
    RED,    // RGB(255, 0, 0)
    BLUE,   // RGB(0, 0, 255)
    YELLOW, // RGB(255, 255, 0)
    CYAN,   // RGB(0, 255, 255)
    MAGENTA,// RGB(255, 0, 255)
    GREEN,  // RGB(0, 128, 0)
    ORANGE, // RGB(255, 165, 0)
    PINK,   // RGB(255, 20, 147)
    BROWN,  // RGB(165, 42, 42)
    LIME,   // RGB(0, 255, 0)
    PURPLE, // RGB(128, 0, 128)
    GOLD,   // RGB(255, 215, 0)
    GREY,   // RGB(128, 128, 128)
    NONE,   // RGB(255, 255 , 255)
}

public class ColorNameComparer : IEqualityComparer<ColorName>
{
    public bool Equals(ColorName x, ColorName y)
    {
        return x == y;
    }

    public int GetHashCode(ColorName obj)
    {
        return (int)obj;
    }
}

public class ColorHelper
{
    private static Dictionary<ColorName, Color32> colorDict;

    public static Color32 GetColorValue(ColorName name)
    {
        if (colorDict.ContainsKey(name))
        {
            return colorDict[name];
        }
        return colorDict[ColorName.NONE];
    }

    public static void BuildColorDict()
    {
        if (colorDict != null)
            return;

        colorDict = new Dictionary<ColorName, Color32>(new ColorNameComparer());

        if (!colorDict.ContainsKey(ColorName.RED))
        {
            colorDict.Add(ColorName.RED, new Color32(255, 0, 0, 255));
        }

        if (!colorDict.ContainsKey(ColorName.BLUE))
        {
            colorDict.Add(ColorName.BLUE, new Color32(0, 0, 255, 255));
        }

        if (!colorDict.ContainsKey(ColorName.YELLOW))
        {
            colorDict.Add(ColorName.YELLOW, new Color32(255, 255, 0, 255));
        }

        if (!colorDict.ContainsKey(ColorName.CYAN))
        {
            colorDict.Add(ColorName.CYAN, new Color32(0, 255, 255, 255));
        }

        if (!colorDict.ContainsKey(ColorName.MAGENTA))
        {
            colorDict.Add(ColorName.MAGENTA, new Color32(255, 0, 255, 255));
        }

        if (!colorDict.ContainsKey(ColorName.GREEN))
        {
            colorDict.Add(ColorName.GREEN, new Color32(0, 128, 0, 255));
        }

        if (!colorDict.ContainsKey(ColorName.ORANGE))
        {
            colorDict.Add(ColorName.ORANGE, new Color32(255, 165, 0, 255));
        }

        if (!colorDict.ContainsKey(ColorName.PINK))
        {
            colorDict.Add(ColorName.PINK, new Color32(255, 20, 147, 255));
        }

        if (!colorDict.ContainsKey(ColorName.BROWN))
        {
            colorDict.Add(ColorName.BROWN, new Color32(165, 42, 42, 255));
        }

        if (!colorDict.ContainsKey(ColorName.LIME))
        {
            colorDict.Add(ColorName.LIME, new Color32(0, 255, 0, 255));
        }

        if (!colorDict.ContainsKey(ColorName.PURPLE))
        {
            colorDict.Add(ColorName.PURPLE, new Color32(128, 0, 128, 255));
        }

        if (!colorDict.ContainsKey(ColorName.GOLD))
        {
            colorDict.Add(ColorName.GOLD, new Color32(255, 215, 0, 255));
        }

        if (!colorDict.ContainsKey(ColorName.GREY))
        {
            colorDict.Add(ColorName.GREY, new Color32(128, 128, 128, 255));
        }

        if (!colorDict.ContainsKey(ColorName.NONE))
        {
            colorDict.Add(ColorName.NONE, new Color32(255, 255, 255, 255));
        }
    }
}
