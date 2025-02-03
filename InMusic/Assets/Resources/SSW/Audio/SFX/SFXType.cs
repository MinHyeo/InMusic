using UnityEngine;

/// <summary>
/// 게임 내에서 사용되는 '효과음'들을 구분하기 위한 열거형
/// 여기서 선언한 값들과 실제 AudioClip을 매핑해놓고,
/// 필요할 때 열거형만 넘겨주면 적절한 소리가 재생됩니다.
/// </summary>
public enum SFXType
{
    // 예시: UI/메뉴 관련
    MenuSelect,
    MenuBack,
    OptionToggle,

    // 곡 목록 화면
    Scroll,
    SelectSong,

    // 게임 플레이(리듬게임, 액션 등)
    KeyHit_1,
    KeyHit_2,
    KeyHit_3,
    KeyHit_4,
    JudgeMiss,
    JudgeGood,
    JudgePerfect,

    // 게임 플레이 시작
    PlayStart,

    // 상황별로 계속 추가 가능
}

