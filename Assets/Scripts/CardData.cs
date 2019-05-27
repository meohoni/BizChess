using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CardData", menuName = "Card Data", order = 51)]
public class CardData : ScriptableObject
{
    [SerializeField] private int id;
    [SerializeField] private CardType type;
    [SerializeField] private ColorName colorName;
    [SerializeField] private string titleKey;
    [SerializeField] private int value;
    [SerializeField] private string descriptionKey;
    [SerializeField] private int feeBuildHouse;
    [SerializeField] private int feeBuildHotel;
    [SerializeField] private int morgate;
    [SerializeField] private int[] landingFees;

    public int Id { get => id; }
    public CardType Type { get => type; }
    public ColorName ColorName { get => colorName; }
    public string TitleKey { get => titleKey; set => titleKey = value; }
    public int Value { get => value; }
    public string DescriptionKey { get => descriptionKey; }
    public int FeeBuildHouse { get => feeBuildHouse; }
    public int FeeBuildHotel { get => feeBuildHotel; }
    public int Morgate { get => morgate; }
    public int[] LandingFees { get => landingFees; }

}
