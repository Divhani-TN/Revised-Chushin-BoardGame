using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject pnlChange;
    public GameObject pnlChangeBack;

    public void Change()
    {
        pnlChangeBack.SetActive(false);
        pnlChange.SetActive(true);
    }
    
    public void ChangeBack()
    {
        pnlChangeBack.SetActive(true);
        pnlChange.SetActive(false);
    }
}
