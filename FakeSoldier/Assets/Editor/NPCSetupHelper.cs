using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;

/// FakeSoldier/Generate NPC Sprites + Animators 메뉴에서 실행.
/// SMS Soldier(16x24)와 같은 비율의 NPC 스프라이트를 생성한다.
public static class NPCSetupHelper
{
    const string SPRITES = "Assets/Sprites/NPC";
    const string ANIMS   = "Assets/Animations/NPC";

    // 스프라이트 크기 (SMS Soldier와 동일한 16x24)
    const int W = 16, H = 24;

    // ── 학생 팔레트 ────────────────────────────────────────────────
    static readonly Color sHair = new Color(0.10f, 0.07f, 0.05f, 1f);
    static readonly Color sSkin = new Color(0.85f, 0.68f, 0.48f, 1f);
    static readonly Color sUnif = new Color(0.12f, 0.15f, 0.35f, 1f);
    static readonly Color sShrt = new Color(0.93f, 0.93f, 0.88f, 1f);
    static readonly Color sTie  = new Color(0.70f, 0.10f, 0.12f, 1f);
    static readonly Color sShoe = new Color(0.12f, 0.10f, 0.10f, 1f);
    static readonly Color sEye  = new Color(0.05f, 0.04f, 0.04f, 1f);
    static readonly Color sBelt = new Color(0.22f, 0.18f, 0.14f, 1f);

    // ── 시민 팔레트 ────────────────────────────────────────────────
    static readonly Color cHair = new Color(0.15f, 0.12f, 0.08f, 1f);
    static readonly Color cSkin = new Color(0.80f, 0.63f, 0.43f, 1f);
    static readonly Color cOtr  = new Color(0.35f, 0.27f, 0.18f, 1f);
    static readonly Color cInn  = new Color(0.72f, 0.64f, 0.44f, 1f);
    static readonly Color cBelt = new Color(0.20f, 0.15f, 0.10f, 1f);
    static readonly Color cShoe = new Color(0.25f, 0.18f, 0.12f, 1f);
    static readonly Color cEye  = new Color(0.06f, 0.05f, 0.04f, 1f);

    // ── 픽셀 헬퍼 ──────────────────────────────────────────────────
    static Color[] Blank()
    {
        var p = new Color[W * H];
        for (int i = 0; i < p.Length; i++) p[i] = Color.clear;
        return p;
    }

    static void P(Color[] p, int x, int y, Color c)
    {
        if (x >= 0 && x < W && y >= 0 && y < H) p[y * W + x] = c;
    }

    static void R(Color[] p, int x, int y, int w, int h, Color c)
    {
        for (int py = y; py < y + h; py++)
            for (int px = x; px < x + w; px++)
                P(p, px, py, c);
    }

    // 외곽선 자동 추가 (4방향 이웃 검사)
    static Color[] AddOutline(Color[] pixels)
    {
        var result = (Color[])pixels.Clone();
        int[] dx = { -1, 1,  0, 0 };
        int[] dy = {  0, 0, -1, 1 };
        for (int y = 0; y < H; y++)
        {
            for (int x = 0; x < W; x++)
            {
                if (pixels[y * W + x].a > 0.1f) continue;
                for (int d = 0; d < 4; d++)
                {
                    int nx = x + dx[d], ny = y + dy[d];
                    if (nx >= 0 && nx < W && ny >= 0 && ny < H && pixels[ny * W + nx].a > 0.1f)
                    {
                        result[y * W + x] = Color.black;
                        break;
                    }
                }
            }
        }
        return result;
    }

    // ═══════════════════════════════════════════════════════════════
    //  학생 스프라이트 (16x24)
    //  y=0 하단, y=23 상단 (Unity SetPixel 기준)
    //  레이아웃: 머리(y=13-23), 몸통(y=3-12), 다리/신발(y=0-2)
    // ═══════════════════════════════════════════════════════════════

    static Color[] StudentSouth(int f)
    {
        var p = Blank();

        // --- 머리 (y=14~23) ---
        R(p, 5, 21, 6, 3, sHair);   // 머리 위쪽 y=21-23, x=5-10
        R(p, 4, 14, 1, 8, sHair);   // 왼쪽 머리카락 y=14-21, x=4
        R(p, 11,14, 1, 8, sHair);   // 오른쪽 머리카락 y=14-21, x=11
        R(p, 5, 14, 6, 7, sSkin);   // 얼굴 y=14-20, x=5-10
        P(p, 6, 19, sEye); P(p, 9, 19, sEye);  // 눈 y=19
        P(p, 7, 16, sEye); P(p, 8, 16, sEye);  // 입 y=16

        // --- 목/칼라 (y=12~13) ---
        R(p, 7, 12, 2, 2, sSkin);   // 목 y=12-13, x=7-8
        P(p, 6, 13, sShrt); P(p, 9, 13, sShrt);  // 셔츠 칼라 y=13

        // --- 어깨 (y=11~12) ---
        R(p, 3, 11, 10, 2, sUnif);  // 어깨 y=11-12, x=3-12

        // --- 몸통 (y=3~11) ---
        R(p, 3,  3, 10, 9, sUnif);  // 교복 몸통 y=3-11, x=3-12
        R(p, 6,  6,  4, 5, sShrt);  // 셔츠 y=6-10, x=6-9
        P(p, 7,  9, sTie); P(p, 8, 9, sTie);   // 넥타이 y=9
        P(p, 7,  8, sTie); P(p, 8, 8, sTie);   // 넥타이 y=8
        P(p, 7,  7, sTie); P(p, 8, 7, sTie);   // 넥타이 y=7
        P(p, 8,  6, sTie);                       // 넥타이 끝

        // --- 벨트 (y=3) ---
        R(p, 3, 3, 10, 1, sBelt);

        // --- 다리 (y=0~2, 프레임별 보행) ---
        int llx = 4, rlx = 10;
        if (f == 1) { llx = 3; rlx = 10; }
        if (f == 3) { llx = 4; rlx = 11; }
        R(p, llx, 1, 2, 2, sUnif);  // 왼다리 y=1-2
        R(p, rlx, 1, 2, 2, sUnif);  // 오른다리 y=1-2

        // 신발
        int lsx = llx - 1, rsx = rlx - 1;
        R(p, lsx, 0, 3, 1, sShoe);  // 왼신발 y=0
        R(p, rsx, 0, 3, 1, sShoe);  // 오른신발 y=0

        return AddOutline(p);
    }

    static Color[] StudentNorth(int f)
    {
        var p = Blank();

        // --- 뒷머리 ---
        R(p, 5, 21, 6, 3, sHair);
        R(p, 4, 14, 1, 8, sHair);
        R(p, 11,14, 1, 8, sHair);
        R(p, 5, 14, 6, 7, sHair);   // 얼굴 대신 뒷머리

        // --- 목 ---
        R(p, 7, 12, 2, 2, sSkin);

        // --- 어깨+몸통 (뒷모습) ---
        R(p, 3, 11, 10, 2, sUnif);
        R(p, 3,  3, 10, 9, sUnif);
        R(p, 3,  3, 10, 1, sBelt);

        // --- 다리 ---
        int llx = 4, rlx = 10;
        if (f == 1) { llx = 3; rlx = 10; }
        if (f == 3) { llx = 4; rlx = 11; }
        R(p, llx, 1, 2, 2, sUnif);
        R(p, rlx, 1, 2, 2, sUnif);

        int lsx = llx - 1, rsx = rlx - 1;
        R(p, lsx, 0, 3, 1, sShoe);
        R(p, rsx, 0, 3, 1, sShoe);

        return AddOutline(p);
    }

    static Color[] StudentEast(int f)
    {
        var p = Blank();

        // 측면: 앞발/뒷발 위치
        int frontX = (f == 1) ? 9 : 8;
        int backX  = (f == 1) ? 6 : 7;

        // --- 머리 (측면) ---
        R(p, 6, 20, 5, 4, sHair);   // 머리 위 y=20-23
        R(p, 6, 14, 1, 7, sHair);   // 뒷머리 y=14-20
        R(p, 7, 14, 4, 7, sSkin);   // 얼굴 y=14-20
        P(p, 8, 19, sEye);           // 눈
        P(p, 9, 16, sEye);           // 입

        // --- 목 ---
        R(p, 7, 12, 3, 2, sSkin);

        // --- 어깨+몸통 ---
        R(p, 5, 11, 6, 2, sUnif);
        R(p, 5,  3, 6, 9, sUnif);
        P(p, 5,  9, sShrt); P(p, 5, 8, sShrt); P(p, 5, 7, sShrt);  // 셔츠 측면
        P(p, 5,  7, sTie);
        R(p, 5,  3, 6, 1, sBelt);

        // --- 다리 ---
        R(p, frontX, 1, 2, 2, sUnif);
        R(p, backX,  1, 1, 2, sUnif);
        R(p, frontX - 1, 0, 3, 1, sShoe);
        R(p, backX,  0, 2, 1, sShoe);

        return AddOutline(p);
    }

    static Color[] StudentWest(int f)
    {
        var src = StudentEast(f);
        var p   = Blank();
        for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
                p[y * W + x] = src[y * W + (W - 1 - x)];
        return p;
    }

    static Color[] DrawStudent(string dir, int f) => dir switch
    {
        "NORTH" => StudentNorth(f),
        "EAST"  => StudentEast(f),
        "WEST"  => StudentWest(f),
        _       => StudentSouth(f),
    };

    // ═══════════════════════════════════════════════════════════════
    //  시민 스프라이트 (16x24)
    // ═══════════════════════════════════════════════════════════════

    static Color[] CivilianSouth(int f)
    {
        var p = Blank();

        // --- 머리 ---
        R(p, 5, 21, 6, 3, cHair);
        R(p, 4, 14, 1, 8, cHair);
        R(p, 11,14, 1, 8, cHair);
        R(p, 5, 14, 6, 7, cSkin);
        P(p, 6, 19, cEye); P(p, 9, 19, cEye);
        P(p, 7, 16, cEye); P(p, 8, 16, cEye);

        // --- 목 ---
        R(p, 7, 12, 2, 2, cSkin);
        P(p, 6, 13, cInn); P(p, 9, 13, cInn);

        // --- 어깨 (시민은 어깨가 더 넓음) ---
        R(p, 2, 11, 12, 2, cOtr);

        // --- 몸통 (작업복) ---
        R(p, 2,  3, 12, 9, cOtr);
        R(p, 6,  6,  4, 5, cInn);   // 속셔츠 y=6-10, x=6-9
        R(p, 2,  3, 12, 1, cBelt);  // 벨트 y=3

        // --- 다리 ---
        int llx = 4, rlx = 10;
        if (f == 1) { llx = 3; rlx = 10; }
        if (f == 3) { llx = 4; rlx = 11; }
        R(p, llx, 1, 2, 2, cOtr);
        R(p, rlx, 1, 2, 2, cOtr);

        int lsx = llx - 1, rsx = rlx - 1;
        R(p, lsx, 0, 3, 1, cShoe);
        R(p, rsx, 0, 3, 1, cShoe);

        return AddOutline(p);
    }

    static Color[] CivilianNorth(int f)
    {
        var p = Blank();

        R(p, 5, 21, 6, 3, cHair);
        R(p, 4, 14, 1, 8, cHair);
        R(p, 11,14, 1, 8, cHair);
        R(p, 5, 14, 6, 7, cHair);

        R(p, 7, 12, 2, 2, cSkin);
        R(p, 2, 11, 12, 2, cOtr);
        R(p, 2,  3, 12, 9, cOtr);
        R(p, 2,  3, 12, 1, cBelt);

        int llx = 4, rlx = 10;
        if (f == 1) { llx = 3; rlx = 10; }
        if (f == 3) { llx = 4; rlx = 11; }
        R(p, llx, 1, 2, 2, cOtr);
        R(p, rlx, 1, 2, 2, cOtr);

        int lsx = llx - 1, rsx = rlx - 1;
        R(p, lsx, 0, 3, 1, cShoe);
        R(p, rsx, 0, 3, 1, cShoe);

        return AddOutline(p);
    }

    static Color[] CivilianEast(int f)
    {
        var p = Blank();

        int frontX = (f == 1) ? 9 : 8;
        int backX  = (f == 1) ? 6 : 7;

        R(p, 6, 20, 5, 4, cHair);
        R(p, 6, 14, 1, 7, cHair);
        R(p, 7, 14, 4, 7, cSkin);
        P(p, 8, 19, cEye);
        P(p, 9, 16, cEye);

        R(p, 7, 12, 3, 2, cSkin);
        R(p, 4, 11, 7, 2, cOtr);
        R(p, 4,  3, 7, 9, cOtr);
        P(p, 4,  9, cInn); P(p, 4, 8, cInn); P(p, 4, 7, cInn);
        R(p, 4,  3, 7, 1, cBelt);

        R(p, frontX, 1, 2, 2, cOtr);
        R(p, backX,  1, 1, 2, cOtr);
        R(p, frontX - 1, 0, 3, 1, cShoe);
        R(p, backX,  0, 2, 1, cShoe);

        return AddOutline(p);
    }

    static Color[] CivilianWest(int f)
    {
        var src = CivilianEast(f);
        var p   = Blank();
        for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
                p[y * W + x] = src[y * W + (W - 1 - x)];
        return p;
    }

    static Color[] DrawCivilian(string dir, int f) => dir switch
    {
        "NORTH" => CivilianNorth(f),
        "EAST"  => CivilianEast(f),
        "WEST"  => CivilianWest(f),
        _       => CivilianSouth(f),
    };

    // ═══════════════════════════════════════════════════════════════
    //  스트립 생성 + 저장 + 임포터
    // ═══════════════════════════════════════════════════════════════

    static Texture2D MakeStrip(System.Func<string, int, Color[]> drawFn, string dir)
    {
        const int F = 4;
        var tex = new Texture2D(W * F, H, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        for (int f = 0; f < F; f++)
        {
            var frame = drawFn(dir, f);
            for (int y = 0; y < H; y++)
                for (int x = 0; x < W; x++)
                    tex.SetPixel(f * W + x, y, frame[y * W + x]);
        }
        tex.Apply();
        return tex;
    }

    static void SavePNG(Texture2D tex, string name)
    {
        File.WriteAllBytes(Application.dataPath + "/Sprites/NPC/" + name + ".png", tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
    }

    static void SetupImporter(string assetPath)
    {
        var imp = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (imp == null) return;
        imp.textureType         = TextureImporterType.Sprite;
        imp.spriteImportMode    = SpriteImportMode.Multiple;
        imp.filterMode          = FilterMode.Point;
        imp.spritePixelsPerUnit = 16;
        imp.textureCompression  = TextureImporterCompression.Uncompressed;
        var metas = new SpriteMetaData[4];
        string baseName = Path.GetFileNameWithoutExtension(assetPath);
        for (int i = 0; i < 4; i++)
            metas[i] = new SpriteMetaData
            {
                name      = baseName + "_" + i,
                rect      = new Rect(i * W, 0, W, H),
                pivot     = new Vector2(0.5f, 0.5f),
                alignment = (int)SpriteAlignment.Center
            };
#pragma warning disable CS0618
        imp.spritesheet = metas;
#pragma warning restore CS0618
        imp.SaveAndReimport();
    }

    static Sprite GetSprite(string assetPath, int idx)
    {
        string target = Path.GetFileNameWithoutExtension(assetPath) + "_" + idx;
        foreach (var a in AssetDatabase.LoadAllAssetsAtPath(assetPath))
            if (a is Sprite s && s.name == target) return s;
        return null;
    }

    // ═══════════════════════════════════════════════════════════════
    //  애니메이터 생성
    // ═══════════════════════════════════════════════════════════════

    static AnimationClip MakeClip(string clipPath, Sprite[] frames, float fps)
    {
        var clip = new AnimationClip { frameRate = fps };
        var binding = new EditorCurveBinding
            { path = "", type = typeof(SpriteRenderer), propertyName = "m_Sprite" };
        var kf = new ObjectReferenceKeyframe[frames.Length];
        for (int i = 0; i < frames.Length; i++) { kf[i].time = i / fps; kf[i].value = frames[i]; }
        AnimationUtility.SetObjectReferenceCurve(clip, binding, kf);
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);
        if (File.Exists(Application.dataPath.Replace("Assets", "") + clipPath))
            AssetDatabase.DeleteAsset(clipPath);
        AssetDatabase.CreateAsset(clip, clipPath);
        return clip;
    }

    static void CreateAnimator(string npcName)
    {
        string[] dirs = { "SOUTH", "NORTH", "EAST", "WEST" };
        var      vecs = new Vector2[] { new(0, -1), new(0, 1), new(1, 0), new(-1, 0) };

        string ctrlPath = $"{ANIMS}/NPC_{npcName}Controller.controller";
        if (AssetDatabase.AssetPathToGUID(ctrlPath) != "")
            AssetDatabase.DeleteAsset(ctrlPath);

        var ctrl = AnimatorController.CreateAnimatorControllerAtPath(ctrlPath);
        ctrl.AddParameter("MoveX",    AnimatorControllerParameterType.Float);
        ctrl.AddParameter("MoveY",    AnimatorControllerParameterType.Float);
        ctrl.AddParameter("IsMoving", AnimatorControllerParameterType.Bool);

        var sm        = ctrl.layers[0].stateMachine;
        var idleState = sm.AddState("Idle");
        var walkState = sm.AddState("Walk");
        sm.defaultState = idleState;

        BlendTree MakeBlend(string bname, (AnimationClip c, Vector2 v)[] entries)
        {
            var tree = new BlendTree
            {
                name            = bname,
                blendType       = BlendTreeType.SimpleDirectional2D,
                blendParameter  = "MoveX",
                blendParameterY = "MoveY"
            };
            foreach (var e in entries) tree.AddChild(e.c, e.v);
            AssetDatabase.AddObjectToAsset(tree, ctrl);
            return tree;
        }

        var idleE = new (AnimationClip, Vector2)[4];
        var walkE = new (AnimationClip, Vector2)[4];

        for (int d = 0; d < 4; d++)
        {
            string dir     = dirs[d];
            string pngPath = $"{SPRITES}/NPC_{npcName}_{dir}_strip4.png";
            var    allSpr  = new Sprite[4];
            for (int i = 0; i < 4; i++) allSpr[i] = GetSprite(pngPath, i);

            var idleClip = MakeClip($"{ANIMS}/{npcName}_{dir}_idle.anim", new[] { allSpr[0] }, 1f);
            var walkClip = MakeClip($"{ANIMS}/{npcName}_{dir}_walk.anim", allSpr, 8f);

            idleE[d] = (idleClip, vecs[d]);
            walkE[d] = (walkClip, vecs[d]);
        }

        idleState.motion = MakeBlend("IdleBlend", idleE);
        walkState.motion = MakeBlend("WalkBlend", walkE);

        var toWalk = idleState.AddTransition(walkState);
        toWalk.AddCondition(AnimatorConditionMode.If, 0, "IsMoving");
        toWalk.hasExitTime = false; toWalk.duration = 0;

        var toIdle = walkState.AddTransition(idleState);
        toIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsMoving");
        toIdle.hasExitTime = false; toIdle.duration = 0;

        AssetDatabase.SaveAssets();
        Debug.Log($"NPC_{npcName}Controller 생성 완료");
    }

    // ═══════════════════════════════════════════════════════════════
    //  메인 진입점
    // ═══════════════════════════════════════════════════════════════

    [MenuItem("FakeSoldier/Generate NPC Sprites + Animators")]
    public static void GenerateAll()
    {
        Directory.CreateDirectory(Application.dataPath + "/Sprites/NPC");
        Directory.CreateDirectory(Application.dataPath + "/Animations/NPC");
        AssetDatabase.Refresh();

        var drawFuncs = new System.Func<string, int, Color[]>[] { DrawStudent, DrawCivilian };
        string[] npcNames = { "Student", "Civilian" };
        string[] dirs = { "SOUTH", "NORTH", "EAST", "WEST" };

        for (int n = 0; n < 2; n++)
            for (int d = 0; d < 4; d++)
            {
                var strip = MakeStrip(drawFuncs[n], dirs[d]);
                SavePNG(strip, $"NPC_{npcNames[n]}_{dirs[d]}_strip4");
            }

        AssetDatabase.Refresh();

        for (int n = 0; n < 2; n++)
            for (int d = 0; d < 4; d++)
                SetupImporter($"{SPRITES}/NPC_{npcNames[n]}_{dirs[d]}_strip4.png");

        AssetDatabase.Refresh();

        foreach (var name in npcNames)
            CreateAnimator(name);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("NPC 스프라이트(16x24) + 애니메이터 생성 완료!");
    }
}
