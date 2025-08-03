using UnityEngine;

public interface IGameManager
{
    // ����
    bool IsGameActive { get; }

    // ���� �帧 ����
    void InitializeGame();
    void StartGame();
    void PauseGame();
    void ResumeGame();

    // ���� ����
    void AddScore(string judgement);
    void StartMusic();

    // ���� ���� ����
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
