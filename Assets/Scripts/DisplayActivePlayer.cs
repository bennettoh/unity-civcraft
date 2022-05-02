using UnityEngine;
using UnityEngine.UI;

public class DisplayActivePlayer : MonoBehaviour
{
    [SerializeField] Text textRef;

    private void Update()
    {
        if (GameManager.Instance.isWhiteTurn)
        {
            textRef.text = "White's turn";
        }
        else
        {
            textRef.text = "Black's turn";
        }
    }
}
