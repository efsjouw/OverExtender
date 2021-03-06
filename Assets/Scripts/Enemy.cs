using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour {

    private float forceStrength = 4;
    private Vector3 forceVector;

    public GameObject pickupPrefab;

    public GameObject headEnemy;

    public List<GameObject> depth;

    public enum EnemyType {
        DropPickup, //Drop a pickup
        Exploder //Explode all enemies
    };
    public EnemyType enemyType;

    private Pickup.PickupType pickupType;
    private System.Random random;

    public Material greenMaterial;
    public Material goldmaterial;
    public Material purpleMaterial;
    public Material redMaterial;
    public Material blueMaterial;
    
    private List<Transform> enabledChildren;
    private bool readyToDespawn = false;

    private Vector3 movePosition;
    private bool doMove;

    // Use this for initialization
    void Awake() {

        random = new System.Random();
        forceStrength = random.Next(1, 5);

        //Fill list of enabled children
        enabledChildren = new List<Transform>();
        enabledChildren.Add(headEnemy.transform);
        enabledChildren.AddRange(enableDepths());

        foreach (Transform child in enabledChildren)
        {
            EnemyCollider enemyCollider = child.gameObject.AddComponent<EnemyCollider>();
            enemyCollider.explosionPrefab = Game.instance.enemyExplosionPrefab;            
        }

        Game.instance.enemySpawned(enabledChildren.Count);
    }

    private void Update()
    {
        if (doMove) transform.Translate(Vector3.forward * Game.instance.getEnemySpeedMultiplier() * Time.deltaTime);
    }

    /// <summary>
    /// Despawn enemy when distance to center becomes to great
    /// </summary>
    /// <returns></returns>
    private IEnumerator despawnRoutine()
    {
        while(gameObject.activeSelf)
        {
            yield return new WaitForSeconds(2);
            float distance = Vector3.Distance(Vector3.zero, transform.position);
            if (distance > 30)
            {
                Destroy(gameObject);
                Game.instance.enemyDestroyed();
            }
        }        
    }

    /// <summary>
    /// Start moving towards the center with z offset
    /// </summary>
    public void setMovePosition(Vector3 movePosition)
    {
        this.movePosition = movePosition;

        float singleStep = 9999; //moveSpeed * Time.deltaTime;
        Vector3 targetDirection = movePosition - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);

        doMove = true;

        //transform.LookAt(new Vector3(0, 0, Random.Range(-14, 14)));
        //forceVector = (forceStrength * Game.instance.getEnemySpeedMultiplier()) * transform.forward;
        //GetComponent<Rigidbody>().AddForce(forceVector, ForceMode.VelocityChange);        
    }

    /// <summary>
    /// Set enemy type determines behaviour
    /// </summary>
    /// <param name="type"></param>
    public void setEnemyType(EnemyType type)
    {
        enemyType = type;
        switch(type)
        {
            case EnemyType.DropPickup:
                Pickup.PickupType enemyPickupType = Pickup.PickupType.Score;
                int doubleChance = random.Next(0, 100);
                if (doubleChance > 75) enemyPickupType = Pickup.PickupType.DoubleScore;
                setPickupEnemy(enemyPickupType);
                break;
            case EnemyType.Exploder:

                int explodeIndex = random.Next(0, enabledChildren.Count);
                for (int i = 0; i < enabledChildren.Count; i++)
                {
                    if (i == explodeIndex)
                    {
                        enabledChildren[i].tag = Game.instance.TAG_ENEMY_PICKUP;
                        enabledChildren[i].GetComponent<Renderer>().material = redMaterial; break;
                    }
                }

                break;
        }

    }

    /// <summary>
    /// Call after enable depths for more than 1 enemy
    /// Set enemy pickup drop type
    /// </summary>
    /// <param name="type"></param>
    public void setPickupEnemy(Pickup.PickupType type)
    {
        pickupType = type;

        //Quickens are a little faster
        if (pickupType == Pickup.PickupType.Quicken) forceStrength += 1;
        int pickUpIndex = random.Next(0, enabledChildren.Count);

        for (int i = 0; i < enabledChildren.Count; i++)
        {
            if (i == pickUpIndex)
            {
                enabledChildren[i].tag = Game.instance.TAG_ENEMY_PICKUP;
                switch (type) {
                    case Pickup.PickupType.Score: enabledChildren[i].GetComponent<Renderer>().material = greenMaterial; break;
                    case Pickup.PickupType.DoubleScore: enabledChildren[i].GetComponent<Renderer>().material = goldmaterial; break;
                    case Pickup.PickupType.Quicken: enabledChildren[i].GetComponent<Renderer>().material = purpleMaterial; break;
                    case Pickup.PickupType.Multiplier: enabledChildren[i].GetComponent<Renderer>().material = blueMaterial; break;                                                         
                }
                break;
            }
        }
    }

    public Transform getEnabledChild(int index)
    {
        var ret = enabledChildren.ElementAtOrDefault(index) ? enabledChildren[index] : null;
        if (ret == null) Debug.Log("wat");
        return enabledChildren.ElementAtOrDefault(index) ? enabledChildren[index] : null;
    }

    public List<Transform> getLeafChildren()
    {
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < depth.Count - 1; i++)
        {
            //If the next depth is not active return children of current depth
            int nextIndex = i + 1;
            if(!depth[nextIndex].activeSelf)
            {
                foreach(Transform child in depth[i].transform)
                {
                    children.Add(child);
                }
            }
        }
        return children;
    }

    /// <summary>
    /// Enable depths/rows of enemies
    /// </summary>
    private List<Transform> enableDepths()
    {
        List<Transform> enabled = new List<Transform>();
        for (int i = 0; i < depth.Count; i++)
        {
            bool previousEnabled = false;

            //Depth will only be enabled if the previous one was enabled
            if (i == 0) previousEnabled = true;
            else previousEnabled = depth[i - 1].gameObject.activeSelf;

            bool enableDepth = random.Next(0, 2) == 1 && previousEnabled;
            depth[i].SetActive(enableDepth);
            if (enableDepth)
            {
                foreach(Transform transform in depth[i].transform)
                {
                    enabled.Add(transform);
                }
            }
        }
        return enabled;
    }

    /// <summary>
    /// Spawn a pickup based on pickup type
    /// </summary>
    /// <param name="tr"></param>
    public void spawnPickup(Transform tr)
    {
        GameObject pickUp = Instantiate(pickupPrefab, tr.position, tr.rotation);
        pickUp.SetActive(true); //in case of disabled
        pickUp.GetComponent<Pickup>().setPickupType(pickupType);

        forceVector = forceStrength / 4 * transform.forward;
        pickUp.GetComponent<Rigidbody>().AddForce(forceVector, ForceMode.VelocityChange);
    }

    /// <summary>
    /// Mark enemy for despawn on finish tag
    /// Destroy the player on trigger enter
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {        
        if (!readyToDespawn && other.tag == "Finish")
        {
            StartCoroutine(despawnRoutine());
        }
        else if (other.tag == "Player")
        {
            other.gameObject.SetActive(false);
            Game.instance.spawnPlayer(3);
        }
    }

}
