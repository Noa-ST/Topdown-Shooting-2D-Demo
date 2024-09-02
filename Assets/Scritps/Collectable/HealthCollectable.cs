using UnityEngine;

public class HealthCollectable : Collectable
{
    public override void Trigger()
    {
        if (m_player == null) return;

        m_player.CurHp += m_bonus;
        m_player.CurHp = Mathf.Clamp(m_player.CurHp, 0, m_player.PlayerStats.hp);
        GUIManager.Ins.UpdateHpInfo(m_player.CurHp, m_player.PlayerStats.hp);
        AudioController.Ins.PlaySound(AudioController.Ins.healthPickup);
    }
}
