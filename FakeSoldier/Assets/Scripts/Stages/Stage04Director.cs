using System.Collections;
using UnityEngine;

public class Stage04Director : StageDirector
{
    protected override IEnumerator RunStage()
    {
        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "상관",   text = "전남도청을 점거한 시민군 진압 작전이다. 전원 발포 대기!" },
            new DialogueLine { speaker = "나 (속)", text = "총구 앞에 서 있는 건... 총을 든 시민들이다. 총 쏘는 법도 제대로 모를 것 같은 사람들이." },
            new DialogueLine { speaker = "나 (속)", text = "어제 검문소에서 봤던 그 여성의 얼굴이 저기 보이는 것 같다." },
        });

        AllowMove();
        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "",       text = "[ 발포 명령 위치로 이동하시오. ]" },
        });

        yield return WaitForEventZone();
        LockMove();

        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "상관",   text = "발포 준비! 명령이 떨어지는 순간 쏜다!" },
            new DialogueLine { speaker = "시민군", text = "쏘지 마시오! 우리는 시민이오!" },
            new DialogueLine { speaker = "나 (속)", text = "총이 무거워졌다. 방아쇠에 걸린 손가락이 굳어간다. 10초..." },
        });

        // 핵심 선택 - 10초 타이머
        yield return ShowChoice(new[]
        {
            new ChoiceData { text = "명령대로 시민들을 향해 발포한다.",  scoreChange = -2, isFiringRefusal = false },
            new ChoiceData { text = "총구를 하늘로 돌려 허공에 쏜다.",   scoreChange =  2, isFiringRefusal = false },
            new ChoiceData { text = "총을 땅에 내려놓고 명령을 거부한다.", scoreChange =  3, isFiringRefusal = true  },
        }, null, timer: 10f);

        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "나 (속)", text = "그 0.1초가... 내 나머지 인생을 결정했다." },
        });

        NextScene("Stage_05");
    }
}
