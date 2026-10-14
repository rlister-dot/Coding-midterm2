using UnityEngine;
using Yarn.Unity;

public class huh : MonoBehaviour
{
    public _GameController GameController;
    public void Awake()
    {
        // ... your existing singleton logic ...

        // Register the function to Yarn Spinner
        DialogueRunner dRun = GameObject.FindAnyObjectByType<DialogueRunner>();
        if (dRun != null)
        {
            dRun.Dialogue.Library.RegisterFunction("has_one_coin", () => GameController.currentCoins == 1);
        }
    }

}
