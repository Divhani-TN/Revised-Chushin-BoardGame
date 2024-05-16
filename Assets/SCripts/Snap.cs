using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snap : MonoBehaviour
{
    public List<Transform> SnapAreas;
    public List<Drag> draggableobjects;
    public float snapRange = 0.5f;

    private void Start()
    {
        foreach(Drag draggable in draggableobjects)
        {
            draggable.dragEndedCallback = OnDragEnded;
        }
    }
    private void OnDragEnded(Drag draggable)
    {
        float closestDistance = -1;
        Transform closectSnapArea = null;

        foreach(Transform SnapArea in SnapAreas)
        {
            float currentDistance = Vector2.Distance(draggable.transform.localPosition, SnapArea.localPosition);
            if(closectSnapArea == null || currentDistance < closestDistance)
            {
                closectSnapArea = SnapArea;
                closestDistance = currentDistance;
            }
        }
        if(closectSnapArea !=null && closestDistance<= snapRange)
        {
            draggable.transform.localPosition = closectSnapArea.localPosition;
        }
    }
}