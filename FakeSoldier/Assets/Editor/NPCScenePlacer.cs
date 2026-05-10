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

    const float ZX = 18.22f;
    const float NPC_SCALE = 1.4f;
    const float ZY = -3.15f;

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

    // ════════════════════════════════════════════════════════════════
    //  STAGE 01 — 학생 집회 (전남도청 앞, 1980년 5월 18일 낮)
    //
    //  상황: 계엄 선포 직후, 학생들이 도청 앞 금남로에 집결해 구호를 외침.
    //        아직 총격은 없지만 군인들의 진압봉이 가까워지고 있다.
    //  분위기: 결의·공포·연대. 따뜻한 낮빛. 군중의 열기.
    //
    //  [대화 NPC]  왼쪽 배치 — 플레이어가 지나치며 말 걸 수 있는 학생들
    //  [화살표 군중] 오른쪽 집결지 — 시위대의 핵심 무리, 7명이 빽빽하게 모여있음
    // ════════════════════════════════════════════════════════════════
    static NPCDef[] BuildStage01() => new[]
    {
        // ── 대화 NPC ①: 친구를 잃고 혼자 남은 학생 ──────────────
        // 시각: 차갑고 창백한 파랑 → 밤새 울다 지친 눈빛
        new NPCDef
        {
            goName = "NPC_Student_Alone", displayName = "학생", isStudent = true,
            direction = "SOUTH", pos = new Vector3(-15f, -3.5f, 0), scale = 0.39f,
            tint = new Color(0.80f, 0.87f, 1.00f),
            lines = new[]
            {
                ("학생", "어제 수업 끝나고 친구 다섯이서 여기 왔어요. 오늘 아침에 셋이 끌려갔어요."),
                ("학생", "이유도 안 알려줬어요. 연락도 안 돼요. 그래서 못 가겠어요. 나라도 여기 있어야 해서.")
            }
        },
        // ── 대화 NPC ②: 구호를 외치는 학생 ─────────────────────
        // 시각: 뜨거운 주황빛 → 목청껏 외치는 열기
        new NPCDef
        {
            goName = "NPC_Student_Shout", displayName = "학생", isStudent = true,
            direction = "WEST", pos = new Vector3(-5f, -2.7f, 0), scale = 0.48f,
            tint = new Color(1.00f, 0.83f, 0.65f),
            lines = new[]
            {
                ("학생", "계엄령이 민주주의를 죽이는 거예요! 우리가 그걸 막으러 나온 거고!"),
                ("학생", "총 들이대도 안 갑니다. 여기가 우리 나라예요!")
            }
        },

        // ── 화살표 군중: 집결지 핵심 무리 7명 (무언) ─────────────
        // 다양한 방향·크기·색조 → 실제 군중처럼 어수선하게 몰려있는 느낌
        new NPCDef // 맨 앞 — 팔짱 끼고 결연히 서있는 리더형
        {
            goName = "NPC_Crowd_Leader", displayName = "학생", isStudent = true,
            direction = "WEST", pos = new Vector3(ZX, ZY, 0), scale = 0.50f,
            tint = new Color(1.00f, 0.88f, 0.68f),
            lines = System.Array.Empty<(string, string)>()
        },
        new NPCDef // 좌상단 — 옆 사람과 얘기 중
        {
            goName = "NPC_Crowd_A", displayName = "학생", isStudent = true,
            direction = "EAST", pos = new Vector3(ZX - 1.1f, ZY + 0.55f, 0), scale = 0.43f,
            tint = new Color(0.83f, 0.93f, 1.00f),
            lines = System.Array.Empty<(string, string)>()
        },
        new NPCDef // 우상단 — 하늘 향해 주먹 쥐고 구호
        {
            goName = "NPC_Crowd_B", displayName = "학생", isStudent = true,
            direction = "NORTH", pos = new Vector3(ZX + 1.0f, ZY + 0.50f, 0), scale = 0.40f,
            tint = new Color(1.00f, 0.80f, 0.80f),
            lines = System.Array.Empty<(string, string)>()
        },
        new NPCDef // 좌하단 — 뒤에서 조용히 함께
        {
            goName = "NPC_Crowd_C", displayName = "학생", isStudent = true,
            direction = "SOUTH", pos = new Vector3(ZX - 0.8f, ZY - 0.55f, 0), scale = 0.44f,
            tint = new Color(0.90f, 0.92f, 0.82f),
            lines = System.Array.Empty<(string, string)>()
        },
        new NPCDef // 우하단 — 앉아서 버티는 학생 (작은 스케일)
        {
            goName = "NPC_Crowd_D", displayName = "학생", isStudent = true,
            direction = "SOUTH", pos = new Vector3(ZX + 0.9f, ZY - 0.65f, 0), scale = 0.36f,
            tint = new Color(0.75f, 0.90f, 0.78f),
            lines = System.Array.Empty<(string, string)>()
        },
        new NPCDef // 오른쪽 끝 — 멀리서 막 합류한 학생
        {
            goName = "NPC_Crowd_E", displayName = "학생", isStudent = true,
            direction = "WEST", pos = new Vector3(ZX + 1.6f, ZY - 0.10f, 0), scale = 0.45f,
            tint = new Color(0.95f, 0.85f, 0.65f),
            lines = System.Array.Empty<(string, string)>()
        },
        new NPCDef // 군중 뒤쪽 상단 — 작고 멀어보이는 학생
        {
            goName = "NPC_Crowd_F", displayName = "학생", isStudent = true,
            direction = "SOUTH", pos = new Vector3(ZX + 1.3f, ZY + 0.80f, 0), scale = 0.33f,
            tint = new Color(0.88f, 0.78f, 1.00f),
            lines = System.Array.Empty<(string, string)>()
        }
    };

    // ════════════════════════════════════════════════════════════════
    //  STAGE 02 — 민가 수색 (광주 골목, 5월 19일 새벽)
    //
    //  상황: 군인들이 민가를 수색하며 골목 안으로 들어왔다.
    //        주민들은 문을 잠그거나 숨어있다. 아이 울음소리가 들린다.
    //  분위기: 공포·억울함·무고함. 어두운 새벽빛. 조용한 공포.
    //
    //  [화살표] 골목 막다른 끝 — 숨어있는 주민 2명
    //           한 명은 완전히 몸을 숨기고, 한 명은 문틈으로 살피는 중
    // ════════════════════════════════════════════════════════════════
    static NPCDef[] BuildStage02() => new[]
    {
        // ── 대화 NPC ①: 60년 산 완고한 노인 ─────────────────────
        // 시각: 황갈색, 세월의 무게 → 굳건하지만 당혹스러운 표정
        new NPCDef
        {
            goName = "NPC_Elder", displayName = "노인", isStudent = false,
            direction = "SOUTH", pos = new Vector3(-15f, -3.2f, 0), scale = 0.54f,
            tint = new Color(0.78f, 0.74f, 0.66f),
            lines = new[]
            {
                ("노인", "6·25 때도 이 집에서 버텼소. 그때는 적이 누군지 알았는데."),
                ("노인", "지금은... 총 든 사람들이 우리 편인지 적인지도 모르겠구먼. 참 이상한 세상이오.")
            }
        },
        // ── 대화 NPC ②: 아이를 지키려는 어머니 ─────────────────
        // 시각: 연분홍, 불안과 모성 → 두 팔로 무언가를 감싸는 자세
        new NPCDef
        {
            goName = "NPC_Mother", displayName = "여성", isStudent = false,
            direction = "EAST", pos = new Vector3(-1f, -3.8f, 0), scale = 0.42f,
            tint = new Color(1.00f, 0.86f, 0.86f),
            lines = new[]
            {
                ("여성", "애가 어젯밤에 총소리 듣고 지금도 이불 속에서 안 나와요."),
                ("여성", "이 동네 사람들 다 착해요. 나쁜 사람 없어요. 제발 그냥 가주세요.")
            }
        },

        // ── 화살표: 골목 끝 숨은 주민 2명 ───────────────────────
        new NPCDef // 완전히 몸을 숨긴 주민 — 작고 어둡게, 벽에 바짝 붙음
        {
            goName = "NPC_Hiding_Main", displayName = "주민", isStudent = false,
            direction = "EAST", pos = new Vector3(ZX, ZY, 0), scale = 0.31f,
            tint = new Color(0.50f, 0.55f, 0.65f), // 어두운 회청색 — 극도의 공포
            lines = System.Array.Empty<(string, string)>()
        },
        new NPCDef // 문틈으로 조심스럽게 밖을 살피는 사람
        {
            goName = "NPC_Hiding_Peek", displayName = "주민", isStudent = false,
            direction = "WEST", pos = new Vector3(ZX + 0.7f, ZY + 0.60f, 0), scale = 0.36f,
            tint = new Color(0.58f, 0.62f, 0.70f), // 약간 밝지만 여전히 어두운 파란 회색
            lines = System.Array.Empty<(string, string)>()
        }
    };

    // ════════════════════════════════════════════════════════════════
    //  STAGE 03 — 검문소 (광주 시내 교차로, 5월 20일)
    //
    //  상황: 군인들이 도심 교차로를 봉쇄했다. 총상을 입은 시민이 길바닥에
    //        쓰러져 있고, 아내가 피를 막으려 손으로 누르고 있다.
    //        병원은 두 블록 앞인데 군인들이 통행을 막고 있다.
    //  분위기: 절박함·분노·죽음의 냄새. ★피·부상 요소 핵심★
    //
    //  [대화 NPC] 절박한 아내 (빨간 톤) + 분노한 노인
    //  [화살표]   검문 군인 2명 + 길바닥에 쓰러진 남편 (★ 어두운 혈색 ★)
    // ════════════════════════════════════════════════════════════════
    static NPCDef[] BuildStage03() => new[]
    {
        // ── 대화 NPC ①: 절박한 아내 ── ★피/부상 핵심 장면★ ─────
        // 시각: 진한 빨강 → 남편 피가 손에 묻어있는 상태, 눈이 충혈됨
        new NPCDef
        {
            goName = "NPC_Wife", displayName = "여성", isStudent = false,
            direction = "WEST", pos = new Vector3(-12f, -3.6f, 0), scale = 0.43f,
            tint = new Color(1.00f, 0.58f, 0.58f), // 진한 빨강 — 피, 충혈된 눈, 절박함
            lines = new[]
            {
                ("여성", "남편이 출근하다가... 총에 맞았어요. 지금 저기 길바닥에 쓰러져 있어요."),
                ("여성", "30분째 손으로 누르고 있어요. 피가 안 멈춰요. 병원이 두 블록이에요!"),
                ("여성", "제발요. 당신이 막고 있으면 남편이 죽어요. 지금 죽어가고 있다고요!")
            }
        },
        // ── 대화 NPC ②: 분노한 노인 ─────────────────────────────
        // 시각: 거칠고 붉은 황갈색 → 노발대발, 눈물과 분노
        new NPCDef
        {
            goName = "NPC_AngerElder", displayName = "노인", isStudent = false,
            direction = "EAST", pos = new Vector3(2f, -2.8f, 0), scale = 0.54f,
            tint = new Color(0.85f, 0.68f, 0.55f), // 붉은 황갈색 — 노인의 분노와 당혹
            lines = new[]
            {
                ("노인", "저기 봐요. 핏자국이 저기까지 왔잖아요. 보여요? 아직도 안 보여요?"),
                ("노인", "군인이 시민을 지키는 게 맞잖소. 저 사람이 무슨 죄를 졌다는 거요!")
            }
        },

        // ── 화살표: 검문소 봉쇄 지점 ─────────────────────────────
        new NPCDef // 정면 차단 군인 — 크고 위압적, 팔 벌려 막음
        {
            goName = "NPC_Guard_Main", displayName = "담당자", isStudent = false,
            direction = "WEST", pos = new Vector3(ZX, ZY, 0), scale = 0.62f,
            tint = new Color(0.65f, 0.78f, 0.58f), // 짙은 군복 올리브
            lines = System.Array.Empty<(string, string)>()
        },
        new NPCDef // 보조 군인 — 서류 들고 뒤에서 확인
        {
            goName = "NPC_Guard_Aide", displayName = "담당자", isStudent = false,
            direction = "SOUTH", pos = new Vector3(ZX + 1.0f, ZY + 0.45f, 0), scale = 0.47f,
            tint = new Color(0.70f, 0.80f, 0.62f),
            lines = System.Array.Empty<(string, string)>()
        },
        new NPCDef // ★ 길바닥에 쓰러진 남편 ★ — 핵심 시각 요소
                   // 극히 작은 스케일 + 어두운 혈색 → 총상으로 쓰러진 모습 표현
        {
            goName = "NPC_Husband_Fallen", displayName = "부상자", isStudent = false,
            direction = "SOUTH", pos = new Vector3(ZX - 1.3f, ZY - 0.40f, 0), scale = 0.28f,
            tint = new Color(0.48f, 0.22f, 0.22f), // 거의 검붉음 — 혈흔, 의식 잃어가는 상태
            lines = System.Array.Empty<(string, string)>()
        }
    };

    // ════════════════════════════════════════════════════════════════
    //  STAGE 04 — 도청 진압 (전남도청, 5월 26일 밤)
    //
    //  상황: 외부 시민군이 모두 떠나고 소수만 남아 도청을 지키고 있다.
    //        이미 교전으로 부상자가 나왔다. 새벽에 최후 진압이 시작될 것이다.
    //  분위기: 결사·공포·체념. 어두운 밤. 이미 전투의 흔적.
    //
    //  [화살표] 도청 정문 방어선 — 최전방 2명 + 측면 1명 + ★부상 시민군 1명★
    // ════════════════════════════════════════════════════════════════
    static NPCDef[] BuildStage04() => new[]
    {
        // ── 대화 NPC ①: 아이를 위해 남은 아버지 ─────────────────
        // 시각: 묵직한 황갈 — 중년 가장의 결의, 피로가 쌓인 얼굴
        new NPCDef
        {
            goName = "NPC_Father", displayName = "시민군", isStudent = false,
            direction = "EAST", pos = new Vector3(-15f, -3.1f, 0), scale = 0.54f,
            tint = new Color(0.85f, 0.78f, 0.65f),
            lines = new[]
            {
                ("시민군", "도청이 무너지면 이 도시가 무너지는 거요. 우리가 마지막이에요."),
                ("시민군", "아이들한테 부끄럽지 않으려고요. 아버지가 거기 있었다고 말하고 싶어서.")
            }
        },
        // ── 대화 NPC ②: 처음 총을 잡은 겁먹은 학생 ─────────────
        // 시각: 창백한 연두 — 식은땀, 극도의 긴장
        new NPCDef
        {
            goName = "NPC_YoungMilitia", displayName = "시민군", isStudent = true,
            direction = "SOUTH", pos = new Vector3(-2f, -3.8f, 0), scale = 0.40f,
            tint = new Color(0.82f, 0.92f, 0.80f),
            lines = new[]
            {
                ("시민군", "저 소리 들려요? 저쪽에서 오고 있어요. 얼마 안 남은 것 같아요."),
                ("시민군", "무섭죠, 당연히. 근데 여기 있어야 해요. 왜인지는 설명 못 하겠는데.")
            }
        },

        // ── 화살표: 도청 정문 방어선 ─────────────────────────────
        new NPCDef // 최전방 — 가장 크고 단호, 정면 응시
        {
            goName = "NPC_Defender_Front", displayName = "시민군", isStudent = false,
            direction = "WEST", pos = new Vector3(ZX, ZY, 0), scale = 0.56f,
            tint = new Color(0.78f, 0.84f, 0.65f),
            lines = System.Array.Empty<(string, string)>()
        },
        new NPCDef // 좌측 — 총을 들고 측면 경계
        {
            goName = "NPC_Defender_Left", displayName = "시민군", isStudent = true,
            direction = "WEST", pos = new Vector3(ZX - 1.0f, ZY + 0.55f, 0), scale = 0.46f,
            tint = new Color(0.84f, 0.90f, 0.72f),
            lines = System.Array.Empty<(string, string)>()
        },
        new NPCDef // 우측 — 뒤를 돌아보며 상황 파악
        {
            goName = "NPC_Defender_Right", displayName = "시민군", isStudent = false,
            direction = "SOUTH", pos = new Vector3(ZX + 0.9f, ZY - 0.35f, 0), scale = 0.49f,
            tint = new Color(0.82f, 0.86f, 0.70f),
            lines = System.Array.Empty<(string, string)>()
        },
        new NPCDef // ★ 부상당한 시민군 ★ — 이미 총상을 입고 주저앉은 상태
                   // 작은 스케일 + 어두운 혈색 → 교전이 이미 시작됐음을 암시
        {
            goName = "NPC_Defender_Wounded", displayName = "시민군", isStudent = true,
            direction = "SOUTH", pos = new Vector3(ZX + 0.7f, ZY + 0.65f, 0), scale = 0.34f,
            tint = new Color(0.62f, 0.30f, 0.30f), // 어두운 빨강 — 총상, 출혈
            lines = System.Array.Empty<(string, string)>()
        }
    };

    // ════════════════════════════════════════════════════════════════
    //  STAGE 05 — 진압 이후 (전남도청 일대, 5월 27일 새벽)
    //
    //  상황: 진압이 끝났다. 사람들이 쓰러져 있다.
    //        박정국(38세)은 총상을 입고 피를 흘리며 쓰러져 있다.
    //        상관은 냉담하게 보고서 서명을 요구한다.
    //  분위기: 완전한 침묵. 죽음. 죄책감. ★가장 처참한 장면★
    //
    //  [대화 NPC] ★박정국 — 총상 부상자 (가장 어두운 혈색)★ + 생존자
    //  [화살표]   냉담한 상관 + 무표정한 부관
    // ════════════════════════════════════════════════════════════════
    static NPCDef[] BuildStage05() => new[]
    {
        // ── 대화 NPC ①: ★ 박정국 (38세, 총상으로 쓰러짐) ★ ─────
        // 시각: 극도로 어두운 검붉음 + 매우 작은 스케일
        //       → 출혈이 심해 의식이 희미한 상태, 땅에 쓰러진 모습
        new NPCDef
        {
            goName = "NPC_ParkJungKook", displayName = "부상자", isStudent = false,
            direction = "SOUTH", pos = new Vector3(-15f, -3.9f, 0), scale = 0.32f,
            tint = new Color(0.48f, 0.25f, 0.25f), // 검붉음 — 심각한 출혈, 빈사 상태
            lines = new[]
            {
                ("부상자", "...숨 쉬기가 힘들어요."),
                ("부상자", "박정국이에요. 서른여덟 살. 애가 둘이에요."),
                ("부상자", "아내한테... 잘못했다고 전해줄 수 있어요? 오늘 집에 못 간다고.")
            }
        },
        // ── 대화 NPC ②: 생존자 ───────────────────────────────────
        // 시각: 완전히 탈색된 회백색 → 충격과 공황, 넋이 나간 상태
        new NPCDef
        {
            goName = "NPC_Survivor", displayName = "생존자", isStudent = true,
            direction = "SOUTH", pos = new Vector3(0f, -3.3f, 0), scale = 0.44f,
            tint = new Color(0.72f, 0.72f, 0.78f), // 회백색 — 심리적 충격, 색이 빠진 얼굴
            lines = new[]
            {
                ("생존자", "같이 있던 사람이 여덟이었어요. 저만 나왔어요."),
                ("생존자", "귀에서 총소리가 안 멈춰요. 다 끝났다고 하는데.")
            }
        },

        // ── 화살표: 상관과 부관 ───────────────────────────────────
        new NPCDef // 상관 — 가장 크고 짙은 군복, 냉담하게 서류 들고 서있음
                   // 주변의 처참함과 대조되는 '질서'의 상징
        {
            goName = "NPC_Officer", displayName = "상관", isStudent = false,
            direction = "WEST", pos = new Vector3(ZX, ZY, 0), scale = 0.60f,
            tint = new Color(0.60f, 0.72f, 0.52f), // 짙은 올리브 — 냉담한 권위
            lines = new[]
            {
                ("상관", "다 끝났어. 서명 한 장이면 끝이야."),
                ("상관", "우린 명령을 따른 것뿐이야. 그게 군인이고.")
            }
        },
        new NPCDef // 부관 — 상관 뒤에서 무표정하게, 시선을 피함
        {
            goName = "NPC_Aide", displayName = "부관", isStudent = false,
            direction = "SOUTH", pos = new Vector3(ZX + 1.1f, ZY + 0.40f, 0), scale = 0.46f,
            tint = new Color(0.65f, 0.75f, 0.58f),
            lines = System.Array.Empty<(string, string)>()
        }
    };

    // ════════════════════════════════════════════════════════════════
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
        float s = (def.scale > 0f ? def.scale : 0.45f) * NPC_SCALE;
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
