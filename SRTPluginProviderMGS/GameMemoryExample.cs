using SRTExampleProvider32.Structs.GameStructs;
using System.Diagnostics;
using System.Reflection;

namespace SRTExampleProvider32
{
    public class GameMemoryExample : IGameMemoryExample
    {
        public string GameName => "Metal Gear Solid (PC)";
        public string VersionInfo => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

        public short Alerts { get => _alerts; set => _alerts = value; }
        internal short _alerts;
    }
}
