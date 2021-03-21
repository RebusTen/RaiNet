using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GameManagerScr : MonoBehaviour
{
    public char Side,Sidedebug;
    public SlotsData SD;
    public GameObject[] YCards;
    public GameObject[] BCards;
    public Player_inf PI_y;
    public Player_inf PI_b;
    public Client selfClient;
    public int move,move_p;
    public bool isStarted;
    public string cName;
    public GameObject SwapMenu;
    public GameObject EndMenu;

    private void Awake()
    {
        //SwapMenu = GameObject.Find("Swap Menu");
        SD = FindObjectOfType<SlotsData>();
        selfClient = FindObjectOfType<Client>();
        move = 0;
        cName = selfClient.clientName;
        Debug.Log("Name : "+ cName);
        Side = Convert.ToChar(cName);
        PI_y = GameObject.Find($"Y Hand").GetComponent<Player_inf>();
        PI_b = GameObject.Find($"B Hand").GetComponent<Player_inf>();
        for(int i = 0;i< 8;i++)
        {
            YCards[i].GetComponent<Drag>().selfNum = i;
            BCards[i].GetComponent<Drag>().selfNum = i;

        }
        if (Side == 'Y')
        {
            for (int i = 0; i < 8; i++)
            {
                Color tmp = BCards[i].GetComponent<Image>().color;
                tmp.a = 0f;
                BCards[i].GetComponent<Image>().color = tmp;
                BCards[i].GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
            for (int i = 8; i < 12; i++)
            {
                BCards[i].GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
            //GameObject.Find("General").transform.Rotate(0, 0, 180, Space.Self);

        }
        else
        {
            for (int i = 0; i < 8; i++)
            {
                Color tmp = YCards[i].GetComponent<Image>().color;
                tmp.a = 0f;
                YCards[i].GetComponent<Image>().color = tmp;
                YCards[i].GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
            for (int i = 8; i < 12; i++)
            {
                YCards[i].GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
            GameObject.Find("General").transform.Rotate(0, 0, 180, Space.Self);
        }
    }    
    private void Update()
    {
        if((Side == 'Y' && YCards[10].GetComponent<BonusCards>().swCount == 2) || (Side == 'B' && BCards[10].GetComponent<BonusCards>().swCount == 2))
            SwapMenu.SetActive(true);
        else
            SwapMenu.SetActive(false);

    }
    public void StartGame()
    {
        if (PI_b.CheckCh() && PI_y.CheckCh() && !isStarted)
        {
            if (Side == 'Y')
            {
                for (int i = 0; i < 8; i++)
                {
                    Color tmp = BCards[i].GetComponent<Image>().color;
                    tmp.a = 1f;
                    YCards[i].GetComponent<Image>().sprite = YCards[i].GetComponent<Drag>().Back;
                    BCards[i].GetComponent<Image>().sprite = BCards[i].GetComponent<Drag>().Back;
                    BCards[i].GetComponent<Image>().color = tmp;
                    BCards[i].GetComponent<CanvasGroup>().blocksRaycasts = false;
                    YCards[i].GetComponent<CanvasGroup>().blocksRaycasts = true;
                }
                for (int i = 8; i < 12; i++)
                {
                    YCards[i].GetComponent<CanvasGroup>().blocksRaycasts = true;
                    BCards[i].GetComponent<CanvasGroup>().blocksRaycasts = false;
                }
                Debug.Log("Started!!!!!");
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    Color tmp = YCards[i].GetComponent<Image>().color;
                    tmp.a = 1f;
                    BCards[i].GetComponent<Image>().sprite = BCards[i].GetComponent<Drag>().Back;
                    YCards[i].GetComponent<Image>().sprite = YCards[i].GetComponent<Drag>().Back;
                    YCards[i].GetComponent<Image>().color = tmp;
                    YCards[i].GetComponent<CanvasGroup>().blocksRaycasts = false;
                    BCards[i].GetComponent<CanvasGroup>().blocksRaycasts = true;
                }
                for (int i = 8; i < 12; i++)
                {
                    YCards[i].GetComponent<CanvasGroup>().blocksRaycasts = false;
                    BCards[i].GetComponent<CanvasGroup>().blocksRaycasts = true;
                }
            }
            isStarted = true;
        }
    }
    public void Swap_true()
    {
        if (Side == 'Y')
        {
            YCards[10].GetComponent<BonusCards>().OtherOff(true);
            selfClient.Send($"CBON|3|true|{Side}|{YCards[10].GetComponent<BonusCards>().toSwap[0]}|{YCards[10].GetComponent<BonusCards>().toSwap[1]}");
        }
        else if (Side == 'B')
        {
            BCards[10].GetComponent<BonusCards>().OtherOff(true);
            selfClient.Send($"CBON|3|true|{Side}|{BCards[10].GetComponent<BonusCards>().toSwap[0]}|{BCards[10].GetComponent<BonusCards>().toSwap[1]}");
        }
    }
    public void Swap_false()
    {
        if (Side == 'Y')
        {
            YCards[10].GetComponent<BonusCards>().OtherOff(true);
            selfClient.Send($"CBON|3|false|{Side}|{YCards[10].GetComponent<BonusCards>().toSwap[0]}|{YCards[10].GetComponent<BonusCards>().toSwap[1]}");
        }
        else if (Side == 'B')
        {
            BCards[10].GetComponent<BonusCards>().OtherOff(true);
            selfClient.Send($"CBON|3|false|{Side}|{BCards[10].GetComponent<BonusCards>().toSwap[0]}|{BCards[10].GetComponent<BonusCards>().toSwap[1]}");
        }
    }
    public void Leave()
    {
        selfClient.CloseSocket();
        MenuScr.Instance.StartMenu();
    }
}
