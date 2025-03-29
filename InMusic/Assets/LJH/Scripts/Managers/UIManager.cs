using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager
{
    private GameObject _popupRoot;

    public UIManager()
    {
        _popupRoot = GameObject.Find("@Root");
        if (_popupRoot == null)
        {
            _popupRoot = new GameObject("@Root");
            Debug.Log("@Root created");
        }

        // Canvas가 없으면 추가
        Canvas canvas = _popupRoot.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = _popupRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            Debug.Log("Canvas added to @Root");
        }

        // GraphicRaycaster 추가
        if (_popupRoot.GetComponent<GraphicRaycaster>() == null)
        {
            _popupRoot.AddComponent<GraphicRaycaster>();
            Debug.Log("GraphicRaycaster added to @Root");
        }

        // RectTransform 설정
        RectTransform rectTransform = _popupRoot.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = _popupRoot.AddComponent<RectTransform>();
        }
        rectTransform.anchorMin = Vector2.zero;  // 왼쪽 아래 (0,0)
        rectTransform.anchorMax = Vector2.one;   // 오른쪽 위 (1,1)
        rectTransform.offsetMin = Vector2.zero;  // 부모 기준 최소 크기 (0,0)
        rectTransform.offsetMax = Vector2.zero;  // 부모 기준 최대 크기 (0,0)
        Debug.Log("RectTransform adjusted for full screen");

        // Canvas 활성화 확인
        if (!canvas.enabled)
        {
            canvas.enabled = true;
            Debug.Log("Canvas enabled");
        }
    }

    public void ShowPopup(GameObject popupPrefab)
    {
        if (popupPrefab == null)
        {
            Debug.LogError("Popup Prefab is null");
            return;
        }

        GameObject popup = GameObject.Instantiate(popupPrefab, _popupRoot.transform);
        popup.SetActive(true);

        // 부모에 Image가 있다면 활성화
        Image background = _popupRoot.GetComponent<Image>();
        if (background != null)
        {
            background.enabled = true;
        }
    }

    public static void ToggleComponentInput<T>(GameObject childObject, bool enable) where T : MonoBehaviour
    {
        GameObject rootParent = childObject.transform.parent?.parent?.gameObject;

        T component = rootParent != null ? Managers.FindChild<T>(rootParent, recursive: true) : null;

        if (component == null)
        {
            Debug.LogWarning($"[UIManager] 부모에서 {typeof(T).Name} 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        var methodInfo = typeof(T).GetMethod("HandleKeyPress", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);


        Action<KeyCode> methodDelegate = (Action<KeyCode>)Delegate.CreateDelegate(typeof(Action<KeyCode>), component, methodInfo);

        if (enable)
        {
            Managers.Input.OnKeyPressed += methodDelegate;
            Debug.Log($"[UIManager] {typeof(T).Name} 키 입력 활성화");
        }
        else
        {
            Managers.Input.OnKeyPressed -= methodDelegate;
            Debug.Log($"[UIManager] {typeof(T).Name} 키 입력 비활성화");
        }
    }


}
