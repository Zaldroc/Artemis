using Artemis.DAL;
using Artemis.Settings;
using Artemis.Utilities;
using Artemis.Utilities.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemoryTest
{
    class Program
    {
        private Memory _memory;
        private GamePointersCollection _pointer;
        IntPtr boostAddress;
        IntPtr speedAddress;
        public List<string> ProcessNames { get; protected set; } = new List<string>();

        static void Main(string[] args)
        {
            Program pro = new Program();
            pro.Enable();
            if (pro.ReadAddresses())
            {
                Thread t = new Thread(() =>
                {
                    while (true)
                        SendKeys.SendWait("X");
                });
                t.Start();

                while (true)
                {
                    pro.Update();
                    Thread.Sleep(16);
                }
            }
            Console.ReadLine();
        }

        public Program()
        {
            ProcessNames.Add("RocketLeague");
        }

        public void Enable()
        {
            Updater.GetPointers();
            _pointer = SettingsProvider.Load<OffsetSettings>().RocketLeague;
        }

        public bool ReadAddresses()
        {
            if (_memory == null)
            {
                var tempProcess = MemoryHelpers.GetProcessIfRunning(ProcessNames[0]);
                if (tempProcess == null)
                    return false;

                _memory = new Memory(tempProcess);
            }

            if (_memory == null)
                return false;
            
            var boostOffsets = _pointer.GameAddresses.First(ga => ga.Description == "Boost").ToString();
            boostAddress = _memory.GetAddress("\"RocketLeague.exe\"" + boostOffsets);

            var speedOffsets = _pointer.GameAddresses.First(ga => ga.Description == "Speed").ToString();
            speedAddress = _memory.GetAddress("\"RocketLeague.exe\"" + speedOffsets);

            return true;
        }

        public void Update()
        {
            var boostInt = (int)(_memory.ReadFloat(boostAddress) * 100);
            if (boostInt > 100)
                boostInt = 100;
            if (boostInt < 0)
                boostInt = 0;
            
            var speedInt = (int)_memory.ReadFloat(speedAddress);

            Console.Clear();
            Console.WriteLine("Boost: " + boostInt);
            Console.WriteLine("Speed: " + speedInt);
                  
        }
    }
}
