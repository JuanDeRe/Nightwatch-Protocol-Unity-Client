using System;

namespace Nightwatch.Client.Contracts
{
    [Serializable]
    public class ChangeCameraMessage
    {
        public string type = "ChangeCameraCommand";
        public string matchId;
        public string cameraId;
    }
}