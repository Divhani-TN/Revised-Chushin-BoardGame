using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        Dragitems draggableItem = dropped.GetComponent<Dragitems>();
        draggableItem.parentAfterDrag = transform;
    }
}
