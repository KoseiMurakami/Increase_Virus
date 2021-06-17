using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropArea : MonoBehaviour, IDropHandler
{
    private void Start()
    {
    }

    public void OnDrop(PointerEventData data)
    {
        DragObj dragObj = data.pointerDrag.GetComponent<DragObj>();
        if (dragObj != null)
        {
             dragObj.transform.SetParent(transform);
        }
    }
}
