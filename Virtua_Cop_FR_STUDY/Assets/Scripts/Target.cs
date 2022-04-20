using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public GameObject skeleton;
    public AudioSource audioSource;
    public AudioClip deadSound;

    Rigidbody[] rigidbodies;
    Collider[] colliders;
    Animator animator;

    public bool isDead = false;

    public int targetIndex;

    // Start is called before the first frame update
    void Start()
    {
        rigidbodies = skeleton.GetComponentsInChildren<Rigidbody>();
        //colliders = skeleton.GetComponentsInChildren<Collider>();
        animator = GetComponent<Animator>();

        DeactivateRagdoll();
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = Vector3.ProjectOnPlane((Camera.main.transform.position - transform.position), Vector3.up).normalized;
        
    }

    void DeactivateRagdoll()
    {
        
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = true;
        }
        /*
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }*/

        animator.enabled = true;
    }

    public void ActivateRagdoll()
    {

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = false;
        }
        /*
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }*/
        
        animator.enabled = false;
    }

    public void Die(Vector3 hitpoint)
    {
        isDead = true;
        audioSource.PlayOneShot(deadSound);
        ActivateRagdoll();
        CsvWritingManager.targetWritingTime[MovementFlow.stageCount, targetIndex, 2] = CsvWritingManager.time_current.ToString(); // 타겟 별 제거 시간
        TargetCount();

        foreach (var item in Physics.OverlapSphere(hitpoint, 0.5f))
        {
            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb)
                rb.AddExplosionForce(750, hitpoint, 0.5f);
        }
        this.enabled = false;
        Destroy(gameObject, 5f);
    }

    public virtual void TargetCount()
    {

    }
}
