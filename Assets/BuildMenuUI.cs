using UnityEngine;

public class BuildMenuUI : MonoBehaviour
{
    public static BuildMenuUI Instance;

    public GameObject panel;
    public TowerMenuButton[] buttons;

    private BuildNode currentNode;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(BuildNode node)
    {
        currentNode = node;
        panel.SetActive(true);


        Vector3 worldPos = node.transform.position;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        screenPos.z = 0f;
        screenPos.y += 80f;
        RectTransform canvasRect = panel.transform.parent as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            Camera.main,
            out Vector2 localPos
        );
        panel.GetComponent<RectTransform>().localPosition = localPos;
        StartCoroutine(RefreshNextFrame());
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
        panel.SetActive(false);
        currentNode = null;
    }
}