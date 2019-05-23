using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonoSingleTon<T> : MonoBehaviour where T : Component
{
    private static T _instance;
    public static T Ins
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(T)) as T;
                if (_instance == null)
                {
                    GameObject obj = new GameObject();

                    //隐藏实例化的new game object 
                    obj.hideFlags = HideFlags.HideAndDontSave;

                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }
}


