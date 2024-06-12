using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileNew : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    private FourInARow fourInARow;
    private Color originalColor;
    private Image tileImage;
    
    public bool thisTileSelected;

    public void Start()
    {
        fourInARow = FourInARow.instance;

        tileImage = GetComponent<Image>();
        if (tileImage != null)
        {
            originalColor = tileImage.color;
        }
    }
    
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
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (fourInARow.forcedStoneRemoval)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                OnLeftClick();
            }
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                OnRightClick();
            }
        }
    }

    public void OnLeftClick()
    {
        GameManager.instance.DeselectAllTiles();
        if (tileImage != null)
        {
            tileImage.color = GetSelectedColor();
        }
        
        thisTileSelected = true;
        GameManager.instance.AssignSelectedTile(this);
    }
    
    public void OnRightClick()
    {
        GameManager.instance.DeselectAllTiles();
        if (tileImage != null)
        {
            tileImage.color = GetSelectedColor();
        }
        
        thisTileSelected = true;
        GameManager.instance.AssignSelectedTile(this);
    }
    
    public void Deselect()
    {
        if (tileImage != null)
        {
            tileImage.color = originalColor;
            //originalColor.a = Mathf.Min(1.0f, originalColor.a);
        }
        //tileImage.color = originalColor;
        thisTileSelected = false;
    }

    private Color GetSelectedColor()
    {
        Color selectedColor = originalColor;
        selectedColor.a = Mathf.Min(1.0f, originalColor.a + 0.5f);
        return selectedColor;
    }
}
