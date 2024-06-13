using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    public static PanelManager instance;
    
    public GameObject ErrEndTurnPanel;
    public GameObject ErrStoneMovePanel;
    public GameObject ErrTokenMovePanel;
    public GameObject ErrTokenPushPanel;
    public GameObject ErrTokenMoverOverPanel;
    public GameObject ErrCanPush;
    public GameObject ErrRemoveToken;
    public GameObject ErrEmptyTile;
    public GameObject ErrNoTile;
    
    private GameObject activePanel;
    
    public GameObject overlay;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        DisableErrPanels();
        overlay.SetActive(false);
    }
    
    public void EnableErr(int errNum)
    {
        DisableErrPanels();

        switch (errNum)
        {
            case 0:
                ErrStoneMovePanel.SetActive(true);
                activePanel = ErrStoneMovePanel;
                break;
            case 1:
                ErrTokenMovePanel.SetActive(true);
                activePanel = ErrTokenMovePanel;
                break;
            case 2:
                ErrTokenPushPanel.SetActive(true);
                activePanel = ErrTokenPushPanel; 
                break;
            case 3:
                ErrTokenMoverOverPanel.SetActive(true);
                activePanel = ErrTokenMoverOverPanel;
                break;
            case 4:
                ErrEndTurnPanel.SetActive(true);
                activePanel = ErrEndTurnPanel;
                break;
            case 5:
                ErrCanPush.SetActive(true);
                activePanel = ErrCanPush;
                break;
            case 6:
                ErrRemoveToken.SetActive(true);
                activePanel = ErrRemoveToken;
                break;
            case 7:
                ErrEmptyTile.SetActive(true);
                activePanel = ErrEmptyTile;
                break;
            case 8:
                ErrNoTile.SetActive(true);
                activePanel = ErrNoTile;
                break;
            default:
                Debug.LogWarning("Invalid panel number: " + errNum);
                break;
        }
        
        overlay.SetActive(true);
    }
    
    public void OnOverlayClick()
    {
        if (activePanel != null)
        {
            DisableErrPanels();
        }
    }
    
    public void DisableErrPanels()
    {
        ErrEndTurnPanel.SetActive(false);
        ErrStoneMovePanel.SetActive(false);
        ErrTokenMovePanel.SetActive(false);
        ErrTokenPushPanel.SetActive(false);
        ErrTokenMoverOverPanel.SetActive(false);
        ErrCanPush.SetActive(false);
        ErrRemoveToken.SetActive(false);
        ErrEmptyTile.SetActive(false);
        ErrNoTile.SetActive(false);
        
        activePanel = null;
        
        overlay.SetActive(false);
    }
}
