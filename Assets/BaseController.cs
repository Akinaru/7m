using UnityEngine;

public abstract class BaseController<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>();
                if (_instance == null)
                {
                    Debug.LogError($"Aucune instance de {typeof(T).Name} trouvée dans la scène !");
                }
            }
            return _instance;
        }
    }
}