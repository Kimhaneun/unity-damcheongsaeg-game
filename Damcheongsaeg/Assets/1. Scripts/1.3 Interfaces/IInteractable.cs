using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    #region STATE PARAMETERS
    GameObject Player { get; set; }

    bool CanInteract { get; set; }
    #endregion

    public void OnInteractInput();
}
