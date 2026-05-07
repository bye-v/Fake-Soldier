using System.Collections;
using UnityEngine;

public class Stage05Director : StageDirector
{
    protected override IEnumerator RunStage()
    {
        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "나 (속)",  text = "전투가 끝났다. 나는 살아있다." },
            new DialogueLine { speaker = "나 (속)",  text = "하지만 내 손에 무엇이 묻었는지... 알고 있다." },
        });

        AllowMove();
        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "",        text = "[ 상관에게 이동하시오. ]" },
        });

        yield return WaitForEventZone();
        LockMove();

        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "상관",    text = "수고했어. 진압 완료 보고서에 서명해." },
            new DialogueLine { speaker = "나 (속)",  text = "이 서명이 모든 것을 정당화하게 된다." },
            new DialogueLine { speaker = "부상 시민", text = "... 당신은... 괜찮은 사람 같았는데." },
        });

        yield return ShowChoice(new[]
        {
            new ChoiceData { text = "보고서에 서명하고 군인으로서의 임무를 완수한다.", scoreChange = -2 },
            new ChoiceData { text = "서명을 망설이다 결국 한다.",                     scoreChange = 0  },
            new ChoiceData { text = "서명을 거부하고 부대를 이탈한다.",               scoreChange = 2  },
        }, null);

        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "나 (속)", text = "이제... 어디로 가야 하는가." },
        });

        GoEnding();
    }
}
