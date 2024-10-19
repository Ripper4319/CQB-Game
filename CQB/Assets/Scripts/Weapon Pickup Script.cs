using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public bool isInteracted = false;

    public void Interact()
    {
        isInteracted = true;
    }
}
