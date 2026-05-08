using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPro;

public static class SceneSetupHelper
{
    // ── EventSystem 추가 ──────────────────────────────────────────

    static void EnsureEventSystem(Scene scene)
    {
        foreach (var root in scene.GetRootGameObjects())
            if (root.GetComponent<EventSystem>() != null) return;

        var esGO = new GameObject("EventSystem");
        SceneManager.MoveGameObjectToScene(esGO, scene);
        esGO.AddComponent<EventSystem>();
        esGO.AddComponent<StandaloneInputModule>();
    }

    // ── 모든 씬에 EventSystem 일괄 추가 ──────────────────────────

    [MenuItem("FakeSoldier/Fix All Scenes (Add EventSystem)")]
    public static void FixAllScenes()
    {
        string[] scenePaths = {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/Stage_01.unity","Assets/Scenes/Stage_02.unity",
            "Assets/Scenes/Stage_03.unity","Assets/Scenes/Stage_04.unity",
            "Assets/Scenes/Stage_05.unity",
            "Assets/Scenes/Ending_Bad.unity","Assets/Scenes/Ending_Normal.unity",
            "Assets/Scenes/Ending_True.unity","Assets/Scenes/Credit.unity"
        };
        foreach (var path in scenePaths)
        {
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            EnsureEventSystem(scene);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
        Debug.Log("모든 씬 EventSystem 추가 완료!");
    }

    // ── 공통 헬퍼 ─────────────────────────────────────────────────

    static RectTransform MakeRect(string name, Transform parent, Vector2 anchor, Vector2 pos, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = anchor;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return rt;
    }

    static TextMeshProUGUI MakeTMP(Transform parent, Vector2 anchor, Vector2 pos, Vector2 size, string text, float fontSize, Color color, FontStyles style = FontStyles.Normal)
    {
        var rt = MakeRect(text.Length > 8 ? text.Substring(0, 8) : text, parent, anchor, pos, size);
        var tmp = rt.gameObject.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = color;
        return tmp;
    }

    static Button MakeButton(string name, string label, Transform parent, Vector2 anchor, Vector2 pos, Vector2 size)
    {
        var rt = MakeRect(name, parent, anchor, pos, size);
        var img = rt.gameObject.AddComponent<Image>();
        img.color = new Color(0.1f, 0.1f, 0.1f, 0.88f);
        var btn = rt.gameObject.AddComponent<Button>();
        var colors = btn.colors;
        colors.highlightedColor = new Color(0.75f, 0.1f, 0.1f);
        colors.pressedColor = new Color(0.5f, 0.05f, 0.05f);
        btn.colors = colors;
        var labelRT = MakeRect("Label", rt, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(size.x - 20, size.y - 8));
        var tmp = labelRT.gameObject.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 30;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        return btn;
    }

    static GameObject SetupCamera(Scene scene, Color bgColor)
    {
        var camGO = new GameObject("Main Camera");
        camGO.tag = "MainCamera";
        var cam = camGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = bgColor;
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.transform.position = new Vector3(0, 0, -10);
        camGO.AddComponent<AudioListener>();
        var urpType = System.Type.GetType("UnityEngine.Rendering.Universal.UniversalAdditionalCameraData, Unity.RenderPipelines.Universal.Runtime");
        if (urpType != null) camGO.AddComponent(urpType);
        return camGO;
    }

    static Canvas SetupCanvas(string name, int sortOrder = 0)
    {
        var go = new GameObject(name);
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortOrder;
        var cs = go.AddComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        cs.matchWidthOrHeight = 0.5f;
        go.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    static GameObject SetupFader()
    {
        var faderCanvasGO = new GameObject("SceneFaderCanvas");
        var faderCanvas = faderCanvasGO.AddComponent<Canvas>();
        faderCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        faderCanvas.sortingOrder = 999;
        faderCanvasGO.AddComponent<CanvasScaler>();

        var faderGO = new GameObject("SceneFader");
        faderGO.transform.SetParent(faderCanvasGO.transform, false);
        var faderImg = faderGO.AddComponent<Image>();
        faderImg.color = Color.black;
        var faderRT = faderImg.rectTransform;
        faderRT.anchorMin = Vector2.zero;
        faderRT.anchorMax = Vector2.one;
        faderRT.sizeDelta = Vector2.zero;

        var fader = faderGO.AddComponent<SceneFader>();
        var bf = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
        typeof(SceneFader).GetField("fadeImage", bf)?.SetValue(fader, faderImg);
        return faderCanvasGO;
    }

    // ── MainMenu 씬 ────────────────────────────────────────────────

    [MenuItem("FakeSoldier/Setup MainMenu Scene")]
    public static void SetupMainMenu()
    {
        var scene = SceneManager.GetActiveScene();
        foreach (var root in scene.GetRootGameObjects())
            Object.DestroyImmediate(root);

        SetupCamera(scene, new Color(0.08f, 0.08f, 0.08f));

        // 배경
        var bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/bg_01_main_menu.png");
        var bgGO = new GameObject("Background");
        var bgSR = bgGO.AddComponent<SpriteRenderer>();
        bgSR.sprite = bgSprite;
        bgSR.sortingOrder = -10;
        if (bgSprite != null)
        {
            float s = Mathf.Max(19.2f / bgSprite.bounds.size.x, 10.8f / bgSprite.bounds.size.y);
            bgGO.transform.localScale = new Vector3(s, s, 1);
        }

        // Canvas
        var canvas = SetupCanvas("Canvas");
        var ct = canvas.transform;

        MakeTMP(ct, new Vector2(0.5f, 0.82f), Vector2.zero, new Vector2(1000, 140), "FAKE SOLDIER", 90, Color.white, FontStyles.Bold);
        MakeTMP(ct, new Vector2(0.5f, 0.70f), Vector2.zero, new Vector2(600, 60), "1980. 5. 광주", 34, new Color(0.75f, 0.75f, 0.75f));

        var startBtn = MakeButton("StartButton",    "시 작 하 기", ct, new Vector2(0.5f, 0.5f), new Vector2(0,  60), new Vector2(340, 64));
        var settBtn  = MakeButton("SettingsButton", "설    정",    ct, new Vector2(0.5f, 0.5f), new Vector2(0, -20), new Vector2(340, 64));
        var quitBtn  = MakeButton("QuitButton",     "종    료",    ct, new Vector2(0.5f, 0.5f), new Vector2(0,-100), new Vector2(340, 64));

        // 설정 패널
        var spRT = MakeRect("SettingsPanel", ct, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(520, 320));
        spRT.gameObject.AddComponent<Image>().color = new Color(0.07f, 0.07f, 0.07f, 0.96f);
        MakeTMP(spRT, new Vector2(0.5f, 0.85f), Vector2.zero, new Vector2(400, 50), "설  정", 36, Color.white);
        var closeBtn = MakeButton("CloseBtn", "닫  기", spRT, new Vector2(0.5f, 0.18f), Vector2.zero, new Vector2(200, 54));
        spRT.gameObject.SetActive(false);

        // GameManager
        new GameObject("GameManager").AddComponent<GameManager>();
        SetupFader();

        // MainMenuController
        var mmc = new GameObject("MainMenuController").AddComponent<MainMenuController>();
        var bf = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
        typeof(MainMenuController).GetField("startButton",    bf)?.SetValue(mmc, startBtn);
        typeof(MainMenuController).GetField("settingsButton", bf)?.SetValue(mmc, settBtn);
        typeof(MainMenuController).GetField("quitButton",     bf)?.SetValue(mmc, quitBtn);
        typeof(MainMenuController).GetField("settingsPanel",  bf)?.SetValue(mmc, spRT.gameObject);

        EnsureEventSystem(scene);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("MainMenu 씬 구성 완료!");
    }

    // ── Stage 씬 공통 설정 ─────────────────────────────────────────

    public static void SetupStageBase(string bgPath, System.Type directorType)
    {
        var scene = SceneManager.GetActiveScene();
        foreach (var root in scene.GetRootGameObjects())
            Object.DestroyImmediate(root);

        SetupCamera(scene, new Color(0.06f, 0.06f, 0.06f));

        // 배경
        var bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>(bgPath);
        var bgGO = new GameObject("Background");
        var bgSR = bgGO.AddComponent<SpriteRenderer>();
        bgSR.sprite = bgSprite;
        bgSR.sortingOrder = -10;
        if (bgSprite != null)
        {
            float s = Mathf.Max(19.2f / bgSprite.bounds.size.x, 10.8f / bgSprite.bounds.size.y);
            bgGO.transform.localScale = new Vector3(s, s, 1);
        }

        // Global Light 2D
        var lightGO = new GameObject("Global Light 2D");
        var lightType = System.Type.GetType("UnityEngine.Rendering.Universal.Light2D, Unity.RenderPipelines.Universal.Runtime");
        if (lightType != null)
        {
            var light2d = lightGO.AddComponent(lightType);
            // LightType.Global = 4
            lightType.GetProperty("lightType")?.SetValue(light2d, 4);
            lightType.GetProperty("intensity")?.SetValue(light2d, 1f);
        }

        // Player (임시 스프라이트)
        var playerGO = new GameObject("Player");
        playerGO.tag = "Player";
        var playerSR = playerGO.AddComponent<SpriteRenderer>();
        playerSR.color = new Color(0.9f, 0.9f, 0.9f);
        playerSR.sortingOrder = 1;
        playerGO.transform.position = new Vector3(-3f, -1.5f, 0);
        playerGO.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        var rb2d = playerGO.AddComponent<Rigidbody2D>();
        rb2d.gravityScale = 0;
        rb2d.freezeRotation = true;
        var col2d = playerGO.AddComponent<BoxCollider2D>();
        col2d.size = new Vector2(0.8f, 0.8f);
        playerGO.AddComponent<PlayerController>();

        // UI Canvas (대화창 + 선택지)
        var uiCanvas = SetupCanvas("UICanvas");
        var uiCT = uiCanvas.transform;

        SetupDialogueUI(uiCT);
        SetupChoiceUI(uiCT);

        // GameManager (씬에 없을 경우 대비)
        new GameObject("GameManager").AddComponent<GameManager>();
        SetupFader();

        // StageDirector
        var dirGO = new GameObject("StageDirector");
        dirGO.AddComponent(directorType);

        EnsureEventSystem(scene);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    static void SetupDialogueUI(Transform parent)
    {
        // 대화창 패널 (하단)
        var panelRT = MakeRect("DialoguePanel", parent, new Vector2(0.5f, 0f), new Vector2(0, 120), new Vector2(1700, 200));
        panelRT.gameObject.AddComponent<Image>().color = new Color(0.05f, 0.05f, 0.05f, 0.90f);

        // 화자명
        var speakerRT = MakeRect("SpeakerText", panelRT, new Vector2(0f, 1f), new Vector2(120, -5), new Vector2(340, 46));
        var speakerTMP = speakerRT.gameObject.AddComponent<TextMeshProUGUI>();
        speakerTMP.text = "화자명";
        speakerTMP.fontSize = 26;
        speakerTMP.fontStyle = FontStyles.Bold;
        speakerTMP.color = new Color(0.9f, 0.3f, 0.3f);
        speakerTMP.alignment = TextAlignmentOptions.Left;

        // 본문
        var bodyRT = MakeRect("BodyText", panelRT, new Vector2(0.5f, 0.5f), new Vector2(0, -10), new Vector2(1600, 130));
        var bodyTMP = bodyRT.gameObject.AddComponent<TextMeshProUGUI>();
        bodyTMP.text = "";
        bodyTMP.fontSize = 26;
        bodyTMP.color = Color.white;
        bodyTMP.alignment = TextAlignmentOptions.TopLeft;
        bodyTMP.enableWordWrapping = true;

        // 계속 표시 화살표
        var contRT = MakeRect("ContinueIndicator", panelRT, new Vector2(0.95f, 0.1f), Vector2.zero, new Vector2(40, 40));
        var contTMP = contRT.gameObject.AddComponent<TextMeshProUGUI>();
        contTMP.text = "▼";
        contTMP.fontSize = 22;
        contTMP.color = new Color(0.8f, 0.8f, 0.8f, 0.8f);
        contTMP.alignment = TextAlignmentOptions.Center;

        panelRT.gameObject.SetActive(false);

        // DialogueManager 연결
        var dmGO = new GameObject("DialogueManager");
        var dm = dmGO.AddComponent<DialogueManager>();
        var bf = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
        typeof(DialogueManager).GetField("dialoguePanel",       bf)?.SetValue(dm, panelRT.gameObject);
        typeof(DialogueManager).GetField("speakerText",         bf)?.SetValue(dm, speakerTMP);
        typeof(DialogueManager).GetField("bodyText",            bf)?.SetValue(dm, bodyTMP);
        typeof(DialogueManager).GetField("continueIndicator",   bf)?.SetValue(dm, contRT.gameObject);
    }

    static void SetupChoiceUI(Transform parent)
    {
        // 선택지 패널 (중앙 하단)
        var panelRT = MakeRect("ChoicePanel", parent, new Vector2(0.5f, 0.5f), new Vector2(0, -200), new Vector2(900, 260));
        panelRT.gameObject.AddComponent<Image>().color = new Color(0.05f, 0.05f, 0.05f, 0.88f);

        var buttons = new Button[4];
        var texts = new TextMeshProUGUI[4];

        for (int i = 0; i < 4; i++)
        {
            float yOffset = 90 - i * 60f;
            var btnRT = MakeRect($"Choice{i+1}", panelRT, new Vector2(0.5f, 0.5f), new Vector2(0, yOffset), new Vector2(860, 52));
            var img = btnRT.gameObject.AddComponent<Image>();
            img.color = new Color(0.12f, 0.12f, 0.12f, 0f);
            var btn = btnRT.gameObject.AddComponent<Button>();
            var colors = btn.colors;
            colors.normalColor    = Color.white;
            colors.highlightedColor = new Color(1f, 0.85f, 0.3f);
            btn.colors = colors;
            buttons[i] = btn;

            var labelRT = MakeRect("Label", btnRT, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(840, 46));
            var tmp = labelRT.gameObject.AddComponent<TextMeshProUGUI>();
            tmp.text = $"{i+1}. 선택지";
            tmp.fontSize = 24;
            tmp.alignment = TextAlignmentOptions.Left;
            tmp.color = Color.white;
            texts[i] = tmp;
            btnRT.gameObject.SetActive(false);
        }

        // 타이머 텍스트
        var timerRT = MakeRect("TimerText", panelRT, new Vector2(0.95f, 0.92f), Vector2.zero, new Vector2(80, 46));
        var timerTMP = timerRT.gameObject.AddComponent<TextMeshProUGUI>();
        timerTMP.text = "10";
        timerTMP.fontSize = 34;
        timerTMP.fontStyle = FontStyles.Bold;
        timerTMP.color = new Color(0.9f, 0.2f, 0.2f);
        timerTMP.alignment = TextAlignmentOptions.Center;
        timerRT.gameObject.SetActive(false);

        panelRT.gameObject.SetActive(false);

        // ChoiceSystem 연결
        var csGO = new GameObject("ChoiceSystem");
        var choiceSys = csGO.AddComponent<ChoiceSystem>();
        var bf = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
        typeof(ChoiceSystem).GetField("choicePanel",   bf)?.SetValue(choiceSys, panelRT.gameObject);
        typeof(ChoiceSystem).GetField("choiceButtons", bf)?.SetValue(choiceSys, buttons);
        typeof(ChoiceSystem).GetField("choiceTexts",   bf)?.SetValue(choiceSys, texts);
        typeof(ChoiceSystem).GetField("timerText",     bf)?.SetValue(choiceSys, timerTMP);
    }

    // ── 스테이지 씬 개별 메뉴 ──────────────────────────────────────

    [MenuItem("FakeSoldier/Setup Stage 01")]
    public static void SetupStage01() => SetupStageBase("Assets/bg_03_stage1_university.png", typeof(Stage01Director));

    [MenuItem("FakeSoldier/Setup Stage 02")]
    public static void SetupStage02() => SetupStageBase("Assets/bg_04_stage2_alley.png", typeof(Stage02Director));

    [MenuItem("FakeSoldier/Setup Stage 03")]
    public static void SetupStage03() => SetupStageBase("Assets/bg_05_stage3_checkpoint.png", typeof(Stage03Director));

    [MenuItem("FakeSoldier/Setup Stage 04")]
    public static void SetupStage04() => SetupStageBase("Assets/bg_06_stage4_dochung.png", typeof(Stage04Director));

    [MenuItem("FakeSoldier/Setup Stage 05")]
    public static void SetupStage05() => SetupStageBase("Assets/bg_07_stage5_interior.png", typeof(Stage05Director));

    // ── 엔딩 씬 ────────────────────────────────────────────────────

    static void SetupEndingBase(EndingType type, string title)
    {
        var scene = SceneManager.GetActiveScene();
        foreach (var root in scene.GetRootGameObjects())
            Object.DestroyImmediate(root);

        SetupCamera(scene, new Color(0.04f, 0.04f, 0.04f));

        var canvas = SetupCanvas("Canvas");
        var ct = canvas.transform;

        MakeTMP(ct, new Vector2(0.5f, 0.78f), Vector2.zero, new Vector2(900, 100), title, 60, new Color(0.85f, 0.2f, 0.2f), FontStyles.Bold);

        var bodyRT = MakeRect("BodyText", ct, new Vector2(0.5f, 0.52f), Vector2.zero, new Vector2(1200, 300));
        var bodyTMP = bodyRT.gameObject.AddComponent<TextMeshProUGUI>();
        bodyTMP.text = "";
        bodyTMP.fontSize = 28;
        bodyTMP.color = Color.white;
        bodyTMP.alignment = TextAlignmentOptions.Center;
        bodyTMP.enableWordWrapping = true;

        var scoreRT = MakeRect("ScoreText", ct, new Vector2(0.5f, 0.3f), Vector2.zero, new Vector2(600, 50));
        var scoreTMP = scoreRT.gameObject.AddComponent<TextMeshProUGUI>();
        scoreTMP.fontSize = 24;
        scoreTMP.color = new Color(0.7f, 0.7f, 0.7f);
        scoreTMP.alignment = TextAlignmentOptions.Center;

        // Bad 엔딩 희생자 수
        TextMeshProUGUI victimTMP = null;
        if (type == EndingType.Bad)
        {
            var victimRT = MakeRect("VictimCount", ct, new Vector2(0.5f, 0.22f), Vector2.zero, new Vector2(800, 50));
            victimTMP = victimRT.gameObject.AddComponent<TextMeshProUGUI>();
            victimTMP.fontSize = 22;
            victimTMP.color = new Color(0.8f, 0.2f, 0.2f);
            victimTMP.alignment = TextAlignmentOptions.Center;
        }

        var creditBtn  = MakeButton("CreditButton",  "크레딧 보기", ct, new Vector2(0.5f, 0.12f), new Vector2(-190, 0), new Vector2(300, 55));
        var retryBtn   = MakeButton("RetryButton",   "다시 하기",   ct, new Vector2(0.5f, 0.12f), new Vector2( 190, 0), new Vector2(300, 55));

        new GameObject("GameManager").AddComponent<GameManager>();
        SetupFader();

        var ecGO = new GameObject("EndingController");
        var ec = ecGO.AddComponent<EndingController>();
        var bf = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
        typeof(EndingController).GetField("titleText",      bf)?.SetValue(ec, MakeRect("_tmp",ct,Vector2.zero,Vector2.zero,Vector2.zero).gameObject.AddComponent<TextMeshProUGUI>());
        typeof(EndingController).GetField("bodyText",       bf)?.SetValue(ec, bodyTMP);
        typeof(EndingController).GetField("scoreText",      bf)?.SetValue(ec, scoreTMP);
        typeof(EndingController).GetField("creditButton",   bf)?.SetValue(ec, creditBtn);
        typeof(EndingController).GetField("retryButton",    bf)?.SetValue(ec, retryBtn);
        typeof(EndingController).GetField("endingType",     bf)?.SetValue(ec, type);
        if (victimTMP != null)
            typeof(EndingController).GetField("victimCountText", bf)?.SetValue(ec, victimTMP);

        // titleText를 올바른 오브젝트로 덮어쓰기
        typeof(EndingController).GetField("titleText", bf)?.SetValue(ec, MakeTMP(ct, new Vector2(0.5f, 0.78f), Vector2.zero, new Vector2(900, 100), title, 60, new Color(0.85f, 0.2f, 0.2f), FontStyles.Bold));

        EnsureEventSystem(scene);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log($"{title} 엔딩 씬 구성 완료!");
    }

    [MenuItem("FakeSoldier/Setup Ending Bad")]
    public static void SetupEndingBad()    => SetupEndingBase(EndingType.Bad,    "복종 엔딩");
    [MenuItem("FakeSoldier/Setup Ending Normal")]
    public static void SetupEndingNormal() => SetupEndingBase(EndingType.Normal, "방관 엔딩");
    [MenuItem("FakeSoldier/Setup Ending True")]
    public static void SetupEndingTrue()   => SetupEndingBase(EndingType.True,   "거부 엔딩");

    // ── Credit 씬 ───────────────────────────────────────────────────

    [MenuItem("FakeSoldier/Setup Credit Scene")]
    public static void SetupCredit()
    {
        var scene = SceneManager.GetActiveScene();
        foreach (var root in scene.GetRootGameObjects())
            Object.DestroyImmediate(root);

        SetupCamera(scene, Color.black);

        var canvas = SetupCanvas("Canvas");
        var ct = canvas.transform;

        // 스크롤 영역
        var scrollRT = MakeRect("CreditContent", ct, new Vector2(0.5f, 0f), new Vector2(0, -540), new Vector2(1200, 2000));
        var credits = new System.Text.StringBuilder();
        credits.AppendLine("FAKE SOLDIER\n");
        credits.AppendLine("1980년 5월, 광주\n\n");
        credits.AppendLine("[ 개발팀 ]\n");
        credits.AppendLine("기획 · 개발\n");
        credits.AppendLine("개발\n");
        credits.AppendLine("에셋\n\n");
        credits.AppendLine("[ 사용 에셋 ]\n");
        credits.AppendLine("SMS RPG Soldier FREE\n\n");
        credits.AppendLine("[ 5.18 민주화운동 ]\n");
        credits.AppendLine("1980년 5월 18일부터 27일까지\n광주에서 일어난 민주화운동.\n\n");
        credits.AppendLine("희생자들을 기억합니다.\n\n\n");
        credits.AppendLine("Thank you for playing.");

        var creditTMP = scrollRT.gameObject.AddComponent<TextMeshProUGUI>();
        creditTMP.text = credits.ToString();
        creditTMP.fontSize = 28;
        creditTMP.color = Color.white;
        creditTMP.alignment = TextAlignmentOptions.Center;
        creditTMP.enableWordWrapping = true;

        var skipBtn = MakeButton("SkipButton", "건너뛰기", ct, new Vector2(0.95f, 0.95f), Vector2.zero, new Vector2(200, 50));
        ((TextMeshProUGUI)skipBtn.GetComponentInChildren<TextMeshProUGUI>()).fontSize = 22;

        new GameObject("GameManager").AddComponent<GameManager>();
        SetupFader();

        var ccGO = new GameObject("CreditController");
        var cc = ccGO.AddComponent<CreditController>();
        var bf = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
        typeof(CreditController).GetField("creditContent", bf)?.SetValue(cc, scrollRT);
        typeof(CreditController).GetField("skipButton",    bf)?.SetValue(cc, skipBtn);

        EnsureEventSystem(scene);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Credit 씬 구성 완료!");
    }
}
