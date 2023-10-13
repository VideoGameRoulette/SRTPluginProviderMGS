﻿using SRTPluginBase;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace SRTExampleProvider32
{
    public class SRTPluginProviderMGS : IPluginProvider
    {
        private Process process;
        private GameMemoryExampleScanner gameMemoryScanner;
        private IPluginHostDelegates hostDelegates;
        public IPluginInfo Info => new PluginInfo();
        public bool GameRunning
        {
            get
            {
                if (gameMemoryScanner != null && !gameMemoryScanner.ProcessRunning)
                {
                    process = GetProcess();
                    if (process != null)
                        gameMemoryScanner.Initialize(process); // Re-initialize and attempt to continue.
                }

                return gameMemoryScanner != null && gameMemoryScanner.ProcessRunning;
            }
        }

        public int Startup(IPluginHostDelegates hostDelegates)
        {
            this.hostDelegates = hostDelegates;
            process = GetProcess();
            gameMemoryScanner = new GameMemoryExampleScanner(process);
            return 0;
        }

        public int Shutdown()
        {
            gameMemoryScanner?.Dispose();
            gameMemoryScanner = null;
            return 0;
        }

        public object PullData()
        {
            try
            {
                if (!GameRunning) // Not running? Bail out!
                        return null;

                return gameMemoryScanner.Refresh();
            }
            catch (Win32Exception ex)
            {
                if ((ProcessMemory.Win32Error)ex.NativeErrorCode != ProcessMemory.Win32Error.ERROR_PARTIAL_COPY)
                    hostDelegates.ExceptionMessage(ex);// Only show the error if its not ERROR_PARTIAL_COPY. ERROR_PARTIAL_COPY is typically an issue with reading as the program exits or reading right as the pointers are changing (i.e. switching back to main menu).
            }
            catch (Exception ex)
            {
                hostDelegates.ExceptionMessage(ex);
            }

            return null;
        }

        private Process GetProcess() => Process.GetProcessesByName("mgsi.exe")?.FirstOrDefault();
    }
}
