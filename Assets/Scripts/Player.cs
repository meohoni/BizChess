using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [SerializeField] private int money;

    private int id;
    private string name;
    private ColorName colorName;
    private GameObject avatar;
    private GameObject token;

    private bool isAIPlayer = false;
    private bool isAutoRoll = false;
    private int currentPosition = 0;
    private bool isMoving = false;
    private IEnumerator moveCoroutine;

    public int Id { get => id; set => id = value; }
    public string Name { get => name; set => name = value; }
    public ColorName ColorName { get => colorName; set => colorName = value; }
    public bool IsAIPlayer { get => isAIPlayer; set => isAIPlayer = value; }
    public bool IsAutoRoll { get => isAutoRoll; set => isAutoRoll = value; }
    public int Money { get => money; set => money = value; }

    public int CurrentPosition { get => currentPosition; }
    public bool IsMoving { get => isMoving; }

    public void Init(int _id, string _name, ColorName _colorName, bool _isAIPlayer, GameObject _token)
    {
        Id = _id;
        Name = _name;
        ColorName = _colorName;
        isAIPlayer = _isAIPlayer;
        isAutoRoll = _isAIPlayer;
        token = _token;
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