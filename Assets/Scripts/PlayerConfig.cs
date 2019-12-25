using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConfig
{
    private int id;
    private ColorName colorName;
    private string name;
    private int avatarIdx;
    private bool isAIPlayer;

    public PlayerConfig (int _id, ColorName _colorName, string _name, int _avatarIdx, bool _isAIPlayer)
    {
        id = _id;
        colorName = _colorName;
        name = _name;
        avatarIdx = _avatarIdx;
        isAIPlayer = _isAIPlayer;
    }

    public int Id { get => id; }
    public ColorName ColorName { get => colorName; }
    public string Name { get => name; }
    public int AvatarIdx { get => avatarIdx; }
    public bool IsAIPlayer { get => isAIPlayer; }

}
