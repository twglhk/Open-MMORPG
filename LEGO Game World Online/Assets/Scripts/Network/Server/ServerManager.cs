﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerCore;
using Server;
using System.Net;

namespace Server
{
    public class ServerManager : MonoBehaviour
    {
        static Listener _listener = new Listener();
        public static GameRoom Room = new GameRoom();   // Test room

        static void FlushRoom()
        {
            Room.Push(() => Room.Flush());
            JobTimer.Instance.Push(FlushRoom, 250);
        }

        // Start is called before the first frame update
        void Start()
        {
            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            // TO DO : Send host info to other clients

            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });

            JobTimer.Instance.Push(FlushRoom);
        }

        // Update is called once per frame
        void Update()
        {
            JobTimer.Instance.Flush();
        }
    }
}