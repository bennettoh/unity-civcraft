using UnityEngine;
using UnityEngine.UI;

public class InteractiveUI : MonoBehaviour
{
    [SerializeField] GameObject BuildButton;
    [SerializeField] Text textRef;

    // Update is called once per frame
    void Update()
    {
        bool shouldMove = GameManager.Instance.intent == "move";
        bool shouldBuild = GameManager.Instance.intent == "build";

        if (shouldMove || shouldBuild)
        {
            textRef.text = shouldMove ? "Build" : "Move";
            setVisibility(true);
        }
        else
        {
            setVisibility(false);
        }
    }

    private void setVisibility(bool visible)
    {
        if (visible)
        {
            BuildButton.SetActive(true);
        }
        else
        {
            BuildButton.SetActive(false);
        }
    }
}
