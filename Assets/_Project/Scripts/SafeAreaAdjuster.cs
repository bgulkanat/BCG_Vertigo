using UnityEngine;

public class SafeAreaAdjuster : MonoBehaviour
{
    private RectTransform _panelRectTransform;
    private Rect currentSafeArea = new();

    void Awake()
    {
        _panelRectTransform = GetComponent<RectTransform>();
         currentSafeArea = Screen.safeArea;
        ApplySafeArea();
    }

    void ApplySafeArea()
    {
        if (_panelRectTransform == null) return;

        Rect safeArea = Screen.safeArea;

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        _panelRectTransform.anchorMin = anchorMin;
        _panelRectTransform.anchorMax = anchorMax;

        currentSafeArea = Screen.safeArea;

    }

    void Update()
    {
        if (Screen.safeArea != currentSafeArea)
        {
            ApplySafeArea();
        }
    }
}
