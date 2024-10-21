using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Magazine : MonoBehaviour
{
    public int ammoCount;

    public Magazine(int initialAmmo)
    {
        ammoCount = initialAmmo;
    }
}
