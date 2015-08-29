using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Linq;
using System.Diagnostics;
using HtmlAgilityPack;
using System.Runtime.InteropServices;

namespace RuneScapeLibrary.Search
{
    /// <summary>
    /// Fetches the RuneScape names.
    /// </summary>
    public class NameSearch : List<string>
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern void OutputDebugString(string message);

        public NameSearch()
        {
            this.Clear();

            PopulateList();
        }

        public void PopulateList()
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlDocument();

            List<string> nodes = null;

            for (int i = 1; ;)
            {
                try
                {
                    doc.LoadHtml(new WebClient().DownloadString("http://services.runescape.com/m=hiscore_oldschool/overall.ws?table=0&page=" + i));

                    nodes = doc.DocumentNode.Descendants("td").Where(td => td.Attributes["class"] != null && td.Attributes["class"].Value == "left").SelectMany(td => td.Descendants("a")).Select(a => a.InnerText).ToList();

                    if (i > 1 && nodes.Count != 0 && nodes[0].Contains(this[0]))
                    {
                        break;
                    }

                    if (nodes.Count != 0)
                    {
                        this.AddRange(nodes);

                        OutputDebugString("PAGE # : " + i);

                        OutputDebugString("COUNT # : " + this.Count());
                        
                        OutputDebugString("NAMES : " + nodes[0] + " - " + nodes[24]);

                        i++;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
    }

    /// <summary>
    /// Fetches player statistics from the RuneScape hiscores.
    /// </summary>
    public class HiscoreSearch
    {
        protected string lookupName = null;

        /// <summary>
        /// The RuneScape name to lookup.
        /// </summary>
        public string LookupName { get { return lookupName; } set { lookupName = value; } }

        /// <summary>
        /// Protected variables.
        /// </summary>
        protected WebClient lookupClient;
        protected Stream lookupData;
        protected StreamReader lookupReader;

        /// <summary>
        /// A collection of the hiscore lookup skills.
        /// </summary>
        public List<DataTypes.Skill> SkillList = new List<RuneScapeLibrary.DataTypes.Skill>();

        /// <summary>
        /// Performs a hiscore lookup on a player.
        /// </summary>
        /// <param name="name">The RuneScape name to lookup.</param>
        public HiscoreSearch(string name) 
        { 
            if ((name.Length > 0) && (name.Length < 13)) 
            { 
                lookupName = name; 
                Refresh(); 
            } 
        }

        /// <summary>
        /// Refreshes the skill collection.
        /// </summary>
        public void Refresh()
        {
            SkillList.Clear();

            try
            {
                lookupClient = new WebClient();

                lookupData = lookupClient.OpenRead("http://services.runescape.com/m=hiscore_oldschool/index_lite.ws?player=" + lookupName);
                
                lookupReader = new StreamReader(lookupData);

                string lineBuffer;
                int line = 0;

                while ((lineBuffer = lookupReader.ReadLine()) != null)
                {
                    string[] splitBuffer = lineBuffer.Split(',');

                    if (line != 24)
                    {
                        SkillList.Add(new DataTypes.Skill(DataTypes.SkillArray.SkillsArray[line], Convert.ToInt32(splitBuffer[0]), Convert.ToInt32(splitBuffer[1]), Convert.ToInt32(splitBuffer[2])));

                        line++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (WebException e)
            {
                if (((HttpWebResponse)e.Response).StatusCode == HttpStatusCode.NotFound)
                {
                    foreach (string skillName in DataTypes.SkillArray.SkillsArray)
                    {
                        SkillList.Add(new DataTypes.Skill(skillName, -1, 1, 0));
                    }
                }
                else
                {
                    throw e;
                }
            }
        }
    }
}

namespace RuneScapeLibrary.DataTypes
{
    /// <summary>
    /// An individual RuneScape skill with a name, exp value, ranking value and level.
    /// </summary>
    public class Skill
    {
        protected string skillName = null;
        protected int skillRank = 0;
        protected int skillLevel = 0;
        protected int skillExperience = 0;

        /// <summary>
        /// The name of the RuneScape skill.
        /// </summary>
        public string SkillName { get { return skillName; } set { skillName = value; } }

        /// <summary>
        /// The rank of which the skill value holds on the hiscores.
        /// </summary>
        public int SkillRank { get { return skillRank; } set { skillRank = value; } }

        /// <summary>
        /// The level of the skill.
        /// </summary>
        public int SkillLevel { get { return skillLevel; } set { skillLevel = value; } }

        /// <summary>
        /// The amount of experience points of the skill.
        /// </summary>
        public int SkillExperience { get { return skillExperience; } set { skillExperience = value; } }

        public Skill() { }

        /// <summary>
        /// Creates a skill class with pre-defined variables.
        /// </summary>
        /// <param name="name">The name of the RuneScape skill.</param>
        /// <param name="rank">The rank of which the skill value holds on the hiscores.</param>
        /// <param name="level">The level of the skill.</param>
        /// <param name="experience">The amount of experience points of the skill.</param>
        public Skill(string name, int rank, int level, int experience)
        {
            skillName = name;
            skillRank = rank;
            skillLevel = level;
            skillExperience = experience;
        }
    }

    public class SkillArray
    {
        public static string[] SkillsArray =
        {
            "Overall",
            "Attack",
            "Defence",
            "Strength",
            "Hitpoints",
            "Ranged",
            "Prayer",
            "Magic",
            "Cooking",
            "Woodcutting",
            "Fletching",
            "Fishing",
            "Firemaking",
            "Crafting",
            "Smithing",
            "Mining",
            "Herblore",
            "Agility",
            "Thieving",
            "Slaying",
            "Farming",
            "Runecraft",
            "Hunter",
            "Construction"
        };

        public static string[] CombatSkillsArray =
        {
            "Attack",
            "Defence",
            "Strength",
            "Hitpoints",
            "Ranged",
            "Prayer",
            "Magic",
        };
    }
}

namespace RuneScapeLibrary.BanHammer
{
    /// <summary>
    /// Determines if the user is a bot. 
    /// Programmed by: Jason G. Kim
    /// </summary>
    public class JudgingThee
    {
        public int BOT;

        public JudgingThee(int[] _num, int _skillCount)
        {
            BOT = BanHammer(_num, _skillCount);
        }

        private int BanHammer(int[] num, int skillCount)
        {
            int skill = checkForSingle(num, skillCount);

            if (skill != -1)
            {
                return skill;
            }

            skill = checkForOthers(num, skillCount);

            return skill;
        }

        private int checkForSingle(int[] num, int skillCount)
        {
            bool developmentFound = false;
            int skill = 0;
            int counter = 1;

            while (counter < skillCount && (developmentFound == false))
            {
                if (num[counter] != 1)
                {
                    developmentFound = true;
                    skill = counter + 1;
                }
                counter++;
            }

            while (counter < skillCount)
            {
                if (num[counter] != 1)
                {
                    return -1;
                }
                counter++;
            }
            return skill;
        }

        private int checkForOthers(int[] num, int skillCount)
        {
            int counter = 1;

            while (counter < skillCount)
            {
                if (num[counter] != 1)
                {
                    switch (counter)
                    {
                        case 1:
                            if (num[1] > 30 && num[2] > 30 && checkForOnesUntil(num, skillCount, 3, 21, 44) && checkForOnesEnd(num, skillCount, 22))
                            {
                                return 25; // RuneCrafting Bot
                            }
                            else if (num[1] > 50 && num[2] > 50 && checkForOnesUntil(num, skillCount, 3, 7, 25) && checkForOnesEnd(num, skillCount, 8))
                            {
                                return 26; // Green Dragon Bots
                            }
                            break;

                        case 3:
                            if (checkForOnesUntil(num, skillCount, 4, 14, 60) && checkForOnesEnd(num, skillCount, 5))
                            {
                                return 27; // Smithing Bot
                            }
                            break;

                        case 4:
                            if (checkForOnesUntil(num, skillCount, 5, 7, 60) && checkForOnesEnd(num, skillCount, 8))
                            {
                                return 28; // Lava Dragon Bots
                            }
                            break;

                        case 10:
                            if (num[1] < 30 && num[2] < 30 && num[3] < 30 && num[4] < 30 && checkForOnesUntil(num, skillCount, 5, 10, 60) && checkForOnesEnd(num, skillCount, 11))
                            {
                                return 29; // Fletching Bot
                            }
                            break;
                    }
                }
                counter++;
            }
            return 0;
        }

        private bool checkForOnesUntil(int[] num, int skillCount, int counter, int skill, int level)
        {
            while (counter < skillCount)
            {
                if (num[counter] != 1 && counter == skill && num[counter] >= level)
                {
                    return true;
                }
                else if (num[counter] != 1)
                {
                    return false;
                }
                counter++;
            }
            return false;
        }

        private bool checkForOnesEnd(int[] num, int skillCount, int counter)
        {
            while (counter < skillCount)
            {
                if (num[counter] != 1)
                {
                    return false;
                }
                counter++;
            }
            return true;
        }
    }
}
