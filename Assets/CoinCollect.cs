using UnityEngine;
using Yarn.Unity;

public class _CoinCollect : MonoBehaviour
    //fix yarnspinner error w/ quest phases 
    //fix data persistence for collection
{
    public string coinID;

    public void Awake()
    {
        coinID = this.name + "_" + transform.position.ToString();
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collider Name: " + other.gameObject.name);
        if (other.gameObject.CompareTag("Player")) 
        {
            collectedCoin();
        }
    }

    public void collectedCoin()
    {
        _GameController.instance.coinGot(coinID);

       
        NPC_Interact npcScript = Object.FindFirstObjectByType<NPC_Interact>();
        if (npcScript != null && npcScript.storyData != null)
        {
           //CHECK THIS PART AFTER PLAYTEST
            npcScript.storyData.currentPhase = npcData.dialogPhase.quest_completed;
            Debug.Log("ScriptableObject updated: currentPhase is now quest_completed");
        }

        var storage = Object.FindFirstObjectByType<InMemoryVariableStorage>();
        if (storage != null)
        {
            storage.SetValue("$dialogPhase", "quest_completed");
        }

        if (GetComponent<MeshRenderer>() != null) GetComponent<MeshRenderer>().enabled = false;
        if (GetComponent<CapsuleCollider>() != null) GetComponent<CapsuleCollider>().enabled = false;
        if (GetComponent<SphereCollider>() != null) GetComponent<SphereCollider>().enabled = false;
    }

}
