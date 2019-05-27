using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeedCard : Card
{
    private ColorName color;
    private string cardName;
    private string descriptionHtml;
    private int constructionLevel;
    private int price;
    private int feeBuildHouse;
    private Dictionary<int, int> landingFees; 

    public ColorName Color { get => color; set => color = value; }
    public string CardName { get => cardName; set => cardName = value; }
    public string DescriptionHtml { get => descriptionHtml; set => descriptionHtml = value; }
    public int ConstructionLevel { get => constructionLevel; set => constructionLevel = value; }
    public int Price { get => price; set => price = value; }
    public int FeeBuildHouse { get => feeBuildHouse; set => feeBuildHouse = value; }
    public Dictionary<int, int> LandingFees { get => landingFees; set => landingFees = value; }

}
