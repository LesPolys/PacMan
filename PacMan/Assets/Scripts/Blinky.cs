public class Blinky : Ghost
{
    public override void SetTarget()
    {
        //Blink has the simplest targeting. Only targets direct player position
        m_ChaseTarget = GameController.Instance.Player.transform.position;
        base.SetTarget();
    }
}
