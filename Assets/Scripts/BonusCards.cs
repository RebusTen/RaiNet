using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class BonusCards : MonoBehaviour, IPointerClickHandler
{
    public bool on, Active,used;
    char side;
    public int type,swCount;
    Player_inf YPI,BPI;
    GameManagerScr GM;
    public int[] toSwap;
    public int fireWall;

    public void Awake()
    {
        swCount = 0;
        used = false;
        GM = FindObjectOfType<GameManagerScr>();
        on = false;
        side = transform.name[transform.name.Length - 1];
        type = Convert.ToInt32(transform.name[0].ToString());
        if(type == 3)
            toSwap = new int[2];
        Active = false;
        YPI = GameObject.Find($"Y Hand").GetComponent<Player_inf>();
        BPI = GameObject.Find($"B Hand").GetComponent<Player_inf>();
        transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"{side} {type} Active : {Active} ON: {on}");
        if (side != GM.Side || ((side == 'Y' && GM.move % 2 != 0) || (side == 'B' && GM.move % 2 == 0)) || !GM.isStarted)
            return;
        if (!Active)
        {
            on = !on;
            if (on)
            {
                if (type == 1)
                {
                    GameObject.Find($"{type}_BONUSES_{side}").GetComponent<Image>().color = new Color(1.0f, 0.0f, 1.0f);
                    if (side == 'Y')
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            GM.BCards[i].GetComponent<CanvasGroup>().blocksRaycasts = true;
                        }
                    }
                    else if (side == 'B')
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            GM.YCards[i].GetComponent<CanvasGroup>().blocksRaycasts = true;
                        }
                    }
                }
                else if (type == 2)
                    GameObject.Find($"{type}_BONUSES_{side}").GetComponent<Image>().color = new Color(0.235f, 1.0f, 0.498f);
                else if(type == 3)
                {
                    GameObject.Find($"{type}_BONUSES_{side}").GetComponent<Image>().color = new Color(1.0f, 0.5647059f, 0.0f);
                }
                else if(type == 4)
                {
                    GameObject.Find($"{type}_BONUSES_{side}").GetComponent<Image>().color = new Color(1.0f, 0.1333333f, 0.0f);
                    for(int i = 0;i < 64;i++)
                    {
                        if (i != 3 && i != 4 && i != 59 && i != 60)
                        {
                            if(GM.SD.SData[i].GetCardSide() == 'N' && !GM.SD.SData[i].GetFire())
                                GM.SD.SData[i].GetTrans().GetComponent<Image>().raycastTarget = true;
                        }
                    }
                    if (side == 'Y')
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            GM.YCards[i].GetComponent<CanvasGroup>().blocksRaycasts = false;
                        }
                    }
                    else if (side == 'B')
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            GM.BCards[i].GetComponent<CanvasGroup>().blocksRaycasts = false;
                        }
                    }
                }
                OtherOff(!on);
            }
            else
            {
                if (type == 1)
                {
                    GameObject.Find($"{type}_BONUSES_{side}").GetComponent<Image>().color = new Color(1.0f, 0.0f, 1.0f);
                    if (side == 'Y')
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            GM.BCards[i].GetComponent<CanvasGroup>().blocksRaycasts = false;
                        }
                    }
                    else if (side == 'B')
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            GM.YCards[i].GetComponent<CanvasGroup>().blocksRaycasts = false;
                        }
                    }
                }
                else if (type == 3)
                {
                    if (side == 'Y')
                    {
                        if (swCount > 0)
                        {
                            GM.YCards[toSwap[0]].GetComponent<Drag>().isSwapping = false;
                            if (GM.YCards[toSwap[0]].GetComponent<Drag>().Boosted)
                                GM.YCards[toSwap[0]].GetComponent<Image>().color = new Color(0.235f, 1.0f, 0.498f);
                            else
                                GM.YCards[toSwap[0]].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
                        }
                        if (swCount > 1)
                        {
                            GM.YCards[toSwap[1]].GetComponent<Drag>().isSwapping = false;
                            if (GM.YCards[toSwap[1]].GetComponent<Drag>().Boosted)
                                GM.YCards[toSwap[1]].GetComponent<Image>().color = new Color(0.235f, 1.0f, 0.498f);
                            else
                                GM.YCards[toSwap[1]].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
                        }
                    }
                    else if (side == 'B')
                    {
                        if (swCount > 0)
                        {
                            GM.BCards[toSwap[0]].GetComponent<Drag>().isSwapping = false;
                            if (GM.BCards[toSwap[0]].GetComponent<Drag>().Boosted)
                                GM.BCards[toSwap[0]].GetComponent<Image>().color = new Color(0.235f, 1.0f, 0.498f);
                            else
                                GM.BCards[toSwap[0]].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
                        }
                        if (swCount > 1)
                        {
                            GM.BCards[toSwap[1]].GetComponent<Drag>().isSwapping = false;
                            if (GM.BCards[toSwap[1]].GetComponent<Drag>().Boosted)
                                GM.BCards[toSwap[1]].GetComponent<Image>().color = new Color(0.235f, 1.0f, 0.498f);
                            else
                                GM.BCards[toSwap[1]].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
                        }
                    }
                    swCount = 0;
                }
                else if (type == 4)
                {
                    for (int i = 0; i < 64; i++)
                    {
                        if (i != 3 && i != 4 && i != 59 && i != 60)
                        {
                            if (GM.SD.SData[i].GetCardSide() == 'N' && !GM.SD.SData[i].GetFire())
                                GM.SD.SData[i].GetTrans().GetComponent<Image>().raycastTarget = false;
                        }
                    }
                    if (side == 'Y')
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            GM.YCards[i].GetComponent<CanvasGroup>().blocksRaycasts = true;
                        }
                    }
                    else if (side == 'B')
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            GM.BCards[i].GetComponent<CanvasGroup>().blocksRaycasts = true;
                        }
                    }
                }
                GameObject.Find($"{type}_BONUSES_{side}").GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
                OtherOff(!on);
            }
        }
        else
        {
            GameObject.Find($"{type}_BONUSES_{side}").GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
            Active = false;
            if (type == 2)
            {
                for (int i = 0; i < 8; i++)
                {

                    if (BPI.GetSide() == side && BPI.Cards[i].Boosted)
                    {
                        GM.selfClient.Send($"CBON|2|false|{side}|{i}|");
                        Debug.Log("BC trying off B");

                    }
                    else if (YPI.GetSide() == side && YPI.Cards[i].Boosted)
                    {
                        GM.selfClient.Send($"CBON|2|false|{side}|{i}|");
                        Debug.Log("BC trying off Y");
                    }
                }
            }
            else if(type == 4)
                GM.selfClient.Send($"CBON|4|false|{side}|{fireWall}|");
            
        }
    }
    public void OtherOff(bool state)
    {
        for (int i = 1; i < 5; i++)
        {
            if (i != type && ((side == 'Y' && !GM.YCards[i+7].GetComponent<BonusCards>().used) || (side == 'B' && !GM.BCards[i + 7].GetComponent<BonusCards>().used)))
            {
                GameObject.Find($"{i}_BONUSES_{side}").GetComponent<CanvasGroup>().blocksRaycasts = state;
            }
        }
    }
}
