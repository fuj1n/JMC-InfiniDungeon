using UnityEngine;
using UnityStandardAssets.Utility;

public class ClickMouseRotator : SimpleMouseRotator
{
    protected override void Update()
    {
        if (Input.GetMouseButton(0))
            base.Update();
    }
}
