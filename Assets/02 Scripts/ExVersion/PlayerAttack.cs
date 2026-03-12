using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    public GameObject bullet;

    //�ӽ�
    public float shootCoolTime;

    bool canShoot;

    private void Start()
    {
        canShoot = true;
    }

    private void Update()
    {
        /*if (Input.GetMouseButton(0) && canShoot && PlayerMove.instance.isMoveable)
        {
            StartCoroutine(Shoot());
        }*/
    }

    IEnumerator Shoot()
    {
        canShoot = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetPos = hit.point;
            targetPos.y = transform.position.y;

            Vector3 shootDir = (targetPos - transform.position).normalized;
            Vector3 attackPos = transform.position + shootDir;

            SoundManager.Instance.PlaySfx(Sound.Player_Attack, 0.3f);
            GameObject projectile = Instantiate(bullet, attackPos, Quaternion.LookRotation(shootDir));

            projectile.GetComponent<Bullet>().Shoot(shootDir);
        }

        yield return new WaitForSeconds(shootCoolTime);
        canShoot = true;
    }
}
