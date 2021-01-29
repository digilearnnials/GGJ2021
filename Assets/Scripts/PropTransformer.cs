using System;
using UnityEngine;

public class PropTransformer : MonoBehaviour
{
    [SerializeField] private LayerMask raycastMask = default;
    [SerializeField] private float rayDistance = 5;
    [SerializeField] private float rayRadius = 1f;
    [SerializeField] private MeshRenderer radarEffect = default;
    [SerializeField, Range(1, 10)] private float transformTimer = 5f;

    private float lastTransformTime = 0;

    private Material radarMaterial = default;

    private GameObject propForm = default;
    private MeshFilter meshFilter = default;
    private MeshRenderer meshRenderer = default;
    private MeshCollider meshCollider = default;

    private RaycastHit hit = default;

    private MeshRenderer ownMeshRenderer = default;
    private CapsuleCollider capsuleCollider = default;

    private void Start()
    {
        radarMaterial = radarEffect.material;
        radarMaterial.SetFloat("_Opacity", 0f);
        radarEffect.gameObject.SetActive(true);
        
        ownMeshRenderer = GetComponent<MeshRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        
        propForm = new GameObject("Prop Form");
        propForm.transform.SetParent(transform);
        propForm.transform.localPosition = Vector3.zero;
        propForm.transform.rotation = transform.rotation;

        propForm.AddComponent<MeshRenderer>();
        meshRenderer = propForm.GetComponent<MeshRenderer>();
        propForm.AddComponent<MeshFilter>();
        meshFilter = propForm.GetComponent<MeshFilter>();

        propForm.AddComponent<MeshCollider>();
        meshCollider = propForm.GetComponent<MeshCollider>();
        meshCollider.convex = true;
    }

    private void FixedUpdate()
    {
        if(Physics.SphereCast(transform.position, rayRadius, transform.forward, out hit, rayDistance, raycastMask))
        {
            if (Input.GetMouseButton(0))
            {
                TransformTo(hit.transform.gameObject);
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            BackToOriginalForm();
        }
    }

    private void LateUpdate()
    {
        if (propForm.activeSelf)
        {
            if (Time.time - lastTransformTime > transformTimer)
            {
                radarMaterial.SetFloat("_Opacity", 1f);
            }
        }
    }

    void TransformTo(GameObject to)
    {
        meshFilter.mesh = to.GetComponent<MeshFilter>().mesh;
        meshCollider.sharedMesh = meshFilter.mesh;
        meshRenderer.material = to.GetComponent<MeshRenderer>().material;

        lastTransformTime = Time.time;
        radarMaterial.SetFloat("_Opacity", 0f);
        radarEffect.gameObject.SetActive(true);
        
        propForm.SetActive(true);

        ownMeshRenderer.enabled = false;
        capsuleCollider.enabled = false;
    }

    void BackToOriginalForm()
    {
        ownMeshRenderer.enabled = true;
        capsuleCollider.enabled = true;
        
        propForm.SetActive(false);
        
        radarEffect.gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        if (hit.transform)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hit.point, rayRadius);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position + transform.forward * rayDistance, rayRadius);
        }
        
        Gizmos.DrawRay(transform.position, transform.forward * rayDistance);
    }
}
