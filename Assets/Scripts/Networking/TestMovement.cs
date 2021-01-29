using UnityEngine;

namespace GGJ2021.Networking
{
    [System.Serializable]
    public class TestNetworkData : NetworkData
    {
        public string colorHex;
    }

    public class TestMovement : MonoBehaviour
    {
        [SerializeField] private float movementSpeed;
        [SerializeField] private float rotationSpeed;

        private NetworkEntity networkEntity;
        private TestNetworkData objectNetworkData;

        void Awake()
        {
            objectNetworkData = new TestNetworkData();
            networkEntity = GetComponent<NetworkEntity>();
            networkEntity.SubscribeToLocalInstanceUpdate(UpdateLocalInstance);
            networkEntity.SubscribeToLocalDataRetrieval(GetLocalData);
        }

        void Update()
        {
            if (!networkEntity.ClientIsOwner()) return;

            float verticalAxis = Input.GetAxis("Vertical");
            float horizontalAxis = Input.GetAxis("Horizontal");

            if (verticalAxis != 0)
                transform.Translate(Vector3.forward * verticalAxis * Time.deltaTime * movementSpeed);
            if (horizontalAxis != 0)
                transform.Rotate(Vector3.up * horizontalAxis * Time.deltaTime * rotationSpeed);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Color newColor = new Color(
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f)
                );
                GetComponent<Renderer>().material.color = newColor;
            }

            SetObjectNetworkData();
        }

        private void SetObjectNetworkData()
        {
            Color color = GetComponent<Renderer>().material.color;
            objectNetworkData.colorHex = ColorUtility.ToHtmlStringRGB(color);
        }

        private void UpdateLocalInstance(NetworkData receivedData)
        {
            TestNetworkData data = (TestNetworkData)receivedData;
            Color receivedColor;
            if (!ColorUtility.TryParseHtmlString("#" + data.colorHex, out receivedColor))
                throw new System.FormatException("The color html string is incorrectly formatted");
            GetComponent<Renderer>().material.color = receivedColor;
        }

        private NetworkData GetLocalData()
        {
            return objectNetworkData;
        }
    }
}
