using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Solid : EnvironmentBehavior
{
    public SolidTypes SolidType { get; protected set; }
}
