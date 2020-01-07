using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FiveChessChatRoom : LinstenerCtrl {
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
        GameMessage gameMsg = JsonUtility.FromJson<GameMessage>(msg);
        if (gameMsg.msgType == MsgType.FiveChessGameMsg)
        {
            FiveChessGameData getData = JsonUtility.FromJson<FiveChessGameData>(gameMsg.msgJson);
		     message=string.Format("{0}:走位置：{1},是不是白棋：{2}",getData.name,getData.chessPos,getData.isWhite);
        }
		 
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
