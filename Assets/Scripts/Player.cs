using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public enum PropsType
{
    TTILE_DEED,
    STATION,
    UTILITY,
    HOUSE,
    HOTEL,
    FREE_CARD
}

public class PropsTypeComparer : IEqualityComparer<PropsType>
{
    public bool Equals(PropsType x, PropsType y)
    {
        return x == y;
    }

    public int GetHashCode(PropsType obj)
    {
        return (int)obj;
    }
}

public class Player : MonoBehaviour
{
    [SerializeField] private int money;

    private int id;
    private Sprite avatar;
    private string playerName;
    private ColorName colorName;
    private GameObject token;
    private Dictionary<PropsType, int> props;
    
    private bool isAIPlayer = false;
    private bool isAutoRoll = false;
    private int currentPosition = 0;
    private bool isMoving = false;
    private bool isInPrison = false;
    private int numRollDiceSinceInPrison = 0;
    private IEnumerator moveCoroutine;

    public int Id { get => id; set => id = value; }
    public Sprite Avatar { get => avatar; set => avatar = value; }
    public string PlayerName { get => playerName; set => playerName = value; }
    public ColorName ColorName { get => colorName; set => colorName = value; }
    public Dictionary<PropsType, int> Props { get => props; }

    public bool IsAIPlayer { get => isAIPlayer; set => isAIPlayer = value; }
    public bool IsAutoRoll { get => isAutoRoll; set => isAutoRoll = value; }
    public int Money { get => money; set => money = value; }

    public int CurrentPosition { get => currentPosition; }
    public bool IsMoving { get => isMoving; }
    public bool IsInPrison { get => isInPrison; set => isInPrison = value; }
    public int NumRollDiceSinceInPrison { get => numRollDiceSinceInPrison; set => numRollDiceSinceInPrison = value; }

    public void Init(int _id, string _name, ColorName _colorName, Sprite _avatar, bool _isAIPlayer, GameObject _token)
    {
        Id = _id;
        PlayerName = _name;
        ColorName = _colorName;
        avatar = _avatar;
        isAIPlayer = _isAIPlayer;
        isAutoRoll = _isAIPlayer;
        token = _token;
        props = new Dictionary<PropsType, int>();
        props.Add(PropsType.TTILE_DEED, 0);
        props.Add(PropsType.STATION, 0);
        props.Add(PropsType.UTILITY, 0);
        props.Add(PropsType.HOUSE, 0);
        props.Add(PropsType.HOTEL, 0);
        props.Add(PropsType.FREE_CARD, 0);
    }
   
    public void AddTitleDeed()
    {
        AddProperty(PropsType.TTILE_DEED);
    }

    public void RemoveTitleDeed(int num)
    {
        RemoveProperty(PropsType.TTILE_DEED, num);
    }

    public void AddStation()
    {
        AddProperty(PropsType.STATION);
    }

    public void RemoveStation()
    {
        RemoveProperty(PropsType.STATION, 1);
    }

    public void AddUtility()
    {
        AddProperty(PropsType.UTILITY);
    }

    public void RemoveUtility()
    {
        RemoveProperty(PropsType.UTILITY, 1);
    }

    public void AddHouse()
    {
        AddProperty(PropsType.HOUSE);
    }

    public void RemoveHouse(int num)
    {
        RemoveProperty(PropsType.HOUSE, num);
    }

    public void AddHotel()
    {
        // when build a hotel, we must remove 4 existing houses
        AddProperty(PropsType.HOTEL);
        RemoveProperty(PropsType.HOUSE, 4);
    }

    public void RemoveHotel()
    {
        RemoveProperty(PropsType.HOTEL, 1);
    }

    public void AddFreeCard()
    {
        AddProperty(PropsType.FREE_CARD);
    }

    public void RemoveFreeCard()
    {
        RemoveProperty(PropsType.FREE_CARD, 1);
    }

    private void RemoveProperty(PropsType key, int num)
    {
        if (props.ContainsKey(key))
        {
           props[key] -= num;
        }
    }

    private void AddProperty(PropsType key)
    {
        if (!props.ContainsKey(key))
        {
            props.Add(key, 1);
        }
        else
        {
            props[key] += 1;
        }
    }

    public void Move(Vector3 target, bool moveForward, AnimationCurve curve, float moveTime)
    {
        if (isMoving) return;

        isMoving = true;
        if (moveForward)
        {
            if (currentPosition < Board.NUM_CARDS - 1)
            {
                currentPosition++;
            }
            else
            {
                currentPosition = 0;
            }
        }
        else
        {
            if (currentPosition > 0)
            {
                currentPosition--;
            }
            else
            {
                currentPosition = Board.NUM_CARDS - 1;
            }
        }


        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = MoveCoroutine(token.transform.position, target, curve, moveTime);
        StartCoroutine(moveCoroutine);
    }

    private IEnumerator MoveCoroutine(Vector3 startPos, Vector3 endPos, AnimationCurve curve, float moveTime)
    {
        float timer = 0.0f;
        float x = 0.0f, y = 0.0f;
        float eval = 0.0f;
        Vector3 position;
        Vector3 originalScale = token.transform.localScale;
        Vector3 scale = token.transform.localScale;

        while (timer <= moveTime)
        {
            scale = originalScale;
            if(curve != null)
            {
                eval = curve.Evaluate(timer / moveTime);
            }

            x = Mathf.Lerp(startPos.x, endPos.x, timer / moveTime);
            y = Mathf.Lerp(startPos.y, endPos.y, timer / moveTime) + eval;

            scale.x += eval;
            scale.y += eval;
            scale.z += eval;

            position = new Vector3(x, y, token.transform.position.z);
            token.transform.position = position;
            token.transform.localScale = scale;
            timer += Time.deltaTime;

            yield return null;
        }
        token.transform.position = endPos;
        token.transform.localScale = originalScale;
        isMoving = false;
    }

}