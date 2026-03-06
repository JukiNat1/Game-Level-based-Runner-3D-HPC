using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tạo background đẹp cho scene LevelSelect.
/// Gắn vào một GameObject trong scene LevelSelect.
/// Tự động tạo gradient background + trang trí khi scene load.
/// </summary>
public class LevelSelectBackground : MonoBehaviour
{
    [Header("Màu nền (gradient từ trên xuống dưới)")]
    public Color colorTop    = new Color(0.05f, 0.10f, 0.30f, 1f); // Deep blue
    public Color colorBottom = new Color(0.10f, 0.25f, 0.55f, 1f); // Medium blue

    [Header("Camera")]
    public Camera targetCamera;

    void Awake()
    {
        // Set camera background color
        Camera cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam != null)
        {
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = colorBottom;
        }

        // Find Canvas to attach background image
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        SetupGradientBackground(canvas);
        SetupDecorations(canvas);
    }

    private void SetupGradientBackground(Canvas canvas)
    {
        // Create a full-screen background panel (placed first in Canvas so it renders below everything)
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(canvas.transform, false);
        bg.transform.SetAsFirstSibling(); // push to bottom of render order

        RectTransform rt = bg.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Image img = bg.AddComponent<Image>();
        img.color = colorBottom;

        // Gradient simulation: overlay a lighter panel at the top with low alpha
        GameObject gradTop = new GameObject("GradTop");
        gradTop.transform.SetParent(bg.transform, false);
        RectTransform rtTop = gradTop.AddComponent<RectTransform>();
        rtTop.anchorMin = new Vector2(0, 0.5f);
        rtTop.anchorMax = Vector2.one;
        rtTop.offsetMin = Vector2.zero;
        rtTop.offsetMax = Vector2.zero;
        Image imgTop = gradTop.AddComponent<Image>();
        imgTop.color = new Color(colorTop.r, colorTop.g, colorTop.b, 0.85f);
    }

    private void SetupDecorations(Canvas canvas)
    {
        // Add glowing accents at all four corners
        AddCornerAccent(canvas, new Vector2(0, 1),   new Vector2(0,   1),   new Vector2(120, 120)); // top-left
        AddCornerAccent(canvas, new Vector2(1, 1),   new Vector2(1,   1),   new Vector2(120, 120)); // top-right
        AddCornerAccent(canvas, new Vector2(0, 0),   new Vector2(0,   0),   new Vector2(120, 120)); // bottom-left
        AddCornerAccent(canvas, new Vector2(1, 0),   new Vector2(1,   0),   new Vector2(120, 120)); // bottom-right

        // Decorative horizontal lines at top and bottom
        AddHorizontalLine(canvas, 0.88f, new Color(1f, 0.85f, 0f, 0.7f), 4f);  // gold at top
        AddHorizontalLine(canvas, 0.10f, new Color(1f, 0.85f, 0f, 0.7f), 4f);  // gold at bottom
    }

    private void AddCornerAccent(Canvas canvas, Vector2 anchorMin, Vector2 anchorMax, Vector2 size)
    {
        GameObject obj = new GameObject("Corner");
        obj.transform.SetParent(canvas.transform, false);
        obj.transform.SetSiblingIndex(1); // directly after background

        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot     = anchorMin; // pivot at corner
        rt.sizeDelta = size;
        rt.anchoredPosition = Vector2.zero;

        Image img = obj.AddComponent<Image>();
        img.color = new Color(1f, 0.85f, 0f, 0.15f); // Faint gold
    }

    private void AddHorizontalLine(Canvas canvas, float yAnchor, Color color, float height)
    {
        GameObject obj = new GameObject("HLine");
        obj.transform.SetParent(canvas.transform, false);
        obj.transform.SetSiblingIndex(1);

        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, yAnchor);
        rt.anchorMax = new Vector2(1, yAnchor);
        rt.sizeDelta = new Vector2(0, height);
        rt.anchoredPosition = Vector2.zero;

        Image img = obj.AddComponent<Image>();
        img.color = color;
    }
}
