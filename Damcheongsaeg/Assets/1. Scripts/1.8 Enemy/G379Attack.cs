using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G379Attack : MonoBehaviour
{
    [SerializeField] private GameObject Player;

    [SerializeField] private float _attackDamageAmount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageble iDamageble = collision.gameObject.GetComponent<IDamageble>();

        if (iDamageble != null && collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("enemyÀÇ °ø°Ý");
            Vector2 hitDirection = collision.gameObject.transform.position - transform.position;
            hitDirection.Normalize();
            iDamageble.Damage(_attackDamageAmount, hitDirection);
        }
    }
}
