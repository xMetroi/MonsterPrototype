using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeManager : MonoBehaviour
{
    /// <summary>
    /// This method triggers when the animation end
    /// </summary>
    public void DestroyOnEnd()
    {
        Destroy(transform.parent.gameObject);
    }
}
