using UnityEngine;
using UnityEngine.UI;

public class HorizontalScrollSnap : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform content;
    public float snapSpeed = 10f;
    public float snapThreshold = 0.1f; 

    private bool isSnapping = false;
    private Vector2 targetPosition;
    public int currentFocusingChildIndex = 0; 
    private bool started = false;

    void Update()
    {
        if (isSnapping)
        {
            content.anchoredPosition = Vector2.Lerp(content.anchoredPosition, targetPosition, Time.deltaTime * snapSpeed);

            if (Vector2.Distance(content.anchoredPosition, targetPosition) < snapThreshold)
            {
                isSnapping = false;
                content.anchoredPosition = targetPosition;
            }
        }
    }

    public void FocusOnChild(int index)
    {
        currentFocusingChildIndex = Mathf.Clamp(index, 0, content.childCount - 1);
        SnapToChild(currentFocusingChildIndex);
    }

    private void LateUpdate() {
        if (currentFocusingChildIndex == 0 && started == false)
        {
            FocusOnChild(content.childCount - 1);
            started = true;
        }
    }

    void SnapToChild(int index)
    {
        if (index >= 0 && index < content.childCount)
        {
            RectTransform child = content.GetChild(index) as RectTransform;
            if (child != null)
            {
                // 計算視口的中心位置
                float viewportCenterX = scrollRect.viewport.rect.width / 2f;

                // 計算子物件相對於Content的中心位置
                float childCenterX = child.anchoredPosition.x + (child.rect.width / 2f);

                // 計算目標位置，使子物件的中心對齊到視口的中心
                float newX = -childCenterX + viewportCenterX;

                // 設定 targetPosition
                targetPosition = new Vector2(newX, content.anchoredPosition.y);
                isSnapping = true;
            }
        }
    }

    public void FocusOnNextChild()
    {
        FocusOnChild(currentFocusingChildIndex + 1);
    }

    public void FocusOnPreviousChild()
    {
        FocusOnChild(currentFocusingChildIndex - 1);
    }
}
