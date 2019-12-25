using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI msg;

    [SerializeField] private GameObject bigAvt1;
    [SerializeField] private TextMeshProUGUI nameBigAvt1;
    [SerializeField] private TextMeshProUGUI moneyBigAvt1;

    [SerializeField] private GameObject bigAvt2;
    [SerializeField] private TextMeshProUGUI nameBigAvt2;
    [SerializeField] private TextMeshProUGUI moneyBigAvt2;

    [SerializeField] private TextMeshProUGUI propsText;

    [SerializeField] private Button yesBtn;
    [SerializeField] private Button noBtn;

    [SerializeField] private GameObject coin;
    [SerializeField] private TextMeshProUGUI feeLeft;
    [SerializeField] private TextMeshProUGUI feeRight;

    private Image imgBigAvt1;
    private Image imgBigAvt2;

    private static bool hasClicked = false;
    private static bool isPayFeeProcessing = false;
    private static bool isDisplayMessageProcessing = false;
    private static bool isAskPay50Processing = false;
    private static bool isUseFreeCardProcessing = false;

    private bool isCoinMoving = false;
    private bool isFeeLeftMoving = false;
    private bool isFeeRightMoving = false;

    private IEnumerator displayTapMsgCoroutine;
    private IEnumerator moveCoinCoroutine;
    private IEnumerator moveFeeLeftCoroutine;
    private IEnumerator moveFeeRightCoroutine;

    private Vector3 initialPosition;
    private Vector3 coinPositionLeft;
    private Vector3 coinPositionRight;

    private Vector3 feeLeftPosition;
    private Vector3 feeRightPosition;
    private Vector3 feeLeftTargetPosition;
    private Vector3 feeRightTargetPosition;

    public static bool HasClicked { get => hasClicked; set => hasClicked = value; }
    public static bool IsPayFeeProcessing { get => isPayFeeProcessing; set => isPayFeeProcessing = value; }
    public static bool IsDisplayMessageProcessing { get => isDisplayMessageProcessing; set => isDisplayMessageProcessing = value; }
    public static bool IsAskPay50Processing { get => isAskPay50Processing; set => isAskPay50Processing = value; }
    public static bool IsUseFreeCardProcessing { get => isUseFreeCardProcessing; set => isUseFreeCardProcessing = value; }
    public Button YesBtn { get => yesBtn; }
    public Button NoBtn { get => noBtn; }

    // Start is called before the first frame update
    void Start()
    {
        imgBigAvt1 = bigAvt1.GetComponent<Image>();
        imgBigAvt2 = bigAvt2.GetComponent<Image>();

        initialPosition = coin.transform.position;
        coinPositionLeft = moneyBigAvt1.gameObject.transform.position + new Vector3(0.6f, 0, 0);
       // print("x=" + coinPositionLeft.x + ", y=" + coinPositionLeft.y + ", z=" + coinPositionLeft.z);
        coinPositionRight = moneyBigAvt2.gameObject.transform.position + new Vector3(-0.6f, 0, 0); ;
        // print("x=" + coinPositionRight.x + ", y=" + coinPositionRight.y + ", z=" + coinPositionRight.z);
        feeLeftPosition = moneyBigAvt1.gameObject.transform.position + new Vector3(0.3f, 0.6f, 0);
        feeRightPosition = moneyBigAvt2.gameObject.transform.position + new Vector3(-0.3f, 0.6f, 0);
        feeLeftTargetPosition = feeLeftPosition + new Vector3(0, 0.6f, 0);
        feeRightTargetPosition = feeRightPosition + new Vector3(0, 0.6f, 0);
    }

    public void SetMessage(string text)
    {
        msg.SetText(text);        
    }

    public void ClearMsg()
    {
        msg.SetText("");
    }

    public void DisplayYesButton()
    {
        yesBtn.gameObject.SetActive(true);
    }

    public void DisplayNoButton()
    {
        noBtn.gameObject.SetActive(true);
    }

    public void DisableYesButton()
    {
        yesBtn.interactable = false;
    }

    public void DisableNoButton()
    {
        noBtn.interactable = false;
    }

    public void HideYesButton()
    {
        yesBtn.gameObject.SetActive(false);
    }

    public void HideNoButton()
    {
        noBtn.gameObject.SetActive(false);
    }

    public void UpdateProps(Player player)
    {
        propsText.SetText(String.Format(LocaleHelper.GetLocalizationValue(LocaleHelper.PROPS_KEY),
                        player.Props[PropsType.TTILE_DEED],
                        player.Props[PropsType.STATION],
                        player.Props[PropsType.UTILITY],
                        player.Props[PropsType.HOUSE],
                        player.Props[PropsType.HOTEL],
                        player.Props[PropsType.FREE_CARD]
                        ));
    }

    public void DisplayMessage(string msg)
    {
        if (isDisplayMessageProcessing) return;
        isDisplayMessageProcessing = true;
        StartCoroutine(DisplayMessageCoroutine(msg));
    }

    private IEnumerator DisplayMessageCoroutine(string msg)
    {
        SetMessage(msg);
        yield return new WaitForSeconds(2.0f);
        isDisplayMessageProcessing = false;
    }

    public void DisplayTapMsg()
    {
        StopDisplayTapMsgCoroutine();
        displayTapMsgCoroutine = DisplayTapMsgCoroutine();
        StartCoroutine(displayTapMsgCoroutine);
    }

    private IEnumerator DisplayTapMsgCoroutine()
    {
        string text = LocaleHelper.GetLocalizationValue(LocaleHelper.TAP_REQUEST_KEY);
        while (true)
        {
            //set the Text's text to blank
            msg.SetText("");
            //display blank text for 0.5 seconds
            yield return new WaitForSeconds(.5f);
            //display text for the next 0.5 seconds
            msg.SetText(text);
            yield return new WaitForSeconds(.5f);
        }     
    }

    public void StopDisplayTapMsgCoroutine()
    {
        if (displayTapMsgCoroutine != null)
        {
            StopCoroutine(displayTapMsgCoroutine);
        }
    }

    // update only player money
    public void UpdateMoney(Player player, bool isMainPlayer)
    {
        if (isMainPlayer)
        {
            moneyBigAvt1.SetText(String.Format(LocaleHelper.GetMoneyValueString(), player.Money));
        }
        else
        {
            moneyBigAvt2.SetText(String.Format(LocaleHelper.GetMoneyValueString(), player.Money));
        }
    }

    // update whole player including name, money, properties
    public void UpdatePlayer(Player player, bool isMainPlayer)
    {
        // update main player
        if (isMainPlayer)
        {
            nameBigAvt1.SetText(player.PlayerName);
            moneyBigAvt1.SetText(String.Format(LocaleHelper.GetMoneyValueString(), player.Money));
            imgBigAvt1.sprite = player.Avatar;
            propsText.SetText(String.Format(LocaleHelper.GetLocalizationValue(LocaleHelper.PROPS_KEY), 
                                    player.Props[PropsType.TTILE_DEED], 
                                    player.Props[PropsType.STATION],
                                    player.Props[PropsType.UTILITY],
                                    player.Props[PropsType.HOUSE],
                                    player.Props[PropsType.HOTEL],
                                    player.Props[PropsType.FREE_CARD]
                                    ));
            // hide secondary player
            bigAvt2.SetActive(false);
            // display props text
            propsText.gameObject.SetActive(true);
        }
        else // update secondary player
        {
            nameBigAvt2.SetText(player.PlayerName);
            moneyBigAvt2.SetText(String.Format(LocaleHelper.GetMoneyValueString(), player.Money));
            imgBigAvt2.sprite = player.Avatar;
            // show secondary player
            bigAvt2.SetActive(true);
            // hide props text
            propsText.gameObject.SetActive(false);
        }
    }

    public void ResetMsgBox()
    {
        msg.SetText("");
        yesBtn.gameObject.SetActive(false);
        noBtn.gameObject.SetActive(false);
    }

    public void AskPay50()
    {
        isAskPay50Processing = true;
        DisplayYesButton();
        DisplayNoButton();
        SetMessage(LocaleHelper.ASK_PAY_50_KEY);
    }

    public void AskUseFreeCard()
    {
        isUseFreeCardProcessing = true;
        DisplayYesButton();
        DisplayNoButton();
        SetMessage(LocaleHelper.ASK_USE_FREE_CARD_KEY);
    }

    public void OnStart()
    {
        yesBtn.gameObject.SetActive(false);
        noBtn.gameObject.SetActive(false);
        DisplayMessage(LocaleHelper.GetLocalizationValue(LocaleHelper.REJECT_PASS_START_KEY));
    }

    public void PassStart(Player player)
    {
        PayFeeWithNoOpponent(player, PlayerController.PASS_START_AMOUNT, LocaleHelper.GetLocalizationValue(LocaleHelper.PASS_START_KEY));
    }

    public void PayFeeWithNoOpponent(Player payer, int fee, string msg)
    {
        if (isPayFeeProcessing) return;
        isPayFeeProcessing = true;
        StartCoroutine(PayFeeWithNoOpponentCoroutine(payer, fee, msg));
    }

    private IEnumerator PayFeeWithNoOpponentCoroutine(Player payer, int fee, string msg)
    {
        if(payer.Money < fee)
        {
            yield return StartCoroutine(SellProperties(payer, fee));
        }

        // display message
        if (!String.IsNullOrEmpty(msg))
        {
            SetMessage(msg);
        }

        // add fee for payer
        payer.Money += fee;

        yield return new WaitForSeconds(0.6f);
        // update money for payer
        UpdateMoney(payer, true);

        if(fee >= 0)
        {
            feeLeft.SetText("+" + String.Format(LocaleHelper.GetMoneyValueString(), fee));
        }
        else
        {
            feeLeft.SetText("-" + String.Format(LocaleHelper.GetMoneyValueString(), fee));
        }

        MoveFee(true, 1.5f);

        yield return new WaitForSeconds(2.0f);
        isPayFeeProcessing = false;
    }

    public void PayFeeWithOpponent(Player payer, Player payee, int fee, string msg)
    {
        if (isPayFeeProcessing) return;
        isPayFeeProcessing = true;
        StartCoroutine(PayFeeWithOpponentCoroutine(payer, payee, fee, msg));
    }

    private IEnumerator PayFeeWithOpponentCoroutine(Player payer, Player payee, int fee, string msg)
    {
        // display message
        if (!String.IsNullOrEmpty(msg))
        {
            SetMessage(msg);
            yield return new WaitForSeconds(0.6f);
        }

        // if payer or payee is null, it means it's the bank.
        // display bank 
        // if payer is null, it means the bank is the payer, payee is player
        if(payer == null)
        {
            // moving coin from right to left
            MoveCoin(false, 1.2f);

            // plus fee for payee, bank pay for payee
            payee.Money += fee;

            yield return new WaitForSeconds(1.2f);
            // update money for payee
            UpdateMoney(payee, true);

            feeLeft.SetText("+" + String.Format(LocaleHelper.GetMoneyValueString(), fee));
            feeRight.SetText("-" + String.Format(LocaleHelper.GetMoneyValueString(), fee));
        }
        else if(payee == null)
        {
            // if payee is null, it means the bank is the payee, the payer is player
            // moving coin from left to right
            MoveCoin(true, 1.2f);

            // subtract fee for payer
            payer.Money -= fee;

            yield return new WaitForSeconds(1.2f);
            // update money for payer
            UpdateMoney(payer, true);
               
            feeLeft.SetText("-" + String.Format(LocaleHelper.GetMoneyValueString(), fee));
            feeRight.SetText("+" + String.Format(LocaleHelper.GetMoneyValueString(), fee));
        }
        else
        {
            // display player opponent with old score
            UpdatePlayer(payee, false);
            yield return new WaitForSeconds(0.5f);

            // moving coin from left to right
            MoveCoin(true, 1.2f);

            // subtract fee for payer
            payer.Money -= fee;
            // add fee for opponent
            payee.Money += fee;

            yield return new WaitForSeconds(1.2f);
            // update money for payer
            UpdateMoney(payer, true);
            UpdateMoney(payee, false);

            feeLeft.SetText("-" + String.Format(LocaleHelper.GetMoneyValueString(), fee));
            feeRight.SetText("+" + String.Format(LocaleHelper.GetMoneyValueString(), fee));
        }

        MoveFee(true, 1.5f);
        MoveFee(false, 1.5f);

        yield return new WaitForSeconds(2.0f);
        isPayFeeProcessing = false;
    }

    private IEnumerator SellProperties(Player payer, int fee)
    {

        yield return null;
    }

    private void MoveCoin(bool moveLeft, float moveTime)
    {
        if (isCoinMoving) return;

        isCoinMoving = true;

        if (moveCoinCoroutine != null)
        {
            StopCoroutine(moveCoinCoroutine);
        }

        if (moveLeft)
        {
            moveCoinCoroutine = MoveObjectCoroutine(coin, coinPositionLeft, coinPositionRight, moveTime, (o) => isCoinMoving = o);
        }
        else
        {
            moveCoinCoroutine = MoveObjectCoroutine(coin, coinPositionRight, coinPositionLeft, moveTime, (o) => isCoinMoving = o);
        }

        StartCoroutine(moveCoinCoroutine);
    }

    private void MoveFee(bool moveFeeOnLeft, float moveTime)
    {
        if (moveFeeOnLeft && moveFeeLeftCoroutine != null)
        {
            StopCoroutine(moveFeeLeftCoroutine);
        } else if (!moveFeeOnLeft && moveFeeRightCoroutine != null)
        {
            StopCoroutine(moveFeeRightCoroutine);
        }
        if (moveFeeOnLeft)
        {
            moveFeeLeftCoroutine = MoveObjectCoroutine(feeLeft.gameObject, feeLeftPosition, feeLeftTargetPosition, moveTime, (o) => isFeeLeftMoving = o);
            StartCoroutine(moveFeeLeftCoroutine);
        }
        else
        {
            moveFeeRightCoroutine = MoveObjectCoroutine(feeRight.gameObject, feeRightPosition, feeRightTargetPosition, moveTime, (o) => isFeeRightMoving = o);
            StartCoroutine(moveFeeRightCoroutine);
        }       
    }

    private IEnumerator MoveObjectCoroutine(GameObject obj, Vector3 startPos, Vector3 endPos, float moveTime, System.Action<bool> output)
    {
        float timer = 0.0f;
        float x = 0.0f, y = 0.0f;
        Vector3 position;
        Vector3 originalScale = coin.transform.localScale;

        while (timer <= moveTime)
        {
            x = Mathf.Lerp(startPos.x, endPos.x, timer / moveTime);
            y = Mathf.Lerp(startPos.y, endPos.y, timer / moveTime);

            position = new Vector3(x, y, obj.transform.position.z);
            obj.transform.position = position;
            timer += Time.deltaTime;

            yield return null;
        }
        obj.transform.position = endPos;
        // initial position is a position outside the screen play, where the object is cached.
        obj.transform.position = initialPosition;
        // false mean no more moving, or moving is false
        output(false);
    }

}
