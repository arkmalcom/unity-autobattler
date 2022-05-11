using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractUnit : MonoBehaviour {
    
    public string faction;
    public int HP;
    public int maxHP;
    public int attackDamage;
    public bool isAlive = true;
    public bool inCombat = false;
    public bool seekingTarget = false;
    public float attackTimer = 0;
    public float attackDelay;
    public float attackRange;
    public float moveSpeed;

    Animator animator;
    HealthBar healthBar;
    SpriteRenderer sprite;
    
    void Start() {
        animator = GetComponentInChildren<Animator>();
        healthBar = GetComponentInChildren<HealthBar>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        healthBar.SetMaxHealth(maxHP);
    }

    public AbstractUnit() {
        faction = string.Empty;
        HP = 0;
        maxHP = 0;
        attackDelay = 0;
        attackRange = 0;
        attackDamage = 0;
    }

    public void TakeDamage(int damage) {
        HP -= damage;
        animator.SetTrigger("Hurt");
        healthBar.SetHealth(HP);
    }

    public void Die() {
        if(isAlive) {
            animator.SetTrigger("Death");
            isAlive = false;
            sprite.sortingLayerName = "Corpses";
            Destroy(gameObject.GetComponent<BoxCollider2D>());
        }
    }

    public void AttackTarget(AbstractUnit enemy) {
        animator.SetInteger("State", 2);
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;
        else {
            attackTimer = attackDelay;
            animator.SetTrigger("Attack");
            enemy.TakeDamage(attackDamage);
        }
    }

    void Update() {
        if (HP <= 0 && isAlive)
            Die();
        if (!inCombat)
            animator.SetInteger("State", 0);
        else if (seekingTarget)
            animator.SetInteger("State", 1);
        else if (inCombat)
            animator.SetInteger("State", 2);
    }

}
