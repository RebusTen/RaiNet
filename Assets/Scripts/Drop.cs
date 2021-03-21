using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drop : MonoBehaviour, IDropHandler,IPointerClickHandler
{
    SlotsData Slots;
    Drag card;
    GameManagerScr GM;
    BonusCards[] BC = new BonusCards[2];
    
    void Awake()
    {
        Slots = GameObject.Find("Map").GetComponent<SlotsData>();
        GM = FindObjectOfType<GameManagerScr>();
        BC[0] = GM.YCards[11].GetComponent<BonusCards>();
        BC[1] = GM.BCards[11].GetComponent<BonusCards>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        card = eventData.pointerDrag.GetComponent<Drag>();
        
        if (card && transform.childCount == 0)
        {
            card.DefaultParent = transform;
        }
        else if (card && transform.GetChild(0).name[transform.GetChild(0).name.Length - 1] != card.name[card.name.Length - 1] && (card.name[card.name.Length - 1] == Slots.SData[Slots.GetSlotNum(transform)-1].fside || Slots.SData[Slots.GetSlotNum(transform)-1].fside == 'N') )
        {
            //Debug.Log();
            Drag CurCard = transform.GetChild(0).GetComponent<Drag>();
            card.DefaultParent = transform;
            GM.selfClient.Send($"CCEAT|{CurCard.name[CurCard.name.Length - 1]}|{CurCard.selfNum}|{card.name[card.name.Length - 1]}|{card.selfNum}|{Slots.GetSlotNum(transform)}");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (BC[0].on && !BC[0].Active && GM.move % 2 == 0)
        {
            BC[0].OtherOff(true);
            BC[0].on = false;
            BC[0].Active = true;
            for (int i = 0; i < 8; i++)
            {
                GM.YCards[i].GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            GM.selfClient.Send($"CBON|4|true|Y|{Slots.GetSlotNum(transform)}|");
        }
        else if(BC[1].on && !BC[1].Active && GM.move % 2 != 0)
        {
            BC[1].OtherOff(true);
            BC[1].on = false;
            BC[1].Active = true;
            for (int i = 0; i < 8; i++)
            {
                GM.BCards[i].GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            GM.selfClient.Send($"CBON|4|true|B|{Slots.GetSlotNum(transform)}|");
        }
    }

    void Update()
    {
        if (transform.childCount != 0)
            Slots.SData[Slots.GetSlotNum(transform) - 1].SetInsides($"{transform.GetChild(0).name[0]}{transform.GetChild(0).name[transform.GetChild(0).name.Length - 1]}");
        else
            Slots.SData[Slots.GetSlotNum(transform) - 1].SetInsides("NN");

    }
}
    
