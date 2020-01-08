using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MsgType  {

	NumberGameMsg=100,
    FiveChessGameStart = 1000,
    FiveChessGameMsg=1001,
    FiveChessGameEnd=1002,
}
public class GameMessage
{
    public  MsgType msgType;
    public string msgJson;
    public int roomId;

}
