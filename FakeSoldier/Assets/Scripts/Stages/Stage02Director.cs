using System.Collections;
using UnityEngine;

public class Stage02Director : StageDirector
{
    protected override IEnumerator RunStage()
    {
        yield return ShowStageTitle("1980년 5월 19일\n광주 시내 골목");

        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "상관",    text = "주민 가운데 불순분자가 섞여 있을 수 있다. 이 골목 전부 샅샅이 수색한다!" },
            new DialogueLine { speaker = "나 (속)", text = "민가다. 좁은 골목에 빨래가 널려 있다. 아까 지나칠 때 아이 웃음소리도 들렸는데..." },
            new DialogueLine { speaker = "나 (속)", text = "불순분자. 저 단어 하나로 이 골목 전체를 뒤집어도 된다는 건가." },
        });

        AllowMove();
        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "", text = "[ 골목 안으로 이동하시오. ]" },
        });

        yield return WaitForEventZone();
        LockMove();

        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "노인",    text = "제발... 우리는 아무것도 모릅니다. 이 아이만, 손녀만..." },
        });

        ShakeCamera(0.4f, 0.12f);

        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "상관",    text = "비켜! 숨긴 게 없으면 왜 떠느냐! 전부 끌어내!" },
            new DialogueLine { speaker = "손녀",    text = "(아이 울음소리)" },
            new DialogueLine { speaker = "나 (속)", text = "노인의 손이 떨리고 있다. 아이는 무서워서 울고 있다. 이 사람들이... 위협이라고?" },
            new DialogueLine { speaker = "나 (속)", text = "아이가 나를 본다. 판단도 없이, 그냥 무서운 눈으로. 나는 지금 이 아이에게 무엇으로 보이고 있는가." },
        });

        yield return ShowChoice(new[]
        {
            new ChoiceData { text = "상관을 따라 집 안까지 직접 수색한다.",              scoreChange = -2 },
            new ChoiceData { text = "입구에서 멈추고 안을 들여다보지 않는다.",           scoreChange =  1 },
            new ChoiceData { text = "\"여기는 이상 없습니다.\" 허위 보고하고 돌아선다.", scoreChange =  2 },
        }, null);

        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "나 (속)", text = "어떤 명령도 저 아이의 울음소리를 정당화할 수는 없다." },
        });

        NextScene("Stage_03");
    }
}
