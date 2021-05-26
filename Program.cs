using System;
using System.Timers;
using SRTPluginProviderRE8;
using SRTPluginProviderRE8.Structs;
using SRTHost;

namespace RE8HPViewer
{
    class Program
    {
        static SRTPluginProviderRE8.SRTPluginProviderRE8 provider;
        static PluginHostDelegates hostDelegates;
        static Timer timer;
        static float[] lastEnemyHPs;
        static double[] dmgScaleTable;
        static bool showAllHPs;

        // Equality check between the new HP list and the last record.
        private static bool HaveSameHPs(EnemyHP[] newHPs)
        {
            for (int i = 0; i < newHPs.Length; i++)
            {
                float lastHP = lastEnemyHPs[i];
                float newHP = newHPs[i].CurrentHP;
                if (lastHP != newHP)
                    return false;
            }
            return true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            object rawData = provider.PullData();
            if (rawData != null)
            {
                IGameMemoryRE8 data = (IGameMemoryRE8)rawData;
                EnemyHP[] enemyHPs = data.EnemyHealth;
                if (HaveSameHPs(enemyHPs)) return; // do not update
                Console.Clear();
                double dmgScale = dmgScaleTable[data.Rank];
                Console.WriteLine(string.Format("DA: {0}({1})  dmgScale: {2:F3}", data.Rank, data.RankScore, dmgScale));
                Console.WriteLine("   normDiff  currentHP      maxHP");
                for (int i = 0; i < enemyHPs.Length; i++)
                {
                    EnemyHP enemyHP = enemyHPs[i];
                    if (showAllHPs ? enemyHP.CurrentHP != 0 && !enemyHP.IsNaN : enemyHP.IsAlive)
                    {
                        float diffHP = lastEnemyHPs[i] - enemyHP.CurrentHP;
                        if (diffHP < 0) diffHP = 0;
                        string message = string.Format("[{0,9:F2}] {1,9:F2} / {2,9:F2}", diffHP / dmgScale, enemyHP.CurrentHP, enemyHP.MaximumHP);
                        Console.WriteLine(message);
                    }
                    lastEnemyHPs[i] = enemyHPs[i].CurrentHP; // update lastEnemyHPs anyway
                }
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Select one option then press enter (default 1):");
            Console.WriteLine("1. Only show hurt HPs.");
            Console.WriteLine("2. Show all HPs (including dead enemies).");
            Console.Write("Your selection: ");
            string rawSelection = Console.ReadLine();
            if (rawSelection == "1") showAllHPs = false;
            else if (rawSelection == "2") showAllHPs = true;
            else showAllHPs = false;
            // init
            Console.WriteLine("Init");
            lastEnemyHPs = new float[64];
            for (int i = 0; i < lastEnemyHPs.Length; i++)
            {
                lastEnemyHPs[i] = 0;
            }
            dmgScaleTable = new double[] { 1, 0.85, 0.75, 0.6, 0.5, 0.45, 0.425, 0.375, 0.325, 0.2, 0.2, 0.2, 0.2 };
            provider = new SRTPluginProviderRE8.SRTPluginProviderRE8();
            hostDelegates = new PluginHostDelegates();
            provider.Startup(hostDelegates);
            Console.WriteLine("Start");

            // main loop
            timer = new Timer(500);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
            Console.ReadLine();
            timer.Stop();
            timer.Dispose();

            provider.Shutdown();
        }
    }
}
