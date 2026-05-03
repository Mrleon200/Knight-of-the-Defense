using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildMenuUI : MonoBehaviour
{
    public static BuildMenuUI Instance;

    public RectTransform panel;
    public Image panelImage;
    public Sprite circleSprite;
    public TowerMenuButton[] buttons;
    public Vector2 panelOffset = new Vector2(0f, 120f);

    private BuildNode currentNode;
    private bool ignoreNextClick;
    private CanvasGroup panelCanvasGroup;

    private void Awake()
    {
        Instance = this;
        panel.gameObject.SetActive(false);

        if (panelImage != null && circleSprite != null)
        {
            panelImage.sprite = circleSprite;
            panelImage.type = Image.Type.Simple;
        }

        panelCanvasGroup = panel.GetComponent<CanvasGroup>();
        if (panelCanvasGroup == null)
            panelCanvasGroup = panel.gameObject.AddComponent<CanvasGroup>();
        panelCanvasGroup.blocksRaycasts = false;
    }

    private void Update()
    {
        if (!IsShowing()) return;

        // Click ra ngoài UI
        if (Input.GetMouseButtonDown(0))
        {
            if (ignoreNextClick)
            {
                ignoreNextClick = false;
                return;
            }

            if (!IsPointerOverUI())
            {
                Hide();
            }
        }

        // Zoom
        if (Input.mouseScrollDelta.y != 0)
        {
            Hide();
        }

        // Drag camera
        if (Input.GetMouseButton(1))
        {
            Hide();
        }
    }

    public void Show(BuildNode node)
    {
        currentNode = node;
        panel.gameObject.SetActive(true);
        panel.SetAsLastSibling();

        if (panelCanvasGroup != null)
            panelCanvasGroup.blocksRaycasts = true;

        Vector3 worldPos = node.transform.position;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        screenPos += (Vector3)panelOffset;

        RectTransform canvasRect = panel.parent as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            Camera.main,
            out Vector2 localPos
        );

        panel.localPosition = localPos;
        ignoreNextClick = true;

        ArrangeButtons();
        StartCoroutine(RefreshNextFrame());
    }

    private void ArrangeButtons()
    {
        // Bỏ sắp xếp - để nút giữ vị trí trong panel
        return;
    }

    private System.Collections.IEnumerator RefreshNextFrame()
    {
        yield return null;
        RefreshButtons();
    }

    public void RefreshButtons()
    {
        int gold = GameManager.Instance.gold;

        foreach (var btn in buttons)
        {
            if (btn != null)
                btn.RefreshAffordability(gold);
        }
    }

    public void SelectTower(int index)
    {
        if (currentNode == null) return;

        int cost = buttons[index].towerCost;

        if (!GameManager.Instance.SpendGold(cost))
        {
            buttons[index].PlayErrorFeedback();
            return;
        }

        currentNode.BuildTower(index);
        Hide();
    }

    public void Hide()
    {
        panel.gameObject.SetActive(false);
        currentNode = null;
        if (panelCanvasGroup != null)
            panelCanvasGroup.blocksRaycasts = false;
    }


    public bool IsShowing()
    {
        return panel != null && panel.gameObject.activeSelf;
    }

    private bool IsPointerOverUI()
    {
        if (Input.touchCount > 0)
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

        return EventSystem.current.IsPointerOverGameObject();
    }
}