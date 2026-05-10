using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// FakeSoldier/Fix All Stage Boundaries 메뉴에서 실행.
public static class FixAllStagesHelper
{
    const float WALL_TOP_Y    = -1.72f;
    const float WALL_BOTTOM_Y = -4.58f;
    const float WALL_LEFT_X   = -11.0f;
    const float WALL_RIGHT_X  =  11.0f;
    const float EVENT_ZONE_X  =  7.0f;
    const float PLAYER_SCALE  =  0.45f;
    const float PLAYER_X      = -8.0f;
    const float PLAYER_Y      = -3.15f;
    const float MARKER_SCALE  =  0.5f;

    [MenuItem("FakeSoldier/Fix All Stage Boundaries + Player")]
    public static void FixAll()
    {
        string[] stages = {
            "Assets/Scenes/Stage_01.unity",
            "Assets/Scenes/Stage_02.unity",
            "Assets/Scenes/Stage_03.unity",
            "Assets/Scenes/Stage_04.unity",
            "Assets/Scenes/Stage_05.unity",
        };
        foreach (var path in stages)
        {
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            FixScene(scene);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"[Fix] {path} 완료");
        }
        Debug.Log("모든 스테이지 수정 완료!");
    }

    static void FixScene(UnityEngine.SceneManagement.Scene scene)
    {
        bool hasBoundaryWalls = false;

        foreach (var root in scene.GetRootGameObjects())
        {
            if (root.name == "Player")
            {
                root.transform.localScale = new Vector3(PLAYER_SCALE, PLAYER_SCALE, 1f);
                var p = root.transform.position;
                p.x = PLAYER_X;
                p.y = PLAYER_Y;
                root.transform.position = p;
                EditorUtility.SetDirty(root);
            }
            else if (root.name == "BoundaryWalls")
            {
                hasBoundaryWalls = true;
                ApplyBoundaryWalls(root, scene);
            }
            else if (root.name == "EventTriggerZone")
            {
                var p = root.transform.position;
                p.x = EVENT_ZONE_X;
                p.y = PLAYER_Y;
                root.transform.position = p;
                var col = root.GetComponent<BoxCollider2D>();
                if (col == null) col = root.AddComponent<BoxCollider2D>();
                col.isTrigger = true;
                col.size = new Vector2(Mathf.Max(col.size.x, 1.5f), 6f);
                // 화살표 마커 크기 줄이기
                var markerTr = root.transform.Find("Marker");
                if (markerTr != null)
                {
                    markerTr.localScale = new Vector3(MARKER_SCALE, MARKER_SCALE, 1f);
                    EditorUtility.SetDirty(markerTr.gameObject);
                }
                EditorUtility.SetDirty(root);
            }
        }

        if (!hasBoundaryWalls)
        {
            var parent = new GameObject("BoundaryWalls");
            SceneManager.MoveGameObjectToScene(parent, scene);
            ApplyBoundaryWalls(parent, scene);
        }

        // 대화창 키힌트가 "Space / E" 등 옛 텍스트면 Enter로 교체
        foreach (var root in scene.GetRootGameObjects())
        {
            foreach (var tmp in root.GetComponentsInChildren<TMPro.TMP_Text>(true))
            {
                if (tmp.name == "KeyHint" && (tmp.text.Contains("Space") || tmp.text.Contains("/ E")))
                {
                    tmp.text = "<color=#FFD060>Enter</color>  계속";
                    EditorUtility.SetDirty(tmp.gameObject);
                }
            }
        }

        AddNPCHint(scene);
    }

    static void AddNPCHint(UnityEngine.SceneManagement.Scene scene)
    {
        // UICanvas 찾기
        GameObject canvasGO = null;
        foreach (var root in scene.GetRootGameObjects())
            if (root.name == "UICanvas") { canvasGO = root; break; }
        if (canvasGO == null) return;

        // 기존 힌트 제거 (재실행 시 중복 방지)
        var old = canvasGO.transform.Find("NPCHintText");
        if (old != null) Object.DestroyImmediate(old.gameObject);

        // 힌트 텍스트 생성 (상단 중앙 고정)
        var hintGO = new GameObject("NPCHintText");
        hintGO.transform.SetParent(canvasGO.transform, false);

        var rt = hintGO.AddComponent<RectTransform>();
        rt.anchorMin        = new Vector2(0.5f, 1f);
        rt.anchorMax        = new Vector2(0.5f, 1f);
        rt.pivot            = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0f, -38f);
        rt.sizeDelta        = new Vector2(800f, 52f);

        var tmp = hintGO.AddComponent<TMPro.TextMeshProUGUI>();
        tmp.text      = "사람들과 대화를 해보세요";
        tmp.fontSize  = 26f;
        tmp.alignment = TMPro.TextAlignmentOptions.Center;
        tmp.color     = new Color(1f, 0.95f, 0.70f, 0.85f);

        // ControlsHint가 사라진 후 페이드인 (겹침 방지)
        var cg = hintGO.AddComponent<UnityEngine.CanvasGroup>();
        cg.alpha = 0f;
        hintGO.AddComponent<NPCHintController>();

        EditorUtility.SetDirty(hintGO);
    }

    static void ApplyBoundaryWalls(GameObject parent, UnityEngine.SceneManagement.Scene scene)
    {
        SetWall(parent, "WallTop",    new Vector3(0,           WALL_TOP_Y,    0), new Vector2(30f, 0.5f));
        SetWall(parent, "WallBottom", new Vector3(0,           WALL_BOTTOM_Y, 0), new Vector2(30f, 0.5f));
        SetWall(parent, "WallLeft",   new Vector3(WALL_LEFT_X, PLAYER_Y,      0), new Vector2(0.5f, 12f));
        SetWall(parent, "WallRight",  new Vector3(WALL_RIGHT_X, PLAYER_Y,     0), new Vector2(0.5f, 12f));
        EditorUtility.SetDirty(parent);
    }

    static void SetWall(GameObject parent, string wallName, Vector3 pos, Vector2 size)
    {
        var tr = parent.transform.Find(wallName);
        var go = tr != null ? tr.gameObject : new GameObject(wallName);
        go.transform.SetParent(parent.transform, false);
        go.transform.position = pos;
        var col = go.GetComponent<BoxCollider2D>() ?? go.AddComponent<BoxCollider2D>();
        col.size = size;
        EditorUtility.SetDirty(go);
    }
}
