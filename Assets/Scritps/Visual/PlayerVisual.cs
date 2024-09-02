using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : ActorVisual
{
    [SerializeField] private GameObject m_deathVfxPb;
    private Player m_player;
    private PlayerStats m_playerstats;
    private void Start()
    {
        m_player = (Player)m_actor;
        m_playerstats = m_player.PlayerStats;
    }

    public override void OnTakeDamage()
    {
        base.OnTakeDamage();

        GUIManager.Ins.UpdateHpInfo(m_actor.CurHp, m_actor.statData.hp);
    }

    public void OnLostLife()
    {
        if (m_player == null || m_playerstats == null)  return;

        AudioController.Ins.PlaySound(AudioController.Ins.lostLife);
        GUIManager.Ins.UpdateLifeInfo(GameManager.Ins.CurLife);
        GUIManager.Ins.UpdateHpInfo(m_player.CurHp, m_playerstats.hp);
    }

    public void OnDead()
    {
        if (m_deathVfxPb)
        {
            Instantiate(m_deathVfxPb, transform.position, Quaternion.identity);
        }

        AudioController.Ins.PlaySound(AudioController.Ins.playerDeath);
        GUIManager.Ins.ShowGameoverDialog();
    }

    public void OnAddXp()
    {
        if (m_playerstats == null) return;

        GUIManager.Ins.UpdateLevelInfo(m_playerstats.level, m_playerstats.xp, m_playerstats.levelUpXpRequied);
    }

    public void OnLevelUp()
    {
        AudioController.Ins.PlaySound(AudioController.Ins.levelUp);
    }
}
