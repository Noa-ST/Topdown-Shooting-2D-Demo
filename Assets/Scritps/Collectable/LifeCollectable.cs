public class LifeCollectable : Collectable
{
    public override void Trigger()
    {
        GameManager.Ins.CurLife += m_bonus;
        GUIManager.Ins.UpdateLifeInfo(GameManager.Ins.CurLife);
        AudioController.Ins.PlaySound(AudioController.Ins.lifePickup);
    }
}
