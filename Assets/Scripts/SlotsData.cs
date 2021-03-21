using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CardSlot
{
    Transform CardTrans;
    bool start;
    char Side;
    string IN;
    bool FireWall;
    public char fside;

    public void SetFire(bool state)
    {
        FireWall = state;
    }
    public bool GetFire()
    {
        return FireWall;
    }
    public void SetSide(int num)
    {
        if (num < 33)
            Side = 'Y';
        else
            Side = 'B';
    }
    public void SetTrans(Transform trans)
    {
        CardTrans = trans;
    }
    public void SetTrans(Vector3 trans)
    {
        CardTrans.position = trans;
    }
    public void SetStart()
    {
        start = true;
    }
    public bool GetStart()
    {
        return start;
    }
    public Transform GetTrans()
    {
        return CardTrans;
    }
    public char GetSide()
    {
        return Side;
    }
    public void SetInsides(string inside)
    {
        IN = inside;
    }
    public char GetCardSide()
    {
        return IN[1];
    }
    public char GetCardType()
    {
        return IN[0];
    }
}
public class SlotsData : MonoBehaviour
{
    public CardSlot[] SData = new CardSlot[64];
    public float dif;
    void Awake()
    {
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            SData[i].SetTrans(transform.Find($"Slot_{i + 1}"));
            SData[i].SetSide(i + 1);
            SData[i].SetInsides("NN");
            if ((i >= 0 && i < 3) || (i >= 5 && i < 8) || i == 11 || i == 12 || i == 52 || i == 51 || (i >= 56 && i <= 58) || (i >= 61 && i < 64))
                SData[i].SetStart();
            SData[i].fside = 'N';
        }
        dif = SData[0].GetTrans().position.x - SData[1].GetTrans().position.x;
    }
    public bool IsMoveable(CardSlot NowSlot,CardSlot LSlot,Transform nowCard, bool boost)
    {
        int sw = GetSlotNum(LSlot.GetTrans());
        bool state = false;
        if (NowSlot.GetTrans().name != "Y Hand" && NowSlot.GetTrans().name != "B Hand")
        {
            int now = GetSlotNum(NowSlot.GetTrans());
            if (!boost)
            {
                if ((GameObject.Find("Y Hand").transform.childCount != 0 || GameObject.Find("B Hand").transform.childCount != 0)
                    && SData[sw - 1].GetStart() && nowCard.name[nowCard.name.Length - 1] == SData[sw - 1].GetSide())
                {
                    state = true;
                }
                else if ((sw == now + 8 || sw == now - 8 || (now % 8 != 0 && sw == now + 1) || ((now - 1) % 8 != 0 && sw == now - 1))
                    && (GameObject.Find("Y Hand").transform.childCount == 0 && GameObject.Find("B Hand").transform.childCount == 0) && (SData[sw - 1].fside == 'N' || SData[sw - 1].fside == nowCard.name[nowCard.name.Length - 1]))
                {
                    state = true;
                }
            }
            else
            {
                if (sw == now + 8 || sw == now - 8 || (now % 8 != 0 && sw == now + 1) || ((now - 1) % 8 != 0 && sw == now - 1)
                    || ((now - 2) % 8 != 0 && (now - 1) % 8 != 0 && sw == now - 2) || ((now + 1) % 8 != 0 && (now) % 8 != 0 && sw == now + 2) || sw == now + 16 || sw == now - 16)
                {
                    if (sw == now - 2 && SData[now - 2].GetTrans().childCount == 0 && (SData[now - 2].fside == 'N' || SData[now - 2].fside == nowCard.name[nowCard.name.Length - 1]))
                        state = true;

                    else if (sw == now + 2 && SData[now].GetTrans().childCount == 0 && (SData[now].fside == 'N' || SData[now].fside == nowCard.name[nowCard.name.Length - 1]))
                        state = true;

                    else if (sw == now - 16 && SData[now - 9].GetTrans().childCount == 0 && (SData[now - 9].fside == 'N' || SData[now - 9].fside == nowCard.name[nowCard.name.Length - 1]))
                        state = true;

                    else if (sw == now + 16 && SData[now + 7].GetTrans().childCount == 0 && (SData[now + 7].fside == 'N' || SData[now + 7].fside == nowCard.name[nowCard.name.Length - 1])) 
                        state = true;
                    else if ((sw == now + 8 || sw == now - 8 || (now % 8 != 0 && sw == now + 1) || ((now - 1) % 8 != 0 && sw == now - 1)) && (SData[sw - 1].fside == 'N' || SData[sw - 1].fside == nowCard.name[nowCard.name.Length - 1]))
                    {
                        state = true;
                    }
                }
            }
        }
        else
        {
            if (SData[sw - 1].GetStart() && nowCard.name[nowCard.name.Length-1] == SData[sw-1].GetSide())
                state = true;
        }
        return state;
    }
    public int GetSlotNum(Transform NSlot)
    {
        string temp = Convert.ToString(NSlot.name[NSlot.name.Length - 2]);
        if (temp == "_")
            temp = "0";
        int num = Convert.ToInt32(temp + Convert.ToString(NSlot.name[NSlot.name.Length - 1]));
        return num;
    }
    public bool IsMoveable(Transform NowSlot, Transform LSlot, Transform nowCard, bool boost)
    {
        CardSlot Temp1 = new CardSlot();
        CardSlot Temp2 = new CardSlot();
        Temp1.SetTrans(NowSlot);
        Temp2.SetTrans(LSlot);

        return IsMoveable(Temp1, Temp2, nowCard, boost );
    }

}
