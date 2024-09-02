using System;
using UnityEngine;

public class ActionStats : Stats
{

    [Header("Base Stats:")]
    public float hp;
    public float damage;
    public float moveSpeed;
    public float knockbackForce; // lực đẩy lùi khi nhận sát thương
    public float knockbackTime; 
    public float invincibleTime; // tg bất tử - ko nhận st


    public override bool IsMaxLevel()
    {
        return false;
    }

    public override void Load()
    {

    }

    public override void Save()
    {

    }

    public override void Upgrade(Action OnSuccess = null, Action OnFalled = null)
    {

    }
}
