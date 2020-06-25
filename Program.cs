using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
namespace EliteHack
{
    class Program
    {

        #region Нужные переменные
        static Memory mem;
        static int client_dll;
        static bool glow = false;
        static bool bhop = false;
        static bool trigger = false;
        static bool antiflash = false;
        #endregion
        #region Коды клавиш
        static long glowkey = 0x61;
        static long bhopkey = 0x62;
        static long trigkey = 0x63;
        static long trighkey = 0x12;
        static long antiflashkey = 0x64;
        #endregion
        #region Оффсеты
        class Offsets
        {
            public const Int32 health = 0x100;
            public const Int32 LocalPlayer = 0xD3ABEC;
            public const Int32 TeamNum = 0xF4;
            public const Int32 entitylist = 0x4D4F25C;
            public const Int32 glowobjectmanager = 0x5297080;
            public const Int32 glowindex = 0xA438;
            public const Int32 oFlags = 0x104;
            public const Int32 forceJump = 0x51F8EF4;
            public const Int32 forceAttack = 0x31807A8;
            public const Int32 m_iCrosshairId = 0xB3E4;
            public const Int32 m_flFlashDuration = 0xA420;
        }
        #endregion
        static void Main()
        {
            #region Начальное приветствие
            Console.Title = "EliteHack";
            title();
            #endregion
            #region Поиск процесса CS:GO
            try
            {
                Process csgo = Process.GetProcessesByName("csgo")[0];
                mem = new Memory("csgo");

                foreach (ProcessModule module in csgo.Modules)
                {

                    if (module.ModuleName == "client.dll")

                        client_dll = (int)module.BaseAddress;
                }
                UpdateConsole();
                Console.WriteLine("[SUCCESS] Игра найдена! Чит готов к работе!");
            }
            catch
            {
                Console.WriteLine("[ERROR] Странная ошибка! Может не запущена CS:GO? Пакедава :)");
                Console.ReadKey(true);
                Environment.Exit(1);
            }
            #endregion
            #region Запуск потоков чита
            Thread glowthread = new Thread(new ThreadStart(GlowThread));
            glowthread.Start();
            Thread bhopthread = new Thread(new ThreadStart(BhopThread));
            bhopthread.Start();
            Thread triggerthread = new Thread(new ThreadStart(TriggerThread));
            triggerthread.Start();
            Thread antiflashthread = new Thread(new ThreadStart(AntiFlashThread));
            antiflashthread.Start();
            #endregion
            #region Проверка нажатия клавиш(on/off functions)
            while (true)
            {
                if (Convert.ToBoolean(GetAsyncKeyState(glowkey)))
                {
                    switch (glow)
                    {
                        case true:
                            glow = false;
                            break;
                        case false:
                            glow = true;
                            break;
                    }
                    UpdateConsole();
                }
                if (Convert.ToBoolean(GetAsyncKeyState(bhopkey)))
                {
                    switch (bhop)
                    {
                        case true:
                            bhop = false;
                            break;
                        case false:
                            bhop = true;
                            break;
                    }
                    UpdateConsole();
                }
                if (Convert.ToBoolean(GetAsyncKeyState(trigkey)))
                {
                    switch (trigger)
                    {
                        case true:
                            trigger = false;
                            break;
                        case false:
                            trigger = true;
                            break;
                    }
                    UpdateConsole();
                }
                if (Convert.ToBoolean(GetAsyncKeyState(trigkey)))
                {
                    switch (trigger)
                    {
                        case true:
                            trigger = false;
                            break;
                        case false:
                            trigger = true;
                            break;
                    }
                    UpdateConsole();
                }
                if (Convert.ToBoolean(GetAsyncKeyState(antiflashkey)))
                {
                    switch (antiflash)
                    {
                        case true:
                            antiflash = false;
                            break;
                        case false:
                            antiflash = true;
                            break;
                    }
                    UpdateConsole();
                }
                Thread.Sleep(100);

            }
            #endregion
        }
        #region Интерфейс часть консоли
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(long vKey);
        // Заголовок
        private static void title()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("EliteHack");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Red;
        }

        // Обновление информации
        private static void UpdateConsole()
        {
            Console.Clear();
            title();
            Console.WriteLine("---===+++Функционал+++===---");
            Console.WriteLine("1. GLOW [NUMPAD 1]-[TOGGLE] : [" + Convert.ToString(glow) + "]");
            Console.WriteLine("2. BHOP [NUMPAD 2]-[TOGGLE] : [" + Convert.ToString(bhop) + "]");
            Console.WriteLine("3. TRIGGER [NUMPAD 3]-[TOGGLE] [ALT]-[HOLD] : [" + Convert.ToString(trigger) + "]");
            Console.WriteLine("4. ANTIFLASH [NUMPAD 4]-[TOGGLE] : [" + Convert.ToString(antiflash) + "]");
            Console.WriteLine("---===+++===++++===+++===---");
            Console.WriteLine("");
        }
        #endregion
        #region GLOW
        public static void DrawEntityHP(int GlowIndex, float HP)
        {
            int GlowObject = mem.Read<int>(client_dll + Offsets.glowobjectmanager);
            mem.Write(GlowObject + (GlowIndex * 0x38) + 4, 1 - HP / 100f);
            mem.Write(GlowObject + (GlowIndex * 0x38) + 8, HP / 100f);
            mem.Write(GlowObject + (GlowIndex * 0x38) + 12, 0 / 100f);
            mem.Write(GlowObject + (GlowIndex * 0x38) + 0x10, 255 / 100f);
            mem.Write(GlowObject + (GlowIndex * 0x38) + 0x24, true);
            mem.Write(GlowObject + (GlowIndex * 0x38) + 0x25, false);
        }
        public static void GlowThread()
        {
            while (true)
            {
                if(glow)
                {
                    int LocalPlayer = mem.Read<int>(client_dll + Offsets.LocalPlayer);
                    int PlayerTeam = mem.Read<int>(LocalPlayer + Offsets.TeamNum);
                    for (int i = 0; i < 64; i++)
                    {

                        int EntityList = mem.Read<int>(client_dll + Offsets.entitylist + i * 0x10);
                        int EntityTeam = mem.Read<int>(EntityList + Offsets.TeamNum);

                        if (EntityTeam != 0 && EntityTeam != PlayerTeam)
                        {
                            int GlowIndex = mem.Read<int>(EntityList + Offsets.glowindex);
                            float HP = mem.Read<int>(EntityList + Offsets.health);

                            DrawEntityHP(GlowIndex, HP);
                        }
                    }
                    Thread.Sleep(1);
                }
                else { Thread.Sleep(100); }
            }
        }

        #endregion
        #region BHOP
        public static void BhopThread()
        {
            while (true)
            {
                if (bhop)
                {
                    if(Convert.ToBoolean(GetAsyncKeyState(0x20)))
                    {
                        int LocalBase = mem.Read<int>(client_dll + Offsets.LocalPlayer);
                        int flags = mem.Read<int>(LocalBase + Offsets.oFlags);
                        if (flags == 257)
                        {
                            mem.Write<int>(client_dll + Offsets.forceJump, 6);
                        }
                    }
                }
                else { Thread.Sleep(100); }
            }
        }
        #endregion
        #region TriggerBot
        public static void TriggerThread()
        {
            while (true)
            {
                if (trigger)
                {
                    int LocalBase = mem.Read<int>(client_dll + Offsets.LocalPlayer);
                    int crid = mem.Read<int>(LocalBase + Offsets.m_iCrosshairId);
                    if (crid > 64)
                        continue;
                    if (Convert.ToBoolean(GetAsyncKeyState(trighkey)) && crid > 0)
                    {
                        int entityBase = mem.Read<int>(client_dll + Offsets.entitylist + (crid - 1) * 0x10);
                        int entityTeam = mem.Read<int>(entityBase + Offsets.TeamNum);
                        int LocalPlayer = mem.Read<int>(client_dll + Offsets.LocalPlayer);
                        int PlayerTeam = mem.Read<int>(LocalPlayer + Offsets.TeamNum);
                        if (entityTeam != 0 && entityTeam != PlayerTeam)
                        {
                            mem.Write<int>(client_dll + Offsets.forceAttack, 6);
                        }
                        Thread.Sleep(1);
                    }

                }
                else { Thread.Sleep(100); }
            }
        }
        #endregion
        #region AntiFlash
        public static void AntiFlashThread()
        {
            while(true)
            {
                if(antiflash)
                {
                    int LocalPlayer = mem.Read<int>(client_dll + Offsets.LocalPlayer);
                    int flash = mem.Read<int>(LocalPlayer + Offsets.m_flFlashDuration);
                    if(flash > 0)
                    {
                        mem.Write<int>(LocalPlayer + Offsets.m_flFlashDuration, 0);
                    } else { Thread.Sleep(100); }
                }
                else { Thread.Sleep(100); }
            }
        }
        #endregion
    }
}
