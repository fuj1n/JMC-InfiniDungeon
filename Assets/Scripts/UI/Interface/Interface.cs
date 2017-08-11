using System;
using UnityEngine;

public sealed class Interface : MonoBehaviour
{
    public string id;

    public InterfaceController.Side side;

    public Container container;
    public string findContainer;

    public void Open()
    {
        if (!string.IsNullOrEmpty(findContainer))
        {
            container = Type.GetType(findContainer).GetConstructor(new Type[] { }).Invoke(new object[] { }) as Container;
        }

        if (container != null)
            container.Open(this);
    }

    public void Close()
    {
        if (container != null)
            container.Close();
    }
}