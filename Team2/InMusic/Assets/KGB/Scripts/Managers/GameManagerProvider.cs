using UnityEngine;

public static class GameManagerProvider
{
    private static IGameManager _gameManager;

    public static IGameManager Instance
    {
        get
        {
            if (_gameManager != null) return _gameManager;

            // 우선 순위: 싱글 → 멀티
            if (GameManager.Instance != null)
                _gameManager = GameManager.Instance;
            else if (KGB_GameManager_Multi.Instance != null)
                _gameManager = KGB_GameManager_Multi.Instance;

            return _gameManager;
        }
    }
}
