using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Nokobot/Modern Guns/Simple Shoot")]
public class Practice_SimpleShoot : MonoBehaviour
{
    [Header("Prefab Refrences")]
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;

    public AudioSource audioSource;
    public AudioClip gunSound;

    [Header("Location Refrences")]
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private Camera barrelLocation;
    [SerializeField] private Transform casingExitLocation;

    [Header("Settings")]
    [Tooltip("Specify time to destory the casing object")] [SerializeField] private float destroyTimer = 2f;
    //[Tooltip("Bullet Speed")] [SerializeField] private float shotPower = 500f;
    [Tooltip("Casing Ejection Speed")] [SerializeField] private float ejectPower = 150f;

    Ray ray;
    RaycastHit hitinfo;


    void Start()
    {
        if (gunAnimator == null)
            gunAnimator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        ray = barrelLocation.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
    }

    public void Fire()
    {
        gunAnimator.SetTrigger("Fire");
    }


    //This function creates the bullet behavior
    void Shoot()
    {
        audioSource.PlayOneShot(gunSound);

        if (muzzleFlashPrefab)
        {
            //Create the muzzle flash
            GameObject tempFlash;
            tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.transform.position, barrelLocation.transform.rotation);

            //Destroy the muzzle flash effect
            Destroy(tempFlash, destroyTimer);
        }

        // Create a bullet and add force on it in direction of the barrel
        //Instantiate(bulletPrefab, barrelLocation.transform.position, barrelLocation.transform.rotation).GetComponent<Rigidbody>().AddForce(barrelLocation.transform.forward * shotPower);


        if (Physics.Raycast(barrelLocation.transform.position, barrelLocation.transform.forward, out hitinfo, 50f))
        {
            Practice_Target target = hitinfo.transform.GetComponentInParent<Practice_Target>();


            if (target != null)
            {
                if (target.isDead == false)
                {

                    target.Die(hitinfo.point);

                }

            }

            Practice_StartTarget startTarget = hitinfo.transform.GetComponent<Practice_StartTarget>();
            if (startTarget != null)
            {
                startTarget.Die();
            }
        }



        //cancels if there's no bullet prefeb
        if (!bulletPrefab)
        { return; }
    }

    //This function creates a casing at the ejection slot
    void CasingRelease()
    {
        //Cancels function if ejection slot hasn't been set or there's no casing
        if (!casingExitLocation || !casingPrefab)
        { return; }

        //Create the casing
        GameObject tempCasing;
        tempCasing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation) as GameObject;
        //Add force on casing to push it out
        tempCasing.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(ejectPower * 0.7f, ejectPower), (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
        //Add torque to make casing spin in random direction
        tempCasing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);

        //Destroy casing after X seconds
        Destroy(tempCasing, destroyTimer);
    }

}