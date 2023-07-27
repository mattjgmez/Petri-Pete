using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gas : EnvironmentBehavior
{
    public GasTypes GasType { get; protected set; }
}
