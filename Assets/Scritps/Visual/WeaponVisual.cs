using UnityEngine;

public class WeaponVisual : MonoBehaviour
{
    [SerializeField] private AudioClip m_shootingSound;
    [SerializeField] private AudioClip m_reloadSound;

    private Weapon m_weapon;

    private void Awake()
    {
        m_weapon = GetComponent<Weapon>();
    }

    public void OnShoot()
    {
        AudioController.Ins.PlaySound(m_shootingSound);
        CineController.Ins.ShakeTrigger();
    }

    public void OnReload()
    {
        GUIManager.Ins.ShowReloadText(true);
    }

    public void OnReloadDone()
    {
        AudioController.Ins.PlaySound(m_reloadSound);
        GUIManager.Ins.ShowReloadText(false);
    }
}
