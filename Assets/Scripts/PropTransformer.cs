using System;
using UnityEngine;
using UnityEngine.Events;

public class PropTransformer : MonoBehaviour
{
    [SerializeField] private PropDataList propDataList = default;
    [SerializeField] private LayerMask raycastMask = default;
    [SerializeField] private float rayDistance = 5;
    [SerializeField] private float rayRadius = 1f;
    [SerializeField] private MeshRenderer radarEffect = default;
    [SerializeField, Range(1, 10)] private float transformTimer = 5f;
    [SerializeField] private Animator witch = default;
    [SerializeField] private Animator phantom = default;

    private float lastTransformTime = 0;

    private Material radarMaterial = default;

    private GameObject propForm = default;
    private MeshFilter meshFilter = default;
    private MeshRenderer meshRenderer = default;
    

    private RaycastHit hit = default;

    public RaycastHit Hit => hit;
    
    private MeshCollider meshCollider = default;
    private CapsuleCollider capsuleCollider = default;

    [HideInInspector] public UnityEvent<string> OnPropTransform = new UnityEvent<string>();

    private void Awake()
    {
        radarMaterial = radarEffect.material;
        radarMaterial.SetFloat("_Opacity", 0f);
        radarEffect.gameObject.SetActive(false);
        
        capsuleCollider = GetComponent<CapsuleCollider>();
        
        meshCollider = GetComponent<MeshCollider>();
        meshCollider.convex = true;
        meshCollider.enabled = false;
        
        propForm = new GameObject("Prop Form");
        propForm.transform.SetParent(transform);
        propForm.transform.localPosition = Vector3.zero;
        propForm.transform.rotation = transform.rotation;
        propForm.transform.localScale = Vector3.one * .5f;

        propForm.AddComponent<MeshRenderer>();
        meshRenderer = propForm.GetComponent<MeshRenderer>();
        propForm.AddComponent<MeshFilter>();
        meshFilter = propForm.GetComponent<MeshFilter>();
        
        witch.gameObject.SetActive(false);
        phantom.gameObject.SetActive(false);
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

    public bool CheckCanTransform()
    {
        return Physics.SphereCast(transform.position, rayRadius, transform.forward, out hit, rayDistance, raycastMask);
    }

    public void TransformTo(GameObject to)
    {
        ChangeToProp(to.name);
        OnPropTransform?.Invoke(to.name);
    }

    public void BackToOriginalForm()
    {
        ChangeToProp("TRUE FORM");
        OnPropTransform?.Invoke("TRUE FORM");
    }

    public void ChangeToProp(string propName)
    {
        if(propName != null && propForm && propForm.activeSelf && propForm.name.Contains(propName)) return;
        
        if (propName.Contains("TRUE FORM"))
        {
            int state = GetComponent<PlayerDataConection>().GetState();
            
            switch (state)
            {
                case 1:
                    witch.gameObject.SetActive(true);
                    break;
                case 2:
                    phantom.gameObject.SetActive(true);
                    break;
            }

            meshCollider.enabled = false;
            
            capsuleCollider.enabled = true;

            propForm.name = propName;
            
            propForm.SetActive(false);
        
            radarEffect.gameObject.SetActive(false);
        }
        else
        {
            var propData = propDataList.GetPropDataByName(propName);
        
            meshFilter.mesh = propData.mesh;
            meshRenderer.material = propData.material;

            meshCollider.enabled = true;
            meshCollider.sharedMesh = meshFilter.mesh;


            lastTransformTime = Time.time;
            radarMaterial.SetFloat("_Opacity", 0f);
            radarEffect.gameObject.SetActive(true);
        
            propForm.SetActive(true);
            propForm.name = propName;
            
            witch.gameObject.SetActive(false);
            phantom.gameObject.SetActive(false);
            capsuleCollider.enabled = false;
        }
    }

#if UNITY_EDITOR
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
#endif
}
