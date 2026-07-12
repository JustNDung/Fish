using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Controls the tap-to-tray version of the game. Items never return to the board.
/// </summary>
public class BoardController : MonoBehaviour
{
    // Kept for compatibility with the template's unused move-counter component.
    public event Action OnMoveEvent = delegate { };
    public const int TrayCapacity = 5;
    public const float AutoPlayDelay = 0.5f;

    public bool IsBusy { get; private set; }
    public int TrayCount { get { return m_tray.Count; } }
    public int RemainingItems { get { return m_board == null ? 0 : m_board.RemainingItems; } }

    private readonly List<Item> m_tray = new List<Item>();
    private readonly List<Transform> m_trayCells = new List<Transform>();
    private Board m_board;
    private GameManager m_gameManager;
    private Camera m_camera;
    private bool m_gameOver;
    private GameManager.ePlayMode m_playMode;

    public void StartGame(GameManager gameManager, GameSettings settings, GameManager.ePlayMode playMode)
    {
        m_gameManager = gameManager;
        m_playMode = playMode;
        m_camera = Camera.main;
        m_board = new Board(transform, settings);
        m_board.FillForTripleMatch();
        CreateTray();

        if (playMode != GameManager.ePlayMode.MANUAL)
        {
            StartCoroutine(playMode == GameManager.ePlayMode.AUTO_WIN
                ? AutoWinCoroutine()
                : AutoLoseCoroutine());
        }
    }

    public void Update()
    {
        if (m_gameManager.State != GameManager.eStateGame.GAME_STARTED ||
            m_gameOver || IsBusy || m_playMode != GameManager.ePlayMode.MANUAL) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 point = m_camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(point, Vector2.zero);
            if (hit.collider != null)
            {
                Cell cell = hit.collider.GetComponent<Cell>();
                if (cell != null && !cell.IsEmpty) MoveToTray(cell);
            }
        }
    }

    private void CreateTray()
    {
        GameObject backgroundPrefab = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);
        const float y = -3.75f;
        for (int i = 0; i < TrayCapacity; i++)
        {
            GameObject trayCell = Instantiate(backgroundPrefab, transform);
            trayCell.name = "TrayCell_" + (i + 1);
            trayCell.transform.position = new Vector3(i - (TrayCapacity - 1) * 0.5f, y, 0f);
            Collider2D collider = trayCell.GetComponent<Collider2D>();
            if (collider != null) collider.enabled = false;
            m_trayCells.Add(trayCell.transform);
        }
    }

    private bool MoveToTray(Cell cell)
    {
        if (cell == null || cell.IsEmpty || m_tray.Count >= TrayCapacity || m_gameOver) return false;

        Item item = cell.Item;
        cell.Free();
        OnMoveEvent();
        m_tray.Add(item);
        item.SetViewRoot(transform);
        item.SetSortingLayerHigher();
        item.View.DOKill();
        item.View.DOMove(m_trayCells[m_tray.Count - 1].position, 0.25f);

        ResolveTray(item);
        return true;
    }

    private void ResolveTray(Item movedItem)
    {
        List<Item> identical = m_tray.Where(item => item.IsSameType(movedItem)).ToList();
        if (identical.Count == 3)
        {
            IsBusy = true;
            StartCoroutine(ClearMatchCoroutine(identical));
            return;
        }

        CheckEndState();
    }

    private IEnumerator ClearMatchCoroutine(List<Item> match)
    {
        yield return new WaitForSeconds(0.27f);
        foreach (Item item in match)
        {
            m_tray.Remove(item);
            item.ExplodeView();
        }

        for (int i = 0; i < m_tray.Count; i++)
        {
            m_tray[i].View.DOMove(m_trayCells[i].position, 0.2f);
        }

        yield return new WaitForSeconds(0.21f);
        IsBusy = false;
        CheckEndState();
    }

    private void CheckEndState()
    {
        if (m_board.RemainingItems == 0 && m_tray.Count == 0)
        {
            Finish(true);
        }
        else if (m_tray.Count >= TrayCapacity)
        {
            Finish(false);
        }
    }

    private void Finish(bool won)
    {
        if (m_gameOver) return;
        m_gameOver = true;
        StopAllCoroutines();
        m_gameManager.FinishGame(won);
    }

    private IEnumerator AutoWinCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        while (!m_gameOver && m_board.RemainingItems > 0)
        {
            Cell first = m_board.GetOccupiedCells().FirstOrDefault();
            if (first == null) break;
            NormalItem.eNormalType type = ((NormalItem)first.Item).ItemType;
            List<Cell> triple = m_board.GetOccupiedCells()
                .Where(cell => ((NormalItem)cell.Item).ItemType == type)
                .Take(3).ToList();

            foreach (Cell cell in triple)
            {
                while (IsBusy) yield return null;
                MoveToTray(cell);
                yield return new WaitForSeconds(AutoPlayDelay);
            }
        }
    }

    private IEnumerator AutoLoseCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        List<Cell> distinct = m_board.GetOccupiedCells()
            .GroupBy(cell => ((NormalItem)cell.Item).ItemType)
            .Select(group => group.First()).Take(TrayCapacity).ToList();

        foreach (Cell cell in distinct)
        {
            MoveToTray(cell);
            yield return new WaitForSeconds(AutoPlayDelay);
        }
    }

    public void Clear()
    {
        m_gameOver = true;
        StopAllCoroutines();
        if (m_board != null) m_board.Clear();
        foreach (Item item in m_tray) item.Clear();
        m_tray.Clear();
        foreach (Transform trayCell in m_trayCells)
        {
            if (trayCell != null) Destroy(trayCell.gameObject);
        }
        m_trayCells.Clear();
    }
}
