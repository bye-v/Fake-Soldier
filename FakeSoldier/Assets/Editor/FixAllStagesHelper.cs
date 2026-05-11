using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// FakeSoldier/Fix All Stage Boundaries 메뉴에서 실행.
public static class FixAllStagesHelper
{
    const float WALL_TOP_Y    = -1.72f;
    const float WALL_BOTTOM_Y = -4.58f;
    // WALL_LEFT_X / WALL_RIGHT_X: Background SpriteRenderer.bounds에서 자동 계산
    // fallback (Background 오브젝트를 찾지 못한 경우)
    const float FALLBACK_LEFT_X  = -11.0f;
    const float FALLBACK_RIGHT_X =  11.0f;
    const float PLAYER_SCALE  =  0.80f;
    // PLAYER_X: 왼쪽 벽에서 이 거리만큼 안쪽 (동적 계산)
    const float PLAYER_LEFT_OFFSET = 3.0f;
    const float PLAYER_Y      = -3.15f;
    const float MARKER_SCALE  =  0.5f;
    // EventTriggerZone X: 오른쪽 벽에서 이 거리만큼 안쪽
    const float EVENT_ZONE_RIGHT_OFFSET = 4.0f;
    // 카메라 ortho: 원래 값 유지 (배경이 세로로 꽉 채워지면 스케일 자동 계산)
    const float CAMERA_ORTHO_SIZE = 5.0f;
    // 카메라 Y: 0으로 고정 (배경이 Y=-ortho~+ortho 가득 채울 때 완벽히 맞아떨어짐)
    const float CAMERA_Y = 0f;

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

    // Background SpriteRenderer의 world-space bounds에서 좌우 경계를 읽는다.
    static (float leftX, float rightX) ReadBackgroundBounds(UnityEngine.SceneManagement.Scene scene)
    {
        foreach (var root in scene.GetRootGameObjects())
        {
            if (root.name != "Background") continue;
            var sr = root.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
                return (sr.bounds.min.x, sr.bounds.max.x);
        }
        return (FALLBACK_LEFT_X, FALLBACK_RIGHT_X);
    }

    static void FixScene(UnityEngine.SceneManagement.Scene scene)
    {
        // ── Step 1: Background 스케일을 먼저 설정 (세로가 카메라를 꽉 채우도록) ──
        // scale = (2 × ortho) / sprite.bounds.size.y
        // sprite.bounds는 로컬 공간 크기 (= pixelHeight / PPU)
        foreach (var root in scene.GetRootGameObjects())
        {
            if (root.name != "Background") continue;
            var sr = root.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                float spriteLocalH = sr.sprite.bounds.size.y;   // PPU 기준 높이
                float targetWorldH  = CAMERA_ORTHO_SIZE * 2f;   // 화면 전체 높이
                float newScale      = targetWorldH / spriteLocalH;
                root.transform.localScale = new Vector3(newScale, newScale, 1f);
                EditorUtility.SetDirty(root);
            }
            break;
        }

        // ── Step 2: 스케일 반영 후 bounds 재계산 ──
        var (leftX, rightX) = ReadBackgroundBounds(scene);
        float playerStartX = leftX + PLAYER_LEFT_OFFSET;
        float eventZoneX   = rightX - EVENT_ZONE_RIGHT_OFFSET;
        float wallWidth    = (rightX - leftX) + 2f;

        bool hasBoundaryWalls = false;

        foreach (var root in scene.GetRootGameObjects())
        {
            if (root.name == "Main Camera")
            {
                var cam = root.GetComponent<Camera>();
                if (cam != null) cam.orthographicSize = CAMERA_ORTHO_SIZE;
                // Y 위치: 플레이어가 화면 하단 35% 위치에 오도록 고정
                var cp = root.transform.position;
                cp.y = CAMERA_Y;
                root.transform.position = cp;
                var cfType = System.AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => { try { return a.GetTypes(); } catch { return System.Type.EmptyTypes; } })
                    .FirstOrDefault(t => t.Name == "CameraFollow");
                if (cfType != null && root.GetComponent(cfType) == null)
                    root.AddComponent(cfType);
                EditorUtility.SetDirty(root);
            }
            else if (root.name == "Player")
            {
                root.transform.localScale = new Vector3(PLAYER_SCALE, PLAYER_SCALE, 1f);
                var p = root.transform.position;
                p.x = playerStartX;
                p.y = PLAYER_Y;
                root.transform.position = p;
                EditorUtility.SetDirty(root);
            }
            else if (root.name == "BoundaryWalls")
            {
                hasBoundaryWalls = true;
                ApplyBoundaryWalls(root, leftX, rightX, wallWidth);
            }
            else if (root.name == "EventTriggerZone")
            {
                var p = root.transform.position;
                p.x = eventZoneX;
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
            ApplyBoundaryWalls(parent, leftX, rightX, wallWidth);
        }

        // UI 힌트 텍스트 크기 및 텍스트 업데이트
        foreach (var root in scene.GetRootGameObjects())
        {
            foreach (var tmp in root.GetComponentsInChildren<TMPro.TMP_Text>(true))
            {
                if (tmp.name == "KeyHint")
                {
                    // 옛 텍스트 교체
                    if (tmp.text.Contains("Space") || tmp.text.Contains("/ E"))
                        tmp.text = "<color=#FFD060>Enter</color>  계속";
                    tmp.fontSize = 28f;
                    var rt = tmp.GetComponent<RectTransform>();
                    if (rt != null) rt.sizeDelta = new Vector2(rt.sizeDelta.x, 38f);
                    EditorUtility.SetDirty(tmp.gameObject);
                }
                else if (tmp.name == "ContinueIndicator")
                {
                    tmp.fontSize = 42f;
                    var rt = tmp.GetComponent<RectTransform>();
                    if (rt != null) rt.sizeDelta = new Vector2(60f, 60f);
                    EditorUtility.SetDirty(tmp.gameObject);
                }
            }
        }

        // ── Step N: 대화창 UI 스타일 개선 ──
        foreach (var root in scene.GetRootGameObjects())
        {
            if (root.name != "UICanvas") continue;
            var dialoguePanelTr = root.transform.Find("DialoguePanel");
            if (dialoguePanelTr != null)
            {
                // 패널 배경: 짙은 네이비 (순 검정보다 분위기 있음)
                var panelImg = dialoguePanelTr.GetComponent<UnityEngine.UI.Image>();
                if (panelImg != null)
                {
                    panelImg.color = new Color(0.04f, 0.04f, 0.12f, 0.93f);
                    EditorUtility.SetDirty(panelImg.gameObject);
                }

                // 화자 이름: 황금빛 (빨간색 → 따뜻한 골드)
                var speakerTr = dialoguePanelTr.Find("SpeakerText");
                if (speakerTr != null)
                {
                    var st = speakerTr.GetComponent<TMPro.TMP_Text>();
                    if (st != null)
                    {
                        st.color    = new Color(0.95f, 0.80f, 0.35f);
                        st.fontSize = 24f;
                        EditorUtility.SetDirty(st.gameObject);
                    }
                }

                // 본문: 따뜻한 크림색 + 폰트 키우기
                var bodyTr = dialoguePanelTr.Find("BodyText");
                if (bodyTr != null)
                {
                    var bt = bodyTr.GetComponent<TMPro.TMP_Text>();
                    if (bt != null)
                    {
                        bt.color    = new Color(0.95f, 0.92f, 0.85f);
                        bt.fontSize = 28f;
                        EditorUtility.SetDirty(bt.gameObject);
                    }
                }
            }
            break;
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

    static void ApplyBoundaryWalls(GameObject parent, float leftX, float rightX, float wallWidth)
    {
        float centerX = (leftX + rightX) * 0.5f;
        SetWall(parent, "WallTop",    new Vector3(centerX, WALL_TOP_Y,    0), new Vector2(wallWidth, 0.5f));
        SetWall(parent, "WallBottom", new Vector3(centerX, WALL_BOTTOM_Y, 0), new Vector2(wallWidth, 0.5f));
        SetWall(parent, "WallLeft",   new Vector3(leftX,   PLAYER_Y,      0), new Vector2(0.5f, 12f));
        SetWall(parent, "WallRight",  new Vector3(rightX,  PLAYER_Y,      0), new Vector2(0.5f, 12f));
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
