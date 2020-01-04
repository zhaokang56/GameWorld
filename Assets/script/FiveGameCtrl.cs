using System.Collections;
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
    public RectTransform canvasRect;
    public GameObject winPanel;
    public GameObject whiteWin;
    public GameObject blackWin;
    string myName;
    bool isPlaying=true;
    bool myIsWhite;
	FiveChessGameData getData;
	Dictionary<Vector2, int> chessPosDic = new Dictionary<Vector2, int>();
	void Start()
	{
		if (ChatClient.Instance.GetListenersCount()==0)
		{
			myIsWhite = false;
		}
		if (ChatClient.Instance.GetListenersCount() == 1)
		{
			myIsWhite = true;
		}
		ChatClient.Instance.AddListener(this);

	}
	void OnDestroy()
	{
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
			SetChessPos(getData.chessPos);
			getData = null;
		}
    }
    void SendMsg(Vector2 pos)
    {
        FiveChessGameData sendData = new FiveChessGameData() { chessPos = pos, name = myName, isWhite = myIsWhite };
        ChatClient.Instance.SendMessage(JsonUtility.ToJson(sendData));
    }
    public override void GetMsg(string msg)
    {
		 getData = JsonUtility.FromJson<FiveChessGameData>(msg);
	}
    /// <summary>
    /// 设置棋子位置
    /// </summary>
    /// <param name="chessPos"></param>
    void SetChessPos(Vector2 chessPos)
    {
        if (!chessPosDic.ContainsKey(chessPos))
        {
            Debug.Log("空位");
            GameObject item = Instantiate(itemObj) as GameObject;
            item.transform.SetParent(ItemParent, false);
            if (chessPosDic.Count % 2 == 0)
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
                PlayOver();
            }
            item.GetComponent<RectTransform>().anchoredPosition = chessPos * 50;
            item.SetActive(true);
        }
        else
        {
            Debug.Log("非空位");
        }
    }
    /// <summary>
    /// 游戏结束
    /// </summary>
    void PlayOver()
    {
		isPlaying = false;
		winPanel.SetActive(true);
        if (chessPosDic.Count % 2 ==0)
            blackWin.SetActive(true);
        else
            whiteWin.SetActive(true);
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
}
