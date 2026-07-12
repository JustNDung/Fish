using DG.Tweening;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action<eStateGame> StateChangedAction = delegate { };

    public enum eLevelMode { TIMER, MOVES }
    public enum ePlayMode { MANUAL, AUTO_WIN, AUTO_LOSE, TIME_ATTACK }
    public enum eStateGame { SETUP, MAIN_MENU, GAME_STARTED, PAUSE, GAME_OVER, GAME_WIN }

    private eStateGame m_state;
    public eStateGame State
    {
        get { return m_state; }
        private set
        {
            m_state = value;
            StateChangedAction(m_state);
        }
    }

    public int RemainingItems { get { return m_boardController == null ? 0 : m_boardController.RemainingItems; } }
    public int TrayCount { get { return m_boardController == null ? 0 : m_boardController.TrayCount; } }
    public float RemainingTime { get { return m_boardController == null ? 0f : m_boardController.RemainingTime; } }
    public bool IsTimeAttack { get; private set; }

    private GameSettings m_gameSettings;
    private BoardController m_boardController;
    private UIMainManager m_uiMenu;

    private void Awake()
    {
        State = eStateGame.SETUP;
        m_gameSettings = Resources.Load<GameSettings>(Constants.GAME_SETTINGS_PATH);
        m_uiMenu = FindObjectOfType<UIMainManager>();
        m_uiMenu.Setup(this);
    }

    private void Start()
    {
        State = eStateGame.MAIN_MENU;
    }

    private void Update()
    {
        if (m_boardController != null) m_boardController.Update();
    }

    internal void SetState(eStateGame state)
    {
        State = state;
        if (State == eStateGame.PAUSE) DOTween.PauseAll();
        else DOTween.PlayAll();
    }

    public void LoadLevel(ePlayMode playMode)
    {
        ClearLevel();
        IsTimeAttack = playMode == ePlayMode.TIME_ATTACK;
        m_boardController = new GameObject("BoardController").AddComponent<BoardController>();
        m_boardController.StartGame(this, m_gameSettings, playMode);
        State = eStateGame.GAME_STARTED;
    }

    // Keeps the original public API available for any old scene button/event.
    public void LoadLevel(eLevelMode unusedMode)
    {
        LoadLevel(ePlayMode.MANUAL);
    }

    public void FinishGame(bool won)
    {
        State = won ? eStateGame.GAME_WIN : eStateGame.GAME_OVER;
    }

    internal void ClearLevel()
    {
        if (m_boardController == null) return;
        m_boardController.Clear();
        Destroy(m_boardController.gameObject);
        m_boardController = null;
        IsTimeAttack = false;
    }
}
