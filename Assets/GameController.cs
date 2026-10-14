using StarterAssets;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class _GameController : MonoBehaviour
{
    public bool hasPhone = false;
    public static _GameController instance { get; private set; }

    public updateCoins updateUI;
    public CoinsUI coinInventory;
    public int currentCoins;
    public GameObject ThirdPersonRig;
    public Transform mainStartingTransform;

    public Vector3 loadPos;
    public Quaternion loadRot;
    public List<string> coinsCollected;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            SceneManager.sceneLoaded += OnSceneLoad;
        }
        DontDestroyOnLoad(this);

        // REGISTRATION SCRIPT: Tells Yarn Spinner that "player_has_phone()" exists
        DialogueRunner dRun = GameObject.FindAnyObjectByType<DialogueRunner>();
        if (dRun != null)
        {
            dRun.Dialogue.Library.RegisterFunction("player_has_phone", () => hasPhone);
        }
    }



    public void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            if (loadPos == Vector3.zero)
            {
                SpawnThirdPersonPrefab(mainStartingTransform.position, mainStartingTransform.rotation);
            }
            else
            {
                SpawnThirdPersonPrefab(loadPos, loadRot);
            }
            loadCoins();
        }
        else if (scene.buildIndex == 1)
        {
            DialogueRunner targetRunner = GameObject.FindAnyObjectByType<DialogueRunner>();

            // Fallback string-check if assembly definition isolates the script types
            if (targetRunner == null)
            {
                foreach (MonoBehaviour mono in GameObject.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
                {
                    if (mono.GetType().Name == "MoreDialoguePers")
                    {
                        targetRunner = mono.GetComponent<DialogueRunner>();
                        break;
                    }
                }
            }

            if (targetRunner != null)
            {
                targetRunner.onDialogueStart.RemoveAllListeners();
                targetRunner.onDialogueComplete.RemoveAllListeners();

                targetRunner.onDialogueStart.AddListener(() => setPlayerMovement(true));
                targetRunner.onDialogueComplete.AddListener(() => setPlayerMovement(false));
            }
        }
    }

    public void SpawnThirdPersonPrefab(Vector3 pos, Quaternion rot)
    {
        Instantiate(ThirdPersonRig, pos, rot);
    }

    public void loadLocationData(Vector3 pos, Quaternion rot)
    {
        loadPos = pos;
        loadRot = rot;
    }

    public void coinGot(string coinID)
    {
        coinsCollected.Add(coinID);
        currentCoins++;
        updateUI.AddListener(coinInventory.addCoins);
        updateUI.Invoke(currentCoins);
    }

    public void loadCoins()
    {
        _CoinCollect[] allCoinsInScene = FindObjectsByType<_CoinCollect>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        if (coinsCollected != null)
        {
            foreach (_CoinCollect coin in allCoinsInScene)
            {
                if (coinsCollected.Contains(coin.coinID))
                {
                    Destroy(coin.gameObject);
                }
            }
        }
    }

    public void loadCoinUI(CoinsUI ui)
    {
        coinInventory = ui;
    }

    // Controls player freeze state cleanly without duplicate signatures
    public void setPlayerMovement(bool freezePlayer)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        ThirdPersonController tpc = player.GetComponent<ThirdPersonController>();
        if (tpc != null)
        {
            tpc.enabled = !freezePlayer;
        }
    }

    public void movePlayerOnNPC(Transform newPos)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = newPos.position;
        }
    }

    [YarnCommand("set_int")]
    public void SetInterger(int interger)
    {
        if (currentCoins == 1)
        {
            interger = 1;
        }
    }

    [System.Serializable]
    public class updateCoins : UnityEvent<int> { }
}
