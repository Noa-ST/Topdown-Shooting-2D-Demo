using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class ActorVisual : MonoBehaviour
{
    private FlashVfx m_flashVfx;
    protected Actor m_actor;

    protected virtual void Awake()
    {
        m_flashVfx = GetComponent<FlashVfx>();
        m_actor = GetComponent<Actor>();
    }

    public virtual void OnTakeDamage()
    {
        if (m_flashVfx == null || m_actor == null || m_actor.IsDead) return;

        m_flashVfx.Flash(m_actor.statData.knockbackTime);
    }
}
