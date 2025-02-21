using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SongList {
    public class Singleton<T> : MonoBehaviour where T : Component {
        private static T _instance;

        public static T Instance {
            get {
                if (_instance == null) {
                    _instance = CreateInstance();
                }
                return _instance;
            }
        }

        private static T CreateInstance() {
            T instance = FindFirstObjectByType<T>();
            if (instance == null) {
                GameObject obj = new GameObject();
                obj.name = typeof(T).Name;
                instance = obj.AddComponent<T>();
            }
            return instance;
        }

        protected virtual void Awake() {
            if (_instance == null) {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            } else if (_instance != this) {
                Destroy(gameObject);
            }
        }
    }
}
