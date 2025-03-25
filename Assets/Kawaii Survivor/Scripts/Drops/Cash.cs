using System;
using System.Collections;
using UnityEngine;

public class Cash : DroppableCurrency
{
    [Header("Actions")]
    public static Action<Cash> onCollected;
    protected override void Collected()
    {
        onCollected?.Invoke(this);
    }
}
