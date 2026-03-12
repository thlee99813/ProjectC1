using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody rigid;
    [SerializeField] private int bulletDamage;
    [SerializeField] private float shootSpeed = 5;
    [SerializeField] private float bulletLifeTime = 3f;
    private float Timer;

    private void Update()
    {
        Timer += Time.deltaTime;
        if (Timer > bulletLifeTime)
        {
            Destroy(gameObject);
        }
    }

    public void Shoot(Vector3 direct)
    {
        rigid.AddForce(direct * shootSpeed, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
           // other.gameObject.GetComponent<Enemy>().TakeDamage(bulletDamage);
        }
        Debug.Log(other.gameObject);

        if (!other.gameObject.CompareTag("Item"))
            Destroy(gameObject);
    }


}
