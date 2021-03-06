﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 五子棋
/// </summary>
public class FiveGameCtrl : LinstenerCtrl
{

    public RectTransform imgRect;
    public GameObject itemObj;
    public Transform ItemParent;
     RectTransform canvasRect;
    public GameObject winPanel;
    public GameObject winObj;
    public GameObject loseObj;
    public GameObject waittingPanel;
    public Button backBtn;
    public Button overBtn ;
    string myName;
    bool isPlaying=false;
    bool myIsWhite=true;
    bool isRunQuit;
    bool isWin;
	FiveChessGameData getData;
	int startCount;
	int roomNumber;
	Dictionary<Vector2, int> chessPosDic = new Dictionary<Vector2, int>();
	void Start()
	{
        isPlaying=false;
        backBtn.onClick.AddListener(() => { OnQuit(); Destroy(gameObject); });
        overBtn.onClick.AddListener(() => { OnGameOver(); Destroy(gameObject); });
        canvasRect =transform.parent.GetComponent<RectTransform>();
        myName=GetRandomString(5,true,false,false,false,"");
        GameMessage msg = new GameMessage() {  msgType=MsgType.FiveChessGameStart,msgJson= "content" };
        ChatClient.Instance.SendMessage(JsonUtility.ToJson(msg));
		ChatClient.Instance.AddListener(this);

	}
	void OnQuit()
	{
         GameMessage msg = new GameMessage()
        {
            msgType = MsgType.FiveChessGameQuit,
            msgJson = myIsWhite.ToString(),
            roomId=roomNumber
        };
        ChatClient.Instance.SendMessage(JsonUtility.ToJson(msg));
		ChatClient.Instance.RemoveListener(this);
	}
    void OnGameOver()
	{
         GameMessage msg = new GameMessage()
        {
            msgType = MsgType.FiveChessGameEnd,
            msgJson = "GameOver",
            roomId=roomNumber
        };
        ChatClient.Instance.SendMessage(JsonUtility.ToJson(msg));
		ChatClient.Instance.RemoveListener(this);
	}
	void Update()
    {
        if (Input.GetMouseButtonDown(0) && isPlaying)
        {
            Vector2 outVec;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, null, out outVec))
            {
                float x = (float)Math.Round(outVec.x / 50, MidpointRounding.AwayFromZero) * 50;
                float y = (float)Math.Round(outVec.y / 50, MidpointRounding.AwayFromZero) * 50;
                int xIndex = (int)(x / 50);
                int yIndex = (int)(y / 50);
                if ((-350 <= x && x <= 350) && (-350 <= y && y <= 350))
                {
                     Vector2 itemVector = new Vector2(xIndex, yIndex);
                     if (!chessPosDic.ContainsKey(itemVector))
                        SendMsg(itemVector);
                }
            }
        }
		if (getData!=null)
		{
			if (getData.isWhite)
			{
				Debug.Log("白棋刚走完,允许黑棋走");
				isPlaying = !myIsWhite;
			}
            else
			{
				Debug.Log("黑棋刚走完,允许白棋走");
				isPlaying = myIsWhite;
			}
			SetChessPos(getData.chessPos,getData.isWhite);
			getData = null;
		}
		if (startCount==2)
		{
			startCount = 0;
			Debug.Log("game Begin");
            isPlaying=!myIsWhite;
			waittingPanel.SetActive(false);
		}
        if (isRunQuit)
        {
            IsWin(isWin);
            isRunQuit=false;
        }
    }
    void SendMsg(Vector2 pos)
    {
        FiveChessGameData sendData = new FiveChessGameData() { chessPos = pos, name = myName, isWhite = myIsWhite };
        string msgString = JsonUtility.ToJson(sendData);
        GameMessage msg = new GameMessage()
        {
            msgType = MsgType.FiveChessGameMsg,
            msgJson = msgString,
            roomId=roomNumber
        };
        ChatClient.Instance.SendMessage(JsonUtility.ToJson(msg));
    }
    public override void GetMsg(string msg)

    {
       GameMessage gameMsg=  JsonUtility.FromJson<GameMessage>(msg);
        if (gameMsg.msgType == MsgType.FiveChessGameStart)
        {
				startCount =(int.Parse(gameMsg.msgJson.Substring(0,1)));
                roomNumber=(int.Parse(gameMsg.msgJson.Substring(1)));
                if (startCount==1)
                {
                    myIsWhite=false;
                }
        }
        if (gameMsg.msgType==MsgType.FiveChessGameMsg)
        {
            getData = JsonUtility.FromJson<FiveChessGameData>(gameMsg.msgJson);
        }
        if (gameMsg.msgType==MsgType.FiveChessGameQuit)
        {
            isRunQuit=true;
            // isWhiteWin=gameMsg.msgJson
            if (gameMsg.msgJson==myIsWhite.ToString())
            {
                Debug.Log("输了");
                isWin=false;
            }
            else
            {
                 isWin=true;
                Debug.Log("赢了");
            }
        }
	}
    /// <summary>
    /// 设置棋子位置
    /// </summary>
    /// <param name="chessPos"></param>
    void SetChessPos(Vector2 chessPos, bool isWhite)
    {
        if (!chessPosDic.ContainsKey(chessPos))
        {
            Debug.Log("空位");
            GameObject item = Instantiate(itemObj) as GameObject;
            item.transform.SetParent(ItemParent, false);
            if (isWhite==false)
            {
                item.GetComponent<Image>().color = Color.black;
                chessPosDic.Add(chessPos, 1);
            }
            else
            {
                chessPosDic.Add(chessPos, 2);
            }
            bool isWin = CheckWin(chessPos);
            if (isWin)
            {
                IsWin(isWhite==myIsWhite);
                // PlayOver(isWhite);
            }
            item.GetComponent<RectTransform>().anchoredPosition = chessPos * 50;
            item.SetActive(true);
        }
        else
        {
            Debug.Log("非空位");
        }
    }
    void IsWin( bool isWin)
    {
		isPlaying = false;
		winPanel.SetActive(true);
         winObj.SetActive(isWin);
           loseObj.SetActive(!isWin);
    }
    /// <summary>
    /// 游戏结束
    /// </summary>
    void PlayOver(bool isWhite)
    {
        if (isWhite)
         {
            IsWin(myIsWhite);
         } 
        else
        {
            IsWin(!myIsWhite);
        }
    }
     bool CheckWin(Vector2 indexVector)
    {
        int max = 0;
        int count;
        bool flag;
        int tempXIndex =(int) indexVector.x;
        int tempYIndex = (int)indexVector.y;
        // 三维数组记录横向，纵向，左斜，右斜的移动
        int[,,] dir = new int[4,2,2] {
                // 横向
                { { -1, 0 }, { 1, 0 } },
                // 竖着
                { { 0, -1 }, { 0, 1 } },
                // 左斜
                { { -1, -1 }, { 1, 1 } },
                // 右斜
                { { 1, -1 }, { -1, 1 } } };

        for (int i = 0; i < 4; i++)
        {
            count = 1;
            //j为0,1分别为棋子的两边方向，比如对于横向的时候，j=0,表示下棋位子的左边，j=1的时候表示右边
            for (int j = 0; j < 2; j++)
            {
                flag = true;
                /**
                 while语句中为一直向某一个方向遍历
                 有相同颜色的棋子的时候，Count++
                 否则置flag为false，结束该该方向的遍历
                 **/
                while (flag)
                {
                    tempXIndex = tempXIndex + dir[i,j,0];
                    tempYIndex = tempYIndex + dir[i, j, 1];
                    Vector2 tempVector = new Vector2(tempXIndex, tempYIndex);
                    //这里加上棋盘大小的判断，这里我设置的棋盘大小为20 具体可根据实际情况设置 防止越界
                    if (tempXIndex >=-7 && tempXIndex <=7 && tempYIndex >=-7 && tempYIndex <=7)
                    {
                        if (chessPosDic.ContainsKey(tempVector)&& chessPosDic[tempVector] == chessPosDic[indexVector])
                        {
                            count++;
                        }
                        else
                            flag = false;
                    }
                    else
                    {
                        flag = false;
                    }

                }
                tempXIndex = (int)indexVector.x;
                tempYIndex = (int)indexVector.y;
            }

            if (count >= 5)
            {
                max = 1;
                break;
            }
            else
                max = 0;
        }
        if (max == 1)
            return true;
        else
            return false;
    }
    //随机生成字符串
     string GetRandomString(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
           System. Random r = new System. Random(BitConverter.ToInt32(b, 0));
            string s = 　　"用户", str = custom;
            if (useNum == true) { str += "0123456789"; }
            if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        } 
}
