using System.Collections;
using UnityEngine;

public class Stage03Director : StageDirector
{
    protected override IEnumerator RunStage()
    {
        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "상관",   text = "교차로 전부 봉쇄한다. 통행증 없는 자는 예외 없이 연행해!" },
            new DialogueLine { speaker = "나 (속)", text = "길을 막으면 병원도 못 간다. 지금 이 순간에도 총에 맞은 사람들이 피를 흘리고 있을 텐데." },
        });

        AllowMove();
        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "",       text = "[ 검문소로 이동하시오. ]" },
        });

        yield return WaitForEventZone();
        LockMove();

        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "여성",   text = "제발요! 남편이 총에 맞았어요! 지금 당장 병원에 가야 해요!" },
            new DialogueLine { speaker = "상관",   text = "통행증 없으면 안 돼. 규정대로 연행해." },
            new DialogueLine { speaker = "여성",   text = "죽어요! 지금 죽어가고 있다고요! 제발...!" },
            new DialogueLine { speaker = "나 (속)", text = "남자의 셔츠가 붉게 물들어 있다. 의식이 없다. 이 사람은 지금 죽어가고 있어." },
        });

        yield return ShowChoice(new[]
        {
            new ChoiceData { text = "명령대로 두 사람을 연행한다.",                       scoreChange = -2 },
            new ChoiceData { text = "못 본 척하고 자리를 피한다.",                       scoreChange =  0 },
            new ChoiceData { text = "\"통행증 확인 중입니다.\" 라며 시간을 끌어준다.",   scoreChange =  1 },
            new ChoiceData { text = "아무도 없을 때 몰래 통과시켜준다.",                 scoreChange =  2 },
        }, null);

        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "나 (속)", text = "이 선택이 한 사람의 목숨을 결정한다. 명령인가, 사람인가." },
            new DialogueLine { speaker = "나 (속)", text = "내일은... 도청이다. 시민군이 도청을 점거했다고 들었다." },
        });

        NextScene("Stage_04");
    }
}
