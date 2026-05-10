using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Reflection;

/// FakeSoldier/Place NPCs in All Stages 메뉴에서 실행.
public static class NPCScenePlacer
{
    const string S_PNG  = "Assets/Sprites/NPC/NPC_Student_{0}_strip4.png";
    const string C_PNG  = "Assets/Sprites/NPC/NPC_Civilian_{0}_strip4.png";
    const string S_CTRL = "Assets/Animations/NPC/NPC_StudentController.controller";
    const string C_CTRL = "Assets/Animations/NPC/NPC_CivilianController.controller";

    const float ZONE_X = 7.0f;
    const float ZONE_Y = -3.15f;

    [MenuItem("FakeSoldier/Place NPCs in All Stages")]
    public static void PlaceAll()
    {
        SetupStage("Assets/Scenes/Stage_01.unity", BuildStage01());
        SetupStage("Assets/Scenes/Stage_02.unity", BuildStage02());
        SetupStage("Assets/Scenes/Stage_03.unity", BuildStage03());
        SetupStage("Assets/Scenes/Stage_04.unity", BuildStage04());
        SetupStage("Assets/Scenes/Stage_05.unity", BuildStage05());
        Debug.Log("모든 스테이지 NPC 배치 완료!");
    }

    struct NPCDef
    {
        public string  goName;
        public string  displayName;
        public bool    isStudent;
        public string  direction;
        public Vector3 pos;
        public float   scale;
        public Color   tint;
        public (string spk, string txt)[] lines;
    }

    // ── Stage 01 – 학생 집회 ──────────────────────────────────────
    // 화살표 지점: 학생들이 몰려있는 시위 현장
    // → 무언 NPC 3명이 군중처럼 밀집 배치
    static NPCDef[] BuildStage01() => new[]
    {
        // ▸ 대화 NPC (왼쪽 2명)
        new NPCDef
        {
            goName = "NPC_Student_01", displayName = "학생", isStudent = true,
            direction = "SOUTH", pos = new Vector3(-7f, -3.5f, 0), scale = 0.40f,
            tint = new Color(0.85f, 0.90f, 1.00f),
            lines = new[]
            {
                ("학생", "어제 친구가 끌려갔어요. 이유도 안 알려줬어요."),
                ("학생", "혼자만 집에 갈 수가 없어서. 그래서 여기 있는 거예요.")
            }
        },
        new NPCDef
        {
            goName = "NPC_Student_02", displayName = "학생", isStudent = true,
            direction = "WEST", pos = new Vector3(-2f, -2.8f, 0), scale = 0.47f,
            tint = new Color(1.00f, 0.85f, 0.75f),
            lines = new[]
            {
                ("학생", "계엄령 철폐! 전두환은 물러가라!"),
                ("학생", "군인이면 나라를 지켜야지, 왜 학생들한테 총부리를 들이밀어요!")
            }
        },
        // ▸ 화살표 지점 – 시위 군중 3명 밀집 (무언)
        new NPCDef
        {
            goName = "NPC_Crowd_01", displayName = "학생", isStudent = true,
            direction = "WEST", pos = new Vector3(ZONE_X, ZONE_Y, 0), scale = 0.46f,
            tint = new Color(1.00f, 0.88f, 0.78f),
            lines = System.Array.Empty<(string, string)>()
        },
        new NPCDef
        {
            goName = "NPC_Crowd_02", displayName = "학생", isStudent = true,
            direction = "SOUTH", pos = new Vector3(ZONE_X - 0.9f, ZONE_Y + 0.5f, 0), scale = 0.42f,
            tint = new Color(0.88f, 0.95f, 1.00f),
            lines = System.Array.Empty<(string, string)>()
        },
        new NPCDef
        {
            goName = "NPC_Crowd_03", displayName = "학생", isStudent = true,
            direction = "NORTH", pos = new Vector3(ZONE_X + 0.7f, ZONE_Y - 0.4f, 0), scale = 0.38f,
            tint = new Color(0.95f, 0.88f, 1.00f),
            lines = System.Array.Empty<(string, string)>()
        }
    };

    // ── Stage 02 – 민가 수색 (골목) ──────────────────────────────
    // 화살표 지점: 골목 끝에 홀로 숨어있는 주민 (작은 체구, 겁먹은 자세)
    static NPCDef[] BuildStage02() => new[]
    {
        // ▸ 대화 NPC (2명)
        new NPCDef
        {
            goName = "NPC_Civilian_01", displayName = "노인", isStudent = false,
            direction = "SOUTH", pos = new Vector3(-7f, -3.2f, 0), scale = 0.52f,
            tint = new Color(0.80f, 0.78f, 0.72f),
            lines = new[]
            {
                ("노인", "들어오지 마시오. 여기 마누라 제삿상도 아직 안 치웠소."),
                ("노인", "60년 넘게 여기 살았어. 나한테 뭘 찾겠다는 거요.")
            }
        },
        new NPCDef
        {
            goName = "NPC_Civilian_02", displayName = "여성", isStudent = false,
            direction = "EAST", pos = new Vector3(0f, -3.8f, 0), scale = 0.42f,
            tint = new Color(1.00f, 0.88f, 0.88f),
            lines = new[]
            {
                ("여성", "애가 아까부터 울음을 안 그쳐요. 총소리 들었나봐요."),
                ("여성", "제발 빨리 끝내주세요. 이 골목 사람들 아무도 나쁜 사람 없어요.")
            }
        },
        // ▸ 화살표 지점 – 구석에 몸 숨긴 주민 (작은 스케일, 움츠린 느낌)
        new NPCDef
        {
            goName = "NPC_Hiding", displayName = "주민", isStudent = false,
            direction = "EAST", pos = new Vector3(ZONE_X, ZONE_Y, 0), scale = 0.35f,
            tint = new Color(0.68f, 0.72f, 0.78f),
            lines = System.Array.Empty<(string, string)>()
        }
    };

    // ── Stage 03 – 검문소 ─────────────────────────────────────────
    // 화살표 지점: 검문소 담당자 – 단호하게 길을 막고 서있는 사람
    static NPCDef[] BuildStage03() => new[]
    {
        // ▸ 대화 NPC (2명)
        new NPCDef
        {
            goName = "NPC_Civilian_01", displayName = "여성", isStudent = false,
            direction = "SOUTH", pos = new Vector3(-6f, -3.6f, 0), scale = 0.42f,
            tint = new Color(1.00f, 0.82f, 0.82f),
            lines = new[]
            {
                ("여성", "남편이요... 아침에 출근하다 총에 맞았어요."),
                ("여성", "길바닥에 쓰러져 있는데 아무도 못 지나가게 해요. 제발 살려주세요.")
            }
        },
        new NPCDef
        {
            goName = "NPC_Civilian_02", displayName = "노인", isStudent = false,
            direction = "EAST", pos = new Vector3(0f, -2.8f, 0), scale = 0.52f,
            tint = new Color(0.80f, 0.75f, 0.70f),
            lines = new[]
            {
                ("노인", "병원에 가야 할 사람이 저기 쌓여있는데, 군복 입은 사람들이 막아서면 어디로 가란 말이요!"),
                ("노인", "당신도 어머니가 있지 않소. 그 사람들도 누군가의 자식이에요.")
            }
        },
        // ▸ 화살표 지점 – 검문 담당자 (크고 단호한 체구, 군복 톤)
        new NPCDef
        {
            goName = "NPC_Checkpoint", displayName = "담당자", isStudent = false,
            direction = "WEST", pos = new Vector3(ZONE_X, ZONE_Y, 0), scale = 0.55f,
            tint = new Color(0.72f, 0.82f, 0.65f),
            lines = System.Array.Empty<(string, string)>()
        }
    };

    // ── Stage 04 – 도청 진압 (시민군) ────────────────────────────
    // 화살표 지점: 도청 앞 방어선 – 시민군 2명이 나란히 서서 입구를 지킴
    static NPCDef[] BuildStage04() => new[]
    {
        // ▸ 대화 NPC (2명)
        new NPCDef
        {
            goName = "NPC_Civilian_01", displayName = "시민군", isStudent = false,
            direction = "EAST", pos = new Vector3(-7f, -3.1f, 0), scale = 0.52f,
            tint = new Color(0.88f, 0.83f, 0.73f),
            lines = new[]
            {
                ("시민군", "나 아이가 셋이오. 근데도 여기 있는 거요."),
                ("시민군", "내 아이들이 살 나라가 어떤 나라여야 하는지, 그게 이 자리에 선 이유요.")
            }
        },
        new NPCDef
        {
            goName = "NPC_Student_01", displayName = "시민군", isStudent = true,
            direction = "SOUTH", pos = new Vector3(-1f, -3.8f, 0), scale = 0.40f,
            tint = new Color(0.88f, 0.92f, 0.88f),
            lines = new[]
            {
                ("시민군", "총 처음 잡아봤어요. 되게 무거워요."),
                ("시민군", "이걸 쏜다는 게 상상이 안 가는데, 여기 있는 게 맞는 것 같아요.")
            }
        },
        // ▸ 화살표 지점 – 도청 입구 방어선 2명 (나란히 서서 입구 사수)
        new NPCDef
        {
            goName = "NPC_Defender_01", displayName = "시민군", isStudent = false,
            direction = "WEST", pos = new Vector3(ZONE_X, ZONE_Y, 0), scale = 0.50f,
            tint = new Color(0.82f, 0.85f, 0.70f),
            lines = System.Array.Empty<(string, string)>()
        },
        new NPCDef
        {
            goName = "NPC_Defender_02", displayName = "시민군", isStudent = true,
            direction = "WEST", pos = new Vector3(ZONE_X - 0.8f, ZONE_Y + 0.45f, 0), scale = 0.44f,
            tint = new Color(0.78f, 0.88f, 0.78f),
            lines = System.Array.Empty<(string, string)>()
        }
    };

    // ── Stage 05 – 진압 이후 ──────────────────────────────────────
    // 화살표 지점: 상관 단독 – 냉담하게 서류를 들고 서 있음
    static NPCDef[] BuildStage05() => new[]
    {
        // ▸ 대화 NPC (2명)
        new NPCDef
        {
            goName = "NPC_Civilian_01", displayName = "부상자", isStudent = false,
            direction = "SOUTH", pos = new Vector3(-7f, -3.8f, 0), scale = 0.36f,
            tint = new Color(0.65f, 0.65f, 0.70f),
            lines = new[]
            {
                ("부상자", "...고통스러워요."),
                ("부상자", "제 이름은 박정국이에요. 서른여덟 살이에요. 기억해줄 사람이 없을 것 같아서요.")
            }
        },
        new NPCDef
        {
            goName = "NPC_Student_01", displayName = "생존자", isStudent = true,
            direction = "SOUTH", pos = new Vector3(0f, -3.2f, 0), scale = 0.44f,
            tint = new Color(0.78f, 0.78f, 0.80f),
            lines = new[]
            {
                ("생존자", "끝났대요. 진압 완료됐대요."),
                ("생존자", "...근데 이게 끝이면, 지금 이 느낌은 뭔가요.")
            }
        },
        // ▸ 화살표 지점 – 상관 (서류 들고 냉담히 기다리는 느낌, 크고 당당한 체구)
        new NPCDef
        {
            goName = "NPC_Officer", displayName = "상관", isStudent = false,
            direction = "WEST", pos = new Vector3(ZONE_X, ZONE_Y, 0), scale = 0.54f,
            tint = new Color(0.68f, 0.76f, 0.62f),
            lines = new[]
            {
                ("상관", "왜 아직 거기 서 있나. 다 끝났어."),
                ("상관", "서명 한 장이면 끝이야. 우린 명령을 따른 것뿐이고.")
            }
        }
    };

    // ── 씬 배치 ──────────────────────────────────────────────────
    static void SetupStage(string scenePath, NPCDef[] defs)
    {
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        foreach (var root in scene.GetRootGameObjects())
            if (root.GetComponent<NPCController>() != null)
                Object.DestroyImmediate(root);

        foreach (var def in defs)
            PlaceNPC(scene, def);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log($"{scenePath} → NPC {defs.Length}개 배치 완료");
    }

    static void PlaceNPC(UnityEngine.SceneManagement.Scene scene, NPCDef def)
    {
        var go = new GameObject(def.goName);
        SceneManager.MoveGameObjectToScene(go, scene);
        go.transform.position   = def.pos;
        float s = def.scale > 0f ? def.scale : 0.45f;
        go.transform.localScale = new Vector3(s, s, 1f);

        var sr = go.AddComponent<SpriteRenderer>();
        var pngPath = string.Format(def.isStudent ? S_PNG : C_PNG, def.direction);
        sr.sprite       = LoadFrame0(pngPath);
        sr.color        = def.tint.a > 0f ? def.tint : Color.white;
        sr.sortingOrder = Mathf.Max(1, Mathf.RoundToInt(-def.pos.y));

        var anim = go.AddComponent<Animator>();
        var ctrl = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(
                       def.isStudent ? S_CTRL : C_CTRL);
        if (ctrl != null) anim.runtimeAnimatorController = ctrl;

        var dirVec = DirectionToVec(def.direction);
        anim.SetFloat("MoveX", dirVec.x);
        anim.SetFloat("MoveY", dirVec.y);

        var col   = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size      = new Vector2(1.2f, 1.2f);

        var npc = go.AddComponent<NPCController>();
        var bf  = BindingFlags.NonPublic | BindingFlags.Instance;
        var t   = typeof(NPCController);
        t.GetField("npcName",        bf)?.SetValue(npc, def.displayName);
        t.GetField("dialogueLines",  bf)?.SetValue(npc, BuildLines(def.lines));
        t.GetField("defaultFaceDir", bf)?.SetValue(npc, dirVec);

        EditorUtility.SetDirty(go);
    }

    static Sprite LoadFrame0(string assetPath)
    {
        string baseName = System.IO.Path.GetFileNameWithoutExtension(assetPath) + "_0";
        foreach (var a in AssetDatabase.LoadAllAssetsAtPath(assetPath))
            if (a is Sprite s && s.name == baseName) return s;
        foreach (var a in AssetDatabase.LoadAllAssetsAtPath(assetPath))
            if (a is Sprite s) return s;
        return null;
    }

    static List<DialogueLine> BuildLines((string spk, string txt)[] pairs)
    {
        var list = new List<DialogueLine>();
        foreach (var (spk, txt) in pairs)
            list.Add(new DialogueLine { speaker = spk, text = txt });
        return list;
    }

    static Vector2 DirectionToVec(string dir) => dir switch
    {
        "NORTH" => new Vector2( 0,  1),
        "EAST"  => new Vector2( 1,  0),
        "WEST"  => new Vector2(-1,  0),
        _       => new Vector2( 0, -1),
    };
}
