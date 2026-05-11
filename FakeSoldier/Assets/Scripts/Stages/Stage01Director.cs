using System.Collections;
using UnityEngine;

public class Stage01Director : StageDirector
{
    protected override IEnumerator RunStage()
    {
        yield return ShowStageTitle("1980년 5월 18일\n전남도청 앞");

        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "상관",    text = "전원 집합! 오늘 새벽부로 광주 전역에 비상계엄이 선포됐다. 우리의 임무는 질서 유지다." },
            new DialogueLine { speaker = "상관",    text = "전남도청 앞에 학생들이 집결 중이다. 즉각 강제 해산시켜라. 저항하면 제압해도 좋다." },
            new DialogueLine { speaker = "나 (속)", text = "제압. 그 단어가 머릿속에서 맴돈다. 저 사람들을 향해 그걸 해야 한다는 뜻인가." },
            new DialogueLine { speaker = "나 (속)", text = "저기 모여 있는 건... 그냥 학생들이잖아. 나이도 나랑 비슷해 보인다." },
        });

        AllowMove();
        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "", text = "[ 빨간 지점으로 이동하시오. ]" },
        });

        yield return WaitForEventZone();
        LockMove();

        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "학생",    text = "우리는 민주주의를 외치는 게 죄입니까! 우리가 무슨 잘못을 했어요!" },
            new DialogueLine { speaker = "학생",    text = "총 집어넣어요! 우린 사람이에요! 당신도 사람이잖아요!" },
            new DialogueLine { speaker = "나 (속)", text = "저 눈빛은... 두려움이 아니라 확신이다. 물러서지 않겠다는." },
            new DialogueLine { speaker = "나 (속)", text = "우린 사람이에요. 그 말이 귀 안에서 지워지지 않는다." },
        });

        ShakeCamera(0.3f, 0.1f);

        yield return ShowChoice(new[]
        {
            new ChoiceData { text = "상관 명령대로 학생들을 진압봉으로 밀어붙인다.",      scoreChange = -2 },
            new ChoiceData { text = "멈춰 서서 움직이지 않는다. 아무것도 하지 않는다.", scoreChange =  0 },
            new ChoiceData { text = "병사들 맨 뒤로 물러나 진압에 가담하지 않는다.",    scoreChange =  1 },
        }, null);

        yield return PlayDialogue(new[]
        {
            new DialogueLine { speaker = "나 (속)", text = "방아쇠를 쥔 손이 떨린다. 이게 정말... 내가 해야 할 일인가." },
        });

        NextScene("Stage_02");
    }
}
