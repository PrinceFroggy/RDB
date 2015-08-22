using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RDB
{
    public partial class Form1 : Form
    {
        #region API

        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr oCallback);

        #endregion

        #region GLOBAL STATIC && PRIVATE VARIABLES

        static RuneScapeLibrary.Search.NameSearch list;
        static RuneScapeLibrary.Search.HiscoreSearch stats;
        static Thread Fetch;

        static bool VerifiedListLock = false;

        private Thread Names;
        private List<string> BANNEDBITCHES;

        private
            string
            user = "user",
            overall = "               ??                      ??                                ??",
            attack = "               ??                      ??                                ??",
            defence = "               ??                      ??                                ??",
            strength = "               ??                      ??                                ??",
            hitpoints = "               ??                      ??                                ??",
            ranged = "               ??                      ??                                ??",
            prayer = "               ??                      ??                                ??",
            magic = "               ??                      ??                                ??",
            cooking = "               ??                      ??                                ??",
            woodcutting = "               ??                      ??                                ??",
            flecthing = "               ??                      ??                                ??",
            fishing = "               ??                      ??                                ??",
            firemaking = "               ??                      ??                                ??",
            crafting = "               ??                      ??                                ??",
            smithing = "               ??                      ??                                ??",
            mining = "               ??                      ??                                ??",
            herblore = "               ??                      ??                                ??",
            agility = "               ??                      ??                                ??",
            thieving = "               ??                      ??                                ??",
            slayer = "               ??                      ??                                ??",
            farming = "               ??                      ??                                ??",
            runecraft = "               ??                      ??                                ??",
            hunter = "               ??                      ??                                ??",
            construction = "               ??                      ??                                ??",
            banned = "unknown";

        #endregion

        #region Form1->WndProc->Form-movement

        private const int WM_NCHITTEST = 0x84;
        private const int HT_CLIENT = 0x1;
        private const int HT_CAPTION = 0x2;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_NCHITTEST && m.Result.ToInt32() == HT_CLIENT)
            {
                m.Result = (IntPtr)HT_CAPTION;
            }
        }

        #endregion

        #region Form1->Sound->Form-sound

        static string name, sound;
        static string DIRname = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + "RDB" + "\\";

        private void Create_Sound()
        {
            if (!Directory.Exists(DIRname))
            {
                Directory.CreateDirectory(DIRname);
            }

            if (!Properties.Settings.Default.check)
            {
                name = DIRname + "sound";
                File.WriteAllBytes(name + ".mp3", Properties.Resources.Old_RuneScape_Soundtrack__Harmony);
                Properties.Settings.Default.sound = name + ".mp3";

                Properties.Settings.Default.check = true;
                Properties.Settings.Default.Save();
            }
        }

        private void Play_Sound()
        {
            sound = "open \"" + Properties.Settings.Default.sound + "\" type mpegvideo alias MediaFile";
            mciSendString(sound, null, 0, IntPtr.Zero);

            sound = "play MediaFile";
            sound += " REPEAT";
            mciSendString(sound, null, 0, IntPtr.Zero);
        }

        #endregion

        #region List->Generation & Logging->Username-database-clean

        static void GenerateList()
        {
            list = new RuneScapeLibrary.Search.NameSearch();
            
            try
            {
                TextWriter file = new StreamWriter("cNames.txt");

                foreach (string name in list)
                {
                    file.WriteLine(name);
                }

                file.Close();

                VerifiedListLock = true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region List->Generation & Logging->Username-database-banned

        private void REMOVE_DE_BOTS()
        {
            BANNEDBITCHES = new List<string>();

            foreach (string name in list)
            {
                stats = new RuneScapeLibrary.Search.HiscoreSearch(name);

                user = name;
                overall = "               " + stats.SkillList[0].SkillRank.ToString() + "                      " + stats.SkillList[0].SkillLevel + "                      " + stats.SkillList[0].SkillExperience;
                attack = "               " + stats.SkillList[1].SkillRank.ToString() + "                      " + stats.SkillList[1].SkillLevel + "                      " + stats.SkillList[1].SkillExperience;
                defence = "               " + stats.SkillList[2].SkillRank.ToString() + "                      " + stats.SkillList[2].SkillLevel + "                      " + stats.SkillList[2].SkillExperience;
                strength = "               " + stats.SkillList[3].SkillRank.ToString() + "                      " + stats.SkillList[3].SkillLevel + "                      " + stats.SkillList[3].SkillExperience;
                hitpoints = "               " + stats.SkillList[4].SkillRank.ToString() + "                      " + stats.SkillList[4].SkillLevel + "                      " + stats.SkillList[4].SkillExperience;
                ranged = "               " + stats.SkillList[5].SkillRank.ToString() + "                      " + stats.SkillList[5].SkillLevel + "                      " + stats.SkillList[5].SkillExperience;
                prayer = "               " + stats.SkillList[6].SkillRank.ToString() + "                      " + stats.SkillList[6].SkillLevel + "                      " + stats.SkillList[6].SkillExperience;
                magic = "               " + stats.SkillList[7].SkillRank.ToString() + "                      " + stats.SkillList[7].SkillLevel + "                      " + stats.SkillList[7].SkillExperience;
                cooking = "               " + stats.SkillList[8].SkillRank.ToString() + "                      " + stats.SkillList[8].SkillLevel + "                      " + stats.SkillList[8].SkillExperience;
                woodcutting = "               " + stats.SkillList[9].SkillRank.ToString() + "                      " + stats.SkillList[9].SkillLevel + "                      " + stats.SkillList[9].SkillExperience;
                flecthing = "               " + stats.SkillList[10].SkillRank.ToString() + "                      " + stats.SkillList[10].SkillLevel + "                      " + stats.SkillList[10].SkillExperience;
                fishing = "               " + stats.SkillList[11].SkillRank.ToString() + "                      " + stats.SkillList[11].SkillLevel + "                      " + stats.SkillList[11].SkillExperience;
                firemaking = "               " + stats.SkillList[12].SkillRank.ToString() + "                      " + stats.SkillList[12].SkillLevel + "                      " + stats.SkillList[12].SkillExperience;
                crafting = "               " + stats.SkillList[13].SkillRank.ToString() + "                      " + stats.SkillList[13].SkillLevel + "                      " + stats.SkillList[13].SkillExperience;
                smithing = "               " + stats.SkillList[14].SkillRank.ToString() + "                      " + stats.SkillList[14].SkillLevel + "                      " + stats.SkillList[14].SkillExperience;
                mining = "               " + stats.SkillList[15].SkillRank.ToString() + "                      " + stats.SkillList[15].SkillLevel + "                      " + stats.SkillList[15].SkillExperience;
                herblore = "               " + stats.SkillList[16].SkillRank.ToString() + "                      " + stats.SkillList[16].SkillLevel + "                      " + stats.SkillList[16].SkillExperience;
                agility = "               " + stats.SkillList[17].SkillRank.ToString() + "                      " + stats.SkillList[17].SkillLevel + "                      " + stats.SkillList[17].SkillExperience;
                thieving = "               " + stats.SkillList[18].SkillRank.ToString() + "                      " + stats.SkillList[18].SkillLevel + "                      " + stats.SkillList[18].SkillExperience;
                slayer = "               " + stats.SkillList[19].SkillRank.ToString() + "                      " + stats.SkillList[19].SkillLevel + "                      " + stats.SkillList[19].SkillExperience;
                farming = "               " + stats.SkillList[20].SkillRank.ToString() + "                      " + stats.SkillList[20].SkillLevel + "                      " + stats.SkillList[20].SkillExperience;
                runecraft = "               " + stats.SkillList[21].SkillRank.ToString() + "                      " + stats.SkillList[21].SkillLevel + "                      " + stats.SkillList[21].SkillExperience;
                hunter = "               " + stats.SkillList[22].SkillRank.ToString() + "                      " + stats.SkillList[22].SkillLevel + "                      " + stats.SkillList[22].SkillExperience;
                construction = "               " + stats.SkillList[23].SkillRank.ToString() + "                      " + stats.SkillList[23].SkillLevel + "                      " + stats.SkillList[23].SkillExperience;
                banned = "unknown";

                int[] num = 
                {
                    stats.SkillList[0].SkillLevel, 
                    stats.SkillList[1].SkillLevel, 
                    stats.SkillList[2].SkillLevel,
                    stats.SkillList[3].SkillLevel, 
                    stats.SkillList[4].SkillLevel, 
                    stats.SkillList[5].SkillLevel,
                    stats.SkillList[6].SkillLevel, 
                    stats.SkillList[7].SkillLevel, 
                    stats.SkillList[8].SkillLevel,
                    stats.SkillList[9].SkillLevel, 
                    stats.SkillList[10].SkillLevel, 
                    stats.SkillList[11].SkillLevel,
                    stats.SkillList[12].SkillLevel, 
                    stats.SkillList[13].SkillLevel, 
                    stats.SkillList[14].SkillLevel,
                    stats.SkillList[15].SkillLevel, 
                    stats.SkillList[16].SkillLevel, 
                    stats.SkillList[17].SkillLevel,
                    stats.SkillList[18].SkillLevel, 
                    stats.SkillList[19].SkillLevel, 
                    stats.SkillList[20].SkillLevel,
                    stats.SkillList[21].SkillLevel, 
                    stats.SkillList[22].SkillLevel, 
                    stats.SkillList[23].SkillLevel
                };

                var lazyGOD = new RuneScapeLibrary.BanHammer.JudgingThee(num, 24);

                switch (lazyGOD.BOT)
                {
                    case 2:
                        banned = "(Potential) Attack Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Potential) Attack Bot Detected");
                        break;

                    case 3:
                        banned = "(Potential) Defence Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Potential) Defence Bot Detected");
                        break;

                    case 4:
                        banned = "(Potential) Stregnth Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Potential) Stregnth Bot Detected");
                        break;

                    case 5:
                        banned = "(Potential) Hitpoints Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Potential) Hitpoints Bot Detected");
                        break;

                    case 6:
                        banned = "(Potential) Ranged Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Potential) Ranged Bot Detected");
                        break;

                    case 7:
                        banned = "(Potential) Prayer Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Potential) Prayer Bot Detected");
                        break;

                    case 8:
                        banned = "(Potential) Magic Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Potential) Magic Bot Detected");
                        break;

                    case 9:
                        banned = "(Potential) Cooking Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Potential) Cooking Bot Detected");
                        break;

                    case 10:
                        banned = "(Potential) Woodcutting Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Potential) Woodcutting Bot Detected");
                        break;

                    case 11:
                        banned = "(Potential) Fletching Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Potential) Fletching Bot Detected");
                        break;

                    case 12:
                        banned = "(Potential) Fishing Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Potential) Fishing Bot Detected");
                        break;

                    case 13:
                        banned = "(Potential) Firemaking Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Potential) Firemaking Bot Detected");
                        break;

                    case 14:
                        banned = "(Potential) Crafting Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Potential) Crafting Bot Detected");
                        break;

                    case 15:
                        banned = "(Potential) Smithing Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Potential) Smithing Bot Detected");
                        break;

                    case 16:
                        banned = "(Potential) Mining Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Potential) Mining Bot Detected");
                        break;

                    case 17:
                        banned = "(Potential) Herblore Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Potential) Herblore Bot Detected");
                        break;

                    case 18:
                        banned = "(Potential) Agility Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Potential) Agility Bot Detected");
                        break;

                    case 19:
                        banned = "(Potential) Theiving Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Potential) Thieving Bot Detected");
                        break;

                    case 20:
                        banned = "(Potential) Slayer Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Potential) Slayer Bot Detected");
                        break;

                    case 21:
                        banned = "(Potential) Farming Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Potential) Farming Bot Detected");
                        break;

                    case 22:
                        banned = "(Potential) Runecraft Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Potential) Runecraft Bot Detected");
                        break;

                    case 23:
                        banned = "(Potential) Hunter Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Potential) Hunter Bot Detected");
                        break;

                    case 24:
                        banned = "(Potential) Construction Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Potential) Construction Bot Detected");
                        break;

                    case 25:
                        banned = "(Certain) Runecrafting Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Certain) Runecrafting Bot Detected");
                        break;

                    case 26:
                        banned = "(Certain) Green Dragons Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Certain) Green Dragons Bot Detected");
                        break;

                    case 27:
                        banned = "(Certain) Smithing Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Certain) Smithing Bot Detected");
                        break;

                    case 28:
                        banned = "(Certain) Lava Dragons Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Certain) Lava Dragons Bot Detected");
                        break;

                    case 29:
                        banned = "(Certain) Fletching Bot Detected";
                        BANNEDBITCHES.Add(name + " - (Certain) Fletching Bot Detected");
                        break;
                }
            }

            try
            {
                TextWriter file = new StreamWriter("bNames.txt");

                foreach (string name in BANNEDBITCHES)
                {
                    file.WriteLine(name);
                }

                file.Close();

                VerifiedListLock = false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        public Form1()
        {
            InitializeComponent();

            DialogResult soundResult = MessageBox.Show("                            SOUND?", "RDB", MessageBoxButtons.YesNo);

            if (soundResult == DialogResult.Yes)
            {
                Create_Sound();
                Play_Sound();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Fetch = new Thread(GenerateList);
            Fetch.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (list != null && VerifiedListLock)
            {
                Names = new Thread(REMOVE_DE_BOTS);
                Names.Start();

                timer2.Enabled = true;
                timer2.Start();

                timer1.Dispose();
                timer1.Stop();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (VerifiedListLock)
            {
                label1.Text = user;
                label6.Text = banned;
                label7.Text = attack;
                label8.Text = defence;
                label9.Text = strength;
                label10.Text = hitpoints;
                label11.Text = ranged;
                label12.Text = prayer;
                label13.Text = magic;
                label14.Text = cooking;
                label15.Text = woodcutting;
                label16.Text = flecthing;
                label17.Text = fishing;
                label18.Text = firemaking;
                label19.Text = crafting;
                label20.Text = smithing;
                label21.Text = mining;
                label22.Text = herblore;
                label23.Text = agility;
                label24.Text = thieving;
                label25.Text = slayer;
                label26.Text = farming;
                label27.Text = runecraft;
                label28.Text = hunter;
                label29.Text = construction;
            }
            else
            {
                Environment.Exit(0);
            }
        }
    }
}
