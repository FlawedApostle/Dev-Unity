using System.Collections;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    private Rigidbody rb;
    public float recoveryDelay = 2f;
    private bool isRagdoll = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        FreezeUpright();
    }

    public void EnterRagdoll()
    {
        if (isRagdoll) return;
        isRagdoll = true;
        rb.constraints = RigidbodyConstraints.None;
        StartCoroutine(Recover());
    }

    private IEnumerator Recover()
    {
        yield return new WaitForSeconds(recoveryDelay);
        FreezeUpright();
        isRagdoll = false;
    }

    private void FreezeUpright()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }
}
