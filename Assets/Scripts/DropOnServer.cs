using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class DropOnServer : MonoBehaviour, IDropHandler
{
    Player_inf PI;
    private char Side;
    GameManagerScr GM;
    void Start()
    {
        Side = transform.name[0];
        PI = GameObject.Find($"{transform.name[0]} Hand").GetComponent<Player_inf>();
        GM = FindObjectOfType<GameManagerScr>();
    }
    void Update()
    {
        if (PI.CheckCh())
        {
            PI.GetComponent<Image>().raycastTarget = false;
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        Drag card = eventData.pointerDrag.GetComponent<Drag>();
        if ((card.name[card.name.Length - 1] == 'Y' && GM.move % 2 == 0) || (card.name[card.name.Length - 1] == 'B' && GM.move % 2 != 0))
        {
            if (card.name[card.name.Length - 1] != Side && card && (((card.DefaultParent.name == "Slot_60" || card.DefaultParent.name == "Slot_61") && Side == 'B') 
                || ((card.DefaultParent.name == "Slot_4" || card.DefaultParent.name == "Slot_5") && Side == 'Y')))
            {
                GM.selfClient.Send($"CDROP|{card.name[card.name.Length - 1]}|{card.selfNum}");
            }
        }
    }
    public void GetCard(ref Drag card, int WTD)
    {
        char side = 'N';
        if (WTD == 0)
            side = card.name[card.name.Length - 1];
        else if(WTD == 1)
        {
            if (card.name[card.name.Length - 1] == 'Y')
                side = 'B';
            else
                side = 'Y';
        }
        Player_inf PI = GameObject.Find($"{side.ToString()} Hand").GetComponent<Player_inf>();
        Transform tmpTrans = GameObject.Find($"{card.name[0]}_Slot_{side}{PI.GetAmount(card.name[0])}").GetComponent<Transform>();
        Debug.Log($"{card.name[0]}_Slot_{side}{PI.GetAmount(card.name[0])}");
        if (WTD == 1)
        {
            Quaternion qtmp = new Quaternion();
            if (card.transform.localRotation.eulerAngles.z == 0)
                qtmp.z = 180;
            else
                qtmp.z = 0;
            //Debug.Log("Before:" + card.transform.localRotation.eulerAngles.z) ;
            card.transform.SetParent(tmpTrans);
            card.transform.localRotation = qtmp;
            //Debug.Log("After:" + card.transform.localRotation.eulerAngles.z);
        }
        else
        {
            card.transform.SetParent(tmpTrans);
            Debug.Log($"Dropped on server: {card.transform.parent.name}");
        }
        PI.Plus(card.name[0]);
        card.GetComponent<CanvasGroup>().blocksRaycasts = false;
        card.transform.GetComponent<Image>().sprite = card.Front;
        Debug.Log(card.transform.GetComponent<Image>().sprite);
        //Debug.Log(card.GetComponent<CanvasGroup>().blocksRaycasts);

    }
}
 
