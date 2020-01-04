using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
//使用泛型
public class MonoSingleton<T> : MonoBehaviour
where T : MonoBehaviour //对T使用where进行约束，泛型T必须继承自MonoBehaviour
{
    private static T m_instance;
 
    //封装字段的快捷键 ctrl + r +e
    public static T Instance
    {
        get
        {
            return m_instance;
        }
    }
 
    //Awake必须为protect和virtual类型，可以被子类继承也可被子类重写
    protected virtual void Awake()
    {
        m_instance = this as T;
    }
}