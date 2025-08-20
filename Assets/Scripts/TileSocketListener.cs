
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TileSocketListener : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket;

    void Awake()
    {
        socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
        socket.selectEntered.AddListener(OnSocketed);
        socket.selectExited.AddListener(OnUnsocketed);
    }

    void OnDestroy()
    {
        socket.selectEntered.RemoveListener(OnSocketed);
        socket.selectExited.RemoveListener(OnUnsocketed);
    }

    private void OnSocketed(SelectEnterEventArgs args)
    {
        GameObject socketObj = gameObject;
        GameObject insertedObj = args.interactableObject.transform.gameObject;

        TilesManage.Instance.IncrementSocketUsage(socketObj);
        Debug.Log($"{socketObj.name} now socketed with {insertedObj.name}");
    }

    private void OnUnsocketed(SelectExitEventArgs args)
    {
        GameObject socketObj = gameObject;
        GameObject removedObj = args.interactableObject.transform.gameObject;

        TilesManage.Instance.DecrementSocketUsage(socketObj);
        Debug.Log($"{socketObj.name} unsocketed from {removedObj.name}");
    }
}

