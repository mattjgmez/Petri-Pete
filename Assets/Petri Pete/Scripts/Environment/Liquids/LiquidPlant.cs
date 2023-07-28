using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidPlant : Liquid
{
    protected override void Initialization()
    {
        LiquidType = LiquidTypes.Plant;
    }
}
