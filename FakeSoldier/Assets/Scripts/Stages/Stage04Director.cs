using System.Collections;
using UnityEngine;

public class Stage04Director : StageDirector
{
    protected override IEnumerator RunStage()
    {
        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "상관",    text = "도청을 점거 중인 시민군을 진압한다. 전원 대기." },
            new DialogueLine { speaker = "나 (속)",  text = "총구 앞에 서 있는 건... 총을 든 시민들이다." },
            new DialogueLine { speaker = "나 (속)",  text = "어젯밤에 만났던 그 청년도 보인다." },
        });

        AllowMove();
        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "",        text = "[ 발포 명령 위치로 이동하시오. ]" },
        });

        yield return WaitForEventZone();
        LockMove();

        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "상관",    text = "발포 준비! 명령이 떨어지면 쏜다!" },
            new DialogueLine { speaker = "나 (속)",  text = "총을 들었다. 방아쇠에 손이 걸렸다. 10초..." },
        });

        // 핵심 선택 - 10초 타이머
        yield return ShowChoice(new[]
        {
            new ChoiceData { text = "명령대로 발포한다.",          scoreChange = -2, isFiringRefusal = false },
            new ChoiceData { text = "총구를 하늘로 돌려 쏜다.",    scoreChange = 2,  isFiringRefusal = false },
            new ChoiceData { text = "총을 내려놓고 거부한다.",     scoreChange = 3,  isFiringRefusal = true  },
        }, null, timer: 10f);

        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "나 (속)", text = "그 순간이 모든 것을 결정했다." },
        });

        NextScene("Stage_05");
    }
}
