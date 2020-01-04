using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatRoom : LinstenerCtrl {
	    public Text MessageText;
    public ScrollRect msgScroll;
	private string message="";
	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Start()
	{
            ChatClient.Instance.AddListener(this);
		
	}
	 void OnDestroy() {
		 ChatClient.Instance.RemoveListener(this);
	}
	public override void GetMsg(string msg)
	{
	 	 message= msg;
		// (getMsg)=>{ MessageText.text +="\n"+ getMsg;
        //      msgScroll.verticalNormalizedPosition =0f;
	    //     getMsg = "";};
		 
	}
	void Update () {
        //只有在主线程才能更新UI
	  if (message!="" && message!=null)
	   {
	        MessageText.text +="\n"+ message;
             msgScroll.verticalNormalizedPosition =0f;
	        message = "";
	    }
	}
}
