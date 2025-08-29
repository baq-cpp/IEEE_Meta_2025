using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class TileSocketListener : MonoBehaviour
{

    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket;
private Collider socketCollider;

    private TextHintManager _hintManager;


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

        _hintManager = FindFirstObjectByType<TextHintManager>();
        if (_hintManager == null)
            Debug.LogError("TextHintManager not found in scene!");
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
    (int, int) socketCoordParsed = Component2.ParseOrderedPair(socketCoordName);
        TilesManage.Instance._Connections.Add(TilesManage.Instance.GetOrderedPair(socketCoordName, insertedObj.name));/////////////////////
        Breadboard.Instance.PlaceComponent(socketCoordParsed.Item1,socketCoordParsed.Item2, new Component2(insertedObj,socketCoordParsed.Item1,socketCoordParsed.Item2,Breadboard.Instance));
        _hintManager.OnComponentInteracted(insertedObj.name); //This interacts with TextHintManager to push component into stack

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
        (int, int) socketCoordParsed = Component2.ParseOrderedPair(socketCoordName);
        // Breadboard.Instance.UnplaceAt(socketCoordParsed.Item1,socketCoordParsed.Item2);
        


        TilesManage.Instance.DecrementSocketUsage(socketObj);
        //Debug.Log($"{socketObj.name} unsocketed from {removedObj.name}");
        
        TilesManage.Instance._Connections.Remove(TilesManage.Instance.GetOrderedPair(socketCoordName, removedObj.name));
}
}

