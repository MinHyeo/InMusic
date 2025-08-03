using UnityEngine;

public interface IGameManager
{
    // 상태
    bool IsGameActive { get; }

    // 게임 흐름 제어
    void InitializeGame();
    void StartGame();
    void PauseGame();
    void ResumeGame();

    // 점수 관련
    void AddScore(string judgement);
    void StartMusic();

    // 판정 점수 관련
    int TotalNotes { get; }
    float TotalScore { get; }
    float Accuracy { get; }
    int GreatCount { get; }
    int GoodCount { get; }
    int BadCount { get; }
    int MissCount { get; }
    int MaxCombo { get; }
    int Combo { get; }

    float CurHP { get; }
    float MaxHP { get; }
}
