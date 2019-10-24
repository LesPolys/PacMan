using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinky : Ghost
{
    public override void SetTarget()
    {
        m_ChaseTarget = GameController.Instance.Player.transform.position;
        base.SetTarget();
    }
}
