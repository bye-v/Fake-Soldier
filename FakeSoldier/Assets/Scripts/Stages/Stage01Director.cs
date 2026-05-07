using System.Collections;
using UnityEngine;

public class Stage01Director : StageDirector
{
    protected override IEnumerator RunStage()
    {
        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "상관",    text = "전원 집합! 오늘부로 광주는 계엄령 하에 놓인다. 우리의 임무는 질서 유지다." },
            new DialogueLine { speaker = "상관",    text = "저 학생 집회를 강제 해산시킨다. 명령에 따라 움직여라." },
            new DialogueLine { speaker = "나 (속)",  text = "저기 모여 있는 건... 그냥 학생들이잖아." },
        });

        AllowMove();
        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "",        text = "[ 빨간 지점으로 이동하시오. ]" },
        });

        yield return WaitForEventZone();
        LockMove();

        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "학생",    text = "우리가 무슨 죄가 있어요! 물러나세요!" },
            new DialogueLine { speaker = "나 (속)",  text = "저 눈빛은... 두려움이 아니라 분노다." },
        });

        yield return ShowChoice(new[]
        {
            new ChoiceData { text = "상관 명령대로 학생들을 밀어붙인다.",     scoreChange = -2 },
            new ChoiceData { text = "멈춰 서서 아무것도 하지 않는다.",        scoreChange = 0  },
            new ChoiceData { text = "다른 병사들 뒤에 숨어 움직이지 않는다.", scoreChange = 1  },
        }, null);

        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "나 (속)", text = "이게 정말 내가 해야 할 일인가." },
        });

        NextScene("Stage_02");
    }
}
