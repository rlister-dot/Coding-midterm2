using StarterAssets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class _GameController : MonoBehaviour
{
    //FIX SPAWN POINT!!!!!!!!
    //Change Coingot method to work


    public static _GameController instance { get; private set; }
    public updateCoins updateUI;
    public CoinsUI coinInventory;
    public int currentCoins;

    public GameObject ThirdPersonRig;

    public Transform mainStartingTransform;
    public Vector3 loadPos;
    public Quaternion loadRot;
    public List<string> coinsCollected = new List<string>();

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
            DontDestroyOnLoad(this);
        }
    }
    //Potential Spawn fix 
    public void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        
        GameObject spawnPointObject = GameObject.FindWithTag("SceneSpawnPoint");
        if (spawnPointObject != null)
        {
            mainStartingTransform = spawnPointObject.transform;
        }

       
        if (loadPos == Vector3.zero)
        {
           
            if (mainStartingTransform != null)
            {
                SpawnThirdPersonPrefab(mainStartingTransform.position, mainStartingTransform.rotation);
            }
            else
            {
              
                SpawnThirdPersonPrefab(Vector3.zero, Quaternion.identity);
            }
        }
        else
        {
           
            SpawnThirdPersonPrefab(loadPos, loadRot);
        }

       
        loadPos = Vector3.zero;
        loadRot = Quaternion.identity;

        loadCoins();

        if (scene.buildIndex == 1)
        {
            DialogueRunner dRun = GameObject.FindAnyObjectByType<DialogueRunner>();
            if (dRun != null)
            {
                dRun.onDialogueStart.AddListener(() => setPlayerMovement(true));
                dRun.onDialogueComplete.AddListener(() => setPlayerMovement(false));
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
        if (coinsCollected == null) coinsCollected = new List<string>();
        
        if (!coinsCollected.Contains(coinID))
        {
            coinsCollected.Add(coinID);
            currentCoins++;
            
            
            
            updateUI.Invoke(currentCoins);
        }
    }

    
    public void loadCoinUI(CoinsUI ui)
    {
        coinInventory = ui;
        
        if (coinInventory != null)
        {
           
            updateUI.RemoveListener(coinInventory.addCoins);
            updateUI.AddListener(coinInventory.addCoins);
        }
    }


    public void loadCoins()
    {
        
        _CoinCollect[] allCoinsInScene = FindObjectsByType<_CoinCollect>(FindObjectsSortMode.None);

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

   

    public void setPlayerMovement(bool b)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        ThirdPersonController tpc = player.GetComponent<ThirdPersonController>();
        if (tpc != null)
        {
            tpc.enabled = !b; 
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
}

[System.Serializable]
public class updateCoins : UnityEvent<int> { }
