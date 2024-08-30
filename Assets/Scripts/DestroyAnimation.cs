using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAnimation : MonoBehaviour
{
    public void DestroyPanel()
    {
        this.gameObject.SetActive(false);
    }

    public void ActivePanel()
    {

        transform.parent.gameObject.SetActive(true);
    }
}
