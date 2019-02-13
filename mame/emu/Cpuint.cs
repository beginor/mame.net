using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace mame
{
    [StructLayout(LayoutKind.Explicit)]
    [Serializable()]
    public struct Register
    {
        [FieldOffset(0)]
        public uint d;
        [FieldOffset(0)]
        public int sd;

        [FieldOffset(0)]
        public ushort LowWord;
        [FieldOffset(2)]
        public ushort HighWord;

        [FieldOffset(0)]
        public byte LowByte;
        [FieldOffset(1)]
        public byte HighByte;
        [FieldOffset(2)]
        public byte HighByte2;
        [FieldOffset(3)]
        public byte HighByte3;

        public override string ToString()
        {
            return String.Format("{0:X8}", d);
        }
    }
    public enum LineState
    {
        /* line states */
        CLEAR_LINE = 0,				/* clear (a fired, held or pulsed) line */
        ASSERT_LINE,				/* assert an interrupt immediately */
        HOLD_LINE,					/* hold interrupt line until acknowledged */
        PULSE_LINE,					/* pulse interrupt line for one instruction */
        INTERNAL_CLEAR_LINE = 100 + CLEAR_LINE,
        INTERNAL_ASSERT_LINE = 100 + ASSERT_LINE,
        MAX_INPUT_LINES = 32 + 3,
        INPUT_LINE_NMI = MAX_INPUT_LINES - 3,
        INPUT_LINE_RESET = MAX_INPUT_LINES - 2,
        INPUT_LINE_HALT = MAX_INPUT_LINES - 1,
    }
    public class irq
    {
        public int cpunum;
        public int line;
        public LineState state;
        public Atime time;
        public irq()
        {

        }
        public irq(int i1, int i2, LineState s1, Atime t1)
        {
            cpunum = i1;
            line = i2;
            state = s1;
            time = t1;
        }
    }
    public class Cpuint
    {
        public static void cps1_irq_handler_mus(int irq)
        {
            cpunum_set_input_line(1, 0, (irq != 0) ? LineState.ASSERT_LINE : LineState.CLEAR_LINE);
        }
        public static void namcos1_sound_interrupt(int irq)
        {
            cpunum_set_input_line(2, 1, (irq != 0) ? LineState.ASSERT_LINE : LineState.CLEAR_LINE);
        }
        public static void cpunum_set_input_line(int cpunum, int line, LineState state)
        {
            lirq.Add(new irq(cpunum, line, state, Timer.get_current_time()));
            Cpuexec.cpu[cpunum].set_input_line_and_vector(line, state, 0);
        }
        public static List<irq> lirq=new List<irq>();
        public static int[,] input_event_index = new int[8, 35],input_line_state=new int[8,35];
        public static int[, ,] input_state = new int[8, 35, 32];
        public static void cpunum_empty_event_queue()
        {
            List<irq> lsirq = new List<irq>();
            foreach(irq irq1 in lirq)
            {
                if (Attotime.attotime_compare(irq1.time, Timer.global_basetime) <= 0)
                {
                    if (irq1.line == (int)LineState.INPUT_LINE_RESET)
                    {
                        if (irq1.state == LineState.ASSERT_LINE)
                        {
                            Cpuexec.cpunum_suspend(irq1.cpunum, Cpuexec.SUSPEND_REASON_RESET, 1);
                        }
                        else
                        {
                            if ((irq1.state == LineState.CLEAR_LINE && Cpuexec.cpunum_is_suspended(irq1.cpunum, Cpuexec.SUSPEND_REASON_RESET)) || irq1.state == LineState.PULSE_LINE)
                            {
                                Cpuexec.cpu[irq1.cpunum].Reset();
                            }
                            Cpuexec.cpunum_resume(irq1.cpunum, Cpuexec.SUSPEND_REASON_RESET);
                        }
                    }
                    else if (irq1.line == (int)LineState.INPUT_LINE_HALT)
                    {
                        if (irq1.state == LineState.ASSERT_LINE)
                            Cpuexec.cpunum_suspend(irq1.cpunum, Cpuexec.SUSPEND_REASON_HALT, 1);
                        else if (irq1.state == LineState.CLEAR_LINE)
                            Cpuexec.cpunum_resume(irq1.cpunum, Cpuexec.SUSPEND_REASON_HALT);
                    }
                    else
                    {
                        switch (irq1.state)
                        {
                            case LineState.PULSE_LINE:
                                Cpuexec.cpu[irq1.cpunum].set_irq_line(irq1.line, LineState.ASSERT_LINE);
                                Cpuexec.cpu[irq1.cpunum].set_irq_line(irq1.line, LineState.CLEAR_LINE);
                                break;
                            case LineState.HOLD_LINE:
                            case LineState.ASSERT_LINE:
                                Cpuexec.cpu[irq1.cpunum].set_irq_line(irq1.line, LineState.ASSERT_LINE);
                                break;
                            case LineState.CLEAR_LINE:
                                Cpuexec.cpu[irq1.cpunum].set_irq_line(irq1.line, LineState.CLEAR_LINE);
                                break;
                        }

                    }                    
                    lsirq.Add(irq1);
                }
            }
            foreach (irq irq1 in lsirq)
            {
                lirq.Remove(irq1);
            }
        }
        public static void SaveStateBinary(BinaryWriter writer)
        {
            int i, n;
            n = lirq.Count;
            writer.Write(n);
            for (i = 0; i < n; i++)
            {
                writer.Write(lirq[i].cpunum);
                writer.Write(lirq[i].line);
                writer.Write((int)lirq[i].state);
                writer.Write(lirq[i].time.seconds);
                writer.Write(lirq[i].time.attoseconds);
            }
            for (i = n; i < 16; i++)
            {
                writer.Write(0);
                writer.Write(0);
                writer.Write(0);
                writer.Write(0);
                writer.Write((long)0);
            }
        }
        public static void LoadStateBinary(BinaryReader reader)
        {
            int i, n;
            n = reader.ReadInt32();
            lirq = new List<irq>();
            for (i = 0; i < n; i++)
            {
                lirq.Add(new irq());
                lirq[i].cpunum = reader.ReadInt32();
                lirq[i].line = reader.ReadInt32();
                lirq[i].state = (LineState)reader.ReadInt32();
                lirq[i].time.seconds = reader.ReadInt32();
                lirq[i].time.attoseconds = reader.ReadInt64();
            }
            for (i = n; i < 16; i++)
            {
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt64();
            }
        }
    }
}
