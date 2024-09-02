public class MoneyCollectable : Collectable
{

    public override void Trigger()
    {
        Pref.coins += m_bonus;
        GUIManager.Ins.UpdateCoinsCounting(Pref.coins);
        AudioController.Ins.PlaySound(AudioController.Ins.coinPickup);
    }
}
