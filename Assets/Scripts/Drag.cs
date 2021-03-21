using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Drag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{
    Camera MainCamera;
    Vector3 newPos, offset, alpha;
    public Transform DefaultParent,ParentCheck;
    SlotsData SD;
    Player_inf ply, plb;
    BonusCards[] BC = new BonusCards[4];
    BonusCards[] BCE = new BonusCards[4];
    public bool Boosted,Turned, isSwapped, isSwapping;
    public Sprite Back,Front;
    public int selfNum;
    GameManagerScr Controller;

    public void Awake()
    {
        isSwapped = false;
        isSwapping = false;
        char temps;
        ply = GameObject.Find("Y Hand").GetComponent<Player_inf>();
        plb = GameObject.Find("B Hand").GetComponent<Player_inf>();
        SD = GameObject.Find("Map").GetComponent<SlotsData>();
        for (int i = 0; i < 4; i++)
            BC[i] = GameObject.Find($"{i + 1}_BONUSES_{transform.name[transform.name.Length - 1]}").GetComponent<BonusCards>();
        if (transform.name[transform.name.Length - 1] == 'Y')
            temps = 'B';
        else
            temps = 'Y';
        for (int i = 0; i < 4; i++)
            BCE[i] = GameObject.Find($"{i + 1}_BONUSES_{temps}").GetComponent<BonusCards>();
        MainCamera = Camera.allCameras[0];
        Boosted = false;
        Turned = false;
        Controller = FindObjectOfType<GameManagerScr>();
    }
    void Update()
    {
        if(((transform.name[transform.name.Length - 1] == 'Y' && Controller.move % 2 == 0) || (transform.name[transform.name.Length - 1] == 'B' && Controller.move % 2 != 0)) && isSwapped)
        {
            if (Boosted)
                GetComponent<Image>().color = new Color(0.235f, 1.0f, 0.498f);
            else
                GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (BCE[0].on && !BCE[0].used)
        {
            for(int i = 0;i< 8;i++)
            {
                if (transform.name[transform.name.Length - 1] == 'Y')
                {
                    Controller.YCards[i].GetComponent<CanvasGroup>().blocksRaycasts = false;
                }
                else if (transform.name[transform.name.Length - 1] == 'B')
                {
                    Controller.BCards[i].GetComponent<CanvasGroup>().blocksRaycasts = false;
                }
            }
            BC[0].on = false;
            BCE[0].OtherOff(true);
            Controller.selfClient.Send($"CBON|1|true|{transform.name[transform.name.Length - 1]}|{selfNum}|");
        }
        if (BC[1].on && !BC[1].Active)
        {
            Boosted = true;
            BC[1].OtherOff(true);
            BC[1].on = false;
            BC[1].Active = true;
            Controller.selfClient.Send($"CBON|2|true|{transform.name[transform.name.Length - 1]}|{selfNum}|");
        }
        if(BC[2].on && BC[2].swCount < 2 && !isSwapping)
        {
            BC[2].toSwap[BC[2].swCount] = selfNum;
            BC[2].swCount++;
            isSwapping = true;
            GetComponent<Image>().color = new Color(1.0f, 0.5647059f, 0.0f);
        }
        else if(isSwapping)
        {
            if (BC[2].toSwap[0] == selfNum)
                BC[2].toSwap[0] = BC[2].toSwap[1];
            BC[2].swCount--;
            isSwapping = false;
            if (Boosted)
                GetComponent<Image>().color = new Color(0.235f, 1.0f, 0.498f);
            else
                GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if ((transform.name[transform.name.Length - 1] == 'Y' && Controller.move % 2 == 0) || (transform.name[transform.name.Length - 1] == 'B' && Controller.move % 2 != 0)|| !Controller.isStarted)
        {

            alpha.x = transform.position.x;
            alpha.y = transform.position.y;
            offset = transform.position - MainCamera.ScreenToWorldPoint(eventData.position);
            DefaultParent = transform.parent;
            ParentCheck = DefaultParent;
            for (int i = 0; i < 64; i++)
            {
                if (SD.IsMoveable(DefaultParent, SD.SData[i].GetTrans(), transform, Boosted))
                {
                    Color tmp = GameObject.Find($"Slot_{i + 1}").GetComponent<Image>().color;
                    tmp.a = 0.5f;
                    if (!SD.SData[i].GetFire())
                        GameObject.Find($"Slot_{i + 1}").GetComponent<Image>().color = tmp;
                    GameObject.Find($"Slot_{i + 1}").GetComponent<Image>().raycastTarget = true;
                }
                else
                {
                    GameObject.Find($"Slot_{i + 1}").GetComponent<Image>().raycastTarget = false;
                }
            }
            transform.SetParent(DefaultParent.parent);
            transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if ((transform.name[transform.name.Length - 1] == 'Y' && Controller.move % 2 == 0) || (transform.name[transform.name.Length - 1] == 'B' && Controller.move % 2 != 0) || !Controller.isStarted)
        {
            newPos = MainCamera.ScreenToWorldPoint(eventData.position);
            newPos.z = 0;
            transform.position = newPos + offset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if ((transform.name[transform.name.Length - 1] == 'Y' && Controller.move % 2 == 0) || (transform.name[transform.name.Length - 1] == 'B' && Controller.move % 2 != 0) || !Controller.isStarted)
        {
            transform.SetParent(DefaultParent);
            if (transform.parent.parent.name != "TechMap")
                transform.GetComponent<CanvasGroup>().blocksRaycasts = true;

            for (int i = 0; i < 64; i++)
            {
                Color tmp = GameObject.Find($"Slot_{i + 1}").GetComponent<Image>().color;
                tmp.a = 0;
                if(!SD.SData[i].GetFire())
                    GameObject.Find($"Slot_{i + 1}").GetComponent<Image>().color = tmp;
                GameObject.Find($"Slot_{i + 1}").GetComponent<Image>().raycastTarget = false;
            }
            if (Controller.isStarted && DefaultParent != ParentCheck)
                Controller.selfClient.Send($"CMOVE|{transform.name[transform.name.Length-1]}|{selfNum}|{SD.GetSlotNum(DefaultParent)}");
            if(DefaultParent != ParentCheck && !Controller.isStarted)
                Controller.selfClient.Send($"CCPL|{transform.name[transform.name.Length - 1]}|{selfNum}|{SD.GetSlotNum(DefaultParent)}");   
        }
    }
}
