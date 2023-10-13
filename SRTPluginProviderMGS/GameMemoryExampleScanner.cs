using ProcessMemory;
using static ProcessMemory.Extensions;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SRTExampleProvider32.Structs;
using System.Text;
using SRTExampleProvider32.Structs.GameStructs;

namespace SRTExampleProvider32
{
    internal class GameMemoryExampleScanner : IDisposable
    {
        /// <summary>
        /// READ ONLY VARIABLES
        /// </summary>
        private static readonly int MAX_ENEMIES = 64; // USE FOR ARRAYS OF ENEMIES MAX COUNT
        private static readonly int MAX_ITEMS = 24; // USE FOR ARRAYS OF ITEMS MAX COUNT

        /// <summary>
        /// VARIABLES
        /// </summary>
        private ProcessMemoryHandler memoryAccess;
        private GameMemoryExample gameMemoryValues;
        public bool HasScanned;
        public bool ProcessRunning => memoryAccess != null && memoryAccess.ProcessRunning;
        public int ProcessExitCode => (memoryAccess != null) ? memoryAccess.ProcessExitCode : 0;


        /// <summary>
        /// POINTER ADDRESS VARIABLES
        /// </summary>
        private int pGameTimer;
        private int pLocation;
        private int pProgress;
        private int pDifficulty;
        private int pCurrentHP;
        private int pMaxHP;
        private int pAlerts;
        private int pKills;
        private int pRations;
        private int pContinues;
        private int pSaves;
        private int pElevatorTimer;

        /// <summary>
        /// POINTER VARIABLES
        /// </summary>
        private IntPtr BaseAddress { get; set; }
        private MultilevelPointer PointerGameTimer { get; set; }
        private MultilevelPointer PointerLocation { get; set; }
        private MultilevelPointer PointerProgress { get; set; }
        private MultilevelPointer PointerDifficulty { get; set; }
        private MultilevelPointer PointerCurrentHP { get; set; }
        private MultilevelPointer PointerMaxHP { get; set; }
        private MultilevelPointer PointerAlerts { get; set; }
        private MultilevelPointer PointerKills { get; set; }
        private MultilevelPointer PointerRations { get; set; }
        private MultilevelPointer PointerContinues { get; set; }
        private MultilevelPointer PointerSaves { get; set; }
        private MultilevelPointer PointerElevatorTimer { get; set; }

        /// <summary>
        /// CLASS CONTRUCTOR
        /// </summary>
        /// <param name="proc"></param>
        internal GameMemoryExampleScanner(Process process = null)
        {
            gameMemoryValues = new GameMemoryExample();
            if (process != null)
                Initialize(process);
        }

        internal void Initialize(Process process)
        {
            if (process == null)
                return; // Do not continue if this is null.

            SelectPointerAddresses(GameHashes.DetectVersion(process.MainModule.FileName));

            int pid = GetProcessId(process).Value;
            memoryAccess = new ProcessMemoryHandler(pid);
            if (ProcessRunning)
            {
                BaseAddress = NativeWrappers.GetProcessBaseAddress(pid, PInvoke.ListModules.LIST_MODULES_64BIT); // Bypass .NET's managed solution for getting this and attempt to get this info ourselves via PInvoke since some users are getting 299 PARTIAL COPY when they seemingly shouldn't.
                PointerAlerts = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pAlerts));
            }
        }

        private void SelectPointerAddresses(GameVersion version)
        {
            if (version == GameVersion.PCRedditPatched)
            {
                pGameTimer = 0x595344;
                pLocation = 0x2504CE;
                pProgress = 0x38D7CA;
                pDifficulty = 0x38E7E2;
                pCurrentHP = 0x38E7F6;
                pMaxHP = 0x38E7F8;
                pAlerts = 0x38E87C;
                pKills = 0x38E87E;
                pRations = 0x38E88C;
                pContinues = 0x38E88E;
                pSaves = 0x38E890;
                pElevatorTimer = 0x4F56AC;
            }
            else if (version == GameVersion.UNKNOWN)
            {
                pGameTimer = 0x595344;
                pLocation = 0x2504CE;
                pProgress = 0x38D7CA;
                pDifficulty = 0x38E7E2;
                pCurrentHP = 0x38E7F6;
                pMaxHP = 0x38E7F8;
                pAlerts = 0x38E87C;
                pKills = 0x38E87E;
                pRations = 0x38E88C;
                pContinues = 0x38E88E;
                pSaves = 0x38E890;
                pElevatorTimer = 0x4F56AC;
            }
        }

        internal void UpdatePointers()
        {
            PointerAlerts.UpdatePointers();
        }

        internal IGameMemoryExample Refresh()
        {

            // Example With MultiLevelPointer
            gameMemoryValues._alerts = PointerAlerts.DerefShort(0x0);

            HasScanned = true;
            return gameMemoryValues;
        }

        private int? GetProcessId(Process process) => process?.Id;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (memoryAccess != null)
                        memoryAccess.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~REmake1Memory() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}