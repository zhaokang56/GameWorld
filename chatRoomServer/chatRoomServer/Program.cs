using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace 聊天室_scoket_tcp服务器
{
    class Program
    {
        static List<Client> clientlists = new List<Client>();
        static Dictionary<int, List<Client>> fiveGameRoomDic = new Dictionary<int, List<Client>>();
        static int fiveChessCount = 0;
        static int currentEmptyRoom =100001;
        /// <summary>
        /// 广播信息
        /// </summary>
        /// <param name="message"></param>
        public static void BroadcastMessage(string message, Client msgClient)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            var NotConnectClient = new List<Client>();//掉线线客户端集合
            JObject jo = (JObject)JsonConvert.DeserializeObject(message);
            if ((int)jo["msgType"]==1000)
            {
                Console.WriteLine("game start");
                if (fiveGameRoomDic.ContainsKey(currentEmptyRoom))
                {
                    if (fiveGameRoomDic[currentEmptyRoom].Count==1)
                    {
                        fiveGameRoomDic[currentEmptyRoom].Add(msgClient);
                    }
                    else if (fiveGameRoomDic[currentEmptyRoom].Count ==2)
                    {
                        currentEmptyRoom += 1;
                        List<Client> fiveGameList = new List<Client>();
                        fiveGameList.Add(msgClient);
                        fiveGameRoomDic.Add(currentEmptyRoom, fiveGameList);
                    }
                }
                else
                {
                    List< Client > fiveGameList = new List<Client>();
                    fiveGameList.Add(msgClient);
                    fiveGameRoomDic.Add(currentEmptyRoom, fiveGameList);
                }
                for (int i = 0; i < fiveGameRoomDic[currentEmptyRoom].Count; i++)
                {
                    string msg = fiveGameRoomDic[currentEmptyRoom].Count.ToString();
                    msg= msg + currentEmptyRoom.ToString();
                    message= message.Replace("content",msg);
                    Console.WriteLine("msg"+ message);
                    if (fiveGameRoomDic[currentEmptyRoom][i].Connected())
                    {
                        fiveGameRoomDic[currentEmptyRoom][i].SendMessage(message);  
                    }
                    else
                    {
                        NotConnectClient.Add(fiveGameRoomDic[currentEmptyRoom][i]);
                    }
                }
            }
            else if ((int)jo["msgType"] == 1002)
            {
                Console.WriteLine("game over");
                for (int i = 0; i < fiveGameRoomDic[currentEmptyRoom].Count; i++)
                {
                    string msg = fiveGameRoomDic[currentEmptyRoom].Count.ToString();
                    msg = msg + currentEmptyRoom.ToString();
                    message = message.Replace("content", msg);
                    Console.WriteLine("msg" + message);
                    if (fiveGameRoomDic[currentEmptyRoom][i].Connected())
                    {
                        fiveGameRoomDic[currentEmptyRoom][i].SendMessage(message);
                    }
                    else
                    {
                        NotConnectClient.Add(fiveGameRoomDic[currentEmptyRoom][i]);
                    }
                }
            }
            else
            {
                if ((int)jo["roomId"]!=0)
                {
                    for (int i = 0; i < fiveGameRoomDic[(int)jo["roomId"]].Count; i++)
                    {
                        if (fiveGameRoomDic[(int)jo["roomId"]][i].Connected())
                        {

                            fiveGameRoomDic[(int)jo["roomId"]][i].SendMessage(message);
                        }
                        else
                        {
                            NotConnectClient.Add(fiveGameRoomDic[(int)jo["roomId"]][i]);
                        }
                    }  
                }
                //foreach (var client in clientlists)
                //{
                //    if (client.Connected())//判断是否在线
                //    {
                //            client.SendMessage(message);
                //    }
                //    else
                //    {
                //        NotConnectClient.Add(client);
                //    }
                //}
            }
           
            //将掉线的客户端从集合里移除
            foreach (var temp in NotConnectClient)
            {
                fiveChessCount=0;
                clientlists.Remove(temp);
            }
        }

        static void Main(string[] args)
        {
            Socket tcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Parse("172.16.37.134");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 7799);
            tcpServer.Bind(ipEndPoint);

            Console.WriteLine("服务器开启....");
            tcpServer.Listen(2);
            //循环，每连接一个客户端建立一个Client对象
            while (true)
            {
                Socket clientSocket = tcpServer.Accept();//暂停等待客户端连接，连接后执行后面的代码
                Client client = new Client(clientSocket);//连接后，客户端与服务器的操作封装到Client类中
                Console.WriteLine("一个客户端连接....");
                clientlists.Add(client);//放入集合
            }
        }
    }
}