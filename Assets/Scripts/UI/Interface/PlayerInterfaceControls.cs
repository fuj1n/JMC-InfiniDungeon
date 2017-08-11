using UnityEngine;

public class PlayerInterfaceControls : MonoBehaviour {
    public Interface interfaceInventory;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            InterfaceController.GetInstance().OpenInterface(interfaceInventory);
    }
}
