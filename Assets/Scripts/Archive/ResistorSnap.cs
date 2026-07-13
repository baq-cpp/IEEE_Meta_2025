using UnityEngine;

public class ResistorSnap : MonoBehaviour
{

    private Transform _SnapTarget;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BreadboardPin"))
        {
            _SnapTarget = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("breadboardPin"))
        {
            if (_SnapTarget == other.transform)
                _SnapTarget = null;
        }
    }

    public void OnRelease()//set on "onselectexit"
    {
        if (_SnapTarget != null)
        {
            transform.position = _SnapTarget.position;
            transform.rotation = _SnapTarget.rotation;
            //transfrom.SetParent(_SnapTarget);// option parent set to breadboard
        }
    }
    
}
