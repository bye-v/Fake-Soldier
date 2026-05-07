using System.Collections;
using UnityEngine;

public class Stage03Director : StageDirector
{
    protected override IEnumerator RunStage()
    {
        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "상관",    text = "검문소를 통제한다. 통행증 없는 자는 전부 연행해라." },
            new DialogueLine { speaker = "나 (속)",  text = "길을 막으면 병원도 못 간다. 부상자들이 죽을 수도 있어." },
        });

        AllowMove();
        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "",        text = "[ 검문소로 이동하시오. ]" },
        });

        yield return WaitForEventZone();
        LockMove();

        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "여성",    text = "제발요, 남편이 총에 맞았어요! 병원에 데려가야 해요!" },
            new DialogueLine { speaker = "상관",    text = "통행증 없으면 안 돼. 연행해." },
            new DialogueLine { speaker = "나 (속)",  text = "피가 흐르고 있다. 이 남자는 죽어가고 있어." },
        });

        yield return ShowChoice(new[]
        {
            new ChoiceData { text = "명령대로 연행한다.",                             scoreChange = -2 },
            new ChoiceData { text = "못 본 척하고 자리를 피한다.",                   scoreChange = 0  },
            new ChoiceData { text = "\"통행증 확인 중\"이라며 시간을 끌어준다.",     scoreChange = 1  },
            new ChoiceData { text = "몰래 통과시켜준다.",                             scoreChange = 2  },
        }, null);

        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "나 (속)", text = "다음은... 도청이다. 상황이 다르다고 들었다." },
        });

        NextScene("Stage_04");
    }
}
