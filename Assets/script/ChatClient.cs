using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
 using System.Collections.Generic;
public class ChatClient : MonoSingleton<ChatClient>
{
 
    public string ipaddress = "172.16.37.134";
    public int  port = 7799;
    private Socket clientSocket;
    private Thread thread;
    private byte[] data=new byte[1024];// 数据容器
    private string message = "";
    List<LinstenerCtrl> listenerList = new List<LinstenerCtrl>();
	void Start () {
	ConnectToServer();
	}
	
	// void Update () {
    //     //只有在主线程才能更新UI
	//   if (message!="" && message!=null)
	//    {
	//         MessageText.text +="\n"+ message;
    //          msgScroll.verticalNormalizedPosition =0f;
	//         message = "";
	//     }
	// }
    /**
     * 连接服务器端函数
     * */
    void ConnectToServer()
    {
        clientSocket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        //跟服务器连接
        try
        {
             clientSocket.Connect(new IPEndPoint(IPAddress.Parse(ipaddress),port));
        }
        catch (System.Exception)
        {
            Debug.Log("连接错误");
            throw ;
            return;
        }
        //客户端开启线程接收数据
        thread = new Thread(ReceiveMessage);
        thread.Start();
 
    }
	public int GetListenersCount()
	{
		return listenerList.Count;
	}

	void ReceiveMessage()
    {
        while (true)
        {
            if (clientSocket.Connected == false)
            {
                break;
            }
        int length=clientSocket.Receive(data);
        message = Encoding.UTF8.GetString(data,0,length);
         print(message);
          for (int i = 0; i < listenerList.Count; i++)
        {
            if (listenerList[i] )
            {
                listenerList[i].GetMsg(message);
            }
        }
        }
     
    }
 
    public void SendMessage(string message)
    {
        if (clientSocket.Connected)
        {
            byte[] data=Encoding.UTF8.GetBytes(message);
            clientSocket.Send(data);
        }
       
    }
    public  void AddListener( LinstenerCtrl  listener)
    {
        listenerList.Add(listener);
    }
    public  void RemoveListener( LinstenerCtrl listener)
    {
        listenerList.Remove(listener);
    }
   
    /**
     * unity自带方法
     * 停止运行时会执行
     * */
     void OnDestroy()
    {
        Debug.Log("关闭服务");
        if (thread != null)
        {
            thread.Interrupt();
            thread.Abort();
        }
        if (clientSocket!=null && clientSocket.Connected)
        {
            //关闭连接，分接收功能和发送功能，both为两者均关闭
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }
}