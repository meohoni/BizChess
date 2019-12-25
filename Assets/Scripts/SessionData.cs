using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    BUY_LAND,
    BUILD,
    SELL_CARD,
    SELL_HOUSE,
    ASK_USE_FREE_CARD,
    ASK_PAY_50,
    FORCE_GET_OUT_PRISON,
    REJECT
}
public class SessionData
{
    private static Card card;
    private static Player currentPlayer;
    private static List<Player> players;
    private static bool autoClickHasClicked;

    private static ActionType actionType;

    public static ActionType ActionType { get => actionType; set => actionType = value; }
    public static Card Card { get => card; set => card = value; }
    public static Player CurrentPlayer { get => currentPlayer; set => currentPlayer = value; }
    public static List<Player> Players { get => players; set => players = value; }
    public static bool AutoClickHasClicked { get => autoClickHasClicked; set => autoClickHasClicked = value; }

    public static void UpdateSessionData(ActionType _actionType, Card _card, Player _player, List<Player> _players)
    {
        actionType = _actionType;
        card = _card;
        currentPlayer = _player;
        players = _players;
    }


}
