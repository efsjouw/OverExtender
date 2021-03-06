using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(MeshRenderer))]
public class EnemyCollider : MonoBehaviour
{
    Rigidbody rigidBody;
    BoxCollider boxCollider;
    MeshRenderer meshRenderer;

    public GameObject explosionPrefab;

    [SerializeField] float explosionLifeTime = 0.2f;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        rigidBody.useGravity = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch(other.gameObject.tag)
        {
            case "Player":
                Game.instance.killPlayer();
                break;
            case "Explosion":
                boxCollider.enabled = false;

                if (transform.gameObject.tag == Game.instance.TAG_ENEMY_PICKUP)
                    transform.gameObject.GetComponentInParent<Enemy>().spawnPickup(transform);

                //behaviour.StartCoroutine(destroyEffect(transform));
                destroyByExplosion();

                break;
        }
    }

    public void destroyByExplosion()
    {
        StartCoroutine(explosionRoutine());
    }

    private IEnumerator explosionRoutine()
    {
        meshRenderer.enabled = false;

        yield return new WaitForSeconds(Random.Range(0.1f,0.2f));

        GameObject explosion = Instantiate(explosionPrefab);
        explosion.transform.position = this.transform.position;

        yield return new WaitForSeconds(explosionLifeTime);
        
        Destroy(explosion);
        Destroy(this);
        Game.instance.enemyDestroyed();
    }   
}
