using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCtrl : MonoBehaviour
{

    public Transform numberGameChioceCtrl;
    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < numberGameChioceCtrl.childCount; i++)
        {
             Transform objTrans = numberGameChioceCtrl.GetChild(i);
           objTrans.GetComponent<Button>().onClick.AddListener(() =>
        {
            int index=objTrans.GetSiblingIndex();
            Object numberGame = Resources.Load("prefab/numberGamePanel", typeof(GameObject));
            //用加载得到的资源对象，实例化游戏对象，实现游戏物体的动态加载
            GameObject numberGameObj = Instantiate(numberGame) as GameObject;
            numberGameObj.transform.SetParent(transform.parent, false);
            numberGameObj.GetComponent<numberCtrl>().SetLineCount(index+3);
        });
        }
    }

    
}
