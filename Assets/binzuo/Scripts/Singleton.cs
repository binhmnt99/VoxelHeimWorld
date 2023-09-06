using UnityEngine;

namespace binzuo
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        GameObject gameObject = new GameObject();
                        gameObject.name = typeof(T).Name;
                        gameObject.hideFlags = HideFlags.HideAndDontSave;
                        instance = gameObject.AddComponent<T>();
                    }
                }
                return instance;
            }
        }
        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
            }
            else
            {
                Destroy(this);
            }
        }   
    }
}
