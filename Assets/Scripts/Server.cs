
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System;
using System.IO;

public class Server : MonoBehaviour
{
    public int port = 6321;

    private List<ServerClient> clients;
    private List<ServerClient> disconnectlist;

    private TcpListener server;
    private bool serverStarted;

    public void Init()
    {
        DontDestroyOnLoad(gameObject);
        clients = new List<ServerClient>();
        disconnectlist = new List<ServerClient>();

        try
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            serverStarted = true;

            StartListening();
        }
        catch(Exception e)
        {
            Debug.Log("Socket Error:" + e.Message);
        }
    }
    private void Update()
    {
        if (!serverStarted)
            return;

        foreach(ServerClient c in clients)
        {
            if(!IsConnected(c.tcp))
            {
                c.tcp.Close();
                disconnectlist.Add(c);
                continue;
            }
            else 
            {
                NetworkStream s = c.tcp.GetStream();
                if(s.DataAvailable)
                {
                    StreamReader reader = new StreamReader(s, true);
                    string data = reader.ReadLine();

                    if (data != null)
                        OnIncomingData(c, data);
                }
            }
        }
    
        for(int i = 0;i< disconnectlist.Count - 1;i++)
        {
            clients.Remove(disconnectlist[i]);
            disconnectlist.RemoveAt(i);
        }
    }
   
    private void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }
    private void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        
        string allusers = "";
        foreach (ServerClient i in clients)
        {
            allusers += i.clientname + '|';
        }
        
        ServerClient sc = new ServerClient(listener.EndAcceptTcpClient(ar));
        clients.Add(sc);

        StartListening();
        
        Broadcast("SWHO|" + allusers, clients[clients.Count - 1]);
    }

    private bool IsConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                {
                    Debug.Log("On is Connected: " + !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0));
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }

                return true;
            }
            else
                return false;
        }
        catch
        {
            return false;
        }
    }

    private void Broadcast(string data, List<ServerClient> cl)
    {
        foreach (ServerClient sc in cl)
        {
            try
            {
                StreamWriter writer = new StreamWriter(sc.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush();

            }
            catch (Exception e)
            {
                Debug.Log("Write error:" + e.Message);
            }
        }
    }
    private void Broadcast(string data, ServerClient c)
    {
        List<ServerClient> sc = new List<ServerClient> { c };
        Broadcast(data,sc);
    }

    private void OnIncomingData(ServerClient c, string data)
    {
        Debug.Log("Server:" + data);
        string[] aData = data.Split('|');
        switch (aData[0])
        {
            case "CWHO":
                c.clientname = aData[1];
                c.isHost = (aData[2] == "0") ? false : true;
                Broadcast("SCNN|"+ c.clientname,clients);
                break;
            case "CCPL":
                string cardCol = aData[1], cardn = aData[2], parent = aData[3];
                Broadcast($"SCPL|{cardCol}|{cardn}|{parent}", clients);
                break;
            case "CMOVE":
                cardCol = aData[1]; cardn = aData[2]; parent = aData[3];
                Broadcast($"SMOVE|{cardCol}|{cardn}|{parent}", clients);
                break;
            case "CCEAT":
                cardCol = aData[1];
                cardn = aData[2];
                Broadcast($"SCEAT|{cardCol}|{cardn}|{aData[3]}|{aData[4]}|{aData[5]}",clients);
                break;
            case "CDROP":
                cardCol = aData[1];
                cardn = aData[2];
                Broadcast($"SDROP|{cardCol}|{cardn}", clients);
                break;
            case "CBON":
                Broadcast($"SBON|{aData[1]}|{aData[2]}|{aData[3]}|{aData[4]}|{aData[5]}", clients);
                break;
            case "CWILO":
                Broadcast($"SWILO|{aData[1]}|{aData[2]}",clients);
                break;
        }
    }
    public void EndHost()
    {
        server.Stop();
        clients.Clear();
        disconnectlist.Clear();
    }
}

public class ServerClient
{
    public string clientname;
    public TcpClient tcp;
    public bool isHost;

    public ServerClient(TcpClient tcp)
    {
        this.tcp = tcp;
    }
}