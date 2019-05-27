using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum CardType
{
    DEED,
    COMMUNITY_CHEST,
    CHANCE,
    START,
    VISIT_PRISON,
    PARKING,
    PRISON,
    UTILITY,
    STATION
}

public class Card : MonoBehaviour
{
    private CardType type;

    public CardType Type { get => type; }

    public void Init(int index)
    {
        switch (index)
        {
            case 1:
                {
                    type = CardType.START;
                    break;
                }
            case 8:
                {
                    type = CardType.VISIT_PRISON;
                    break;
                }
            case 19:
                {
                    type = CardType.PARKING;
                    break;
                }
            case 26:
                {
                    type = CardType.PRISON;
                    break;
                }
            default:
                {
                    type = CardType.DEED;
                    break;
                }
        }
    }

}
