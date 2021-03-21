using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Player_inf : MonoBehaviour
{
    private int viruses,links;
    private char Side;
    public Drag[] Cards = new Drag[8];
    private GameManagerScr GM;
    void Awake()
    {
        GM = FindObjectOfType<GameManagerScr>();
        Side = transform.name[0];
        links = 0;
        viruses = 0;
        if (Side == 'B')
            for (int i = 0; i < 8; i++)
                Cards[i] = GM.BCards[i].GetComponent<Drag>();
        else if (Side == 'Y')
            for (int i = 0; i < 8; i++)
                Cards[i] = GM.YCards[i].GetComponent<Drag>();


    }
    void Update()
    {
        if(links == 4)
        {
            links++;
            GM.selfClient.Send($"CWILO|{Side}|true");
        }
        else if(viruses == 4)
        {
            viruses++;
            GM.selfClient.Send($"CWILO|{Side}|false");
        }
    }
    public char GetSide()
    {
        return Side;
    }
    public int GetAmount(char check)
    {
        if(check == 'V')
            return viruses;
        else
            return links;
    }
    public void Plus(char check)
    {
        if(check == 'V')
            viruses++;
        else 
            links++;
    }
    public bool CheckCh()
    {
        return transform.childCount == 0;
    }
}
