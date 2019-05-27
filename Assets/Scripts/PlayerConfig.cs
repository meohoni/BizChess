using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConfig
{
    private int id;
    private ColorName colorName;
    private string name;
    private bool isAIPlayer;

    public PlayerConfig (int _id, ColorName _colorName, string _name, bool _isAIPlayer)
    {
        id = _id;
        colorName = _colorName;
        name = _name;
        isAIPlayer = _isAIPlayer;
    }

    public int Id { get => id; }
    public ColorName ColorName { get => colorName; }
    public string Name { get => name; }
    public bool IsAIPlayer { get => isAIPlayer; }
}
