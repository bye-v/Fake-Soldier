using System.Collections;
using UnityEngine;

public class Stage02Director : StageDirector
{
    protected override IEnumerator RunStage()
    {
        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "상관",    text = "주민들이 숨어 있을 수 있다. 이 골목 전부 수색한다." },
            new DialogueLine { speaker = "나 (속)",  text = "민가다. 평범한 사람들이 사는 곳이다." },
        });

        AllowMove();
        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "",        text = "[ 골목 안으로 이동하시오. ]" },
        });

        yield return WaitForEventZone();
        LockMove();

        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "노인",    text = "제발... 우리는 아무것도 모릅니다. 손녀만 봐주세요." },
            new DialogueLine { speaker = "상관",    text = "비켜! 전부 수색한다고 했잖아." },
            new DialogueLine { speaker = "나 (속)",  text = "노인과 아이다. 이 사람들이 위협이 된다고?" },
        });

        yield return ShowChoice(new[]
        {
            new ChoiceData { text = "상관을 따라 집 안까지 수색한다.",                scoreChange = -2 },
            new ChoiceData { text = "입구에서 멈추고 안을 들여다보지 않는다.",        scoreChange = 1  },
            new ChoiceData { text = "\"여기는 이상 없습니다\" 라고 허위 보고한다.",   scoreChange = 2  },
        }, null);

        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "나 (속)", text = "가족을 지키려는 마음은 누구나 같다." },
        });

        NextScene("Stage_03");
    }
}
