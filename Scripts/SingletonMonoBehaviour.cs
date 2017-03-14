using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static bool isQuit = false;

    public static T Instance {
        get{
            if (isQuit) {
                return null;
            }

            if (instance == null) {
                instance = FindObjectOfType<T> () as T;
                if (instance == null) {
                    var obj = new GameObject (typeof(T).ToString() + "(Singleton)");
                    instance = obj.AddComponent<T> ();

                    DontDestroyOnLoad (obj);
                }
            }

            return instance;
        }   
    }

    protected SingletonMonoBehaviour()
    {
    }

    void OnApplicationQuit()
    {
        isQuit = true;
    }
}
