using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerInteractionBase : MonoBehaviour, IInteractable
{
    public GameObject Player { get; set; }
    public bool CanInteract { get; set; } 

    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        #region INPUT HANDLER
        if (CanInteract)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                OnInteractInput();
            }
        }
        #endregion
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == Player)
        {
            CanInteract = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == Player)
        {
            CanInteract = false;
        }
    }

    public virtual void OnInteractInput()
    {
    }
}
