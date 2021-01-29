using UnityEngine;

public class PropTransformer : MonoBehaviour
{
    [SerializeField] private LayerMask raycastMask = default;
    [SerializeField] private float rayDistance = 5;
    [SerializeField] private float rayRadius = .5f;

    private GameObject propForm = default;
    private MeshFilter meshFilter = default;
    private MeshRenderer meshRenderer = default;
    private MeshCollider meshCollider = default;

    private RaycastHit hit = default;

    private MeshRenderer ownMeshRenderer = default;
    private CapsuleCollider capsuleCollider = default;

    private void Start()
    {
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
            if (Input.GetMouseButtonDown(0))
            {
                TransformTo(hit.transform.gameObject);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            BackToOriginalForm();
        }
    }

    void TransformTo(GameObject to)
    {
        meshFilter.mesh = to.GetComponent<MeshFilter>().mesh;
        meshCollider.sharedMesh = meshFilter.mesh;
        meshRenderer.material = to.GetComponent<MeshRenderer>().material;

        propForm.SetActive(true);

        ownMeshRenderer.enabled = false;
        capsuleCollider.enabled = false;
    }

    void BackToOriginalForm()
    {
        ownMeshRenderer.enabled = true;
        capsuleCollider.enabled = true;
        
        propForm.SetActive(false);
    }
}
