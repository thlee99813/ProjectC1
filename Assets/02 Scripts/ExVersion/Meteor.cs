using UnityEngine;

public class Meteor : MonoBehaviour
{
    [SerializeField] private float lifeTime = 3f;
    private void OnEnable()
    {
        Invoke(nameof(DestroyObject), lifeTime);
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }
}
