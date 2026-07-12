using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIMainManager : MonoBehaviour
{
    private IMenu[] m_menuList;

    private GameManager m_gameManager;

    private void Awake()
    {
        m_menuList = GetComponentsInChildren<IMenu>(true);
    }

    void Start()
    {
        for (int i = 0; i < m_menuList.Length; i++)
        {
            m_menuList[i].Setup(this);
        }
    }

    internal void ShowMainMenu()
    {
        m_gameManager.ClearLevel();
        m_gameManager.SetState(GameManager.eStateGame.MAIN_MENU);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_gameManager.State == GameManager.eStateGame.GAME_STARTED)
            {
                m_gameManager.SetState(GameManager.eStateGame.PAUSE);
            }
            else if (m_gameManager.State == GameManager.eStateGame.PAUSE)
            {
                m_gameManager.SetState(GameManager.eStateGame.GAME_STARTED);
            }
        }
    }

    internal void Setup(GameManager gameManager)
    {
        m_gameManager = gameManager;
        m_gameManager.StateChangedAction += OnGameStateChange;
    }

    private void OnGameStateChange(GameManager.eStateGame state)
    {
        switch (state)
        {
            case GameManager.eStateGame.SETUP:
                break;
            case GameManager.eStateGame.MAIN_MENU:
                ShowMenu<UIPanelMain>();
                break;
            case GameManager.eStateGame.GAME_STARTED:
                ShowMenu<UIPanelGame>();
                break;
            case GameManager.eStateGame.PAUSE:
                ShowMenu<UIPanelPause>();
                break;
            case GameManager.eStateGame.GAME_OVER:
                ShowMenu("PanelGameOver");
                break;
            case GameManager.eStateGame.GAME_WIN:
                ShowMenu("PanelWin");
                break;
        }
    }

    private void ShowMenu(string menuObjectName)
    {
        for (int i = 0; i < m_menuList.Length; i++)
        {
            MonoBehaviour menuBehaviour = m_menuList[i] as MonoBehaviour;
            if (menuBehaviour != null && menuBehaviour.gameObject.name == menuObjectName)
                m_menuList[i].Show();
            else
                m_menuList[i].Hide();
        }
    }

    private void ShowMenu<T>() where T : IMenu
    {
        for (int i = 0; i < m_menuList.Length; i++)
        {
            IMenu menu = m_menuList[i];
            if(menu is T)
            {
                menu.Show();
            }
            else
            {
                menu.Hide();
            }            
        }
    }

    internal Text GetLevelConditionView()
    {
        UIPanelGame game = m_menuList.Where(x => x is UIPanelGame).Cast<UIPanelGame>().FirstOrDefault();
        if (game)
        {
            return game.LevelConditionView;
        }

        return null;
    }

    internal void ShowPauseMenu()
    {
        m_gameManager.SetState(GameManager.eStateGame.PAUSE);
    }

    internal void LoadLevelMoves()
    {
        m_gameManager.LoadLevel(GameManager.ePlayMode.MANUAL);
    }

    internal void LoadLevelTimer()
    {
        m_gameManager.LoadLevel(GameManager.ePlayMode.AUTO_WIN);
    }

    internal void LoadAutoWin()
    {
        m_gameManager.LoadLevel(GameManager.ePlayMode.AUTO_WIN);
    }

    internal void LoadAutoLose()
    {
        m_gameManager.LoadLevel(GameManager.ePlayMode.AUTO_LOSE);
    }

    internal void LoadTimeAttack()
    {
        m_gameManager.LoadLevel(GameManager.ePlayMode.TIME_ATTACK);
    }

    internal string GetGameStatus()
    {
        if (m_gameManager.IsTimeAttack)
            return string.Format("TIME: {0:00}\nBOARD: {1}  TRAY: {2}/5",
                Mathf.CeilToInt(m_gameManager.RemainingTime),
                m_gameManager.RemainingItems,
                m_gameManager.TrayCount);

        return string.Format("BOARD: {0}\nTRAY: {1}/5", m_gameManager.RemainingItems, m_gameManager.TrayCount);
    }

    internal void ShowGameMenu()
    {
        m_gameManager.SetState(GameManager.eStateGame.GAME_STARTED);
    }
}
