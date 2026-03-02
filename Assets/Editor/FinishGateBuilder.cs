#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor Script: Tự động tạo FinishGate Prefab.
/// Vào menu: Tools → HPC → Create FinishGate Prefab
/// </summary>
public class FinishGateBuilder : Editor
{
    [MenuItem("Tools/HPC/Create FinishGate Prefab")]
    public static void CreateFinishGatePrefab()
    {
        // ── Root GameObject ──────────────────────────────────
        GameObject root = new GameObject("FinishGate");

        // ── Vật liệu ─────────────────────────────────────────
        Material matPillar = new Material(Shader.Find("Standard"));
        matPillar.color = new Color(0.2f, 0.4f, 0.8f); // xanh nước biển (màu trường)

        Material matBeam = new Material(Shader.Find("Standard"));
        matBeam.color = new Color(0.9f, 0.7f, 0.1f); // vàng nổi bật

        Material matSign = new Material(Shader.Find("Standard"));
        matSign.color = new Color(1f, 1f, 1f); // trắng cho bảng chữ

        // ── Kích thước cổng ──────────────────────────────────
        // Lane width ≈ 4 units mỗi lane, 3 lane → tổng ~12 units
        float gateWidth   = 14f;   // rộng hơn đường một chút
        float gateHeight  = 8f;    // chiều cao cổng
        float pillarWidth = 1.0f;  // bề dày cột
        float pillarDepth = 1.0f;
        float beamHeight  = 1.2f;  // chiều cao thanh ngang

        // ── Cột trái ─────────────────────────────────────────
        GameObject leftPillar = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftPillar.name = "Pillar_Left";
        leftPillar.transform.SetParent(root.transform);
        leftPillar.transform.localPosition = new Vector3(-(gateWidth / 2f), gateHeight / 2f, 0);
        leftPillar.transform.localScale    = new Vector3(pillarWidth, gateHeight, pillarDepth);
        leftPillar.GetComponent<Renderer>().material = matPillar;
        DestroyImmediate(leftPillar.GetComponent<BoxCollider>());

        // ── Cột phải ─────────────────────────────────────────
        GameObject rightPillar = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightPillar.name = "Pillar_Right";
        rightPillar.transform.SetParent(root.transform);
        rightPillar.transform.localPosition = new Vector3(gateWidth / 2f, gateHeight / 2f, 0);
        rightPillar.transform.localScale    = new Vector3(pillarWidth, gateHeight, pillarDepth);
        rightPillar.GetComponent<Renderer>().material = matPillar;
        DestroyImmediate(rightPillar.GetComponent<BoxCollider>());

        // ── Thanh ngang phía trên ─────────────────────────────
        GameObject beam = GameObject.CreatePrimitive(PrimitiveType.Cube);
        beam.name = "TopBeam";
        beam.transform.SetParent(root.transform);
        beam.transform.localPosition = new Vector3(0, gateHeight, 0);
        beam.transform.localScale    = new Vector3(gateWidth + pillarWidth, beamHeight, pillarDepth);
        beam.GetComponent<Renderer>().material = matBeam;
        DestroyImmediate(beam.GetComponent<BoxCollider>());

        // ── Bảng chữ "HPC" ────────────────────────────────────
        GameObject sign = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sign.name = "Sign_HPC";
        sign.transform.SetParent(root.transform);
        sign.transform.localPosition = new Vector3(0, gateHeight - beamHeight / 2f - 1.5f, -0.1f);
        sign.transform.localScale    = new Vector3(5f, 2f, 0.2f);
        sign.GetComponent<Renderer>().material = matSign;
        DestroyImmediate(sign.GetComponent<BoxCollider>());

        // ── Canvas 3D hiển thị chữ "Cổng Trường HPC" ─────────
        GameObject canvasGO = new GameObject("Canvas_GateText");
        canvasGO.transform.SetParent(root.transform);
        canvasGO.transform.localPosition = new Vector3(0, gateHeight - beamHeight / 2f - 1.5f, -0.25f);
        canvasGO.transform.localRotation = Quaternion.identity;
        canvasGO.transform.localScale    = new Vector3(0.02f, 0.02f, 0.02f);

        UnityEngine.Canvas canvas = canvasGO.AddComponent<UnityEngine.Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        UnityEngine.RectTransform rt = canvasGO.GetComponent<UnityEngine.RectTransform>();
        rt.sizeDelta = new Vector2(250, 100);

        GameObject textGO = new GameObject("Text_HPC");
        textGO.transform.SetParent(canvasGO.transform);
        UnityEngine.RectTransform textRt = textGO.AddComponent<UnityEngine.RectTransform>();
        textRt.anchoredPosition = Vector2.zero;
        textRt.sizeDelta        = new Vector2(250, 100);

        UnityEngine.UI.Text uiText = textGO.AddComponent<UnityEngine.UI.Text>();
        uiText.text      = "CỔNG TRƯỜNG HPC";
        uiText.fontSize  = 36;
        uiText.fontStyle = FontStyle.Bold;
        uiText.color     = new Color(0.1f, 0.2f, 0.8f);
        uiText.alignment = TextAnchor.MiddleCenter;
        uiText.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // ── Trigger Collider bao phủ toàn cổng ───────────────
        BoxCollider triggerCol = root.AddComponent<BoxCollider>();
        triggerCol.isTrigger = true;
        triggerCol.center    = new Vector3(0, gateHeight / 2f, 0);
        triggerCol.size      = new Vector3(gateWidth - pillarWidth, gateHeight + beamHeight, 3f);

        // ── Script FinishLine ─────────────────────────────────
        root.AddComponent<FinishLine>();

        // ── Lưu Prefab ───────────────────────────────────────
        string prefabFolder = "Assets/Prefabs";
        string prefabPath   = prefabFolder + "/FinishGate.prefab";

        if (!System.IO.Directory.Exists(prefabFolder))
            System.IO.Directory.CreateDirectory(prefabFolder);

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        DestroyImmediate(root);

        if (prefab != null)
        {
            Debug.Log($"✅ FinishGate Prefab tạo thành công tại: {prefabPath}");
            Selection.activeObject = prefab;
            EditorUtility.FocusProjectWindow();
        }
        else
        {
            Debug.LogError("❌ Không thể tạo FinishGate Prefab!");
        }
    }
}
#endif
