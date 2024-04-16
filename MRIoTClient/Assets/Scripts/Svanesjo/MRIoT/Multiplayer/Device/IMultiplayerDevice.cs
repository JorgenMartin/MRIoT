#nullable enable

using Svanesjo.MRIoT.Multiplayer.Representation;

namespace Svanesjo.MRIoT.Multiplayer.Device
{
    public interface IMultiplayerDevice
    {
        public void SetMultiplayerRepresentation(IMultiplayerRepresentation multiplayerRepresentation);
    }
}
