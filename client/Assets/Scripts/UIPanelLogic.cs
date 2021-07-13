using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelLogic : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    public void ClosePanel()
    {
        if(gameObject.activeSelf)
            animator.SetBool("Enabled", false);
    }

    public void OpenPanel()
    {
        gameObject.SetActive(true);
        animator.SetBool("Enabled", true);
    }

    public void TogglePanel()
    {
        if(!gameObject.activeSelf)
            gameObject.SetActive(true);
        
        animator.SetBool("Enabled", !animator.GetBool("Enabled"));
    }
}
