using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mame
{
    public class Generic
    {
        private static uint[] coin_count;
        private static uint[] coinlockedout;
        private static uint[] lastcoin;
        public static byte[] generic_nvram;
        public static void generic_machine_init()
        {
            int counternum;
            coin_count = new uint[8];
            coinlockedout = new uint[8];
            lastcoin = new uint[8];
            for (counternum = 0; counternum < 8; counternum++)
            {
                lastcoin[counternum] = 0;
                coinlockedout[counternum] = 0;
            }
        }
        public static void coin_counter_w(int num, int on)
        {
            if (num >= 8)
            {
                return;
            }
            if (on != 0 && (lastcoin[num] == 0))
            {
                coin_count[num]++;
            }
            lastcoin[num] = (uint)on;
        }
        public static void coin_lockout_w(int num, int on)
        {
            if (num >= 8)
            {
                return;
            }
            coinlockedout[num] =(uint) on;
        }
        public static void coin_lockout_global_w(int on)
        {
            int i;
            for (i = 0; i < 8; i++)
            {
                coin_lockout_w(i, on);
            }
        }
        public static void nvram_load()
        {
            switch (Machine.sBoard)
            {
                case "Neo Geo":
                    Neogeo.nvram_handler_load_neogeo();
                    break;
            }
        }
        public static void nvram_save()
        {
            switch (Machine.sBoard)
            {
                case "Neo Geo":
                    Neogeo.nvram_handler_save_neogeo();
                    break;
            }
        }
        public static void irq_1_0_line_hold()
        {
            Cpuint.cpunum_set_input_line(1, 0, LineState.HOLD_LINE);
        }
        public static void watchdog_reset_w()
        {
            Watchdog.watchdog_reset();
        }
    }
}