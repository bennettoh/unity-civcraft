using UnityEngine;
using UnityEngine.UI;

public class DisplayResource : MonoBehaviour
{
    [SerializeField] Text textRef;

    private void Update()
    {
        textRef.text = "Resource: " + GameManager.Instance.resource;
    }
}
