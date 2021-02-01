using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GGJ2021.Networking
{
    public delegate NetworkData GetNetworkData();
    public delegate void UpdateLocalInstance(NetworkData receivedData);

    [Serializable]
    public class NetworkData
    {
        #region Serialization
        public static byte[] Serialize(NetworkData instanceToSerialize)
        {
            BinaryFormatter binForm = new BinaryFormatter();
            using (MemoryStream memStream = new MemoryStream())
            {
                binForm.Serialize(memStream, instanceToSerialize);
                return memStream.ToArray();
            }
        }

        public static NetworkData DeSerialize(byte[] serializedData)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binForm = new BinaryFormatter();
                memStream.Write(serializedData, 0, serializedData.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                NetworkData data = (NetworkData)binForm.Deserialize(memStream);
                return data;
            }
        }
        #endregion
    }
}
