using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class TileSocketListener : MonoBehaviour
{

    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket;
private Collider socketCollider;

void Awake()
{
    socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
    socket.selectEntered.AddListener(OnSocketed);
    socket.selectExited.AddListener(OnUnsocketed);

    socketCollider = GetComponent<Collider>();
    if (socketCollider == null)
    {
        Debug.LogWarning("No Collider found on the socket GameObject.");
    }
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

    string socketCoordName = transform.parent != null ? transform.parent.name : gameObject.name;          
    string insertedObjCoordName = insertedObj.name;     

    TilesManage.Instance.IncrementSocketUsage(socketObj);
        Debug.Log($"{socketCoordName},{insertedObj}");
        TilesManage.Instance._Connections.Add(TilesManage.Instance.GetOrderedPair(socketCoordName, insertedObj.name));/////////////////////

    //Debug.Log($"Socket {socketCoordName} now socketed with {insertedObjCoordName}");

        // Enable trigger
        if (socketCollider != null)
            socketCollider.isTrigger = true;

    TilesManage.Instance.IncrementSocketUsage(socketObj);

    // Turn off trigger again 
        if (socketCollider != null)
            socketCollider.isTrigger = false;
}

    private void OnUnsocketed(SelectExitEventArgs args)
    {
        GameObject socketObj = gameObject;
        GameObject removedObj = args.interactableObject.transform.gameObject;

        string socketCoordName = transform.parent != null ? transform.parent.name : gameObject.name;
        string insertedObjCoordName = removedObj.name;



        TilesManage.Instance.DecrementSocketUsage(socketObj);
        Debug.Log($"{socketObj.name} unsocketed from {removedObj.name}");
        
        TilesManage.Instance._Connections.Remove(TilesManage.Instance.GetOrderedPair(socketCoordName, removedObj.name));
}
}

