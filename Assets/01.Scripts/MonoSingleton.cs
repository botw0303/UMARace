using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;
    private static bool IsQuitting = false;

    public static T Instance
    {
        get
        {
            // 비활성화 됐다면 기존꺼 내비두고 새로 만든다.
            if (IsQuitting)
            {
                _instance = null;
            }

            // instance가 NULL일때 새로 생성한다.
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<T>();

                if (_instance == null)
                {
                    Debug.LogError($"{typeof(T).Name} is not exits");
                }
                else
                {
                    IsQuitting = false; //재사용 용도면.
                }

            }
            return _instance;

        }
    }

    private void OnDisable()
    {
        // 비활성화 된다면 null로 변경
        IsQuitting = true;
        _instance = null;
    }
}