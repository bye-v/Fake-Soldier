using System.Collections;
using UnityEngine;

public class Stage05Director : StageDirector
{
    protected override IEnumerator RunStage()
    {
        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "나 (속)", text = "총성이 멎었다. 나는 살아있다." },
            new DialogueLine { speaker = "나 (속)", text = "하지만 발이 움직이지 않는다. 내 손에... 무엇이 묻었는지." },
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
            new DialogueLine { speaker = "상관",    text = "수고했다. 도청 진압 완료 보고서에 서명하면 끝이야." },
            new DialogueLine { speaker = "나 (속)", text = "이 종이 한 장이 오늘 있었던 모든 일을 '적법'으로 만든다." },
            new DialogueLine { speaker = "부상자",  text = "...당신은... 그래도... 사람이었으면 좋겠소." },
            new DialogueLine { speaker = "나 (속)", text = "부상자의 눈이 나를 본다. 총탄 구멍이 난 셔츠 사이로 피가 배어 나오고 있다." },
        });

        yield return ShowChoice(new[]
        {
            new ChoiceData { text = "보고서에 서명하고 군인으로서 임무를 완수한다.", scoreChange = -2 },
            new ChoiceData { text = "오랫동안 망설이다가... 결국 서명한다.",         scoreChange =  0 },
            new ChoiceData { text = "펜을 내려놓고 서명을 거부한 뒤 부대를 이탈한다.", scoreChange =  3 },
        }, null);

        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "나 (속)", text = "1980년 5월 광주. 이제... 나는 어디로 가야 하는가." },
        });

        GoEnding();
    }
}
