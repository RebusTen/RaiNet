using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.IO;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class Client : MonoBehaviour
{
    public string clientName;
    public bool isHost;

    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;
    private GameManagerScr GM;
    private DropOnServer gservy;
    private DropOnServer gservb;
    private List<GameClient> players = new List<GameClient>();

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    public bool ConnectToServer(string host, int port)
    {
        if(socketReady)
            return false;
        
        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            socketReady = true;
        }
        catch(Exception e)
        {

            Debug.Log("Socket error :" + e.Message);
        }
        return socketReady;
    }
    
    private void Update()
    {
        if(socketReady)
        {
            if(stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
                    OnIncomingData(data);
            }
        }
    }

    public void Send(string data)
    {
        if (!socketReady)
            return;

        writer.WriteLine(data);
        writer.Flush();
    }
    
    public void OnIncomingData(string data)
    {
        Debug.Log("Client:" + data);
        string[] aData = data.Split('|');

        switch(aData[0])
        {
            case "SWHO":
                for(int i = 1;i<aData.Length-1;i++)
                {
                    Debug.Log("SWHO:" + aData[i]); 
                    UserConnected(aData[i],false);
                }
                Send("CWHO|" + clientName + "|" + ((isHost)?1:0).ToString());
                break;
            case "SCNN":
                UserConnected(aData[1], false);
                break;
            case "SCPL":
                Debug.Log("Placing");
                GM = GameObject.Find("GameManager_").GetComponent<GameManagerScr>();
                string cardColor = aData[1];
                int cardn = Convert.ToInt32(aData[2]), parent = Convert.ToInt32(aData[3]);
                if (cardColor == "Y")
                    GM.YCards[cardn].transform.SetParent(GM.SD.SData[parent - 1].GetTrans());
                else
                    GM.BCards[cardn].transform.SetParent(GM.SD.SData[parent - 1].GetTrans());
                if (GM.PI_y.CheckCh() && GM.PI_b.CheckCh())
                    GM.StartGame();

                break;
            case "SMOVE":
                Debug.Log("Movingw");
                GM = GameObject.Find("GameManager_").GetComponent<GameManagerScr>();
                cardColor = aData[1];
                cardn = Convert.ToInt32(aData[2]); parent = Convert.ToInt32(aData[3]);
                if (cardColor == "Y")
                    GM.YCards[cardn].transform.SetParent(GM.SD.SData[parent - 1].GetTrans());
                else
                    GM.BCards[cardn].transform.SetParent(GM.SD.SData[parent - 1].GetTrans());
                GM.move++;
                break;
            case "SCEAT":
                string curcardColor = aData[1];
                int curcardn = Convert.ToInt32(aData[2]);
                cardColor = aData[3];
                cardn = Convert.ToInt32(aData[4]);
                parent = Convert.ToInt32(aData[5]);
                gservy = GameObject.Find("Y_Server").GetComponent<DropOnServer>();
                gservb = GameObject.Find("B_Server").GetComponent<DropOnServer>();
                Drag curcard;
                if (curcardColor == "Y")
                {
                    curcard = GM.YCards[curcardn].GetComponent<Drag>();
                    gservb.GetCard(ref curcard, 1);
                    GM.BCards[cardn].transform.SetParent(GM.SD.SData[parent - 1].GetTrans());
                    if (curcard.Boosted) 
                    {
                        curcard.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
                        GM.YCards[9].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
                        curcard.Boosted = false;
                        GM.YCards[9].GetComponent<BonusCards>().Active = false;
                        Debug.Log("SBON turning off 2 Y EATING");
                    }
                    if (curcard.isSwapped)
                        curcard.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
                }
                else
                {
                    curcard = GM.BCards[curcardn].GetComponent<Drag>();
                    gservb.GetCard(ref curcard, 1);
                    GM.YCards[cardn].transform.SetParent(GM.SD.SData[parent - 1].GetTrans());
                    if (curcard.Boosted)
                    {
                        curcard.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
                        GM.BCards[9].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
                        curcard.Boosted = false;
                        GM.BCards[9].GetComponent<BonusCards>().Active = false;
                        Debug.Log("SBON turning off 2 B EATING");
                    }
                    if (curcard.isSwapped)
                        curcard.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);

                }
                GM.move++;
                Debug.Log("Eaten");
                break;
            case "SDROP":
                cardColor = aData[1];
                cardn = Convert.ToInt32(aData[2]);
                if (cardColor == "Y")
                {
                    curcard = GM.YCards[cardn].GetComponent<Drag>();
                    gservb.GetCard(ref curcard,0);
                    if(curcard.Boosted)
                    {
                        GM.PI_y.Cards[cardn].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
                        GM.YCards[9].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
                        GM.YCards[9].GetComponent<BonusCards>().Active = false;
                    }
                }
                else
                {
                    curcard = GM.BCards[cardn].GetComponent<Drag>();
                    gservy.GetCard(ref curcard,0);
                    if (curcard.Boosted)
                    {
                        GM.PI_b.Cards[cardn].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
                        GM.BCards[9].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
                        GM.BCards[9].GetComponent<BonusCards>().Active = false;
                    }
                }
                GM.move++;
                break;
            case "SBON":
                int bon = Convert.ToInt32(aData[1]);
                bool WTD = Convert.ToBoolean(aData[2]);
                if (WTD)
                {
                    if (bon == 1)
                    {
                        cardColor = aData[3];
                        cardn = Convert.ToInt32(aData[4]);
                        if (cardColor == "Y")
                        {
                            GM.BCards[8].GetComponent<BonusCards>().used = true;
                            GM.BCards[8].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                            GM.BCards[8].GetComponent<CanvasGroup>().blocksRaycasts = false;
                            GM.YCards[cardn].GetComponent<Image>().sprite = GM.YCards[cardn].GetComponent<Drag>().Front;
                            GM.YCards[cardn].GetComponent<Drag>().Turned = true;
                        }
                        else if (cardColor == "B")
                        {
                            GM.YCards[8].GetComponent<BonusCards>().used = true;
                            GM.YCards[8].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                            GM.YCards[8].GetComponent<CanvasGroup>().blocksRaycasts = false;
                            GM.BCards[cardn].GetComponent<Image>().sprite = GM.BCards[cardn].GetComponent<Drag>().Front;
                            GM.BCards[cardn].GetComponent<Drag>().Turned = true;
                        }
                    }
                    else if (bon == 2)
                    {
                        cardColor = aData[3];
                        cardn = Convert.ToInt32(aData[4]);
                        if (cardColor == "Y")
                        {
                            GM.PI_y.Cards[cardn].Boosted = true;
                            GM.PI_y.Cards[cardn].GetComponent<Image>().color = new Color(0.235f, 1.0f, 0.498f);
                            GM.YCards[9].GetComponent<Image>().color = new Color(0.235f, 1.0f, 0.498f);
                            GM.YCards[9].GetComponent<BonusCards>().on = false;
                        }
                        else if (cardColor == "B")
                        {
                            GM.PI_b.Cards[cardn].Boosted = true;
                            GM.PI_b.Cards[cardn].GetComponent<Image>().color = new Color(0.235f, 1.0f, 0.498f);
                            GM.BCards[9].GetComponent<Image>().color = new Color(0.235f, 1.0f, 0.498f);
                            GM.BCards[9].GetComponent<BonusCards>().on = false;
                        }
                    }
                    else if(bon == 3)
                    {
                        cardColor = aData[3];
                        cardn = Convert.ToInt32(aData[4]);
                        curcardn = Convert.ToInt32(aData[5]);
                        if (cardColor == "Y")
                        {
                            Transform tmpParent = GM.PI_y.Cards[cardn].transform.parent;
                            GM.PI_y.Cards[cardn].transform.SetParent(GM.PI_y.Cards[curcardn].transform.parent);
                            GM.PI_y.Cards[curcardn].transform.SetParent(tmpParent);

                            GM.PI_y.Cards[cardn].isSwapped = true;
                            GM.PI_y.Cards[curcardn].isSwapped = true;
                            GM.PI_y.Cards[cardn].GetComponent<Image>().color = new Color(1.0f, 0.5647059f, 0.0f);
                            GM.PI_y.Cards[curcardn].GetComponent<Image>().color = new Color(1.0f, 0.5647059f, 0.0f);
                            GM.YCards[cardn].GetComponent<Image>().sprite = GM.YCards[cardn].GetComponent<Drag>().Back;
                            GM.YCards[curcardn].GetComponent<Image>().sprite = GM.YCards[cardn].GetComponent<Drag>().Back;


                            GM.YCards[10].GetComponent<BonusCards>().on = false;
                            GM.YCards[10].GetComponent<BonusCards>().swCount = 0;
                            GM.YCards[10].GetComponent<BonusCards>().used = true;
                            GM.YCards[10].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                            GM.YCards[10].GetComponent<CanvasGroup>().blocksRaycasts = false;

                        }
                        else if (cardColor == "B")
                        {
                            Transform tmpParent = GM.PI_b.Cards[cardn].transform.parent;
                            GM.PI_b.Cards[cardn].transform.SetParent(GM.PI_b.Cards[curcardn].transform.parent);
                            GM.PI_b.Cards[curcardn].transform.SetParent(tmpParent);

                            GM.PI_b.Cards[cardn].isSwapped = true;
                            GM.PI_b.Cards[curcardn].isSwapped = true;
                            GM.PI_b.Cards[cardn].GetComponent<Image>().color = new Color(1.0f, 0.5647059f, 0.0f);
                            GM.PI_b.Cards[curcardn].GetComponent<Image>().color = new Color(1.0f, 0.5647059f, 0.0f);
                            GM.BCards[cardn].GetComponent<Image>().sprite = GM.BCards[cardn].GetComponent<Drag>().Back;
                            GM.BCards[curcardn].GetComponent<Image>().sprite = GM.BCards[cardn].GetComponent<Drag>().Back;


                            GM.BCards[10].GetComponent<BonusCards>().on = false;
                            GM.BCards[10].GetComponent<BonusCards>().swCount = 0;
                            GM.BCards[10].GetComponent<BonusCards>().used = true;
                            GM.BCards[10].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                            GM.BCards[10].GetComponent<CanvasGroup>().blocksRaycasts = false;
                        }
                    }
                    else if(bon == 4)
                    {
                        cardColor = aData[3];
                        cardn = Convert.ToInt32(aData[4]);
                        GM.SD.SData[cardn - 1].SetFire(true);
                        GM.SD.SData[cardn - 1].fside = Convert.ToChar(cardColor);
                        GM.SD.SData[cardn - 1].GetTrans().GetComponent<Image>().color = new Color(1.0f, 0.1333333f, 0.0f,1.0f);
                        if (cardColor == "Y")
                        {
                            GM.YCards[11].GetComponent<Image>().color = new Color(1.0f, 0.1333333f, 0.0f);
                            GM.YCards[11].GetComponent<BonusCards>().fireWall = cardn;
                        }
                        else if (cardColor == "B")
                        {
                            GM.BCards[11].GetComponent<Image>().color = new Color(1.0f, 0.1333333f, 0.0f);
                            GM.BCards[11].GetComponent<BonusCards>().fireWall = cardn;
                        }
                    }
                }
                else
                {
                    if(bon == 2)
                    {
                        cardColor = aData[3];
                        cardn = Convert.ToInt32(aData[4]);
                        if (cardColor == "Y")
                        {
                            GM.YCards[cardn].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
                            GM.YCards[9].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
                            GM.PI_y.Cards[cardn].Boosted = false;
                            GM.YCards[9].GetComponent<BonusCards>().Active = false;
                            Debug.Log("SBON turning off 2 Y");
                        }
                        else if (cardColor == "B")
                        {
                            GM.BCards[cardn].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
                            GM.BCards[9].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
                            GM.PI_b.Cards[cardn].Boosted = false;
                            GM.BCards[9].GetComponent<BonusCards>().Active = false;
                            Debug.Log("SBON turning off 2 B");
                        }
                    }
                    else if (bon == 3)
                    {
                        cardColor = aData[3];
                        cardn = Convert.ToInt32(aData[4]);
                        curcardn = Convert.ToInt32(aData[5]);
                        if (cardColor == "Y")
                        {
                            

                            GM.PI_y.Cards[cardn].isSwapped = true;
                            GM.PI_y.Cards[curcardn].isSwapped = true;
                            GM.PI_y.Cards[cardn].GetComponent<Image>().color = new Color(1.0f, 0.5647059f, 0.0f);
                            GM.PI_y.Cards[curcardn].GetComponent<Image>().color = new Color(1.0f, 0.5647059f, 0.0f);
                            GM.YCards[cardn].GetComponent<Image>().sprite = GM.YCards[cardn].GetComponent<Drag>().Back;
                            GM.YCards[curcardn].GetComponent<Image>().sprite = GM.YCards[cardn].GetComponent<Drag>().Back;


                            GM.YCards[10].GetComponent<BonusCards>().on = false;
                            GM.YCards[10].GetComponent<BonusCards>().swCount = 0;
                            GM.YCards[10].GetComponent<BonusCards>().used = true;
                            GM.YCards[10].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                            GM.YCards[10].GetComponent<CanvasGroup>().blocksRaycasts = false;

                        }
                        else if (cardColor == "B")
                        {
                            

                            GM.PI_b.Cards[cardn].isSwapped = true;
                            GM.PI_b.Cards[curcardn].isSwapped = true;
                            GM.PI_b.Cards[cardn].GetComponent<Image>().color = new Color(1.0f, 0.5647059f, 0.0f);
                            GM.PI_b.Cards[curcardn].GetComponent<Image>().color = new Color(1.0f, 0.5647059f, 0.0f);
                            GM.BCards[cardn].GetComponent<Image>().sprite = GM.BCards[cardn].GetComponent<Drag>().Back;
                            GM.BCards[curcardn].GetComponent<Image>().sprite = GM.BCards[cardn].GetComponent<Drag>().Back;


                            GM.BCards[10].GetComponent<BonusCards>().on = false;
                            GM.BCards[10].GetComponent<BonusCards>().swCount = 0;
                            GM.BCards[10].GetComponent<BonusCards>().used = true;
                            GM.BCards[10].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                            GM.BCards[10].GetComponent<CanvasGroup>().blocksRaycasts = false;
                        }
                    }
                    else if(bon == 4)
                    {
                        cardColor = aData[3];
                        cardn = Convert.ToInt32(aData[4]);
                        GM.SD.SData[cardn - 1].SetFire(false);
                        GM.SD.SData[cardn - 1].fside = 'N';
                        GM.SD.SData[cardn - 1].GetTrans().GetComponent<Image>().color = new Color(0.7155091f, 1.0f, 0.0f, 0.0f);
                        if (cardColor == "Y")
                            GM.YCards[11].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
                        else if (cardColor == "B")
                            GM.BCards[11].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
                    }

                }
                GM.move++;
                break;
            case "SWILO":
                for(int i = 0; i< 12;i++)
                {
                    GM.YCards[i].SetActive(false);
                    GM.BCards[i].SetActive(false);
                }
                if(aData[1] == GM.cName && Convert.ToBoolean(aData[2]))
                {
                    GM.EndMenu.SetActive(true);
                }
                else if(aData[1] == GM.cName && !(Convert.ToBoolean(aData[2])))
                {
                    GM.EndMenu.transform.Find("WILO Text").GetComponent<Text>().text = "ACCESS DENIED";
                    GM.EndMenu.transform.Find("WILO Text").GetComponent<Text>().color = Color.red;
                    GM.EndMenu.SetActive(true);
                }
                else if (aData[1] != GM.cName && !(Convert.ToBoolean(aData[2])))
                {
                    GM.EndMenu.SetActive(true);
                }
                else if (aData[1] != GM.cName && Convert.ToBoolean(aData[2]))
                {
                    GM.EndMenu.transform.Find("WILO Text").GetComponent<Text>().text = "ACCESS DENIED";
                    GM.EndMenu.transform.Find("WILO Text").GetComponent<Text>().color = Color.red;
                    GM.EndMenu.SetActive(true);
                }
                break;
        }
    }
    private void UserConnected(string name, bool host)
    {
        GameClient c = new GameClient();
        c.name = name;
        players.Add(c);

        if (players.Count == 2)
        {
            MenuScr.Instance.StartGame();
        }
    }
    private void OnApplicationQuit()
    {
        CloseSocket();
    }
    public void CloseSocket()
    {
        if (!socketReady)
            return;
        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }
}

public class GameClient
{
    public string name;
    public bool isHost;

}
