using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Pellet : Collectable
{
    public override void HandleOnCollect()
    {
        GameController.Instance.PelletCount++; // increase pelletcollected count on contact
    }
}
