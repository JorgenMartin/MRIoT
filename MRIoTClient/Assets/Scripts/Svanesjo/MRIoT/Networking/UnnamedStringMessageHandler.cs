using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Svanesjo.MRIoT.Networking
{
    /// <summary>
    /// Using an unnamed message to send a string message
    /// <see cref="CustomUnnamedMessageHandler<T>"/> defined
    /// further down below.
    /// </summary>
    public class UnnamedStringMessageHandler : CustomUnnamedMessageHandler<string>
    {
        [Required, SerializeField] private TextMesh receiveText;
        [Required, SerializeField] private TextMesh sendText;
        [Required, SerializeField] private MeshRenderer background;
        [Required, SerializeField] private Material serverMaterial;
        [Required, SerializeField] private Material clientMaterial;
        
        /// <summary>
        /// We override this method to define the unique message type
        /// identifier for this child derived class
        /// </summary>
        protected override byte MessageType()
        {
            // As an example, we can define message type of 1 for string messages
            return 1;
        }

        public override void OnNetworkSpawn()
        {
            // For this example, we always want to invoke the base
            base.OnNetworkSpawn();

            if (IsServer)
            {
                // Server broadcasts to all clients when a new client connects
                // (just for example purposes)
                NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
                background.SetMaterials(new List<Material> { serverMaterial });
            }
            else
            {
                // Clients send a greeting string message to the server
                SendUnnamedMessage("I am a client connecting!");
                background.SetMaterials(new List<Material> { clientMaterial });
            }
        }

        public override void OnNetworkDespawn()
        {
            // For this example, we always want to invoke the base
            base.OnNetworkDespawn();

            // Whether server or not, unregister this.
            NetworkManager.OnClientDisconnectCallback -= OnClientConnectedCallback;
        }

        private void OnClientConnectedCallback(ulong clientId)
        {
            // Server broadcasts a welcome string message to all clients that
            // a new client has joined.
            SendUnnamedMessage($"Everyone welcome the newly joined client ({clientId})!");
        }

        /// <summary>
        /// For this example, we override this message to handle receiving the string
        /// message.
        /// </summary>
        protected override void OnReceivedUnnamedMessage(ulong clientId, FastBufferReader reader)
        {
            reader.ReadValueSafe(out string stringMessage);
            if (IsServer)
            {
                var msg = ($"Server received unnamed message of type ({MessageType()}) from client " +
                          $"({clientId}) that contained the string: \"{stringMessage}\"");
                receiveText.text = msg;
                Debug.Log(msg);

                // As an example, we can also broadcast the client message to everyone
                SendUnnamedMessage($"Newly connected client sent this greeting: \"{stringMessage}\"");
            }
            else
            {
                receiveText.text = stringMessage;
                Debug.Log(stringMessage);
            }
        }

        /// <summary>
        /// For this example, we will send a string as the payload.
        ///
        /// IMPORTANT NOTE: You can construct your own header to be
        /// written for custom message types, this example just uses
        /// the message type value as the "header".  This provides us
        /// with the ability to have "different types" of unnamed
        /// messages.
        /// </summary>
        public override void SendUnnamedMessage(string dataToSend)
        {
            var writer = new FastBufferWriter(1100, Allocator.Temp);
            var customMessagingManager = NetworkManager.CustomMessagingManager;
            // Tip: Placing the writer within a using scope assures it will
            // be disposed upon leaving the using scope
            using (writer)
            {
                // Write our message type
                writer.WriteValueSafe(MessageType());

                // Write our string message
                writer.WriteValueSafe(dataToSend);
                if (IsServer)
                {
                    sendText.text = writer.ToString();
                    // This is a server-only method that will broadcast the unnamed message.
                    // Caution: Invoking this method on a client will throw an exception!
                    customMessagingManager.SendUnnamedMessageToAll(writer);
                }
                else
                {
                    sendText.text = writer.ToString();
                    // This method can be used by a client or server (client to server or server to client)
                    customMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, writer);
                }
            }
        }
    }

    /// <summary>
    /// A templated class to handle sending different data types
    /// per unique unnamed message type/child derived class.
    /// </summary>
    public class CustomUnnamedMessageHandler<T> : NetworkBehaviour
    {
        /// <summary>
        /// Since there is no unique way to identify unnamed messages,
        /// adding a message type identifier to the message itself is
        /// one way to handle know:
        /// "what kind of unnamed message was received?"
        /// </summary>
        protected virtual byte MessageType()
        {
            // The default unnamed message type
            return 0;
        }

        /// <summary>
        /// For most cases, you want to register once your NetworkBehaviour's
        /// NetworkObject (typically in-scene placed) is spawned.
        /// </summary>
        public override void OnNetworkSpawn()
        {
            // Both the server-host and client(s) will always subscribe to the
            // the unnamed message received event
            NetworkManager.CustomMessagingManager.OnUnnamedMessage += ReceiveMessage;
        }

        public override void OnNetworkDespawn()
        {
            // Unsubscribe when the associated NetworkObject is despawned.
            NetworkManager.CustomMessagingManager.OnUnnamedMessage -= ReceiveMessage;
        }

        /// <summary>
        /// This method needs to be overridden to handle reading a unique message type
        /// (that is, derived class)
        /// </summary>
        protected virtual void OnReceivedUnnamedMessage(ulong clientId, FastBufferReader reader)
        {
        }

        /// <summary>
        /// For this unnamed message example, we always read the message type
        /// value to determine if it should be handled by this instance in the
        ///  event it's a child of the CustomUnnamedMessageHandler class.
        /// </summary>
        private void ReceiveMessage(ulong clientId, FastBufferReader reader)
        {
            // Read the message type value that is written first when we send
            // this unnamed message.
            reader.ReadValueSafe(out byte messageType);
            // Example purposes only, you might handle this in a more optimal way
            if (messageType == MessageType())
            {
                OnReceivedUnnamedMessage(clientId, reader);
            }
        }

        /// <summary>
        /// For simplicity, the default does nothing
        /// </summary>
        /// <param name="dataToSend"></param>
        public virtual void SendUnnamedMessage(T dataToSend)
        {

        }
    }
}