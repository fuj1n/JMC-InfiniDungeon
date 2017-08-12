using UnityEngine;

public class PlayerInterfaceControls : MonoBehaviour
{
    public Interface interfaceInventory;
    public Interface interfaceStats;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            InterfaceController.GetInstance().OpenInterface(interfaceInventory);
        else if (Input.GetKeyDown(KeyCode.J))
            InterfaceController.GetInstance().OpenInterface(interfaceStats);
    }
}
