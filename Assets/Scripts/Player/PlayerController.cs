using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private BoxCollider2D boxColliderPlayer;
    private SpriteRenderer playerRenderer;
    private Animator playerAnimator;

    void Start()
    {
        boxColliderPlayer = GetComponent<BoxCollider2D>();
        playerRenderer = GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<Animator>();

    }

    public void AssignPlayerPropertiesCollider(BoxCollider2D boxColliderReference)
    {
        if (boxColliderReference != null && boxColliderPlayer != null)
        {
            boxColliderPlayer.offset = boxColliderReference.offset;
            boxColliderPlayer.size = boxColliderReference.size;
            boxColliderPlayer.isTrigger = boxColliderPlayer.isTrigger;
            boxColliderPlayer.usedByComposite = boxColliderReference.usedByComposite;
            boxColliderPlayer.edgeRadius = boxColliderReference.edgeRadius;
            boxColliderPlayer.autoTiling = boxColliderReference.autoTiling;
            boxColliderPlayer.usedByEffector = boxColliderReference.usedByEffector;
            boxColliderPlayer.sharedMaterial = boxColliderReference.sharedMaterial;
        }
    }

    public void AssignPlayerRenderer(SpriteRenderer spriteRenderer)
    { 
        playerRenderer.sprite = spriteRenderer.sprite;
    }

    public void AssignPlayerAnimator(Animator animator)
    {
        playerAnimator.runtimeAnimatorController = animator.runtimeAnimatorController;
    }
}
