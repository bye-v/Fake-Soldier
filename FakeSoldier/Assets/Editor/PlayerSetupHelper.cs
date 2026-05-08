using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

public static class PlayerSetupHelper
{
    const string SPRITE_FOLDER  = "Assets/Sprites/Player";
    const string ANIM_FOLDER    = "Assets/Animations/Player";
    const string CTRL_PATH      = "Assets/Animations/Player/PlayerController.controller";

    // ── 픽셀아트 솔저 스프라이트 생성 ─────────────────────────────

    [MenuItem("FakeSoldier/Generate Player Sprites")]
    public static void GenerateSprites()
    {
        Directory.CreateDirectory(Application.dataPath + "/Sprites/Player");
        AssetDatabase.Refresh();

        CreateDirectionalSprites();
        AssetDatabase.Refresh();
        Debug.Log("플레이어 스프라이트 생성 완료!");
    }

    static void CreateDirectionalSprites()
    {
        // 4방향 + idle 스프라이트 생성 (16x16 픽셀아트)
        SaveSprite(DrawSoldierDown(false),  "soldier_idle_down");
        SaveSprite(DrawSoldierDown(true),   "soldier_walk_down_0");
        SaveSprite(DrawSoldierUp(false),    "soldier_idle_up");
        SaveSprite(DrawSoldierUp(true),     "soldier_walk_up_0");
        SaveSprite(DrawSoldierSide(false),  "soldier_idle_side");
        SaveSprite(DrawSoldierSide(true),   "soldier_walk_side_0");
    }

    // ── 방향별 스프라이트 픽셀 드로잉 ─────────────────────────────

    static Texture2D DrawSoldierDown(bool walkFrame)
    {
        int size = 16;
        var tex = NewTex(size);
        var pixels = new Color[size * size];
        Clear(pixels);

        var helmetColor = new Color(0.22f, 0.30f, 0.15f);
        var bodyColor   = new Color(0.30f, 0.40f, 0.20f);
        var skinColor   = new Color(0.76f, 0.60f, 0.42f);
        var beltColor   = new Color(0.20f, 0.18f, 0.10f);
        var bootColor   = new Color(0.18f, 0.14f, 0.08f);
        var gunColor    = new Color(0.15f, 0.15f, 0.15f);

        // 몸통 (탑다운: 머리 위쪽)
        FillRect(pixels, size, 5, 11, 6, 4, bodyColor);   // 몸통
        FillRect(pixels, size, 4, 8,  8, 3, bodyColor);   // 어깨
        FillRect(pixels, size, 5, 8,  6, 1, beltColor);   // 허리띠

        // 발 (걷기 애니)
        if (walkFrame)
        {
            SetPixel(pixels, size, 5, 10,  bootColor);
            SetPixel(pixels, size, 6, 10,  bootColor);
            SetPixel(pixels, size, 9, 11,  bootColor);
            SetPixel(pixels, size, 10, 11, bootColor);
        }
        else
        {
            FillRect(pixels, size, 5, 10, 2, 1, bootColor);
            FillRect(pixels, size, 9, 10, 2, 1, bootColor);
        }

        // 총 (오른쪽)
        FillRect(pixels, size, 12, 8, 2, 4, gunColor);

        // 헬멧
        FillRect(pixels, size, 5, 12, 6, 4, helmetColor);
        FillRect(pixels, size, 4, 13, 8, 2, helmetColor);  // 챙

        // 얼굴 (아래 방향 = 얼굴 보임)
        FillRect(pixels, size, 6, 12, 4, 2, skinColor);

        // 눈
        SetPixel(pixels, size, 6, 12, new Color(0.1f, 0.1f, 0.1f));
        SetPixel(pixels, size, 9, 12, new Color(0.1f, 0.1f, 0.1f));

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    static Texture2D DrawSoldierUp(bool walkFrame)
    {
        int size = 16;
        var tex = NewTex(size);
        var pixels = new Color[size * size];
        Clear(pixels);

        var helmetColor = new Color(0.22f, 0.30f, 0.15f);
        var bodyColor   = new Color(0.30f, 0.40f, 0.20f);
        var beltColor   = new Color(0.20f, 0.18f, 0.10f);
        var bootColor   = new Color(0.18f, 0.14f, 0.08f);
        var gunColor    = new Color(0.15f, 0.15f, 0.15f);
        var backpackColor = new Color(0.25f, 0.32f, 0.18f);

        FillRect(pixels, size, 5, 11, 6, 4, bodyColor);
        FillRect(pixels, size, 4, 8,  8, 3, bodyColor);
        FillRect(pixels, size, 5, 8,  6, 1, beltColor);
        FillRect(pixels, size, 6, 10, 4, 3, backpackColor);  // 배낭

        if (walkFrame)
        {
            SetPixel(pixels, size, 5, 10, bootColor);
            SetPixel(pixels, size, 9, 11, bootColor);
        }
        else
        {
            FillRect(pixels, size, 5, 10, 2, 1, bootColor);
            FillRect(pixels, size, 9, 10, 2, 1, bootColor);
        }

        FillRect(pixels, size, 12, 8, 2, 4, gunColor);
        FillRect(pixels, size, 5, 12, 6, 4, helmetColor);
        FillRect(pixels, size, 4, 13, 8, 2, helmetColor);

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    static Texture2D DrawSoldierSide(bool walkFrame)
    {
        int size = 16;
        var tex = NewTex(size);
        var pixels = new Color[size * size];
        Clear(pixels);

        var helmetColor = new Color(0.22f, 0.30f, 0.15f);
        var bodyColor   = new Color(0.30f, 0.40f, 0.20f);
        var skinColor   = new Color(0.76f, 0.60f, 0.42f);
        var beltColor   = new Color(0.20f, 0.18f, 0.10f);
        var bootColor   = new Color(0.18f, 0.14f, 0.08f);
        var gunColor    = new Color(0.15f, 0.15f, 0.15f);

        FillRect(pixels, size, 5, 11, 5, 4, bodyColor);
        FillRect(pixels, size, 4, 9,  6, 2, bodyColor);
        FillRect(pixels, size, 5, 9,  5, 1, beltColor);

        if (walkFrame)
        {
            FillRect(pixels, size, 4, 10, 2, 1, bootColor);
            FillRect(pixels, size, 8, 11, 2, 1, bootColor);
        }
        else
        {
            FillRect(pixels, size, 5, 10, 4, 1, bootColor);
        }

        // 총 (앞으로)
        FillRect(pixels, size, 10, 10, 4, 2, gunColor);

        // 헬멧
        FillRect(pixels, size, 5, 12, 5, 3, helmetColor);
        FillRect(pixels, size, 4, 12, 1, 2, helmetColor);

        // 얼굴
        FillRect(pixels, size, 6, 12, 3, 2, skinColor);
        SetPixel(pixels, size, 7, 12, new Color(0.1f, 0.1f, 0.1f));  // 눈

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    // ── 텍스처 헬퍼 ───────────────────────────────────────────────

    static Texture2D NewTex(int size)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        return tex;
    }

    static void Clear(Color[] pixels) { for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.clear; }

    static void SetPixel(Color[] pixels, int size, int x, int y, Color c)
    {
        if (x < 0 || x >= size || y < 0 || y >= size) return;
        pixels[y * size + x] = c;
    }

    static void FillRect(Color[] pixels, int size, int x, int y, int w, int h, Color c)
    {
        for (int py = y; py < y + h; py++)
            for (int px = x; px < x + w; px++)
                SetPixel(pixels, size, px, py, c);
    }

    static void SaveSprite(Texture2D tex, string name)
    {
        File.WriteAllBytes(Application.dataPath + $"/Sprites/Player/{name}.png", tex.EncodeToPNG());
    }

    // ── AnimatorController 생성 ────────────────────────────────────

    [MenuItem("FakeSoldier/Create Player Animator")]
    public static AnimatorController CreateAnimator()
    {
        Directory.CreateDirectory(Application.dataPath + "/Animations/Player");
        AssetDatabase.Refresh();

        // 스프라이트 로드
        AssetDatabase.ImportAsset(SPRITE_FOLDER, ImportAssetOptions.ForceSynchronousImport);

        var idleDown  = LoadSprite("soldier_idle_down");
        var walkDown0 = LoadSprite("soldier_walk_down_0");
        var idleUp    = LoadSprite("soldier_idle_up");
        var walkUp0   = LoadSprite("soldier_walk_up_0");
        var idleSide  = LoadSprite("soldier_idle_side");
        var walkSide0 = LoadSprite("soldier_walk_side_0");

        // Controller 생성
        var ctrl = AnimatorController.CreateAnimatorControllerAtPath(CTRL_PATH);
        ctrl.AddParameter("MoveX",    AnimatorControllerParameterType.Float);
        ctrl.AddParameter("MoveY",    AnimatorControllerParameterType.Float);
        ctrl.AddParameter("IsMoving", AnimatorControllerParameterType.Bool);

        var sm = ctrl.layers[0].stateMachine;

        // 상태 생성
        var idleState = sm.AddState("Idle");
        var walkState = sm.AddState("Walk");
        sm.defaultState = idleState;

        // 클립 생성 헬퍼
        AnimationClip MakeClip(string clipName, Sprite[] frames, float fps = 8)
        {
            var clip = new AnimationClip();
            clip.frameRate = fps;
            var binding = new UnityEditor.EditorCurveBinding
            {
                path = "",
                type = typeof(SpriteRenderer),
                propertyName = "m_Sprite"
            };
            var keyframes = new ObjectReferenceKeyframe[frames.Length];
            for (int i = 0; i < frames.Length; i++)
            {
                keyframes[i].time = i / fps;
                keyframes[i].value = frames[i];
            }
            AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);
            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
            AssetDatabase.CreateAsset(clip, $"{ANIM_FOLDER}/{clipName}.anim");
            return clip;
        }

        // Idle 클립 (4방향 블렌드트리)
        var idleDownClip  = idleDown  != null ? MakeClip("idle_down",  new[] { idleDown  }) : null;
        var idleUpClip    = idleUp    != null ? MakeClip("idle_up",    new[] { idleUp    }) : null;
        var idleSideClip  = idleSide  != null ? MakeClip("idle_side",  new[] { idleSide  }) : null;

        // Walk 클립
        var walkDownClip  = walkDown0 != null ? MakeClip("walk_down",  new[] { idleDown,  walkDown0 }) : null;
        var walkUpClip    = walkUp0   != null ? MakeClip("walk_up",    new[] { idleUp,    walkUp0   }) : null;
        var walkSideClip  = walkSide0 != null ? MakeClip("walk_side",  new[] { idleSide,  walkSide0 }) : null;

        // Idle BlendTree (2D Simple Directional)
        var idleTree = new BlendTree();
        idleTree.name = "IdleBlend";
        idleTree.blendType = BlendTreeType.SimpleDirectional2D;
        idleTree.blendParameter  = "MoveX";
        idleTree.blendParameterY = "MoveY";
        if (idleDownClip  != null) idleTree.AddChild(idleDownClip,  new Vector2( 0, -1));
        if (idleUpClip    != null) idleTree.AddChild(idleUpClip,    new Vector2( 0,  1));
        if (idleSideClip  != null) idleTree.AddChild(idleSideClip,  new Vector2( 1,  0));
        if (idleSideClip  != null) idleTree.AddChild(idleSideClip,  new Vector2(-1,  0));
        AssetDatabase.AddObjectToAsset(idleTree, ctrl);
        idleState.motion = idleTree;

        // Walk BlendTree
        var walkTree = new BlendTree();
        walkTree.name = "WalkBlend";
        walkTree.blendType = BlendTreeType.SimpleDirectional2D;
        walkTree.blendParameter  = "MoveX";
        walkTree.blendParameterY = "MoveY";
        if (walkDownClip  != null) walkTree.AddChild(walkDownClip,  new Vector2( 0, -1));
        if (walkUpClip    != null) walkTree.AddChild(walkUpClip,    new Vector2( 0,  1));
        if (walkSideClip  != null) walkTree.AddChild(walkSideClip,  new Vector2( 1,  0));
        if (walkSideClip  != null) walkTree.AddChild(walkSideClip,  new Vector2(-1,  0));
        AssetDatabase.AddObjectToAsset(walkTree, ctrl);
        walkState.motion = walkTree;

        // 트랜지션
        var toWalk = idleState.AddTransition(walkState);
        toWalk.AddCondition(AnimatorConditionMode.If, 0, "IsMoving");
        toWalk.hasExitTime = false;
        toWalk.duration = 0;

        var toIdle = walkState.AddTransition(idleState);
        toIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsMoving");
        toIdle.hasExitTime = false;
        toIdle.duration = 0;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("AnimatorController 생성 완료!");
        return ctrl;
    }

    static Sprite LoadSprite(string name)
        => AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITE_FOLDER}/{name}.png");

    // ── 모든 스테이지 플레이어 업데이트 ───────────────────────────

    [MenuItem("FakeSoldier/Update All Stages (Player + Walls)")]
    public static void UpdateAllStages()
    {
        // 스프라이트/애니메이터가 없으면 생성
        if (!File.Exists(Application.dataPath + "/Sprites/Player/soldier_idle_down.png"))
            GenerateSprites();
        if (!File.Exists(Application.dataPath.Replace("Assets", "") + CTRL_PATH))
            CreateAnimator();

        AssetDatabase.Refresh();
        var ctrl = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(CTRL_PATH);
        var idleSprite = LoadSprite("soldier_idle_down");

        string[] stages = { "Stage_01","Stage_02","Stage_03","Stage_04","Stage_05" };
        foreach (var stageName in stages)
        {
            string path = $"Assets/Scenes/{stageName}.unity";
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            UpdateStageScene(scene, ctrl, idleSprite, stageName);
            EditorSceneManager.SaveScene(scene);
        }
        Debug.Log("모든 스테이지 업데이트 완료!");
    }

    static void UpdateStageScene(Scene scene, RuntimeAnimatorController ctrl, Sprite idleSprite, string stageName)
    {
        // 플레이어 찾기
        GameObject player = null;
        foreach (var root in scene.GetRootGameObjects())
            if (root.name == "Player") { player = root; break; }

        if (player == null) return;

        // 스프라이트 + 애니메이터 설정
        var sr = player.GetComponent<SpriteRenderer>();
        if (sr != null && idleSprite != null)
        {
            sr.sprite = idleSprite;
            sr.color = Color.white;
        }
        player.transform.localScale = new Vector3(1.5f, 1.5f, 1f);

        var animator = player.GetComponent<Animator>();
        if (animator == null) animator = player.AddComponent<Animator>();
        if (ctrl != null) animator.runtimeAnimatorController = ctrl;

        // SpriteRenderer flipX 연동을 PlayerController가 처리하도록 함
        // (좌/우 이동 시 flipX)

        // ── 카메라 bounds 기반 벽 콜라이더 ──────────────────────
        AddBoundaryWalls(scene);

        // ── EventTriggerZone 추가 ─────────────────────────────
        AddEventTriggerZone(scene, stageName);
    }

    static void AddBoundaryWalls(Scene scene)
    {
        // 기존 BoundaryWalls 제거
        foreach (var root in scene.GetRootGameObjects())
            if (root.name == "BoundaryWalls") { Object.DestroyImmediate(root); break; }

        var walls = new GameObject("BoundaryWalls");
        SceneManager.MoveGameObjectToScene(walls, scene);

        // orthographic size 5, aspect 16:9 기준
        // 가로: ±8.88, 세로: ±5
        float halfW = 9f, halfH = 5.2f;

        CreateWall(walls.transform, "WallTop",    new Vector3(0,  halfH, 0), new Vector2(halfW * 2, 0.5f));
        CreateWall(walls.transform, "WallBottom", new Vector3(0, -halfH, 0), new Vector2(halfW * 2, 0.5f));
        CreateWall(walls.transform, "WallLeft",   new Vector3(-halfW, 0, 0), new Vector2(0.5f, halfH * 2));
        CreateWall(walls.transform, "WallRight",  new Vector3( halfW, 0, 0), new Vector2(0.5f, halfH * 2));
    }

    static void CreateWall(Transform parent, string name, Vector3 pos, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.position = pos;
        var bc = go.AddComponent<BoxCollider2D>();
        bc.size = size;
        go.layer = LayerMask.NameToLayer("Default");
    }

    static void AddEventTriggerZone(Scene scene, string stageName)
    {
        // 기존 EventTrigger 제거
        foreach (var root in scene.GetRootGameObjects())
            if (root.name == "EventTriggerZone") { Object.DestroyImmediate(root); break; }

        // 마커 스프라이트 에셋 생성 (최초 1회만)
        const string markerAssetPath = "Assets/Sprites/marker_red.png";
        if (!File.Exists(Application.dataPath + "/Sprites/marker_red.png"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Sprites");
            var tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            var c = new Color(1f, 0.2f, 0.2f, 0.9f);
            var clr = Color.clear;
            tex.SetPixels(new Color[] { clr,c,c,clr, clr,c,c,clr, clr,c,c,clr, clr,c,c,clr });
            tex.Apply();
            File.WriteAllBytes(Application.dataPath + "/Sprites/marker_red.png", tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            AssetDatabase.Refresh();

            var importer = AssetImporter.GetAtPath(markerAssetPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.filterMode = FilterMode.Point;
                importer.SaveAndReimport();
            }
        }

        var markerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(markerAssetPath);

        // 이벤트 존 (화면 중앙 우측)
        var zoneGO = new GameObject("EventTriggerZone");
        SceneManager.MoveGameObjectToScene(zoneGO, scene);
        zoneGO.transform.position = new Vector3(2f, -1.5f, 0);
        zoneGO.tag = "Untagged";

        var col = zoneGO.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(2f, 2f);
        zoneGO.AddComponent<EventTriggerZone>();

        // 시각적 마커
        var marker = new GameObject("Marker");
        marker.transform.SetParent(zoneGO.transform, false);
        var markerSR = marker.AddComponent<SpriteRenderer>();
        markerSR.color = new Color(0.9f, 0.2f, 0.2f, 0.35f);
        markerSR.sortingOrder = 2;
        if (markerSprite != null) markerSR.sprite = markerSprite;
        marker.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
    }
}
