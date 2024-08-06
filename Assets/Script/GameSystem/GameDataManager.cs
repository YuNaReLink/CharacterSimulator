using UnityEngine;
using static CharacterManager;

//ゲーム本編のシステムデータを管理する場所
//主に敵の数、待ち時間などを所持
//他にも必要なら追加
public class GameDataManager
{
    private static KeyStates keyStates;
    public static KeyStates GetKeyStates() {  return keyStates; }

    private static DataTag playerTag = DataTag.Null;
    public static DataTag PlayerTag { get { return playerTag; }set { playerTag = value; } }

    public enum GameState
    {
        Null = -1,
        Title,
        NowGame,
        Pause,
        GameClaer,
        GameOver,
        GameEnd,
        DataEnd
    }

    private static GameState currentGameState = GameState.Null;
    public static GameState CurrentGameState {  get { return currentGameState; } set {  currentGameState = value; } }
    
    private static GameState pastGameState = GameState.Null;
    public static GameState PastGameState { get { return pastGameState; }set { pastGameState = value; } }

    public enum GameMode
    {
        Null = -1,
        ZeldaMode,
        RatchetAndClank,
        SuperMario,
        DataEnd
    }
    private static GameMode gameMode = GameMode.Null;
    public static GameMode CurrentGameMode { get { return gameMode; } set { gameMode = value; } }
    
    public static void InitGameData()
    {
        ClearFlag.SetClearFlag(false);
        keyStates = new KeyStates();
    }

    public static bool UpdateGameState()
    {
        if(pastGameState != currentGameState)
        {
            pastGameState = currentGameState;
            return true;
        }
        return false;
    }

    public static void ChangeGameState(GameState state)
    {
        currentGameState = state;
    }
}
