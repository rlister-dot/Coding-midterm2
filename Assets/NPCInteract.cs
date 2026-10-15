using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

public class NPC_Interact : MonoBehaviour
{
    //Need to find a way to detect dialogue runner... it not working
    //computer/phone collection


    public TextMeshProUGUI promtText;
    public string interactText;
    public bool canInteract;

    private DialogueRunner npcDialogue;
    public npcData storyData;

    public void Start()
    {
        FindLivingDialogueRunner();
    }

    private void FindLivingDialogueRunner()
    {
        npcDialogue = Object.FindFirstObjectByType<DialogueRunner>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            setText(interactText);
            canInteract = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            setText("");
            canInteract = false;
        }
    }

    public void setText(string txt)
    {
        if (promtText != null) promtText.text = txt;
    }

    void Update()
    {
        if (canInteract)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                FindLivingDialogueRunner();

                if (npcDialogue == null)
                {
                    Debug.LogError("NPC_Interact: Critical Error! No living DialogueRunner found in the scene.");
                    return;
                }

                if (!npcDialogue.IsDialogueRunning)
                {
                    Debug.Log("Start NPC Dialogue from node: " + storyData.startingNode);
                    setText("");
                    npcDialogue.StartDialogue(storyData.startingNode);
                    canInteract = false;
                }
            }
        }
    }

    
    [YarnCommand("setPhase")]
    public void setStoryPhase()
    {
        if (_GameController.instance != null && _GameController.instance.currentCoins >= 1)
        {
            storyData.currentPhase = npcData.dialogPhase.quest_completed;
        }

        InMemoryVariableStorage vStore = Object.FindFirstObjectByType<InMemoryVariableStorage>();
        if (vStore != null)
        {
            vStore.SetValue("$dialogPhase", storyData.currentPhase.ToString());
        }
    }


    [YarnCommand("getPhase")]
    public void getStoryPhase(string phase)
    {
        if (phase == "comeback")
        {
            storyData.currentPhase = npcData.dialogPhase.comeback;
        }
        else if (phase == "quest_completed")
        {
            storyData.currentPhase = npcData.dialogPhase.quest_completed;
        }
        else
        {
            Debug.LogError("The string you passed in the Yarn Script does not match any current dialogPhase in the npcData scriptableObject");
        }

        if (_GameController.instance != null && _GameController.instance.currentCoins == 1)
        {
            storyData.currentPhase = npcData.dialogPhase.quest_completed;

            InMemoryVariableStorage vStore = Object.FindFirstObjectByType<InMemoryVariableStorage>();
            if (vStore != null) vStore.SetValue("$dialogPhase", "quest_completed");
        }
    }

    public void OnApplicationQuit()
    {
        if (storyData != null) storyData.dataReset();
    }
}
