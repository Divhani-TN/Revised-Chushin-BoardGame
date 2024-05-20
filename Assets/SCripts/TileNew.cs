using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileNew : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
        //draggableItem.parentAfterDrag = transform;
            
        if (draggableItem != null && (!draggableItem.hasMoved || GameManager.instance.initialPlacementComplete))
        {
            draggableItem.parentAfterDrag = transform;
        }
    }
}
