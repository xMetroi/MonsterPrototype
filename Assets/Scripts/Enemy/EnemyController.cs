using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private BoxCollider2D boxColliderEnemy;
    private SpriteRenderer enemyRenderer;
    private Animator enemyAnimator;

    void Start()
    {
        boxColliderEnemy = GetComponent<BoxCollider2D>();
        enemyRenderer = GetComponent<SpriteRenderer>();
        enemyAnimator = GetComponent<Animator>();

    }

    public void AssignPlayerPropertiesCollider(BoxCollider2D boxColliderReference)
    {
        if (boxColliderReference != null && boxColliderEnemy != null)
        {
            boxColliderEnemy.offset = boxColliderReference.offset;
            boxColliderEnemy.size = boxColliderReference.size;
            boxColliderEnemy.isTrigger = boxColliderEnemy.isTrigger;
            boxColliderEnemy.usedByComposite = boxColliderReference.usedByComposite;
            boxColliderEnemy.edgeRadius = boxColliderReference.edgeRadius;
            boxColliderEnemy.autoTiling = boxColliderReference.autoTiling;
            boxColliderEnemy.usedByEffector = boxColliderReference.usedByEffector;
            boxColliderEnemy.sharedMaterial = boxColliderReference.sharedMaterial;
        }
    }

    public void AssignPlayerRenderer(SpriteRenderer spriteRenderer)
    {
        enemyRenderer.sprite = spriteRenderer.sprite;
    }

    public void AssignPlayerAnimator(Animator animator)
    {
        enemyAnimator.enabled = true;

        enemyAnimator.runtimeAnimatorController = animator.runtimeAnimatorController;

        enemyAnimator.Rebind();
    }
}
