using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragObj : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private BattleSceneManager battleSceneManager;

    /*D&D範囲*/
    private Vector2 rangeLeftUp;
    private Vector2 rangeRightDown;

    private Vector3 position;
    private GameObject itemPref;
    private GameObject itemObj;

    public int ItemId { set; get; }

    private void Start()
    {
        battleSceneManager = FindObjectOfType<BattleSceneManager>();

        rangeLeftUp = new Vector2(-6.5f, 2.5f);
        rangeRightDown = new Vector2(-1.7f, -5.0f);

        itemPref = Resources.Load<GameObject>("Prefabs/items/item");
    }

    public void OnBeginDrag(PointerEventData data)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        itemObj = Instantiate(itemPref);
        itemObj.GetComponent<SpriteRenderer>().sprite =
            GameManager.Instance.GetItemSprite(ItemId);
        SoundManager.Instance.PlaySeByName("DragObjSE");
    }

    public void OnDrag(PointerEventData data)
    {
        position = data.position;
        position.z = 10f;
        itemObj.transform.position = Camera.main.ScreenToWorldPoint(position);
    }

    public void OnEndDrag(PointerEventData data)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        position = data.position;
        position.z = 10f;

        //mousePositionが対象の範囲内ならじわじわと消す
        if (InRange(Camera.main.ScreenToWorldPoint(position)))
        {
            battleSceneManager.UseItem(ItemId);
            Destroy(itemObj);
        }
        //範囲外ならすぐにデストロイ
        else
        {
            Destroy(itemObj);
        }
    }

    private bool InRange(Vector3 position)
    {
        if (rangeLeftUp.x < position.x &&
            rangeLeftUp.y > position.y &&
            rangeRightDown.x > position.x &&
            rangeRightDown.y < position.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // オブジェクトの範囲内にマウスポインタが入った際に呼び出されます。
    // this method called by mouse-pointer enter the object.
    public void OnPointerEnter(PointerEventData eventData)
    {
        battleSceneManager.DisplayDescription(ItemId);
    }

    // オブジェクトの範囲内からマウスポインタが出た際に呼び出されます。
    // 
    public void OnPointerExit(PointerEventData eventData)
    {
        battleSceneManager.HiddenDescription();
    }
}
