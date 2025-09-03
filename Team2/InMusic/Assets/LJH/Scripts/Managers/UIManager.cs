using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameObject _popupRoot;
    private Stack<GameObject> _popupStack = new Stack<GameObject>();

    private void Start()
    {
        SetupPopupRoot();
    }

    private void SetupPopupRoot()
    {
        if (_popupRoot != null && !_popupRoot.Equals(null))
            return;

        if (!gameObject.scene.IsValid() || !gameObject.scene.isLoaded)
        {
            Debug.LogWarning("[UIManager] Scene not fully loaded. Skipping SetupPopupRoot.");
            return;
        }

        _popupRoot = GameObject.Find("@Root");

        if (_popupRoot == null)
        {
            _popupRoot = new GameObject("@Root");

            GameObject uiMainMenu = GameObject.Find("UI_MainMenu");
            if (uiMainMenu != null)
            {
                _popupRoot.transform.SetParent(uiMainMenu.transform, false);
                Debug.Log("[UIManager] @Root parent set to UI_MainMenu");
            }
            else
            {
                _popupRoot.transform.SetParent(Managers.Instance.transform, false);
                Debug.LogWarning("[UIManager] UI_MainMenu not found. @Root assigned to Managers.");
            }

            // 선택: 씬 전환 시 유지하고 싶다면
            // DontDestroyOnLoad(_popupRoot);
        }

        Canvas canvas = Managers.Instance.GetOrAddComponent<Canvas>(_popupRoot);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        canvas.enabled = true;

        Managers.Instance.GetOrAddComponent<GraphicRaycaster>(_popupRoot);

        RectTransform rectTransform = Managers.Instance.GetOrAddComponent<RectTransform>(_popupRoot);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    /// <summary>
    /// 팝업 프리팹을 띄움
    /// </summary>
    public void ShowPopup(GameObject popupPrefab)
    {
        if (popupPrefab == null || popupPrefab.Equals(null))
        {
            Debug.LogError("[UIManager] popupPrefab is null or destroyed!");
            return;
        }

        if (_popupRoot == null || _popupRoot.Equals(null))
        {
            SetupPopupRoot();

            if (_popupRoot == null || _popupRoot.Equals(null))
            {
                Debug.LogError("[UIManager] Failed to reinitialize _popupRoot");
                return;
            }
        }

        GameObject popup = Instantiate(popupPrefab, _popupRoot.transform);
        popup.SetActive(true);

        _popupStack.Push(popup);

        Image background = _popupRoot.GetComponent<Image>();
        if (background != null)
            background.enabled = true;
    }

    /// <summary>
    /// 가장 위 팝업을 닫음
    /// </summary>
    public void CloseCurrentPopup()
    {
        if (_popupStack.Count == 0)
            return;

        GameObject topPopup = _popupStack.Pop();
        if (topPopup != null && !topPopup.Equals(null))
            Destroy(topPopup);

        if (_popupStack.Count == 0)
        {
            Image background = _popupRoot?.GetComponent<Image>();
            if (background != null)
                background.enabled = false;
        }
    }

    /// <summary>
    /// 팝업이 떠 있는지 확인
    /// </summary>
    public bool HasAnyPopupOpen()
    {
        return _popupStack.Count > 0;
    }

    /// <summary>
    /// 현재 가장 위 팝업 반환
    /// </summary>
    public GameObject CurrentPopup => _popupStack.Count > 0 ? _popupStack.Peek() : null;

    /// <summary>
    /// 특정 컴포넌트 타입의 HandleKeyPress(KeyCode)를 OnKeyPressed에서 토글
    /// </summary>
    public void ToggleComponentInput<T>(GameObject childObject, bool enable) where T : MonoBehaviour
    {
        GameObject rootParent = childObject.transform.parent?.parent?.gameObject;
        T component = rootParent != null ? Managers.Instance.FindChild<T>(rootParent, recursive: true) : null;

        if (component == null)
        {
            Debug.LogWarning($"[UIManager] 부모에서 {typeof(T).Name} 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        var methodInfo = typeof(T).GetMethod("HandleKeyPress", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
        if (methodInfo == null)
        {
            Debug.LogWarning($"[UIManager] {typeof(T).Name}에 HandleKeyPress(KeyCode)가 없습니다.");
            return;
        }

        Action<KeyCode> methodDelegate = (Action<KeyCode>)Delegate.CreateDelegate(typeof(Action<KeyCode>), component, methodInfo);

        if (enable)
        {
            Managers.Instance.Input.OnKeyPressed += methodDelegate;
            Debug.Log($"[UIManager] {typeof(T).Name} 키 입력 활성화");
        }
        else
        {
            Managers.Instance.Input.OnKeyPressed -= methodDelegate;
            Debug.Log($"[UIManager] {typeof(T).Name} 키 입력 비활성화");
        }
    }
}
