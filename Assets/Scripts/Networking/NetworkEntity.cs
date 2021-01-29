using Photon.Pun;
using System;
using UnityEngine;

namespace GGJ2021.Networking
{
    [RequireComponent(typeof(PhotonView))]
    public class NetworkEntity : MonoBehaviourPun, IPunObservable
    {
        private GetNetworkData getNetworkData;
        private UpdateLocalInstance updateLocalInstance;

        // IPunObservable implementation
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this entity, send the others our data
                if (getNetworkData == null) return;
                object serializedData = NetworkData.Serialize(getNetworkData());
                stream.SendNext(serializedData);
            }
            else
            {
                // Network player, receive data
                if (updateLocalInstance == null) return;
                NetworkData deSerializedData = NetworkData.DeSerialize((byte[])stream.ReceiveNext());
                updateLocalInstance(deSerializedData);
            }
        }

        public void Init(Action localEntityInitialization)
        {
            if (photonView.IsMine)
                localEntityInitialization?.Invoke();
        }

        public bool ClientIsOwner()
        {
            return photonView.IsMine;
        }

        /// <summary>
        /// Subscribe a function to an event which occurs when the client sends the local entity data to the server.
        /// It is vital to keep the subscribed function as lightweight as possible.
        /// </summary>
        public void SubscribeToLocalDataRetrieval(GetNetworkData callback)
        {
            getNetworkData = callback;
        }

        /// <summary>
        /// Subscribe a function to an event which occurs when data is received from a remote instance of this entity.
        /// </summary>
        public void SubscribeToLocalInstanceUpdate(UpdateLocalInstance callback)
        {
            updateLocalInstance = callback;
        }
    }
}
