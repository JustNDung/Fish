using UnityEngine;
using UnityEngine.UI;

public class UIPanelMain : MonoBehaviour, IMenu
{
    // Retain the original serialized fields so the existing scene stays compatible.
    [SerializeField] private Button btnTimer;
    [SerializeField] private Button btnMoves;

    private Button m_btnAutoWin;
    private Button m_btnAutoLose;
    private Text m_modeTitle;
    private Text m_modeDescription;
    private UIMainManager m_mngr;

    private void Awake()
    {
        if (btnMoves == null) return;

        ConfigureButton(btnMoves, "PLAY", 90f, OnClickPlay);
        m_btnAutoWin = CreateButton("btnAutoplay", "AUTOPLAY", 0f, OnClickAutoWin);
        m_btnAutoLose = CreateButton("btnAutoLose", "AUTO LOSE", -90f, OnClickAutoLose);
        CreateModeTexts();

        // Some older scene versions contain a second mode button; it is no longer needed.
        if (btnTimer != null) btnTimer.gameObject.SetActive(false);
    }

    private void CreateModeTexts()
    {
        Text template = btnMoves.GetComponentInChildren<Text>();
        if (template == null) return;

        m_modeTitle = Instantiate(template, transform);
        m_modeTitle.gameObject.name = "txtGameModeTitle";
        m_modeTitle.text = "SELECT GAME MODE";
        m_modeTitle.fontSize = 32;
        m_modeTitle.fontStyle = FontStyle.Bold;
        SetTextRect(m_modeTitle.rectTransform, 190f, 420f, 55f);

        m_modeDescription = Instantiate(template, transform);
        m_modeDescription.gameObject.name = "txtGameModeDescription";
        m_modeDescription.text = "PLAY: Manual    •    AUTOPLAY: Auto Win    •    AUTO LOSE: Auto Lose";
        m_modeDescription.fontSize = 17;
        m_modeDescription.fontStyle = FontStyle.Normal;
        SetTextRect(m_modeDescription.rectTransform, -175f, 700f, 40f);
    }

    private static void SetTextRect(RectTransform rect, float anchoredY, float width, float height)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, anchoredY);
        rect.sizeDelta = new Vector2(width, height);
        rect.localScale = Vector3.one;
    }

    private Button CreateButton(string objectName, string label, float anchoredY, UnityEngine.Events.UnityAction action)
    {
        Button button = Instantiate(btnMoves, btnMoves.transform.parent);
        button.gameObject.name = objectName;
        ConfigureButton(button, label, anchoredY, action);
        return button;
    }

    private static void ConfigureButton(Button button, string label, float anchoredY, UnityEngine.Events.UnityAction action)
    {
        RectTransform rect = button.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0f, anchoredY);
        rect.sizeDelta = new Vector2(300f, 70f);
        Text text = button.GetComponentInChildren<Text>(true);
        if (text == null)
        {
            GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            textObject.transform.SetParent(button.transform, false);
            text = textObject.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        text.gameObject.SetActive(true);
        text.text = label;
        text.color = Color.white;
        text.fontSize = 24;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.raycastTarget = false;

        RectTransform textRect = text.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = Vector2.zero;
        textRect.localScale = Vector3.one;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }

    private void OnDestroy()
    {
        if (btnMoves) btnMoves.onClick.RemoveAllListeners();
        if (m_btnAutoWin) m_btnAutoWin.onClick.RemoveAllListeners();
        if (m_btnAutoLose) m_btnAutoLose.onClick.RemoveAllListeners();
    }

    public void Setup(UIMainManager mngr) { m_mngr = mngr; }
    private void OnClickPlay() { m_mngr.LoadLevelMoves(); }
    private void OnClickAutoWin() { m_mngr.LoadAutoWin(); }
    private void OnClickAutoLose() { m_mngr.LoadAutoLose(); }
    public void Show() { gameObject.SetActive(true); }
    public void Hide() { gameObject.SetActive(false); }
}
