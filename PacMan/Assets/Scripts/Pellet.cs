public class Pellet : Collectable
{
    public override void HandleOnCollect()
    {
        GameController.Instance.PelletCount++; // increase pelletcollected count on contact
    }
}
