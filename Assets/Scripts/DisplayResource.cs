using UnityEngine;
using UnityEngine.UI;

public class DisplayResource : MonoBehaviour
{
    [SerializeField] Text textRef;

    private void Update()
    {
        if (textRef.tag == "White")
        {
            textRef.text = "White resource: " + GameManager.Instance.whiteResource;
        }
        else
        {
            textRef.text = "Black resource: " + GameManager.Instance.blackResource;
        }
    }
}
