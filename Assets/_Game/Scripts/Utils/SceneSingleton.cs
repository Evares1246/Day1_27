using UnityEngine;

namespace GameDesign.Utils
{
    /// <summary>
    /// 场景单例：随场景销毁，不持久化 (不会调用 DontDestroyOnLoad)。
    /// 适用于引用了场景内特定对象（如 UI、作为、变换组件等）的管理器。
    /// </summary>
    public class SceneSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<T>();
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                Debug.LogWarning($"[SceneSingleton] Duplicate instance of {typeof(T).Name} detected in scene. Destroying duplicate.");
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
