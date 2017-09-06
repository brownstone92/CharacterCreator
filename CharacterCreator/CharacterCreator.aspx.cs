using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Data.SQLite;

namespace CharacterCreator
{
    public partial class CharacterCreator : System.Web.UI.Page
    {
        private static Character devCharacter;
        List<Stat> charStats;
        Dictionary<int, TableRow> abilityList;
        Dictionary<int, TableRow> skillList;

          /////////////////////////////////////////
          //      Cross Page/Tab Functions       //
          /////////////////////////////////////////

        protected void Page_Load(object sender, EventArgs e)
        {
            if (CharIDTB.Text != "")
            {
                devCharacter.id = int.Parse(CharIDTB.Text);

                using (SQLiteConnection conn = new SQLiteConnection("Data Source=|DataDirectory|/charDev.db;Version=3;"))
                {
                    conn.Open();

                    using (SQLiteCommand query = conn.CreateCommand())
                    {
                        query.CommandText = "SELECT * FROM Character WHERE Character_ID = " + devCharacter.id.ToString();

                        using (SQLiteDataReader charReader = query.ExecuteReader())
                        {
                            charReader.Read();

                            devCharacter.name       = charReader["Character_Name"].ToString();
                            devCharacter.desc       = charReader["Character_Desc"].ToString();
                            devCharacter.player     = charReader["Player_Name"].ToString();
                            devCharacter.type.id    = int.Parse(charReader["CharacterType_ID"].ToString());
                            devCharacter.rpg.id     = int.Parse(charReader["RPG_ID"].ToString());
                        }

                        query.Dispose();
                    }

                    conn.Close();
                }
            }

            if (CommonIDTB.Text != "")
            {
                devCharacter.commonInfo.id = int.Parse(CommonIDTB.Text);

                using (SQLiteConnection conn = new SQLiteConnection("Data Source=|DataDirectory|/charDev.db;Version=3;"))
                {
                    conn.Open();

                    using (SQLiteCommand query = conn.CreateCommand())
                    {
                        query.CommandText = "SELECT * FROM CharacterCommon WHERE Character_ID = " + devCharacter.id.ToString();

                        using (SQLiteDataReader charReader = query.ExecuteReader())
                        {
                            charReader.Read();

                            devCharacter.commonInfo.charSex         = charReader["Char_Sex"].ToString();
                            devCharacter.commonInfo.level           = int.Parse(charReader["Char_Level"].ToString());
                            devCharacter.commonInfo.charRace.id     = int.Parse(charReader["Char_Race_ID"].ToString());
                            devCharacter.commonInfo.charClass.id    = int.Parse(charReader["Char_Class_ID"].ToString());
                            devCharacter.commonInfo.charAlign.id    = int.Parse(charReader["Char_Align_ID"].ToString());
                        }

                        query.Dispose();
                    }

                    conn.Close();
                }
            }

            if (CampIDTB.Text != "")
            {
                devCharacter.campaign.id = int.Parse(CampIDTB.Text);

                using (SQLiteConnection conn = new SQLiteConnection("Data Source=|DataDirectory|/charDev.db;Version=3;"))
                {
                    conn.Open();

                    using (SQLiteCommand query = conn.CreateCommand())
                    {
                        query.CommandText = "SELECT * FROM Campaign WHERE Campaign_ID = " + devCharacter.campaign.id.ToString();

                        using (SQLiteDataReader charReader = query.ExecuteReader())
                        {
                            charReader.Read();

                            devCharacter.campaign.dm        = charReader["Camp_DM"].ToString();
                            devCharacter.campaign.name      = charReader["Camp_Name"].ToString();
                            devCharacter.campaign.type      = charReader["Camp_Type"].ToString();
                            devCharacter.campaign.desc      = charReader["Camp_Desc"].ToString();
                            devCharacter.campaign.extraDesc = charReader["Camp_ExtraDesc"].ToString();
                            devCharacter.campaign.rpg.id    = devCharacter.rpg.id;
                        }

                        query.Dispose();
                    }

                    conn.Close();
                }
            }

            if (StatTypeIDTB.Text != "")
            {

            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            devCharacter = new Character();
            abilityList = new Dictionary<int, TableRow>();
            skillList = new Dictionary<int, TableRow>();

            PopulateMenu();
            
            CharTypeRBList.SelectedIndexChanged += new EventHandler(CharTypeChanged);
            CharTypeRBList.AutoPostBack = true;

            RPGDD.SelectedIndexChanged += new EventHandler(RPGChanged);
            RPGDD.AutoPostBack = true;

            startResetBtn.Command   += new CommandEventHandler(ResetStartMenu);
            startRefreshBtn.Command += new CommandEventHandler(PopulateMenu);
            startBuildBtn.Command   += new CommandEventHandler(StartBuild);
            SaveBtn.Command         += new CommandEventHandler(SaveCharacter);
        }

        public void SaveCharacter(object sender, CommandEventArgs e)
        {
            try
            {
                SaveCampaignDetails();
                SaveCommonDetails();
                SaveAbilityScores();
            }
            catch (Exception err)
            {
                DescTB.Text += err.ToString();
            }
        }

          /////////////////////////////////////////
         //    Start Menu Page/Tab Functions    //
        /////////////////////////////////////////

        public void PopulateMenu()
        {
            CharTypeRBList.Items.Clear();
            RPGDD.Items.Clear();

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection("Data Source=|DataDirectory|/charDev.db;Version=3;"))
                {
                    conn.Open();

                    using (SQLiteCommand query = conn.CreateCommand())
                    {
                        query.CommandText = "SELECT * FROM CharacterType;";

                        using (SQLiteDataReader CharTypeRead = query.ExecuteReader())
                        {
                            while (CharTypeRead.Read())
                            {
                                CharTypeRBList.Items.Add(CharTypeRead["CharacterType_Name"].ToString());
                            }

                            if (CharTypeRBList.Items.Count > 0)
                            {
                                CharTypeRBList.Items[0].Selected = true;
                            }
                        }

                        query.CommandText = "SELECT * FROM RPG;";

                        using (SQLiteDataReader RPGRead = query.ExecuteReader())
                        {
                            while (RPGRead.Read())
                            {
                                RPGDD.Items.Add(RPGRead["RPG_Name"].ToString());
                            }
                        }

                        RPGChanged();

                        query.Dispose();
                    }

                    conn.Close();
                }
            }
            catch (Exception err)
            {
                DescTB.Text += err.ToString();
            }
        }

        public void PopulateMenu(object sender, CommandEventArgs e)
        {
            PopulateMenu();
        }

        public void ResetStartMenu(object sender, CommandEventArgs e)
        {
            NameTB.Text = "";
            PlayerTB.Text = "";
            DescTB.Text = "";

            if (CharTypeRBList.Items.Count > 0)
            {
                CharTypeRBList.SelectedIndex = 0;
                CharTypeChanged();
            }

            if (RPGDD.Items.Count > 0)
            {
                RPGDD.SelectedIndex = 0;
            }
            
            RPGChanged();
        }

        public void CharTypeChanged()
        {
            if (CharTypeRBList.SelectedIndex == 1)
            {
                PlayerTB.Enabled = false;
                PlayerTB.Text = "";
            }
            else if (!PlayerTB.Enabled)
            {
                PlayerTB.Enabled = true;
            }
        }

        public void CharTypeChanged(object sender, EventArgs e)
        {
            CharTypeChanged();
        }

        public void RPGChanged()
        {
            CampaignDD.Items.Clear();
            CampaignDD.Items.Add("New Campaign");
            
            if (RPGDD.Items.Count > 0)
            {
                ListItem separator = new ListItem("------------");
                separator.Attributes.Add("disabled", "true");
                CampaignDD.Items.Add(separator);

                try
                {
                    using (SQLiteConnection conn = new SQLiteConnection("Data Source=|DataDirectory|/charDev.db;Version=3;"))
                    {
                        conn.Open();

                        using (SQLiteCommand query = conn.CreateCommand())
                        {
                            query.CommandText = "SELECT * FROM Campaign WHERE RPG_ID = " + (RPGDD.SelectedIndex + 1).ToString() + ";";

                            using (SQLiteDataReader CampaignListing = query.ExecuteReader())
                            {
                                while (CampaignListing.Read())
                                {
                                    CampaignDD.Items.Add(CampaignListing["Camp_DM"].ToString() + " - " + CampaignListing["Camp_Name"].ToString());
                                }
                            }

                            query.Dispose();
                        }

                        conn.Close();
                    }
                }
                catch (Exception err)
                {
                    DescTB.Text += err.ToString();
                }
            }
        }

        public void RPGChanged(object sender, EventArgs e)
        {
            RPGChanged();
        }

        public bool CreationFieldCheck()
        {
            bool pass = true;

            if (NameTB.Text == "")
            {
                pass = false;
            }
            else if (CharTypeRBList.Items.Count == 0)
            {
                pass = false;
            }
            else if (RPGDD.Items.Count == 0)
            {
                pass = false;
            }

            return pass;
        }

        public void StartBuild(object sender, CommandEventArgs e)
        {
            if (CreationFieldCheck())
            {
                devCharacter.name   = NameTB.Text;

                devCharacter.type.id    = CharTypeRBList.SelectedIndex + 1;
                devCharacter.type.name  = CharTypeRBList.SelectedValue;

                devCharacter.rpg.id     = RPGDD.SelectedIndex + 1;
                devCharacter.rpg.name   = RPGDD.SelectedValue;

                if (DescTB.Text != "")
                {
                    devCharacter.desc = DescTB.Text;
                }
                else
                {
                    devCharacter.desc = null;
                }

                if (PlayerTB.Text != "")
                {
                    devCharacter.player = PlayerTB.Text;
                }
                else
                {
                    devCharacter.player = null;
                }

                if (CampaignDD.SelectedIndex == 0)
                {
                    devCharacter.campaign.id = -1;
                }

                devCharacter.campaign.rpg = devCharacter.rpg;

                CreateCharacter();                
                SetCampaignDetails();
                SetCommonDetails();
                SetAbilityScores();
            }
        }

        public void CreateCharacter()
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=|DataDirectory|/charDev.db;Version=3;"))
            {
                conn.Open();

                using (SQLiteCommand query = conn.CreateCommand())
                {
                    query.CommandText += "INSERT INTO Character (";
                    query.CommandText += "Character_Name, ";
                    query.CommandText += "Character_Desc, ";
                    query.CommandText += "Player_Name, ";
                    query.CommandText += "CharacterType_ID, ";
                    query.CommandText += "Campaign_ID, ";
                    query.CommandText += "RPG_ID";
                    query.CommandText += ") VALUES (";
                    query.CommandText += "@Name, ";
                    query.CommandText += "@Desc, ";
                    query.CommandText += "@Player, ";
                    query.CommandText += "@Type, ";
                    query.CommandText += "@Campaign, ";
                    query.CommandText += "@RPG";
                    query.CommandText += ");";

                    query.Parameters.Add(new SQLiteParameter("@Name",       devCharacter.name));
                    query.Parameters.Add(new SQLiteParameter("@Type",       devCharacter.type.id));
                    query.Parameters.Add(new SQLiteParameter("@RPG",        devCharacter.rpg.id));
                    query.Parameters.Add(new SQLiteParameter("@Desc",       devCharacter.desc));
                    query.Parameters.Add(new SQLiteParameter("@Player",     devCharacter.player));

                    if (devCharacter.campaign.id >= 0)
                    {
                        query.Parameters.Add(new SQLiteParameter("@Campaign", devCharacter.campaign.id));
                    }
                    else
                    {
                        query.Parameters.Add(new SQLiteParameter("@Campaign", null));
                    }

                    query.ExecuteNonQuery();

                    devCharacter.id = (int)conn.LastInsertRowId;
                    DescTB.Text += "NEW ID = " + devCharacter.id.ToString();
                    CharIDTB.Text = devCharacter.id.ToString();

                    query.Dispose();
                }

                conn.Close();
            }
        }

          /////////////////////////////////////////
         // Campaign Details Page/Tab Functions //
        /////////////////////////////////////////

        public void SetCampaignDetails()
        {
            if (devCharacter.campaign.id >= 0)
            {
                using (SQLiteConnection conn = new SQLiteConnection("Data Source=|DataDirectory|/charDev.db;Version=3;"))
                {
                    conn.Open();

                    using (SQLiteCommand query = conn.CreateCommand())
                    {
                        query.CommandText += "SELECT * FROM Campaign WHERE Campaign_ID=" + devCharacter.campaign.id;

                        using (SQLiteDataReader CampRead = query.ExecuteReader())
                        {
                            Camp_DMTB.Text      = CampRead["Camp_DM"].ToString();
                            Camp_NameTB.Text    = CampRead["Camp_Name"].ToString();
                            Camp_TypeTB.Text    = CampRead["Camp_Type"].ToString();
                            Camp_DescTB.Text    = CampRead["Camp_Desc"].ToString();
                            Camp_ExtraTB.Text   = CampRead["Camp_ExtraDesc"].ToString();
                        }

                        query.Dispose();
                    }

                    conn.Close();
                }
            }
        }

        public void SaveCampaignDetails()
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=|DataDirectory|/charDev.db;Version=3;"))
            {
                conn.Open();

                using (SQLiteCommand query = conn.CreateCommand())
                {
                    if (devCharacter.campaign.id >= 0)
                    {
                        query.CommandText += "UPDATE Campaign SET ";

                        if (Camp_DMTB.Text != devCharacter.campaign.dm)
                        {
                            query.CommandText += "Camp_DM = '" + Camp_DMTB.Text + "', ";
                        }

                        if (Camp_NameTB.Text != devCharacter.campaign.name)
                        {
                            query.CommandText += "Camp_Name = '" + Camp_NameTB.Text + "', ";
                        }

                        if (Camp_TypeTB.Text != devCharacter.campaign.type)
                        {
                            query.CommandText += "Camp_Type = '" + Camp_TypeTB.Text + "', ";
                        }

                        if (Camp_DescTB.Text != devCharacter.campaign.desc)
                        {
                            query.CommandText += "Camp_Desc = '" + Camp_DescTB.Text + "', ";
                        }

                        if (Camp_ExtraTB.Text != devCharacter.campaign.extraDesc)
                        {
                            query.CommandText += "Camp_ExtraDesc = '" + Camp_ExtraTB.Text + "', ";
                        }

                        if (query.CommandText.Contains(","))
                        {
                            query.CommandText = query.CommandText.Substring(0, query.CommandText.LastIndexOf(","));

                            query.CommandText += " WHERE Campaign_ID=" + devCharacter.campaign.id;
                            query.ExecuteNonQuery();
                        }
                    }
                    else if (Camp_DMTB.Text != "" && Camp_NameTB.Text != "")
                    {
                        query.CommandText += "INSERT INTO Campaign (";
                        query.CommandText += "Camp_DM, ";
                        query.CommandText += "Camp_Name, ";
                        query.CommandText += "Camp_Type, ";
                        query.CommandText += "Camp_Desc, ";
                        query.CommandText += "Camp_ExtraDesc, ";
                        query.CommandText += "RPG_ID";
                        query.CommandText += ") VALUES (";
                        query.CommandText += "@DM, ";
                        query.CommandText += "@Name, ";
                        query.CommandText += "@Type, ";
                        query.CommandText += "@Desc, ";
                        query.CommandText += "@Extra, ";
                        query.CommandText += "@RPG";
                        query.CommandText += ");";

                        query.Parameters.Add(new SQLiteParameter("@DM",     Camp_DMTB.Text));
                        query.Parameters.Add(new SQLiteParameter("@Name",   Camp_NameTB.Text));
                        query.Parameters.Add(new SQLiteParameter("@RPG",    devCharacter.rpg.id.ToString()));

                        if (Camp_TypeTB.Text != "")
                        {
                            query.Parameters.Add(new SQLiteParameter("@Type",   Camp_TypeTB.Text));
                        }
                        else
                        {
                            query.Parameters.Add(new SQLiteParameter("@Type", null));
                        }

                        if (Camp_DescTB.Text != "")
                        {
                            query.Parameters.Add(new SQLiteParameter("@Desc", Camp_DescTB.Text));
                        }
                        else
                        {
                            query.Parameters.Add(new SQLiteParameter("@Desc", null));
                        }

                        if (Camp_ExtraTB.Text != "")
                        {
                            query.Parameters.Add(new SQLiteParameter("@Extra", Camp_ExtraTB.Text));
                        }
                        else
                        {
                            query.Parameters.Add(new SQLiteParameter("@Extra", null));
                        }

                        query.ExecuteNonQuery();

                        devCharacter.campaign.id = (int)conn.LastInsertRowId;
                        CampIDTB.Text = devCharacter.campaign.id.ToString();

                        query.CommandText = "UPDATE Character SET Campaign_ID = " + devCharacter.campaign.id.ToString() + " WHERE Character_ID = " +  devCharacter.id.ToString();
                        query.ExecuteNonQuery();
                    }

                    query.Dispose();
                }

                conn.Close();
            }
        }

          /////////////////////////////////////////
         //  Common Details Page/Tab Functions  //
        /////////////////////////////////////////

        public void SetCommonDetails()
        {
            Common_RaceDD.Items.Clear();
            Common_ClassDD.Items.Clear();
            Common_AlignDD.Items.Clear();

            using (SQLiteConnection conn = new SQLiteConnection("Data Source=|DataDirectory|/charDev.db;Version=3;"))
            {
                conn.Open();

                using (SQLiteCommand query = conn.CreateCommand())
                {
                    query.CommandText = "SELECT * FROM CharacterRace JOIN RPGRace WHERE ";
                    query.CommandText += "CharacterRace.Char_Race_ID = RPGRace.Char_Race_ID AND ";
                    query.CommandText += "RPG_ID=" + devCharacter.rpg.id;

                    using (SQLiteDataReader AlignRead = query.ExecuteReader())
                    {
                        while (AlignRead.Read())
                        {
                            Common_RaceDD.Items.Add(AlignRead["Char_Race_Name"].ToString());
                        }
                    }

                    query.CommandText = "SELECT * FROM CharacterClass JOIN RPGClass WHERE ";
                    query.CommandText += "CharacterClass.Char_Class_ID = RPGClass.Char_Class_ID AND ";
                    query.CommandText += "RPG_ID=" + devCharacter.rpg.id;

                    using (SQLiteDataReader AlignRead = query.ExecuteReader())
                    {
                        while (AlignRead.Read())
                        {
                            Common_ClassDD.Items.Add(AlignRead["Char_Class_Name"].ToString());
                        }
                    }

                    query.CommandText = "SELECT * FROM CharacterAlign JOIN RPGAlign WHERE ";
                    query.CommandText += "CharacterAlign.Char_Align_ID = RPGAlign.Char_Align_ID AND ";
                    query.CommandText += "RPG_ID=" + devCharacter.rpg.id;

                    using (SQLiteDataReader AlignRead = query.ExecuteReader())
                    {
                        while(AlignRead.Read())
                        {
                            Common_AlignDD.Items.Add(AlignRead["Char_Align_Name"].ToString());
                        }
                    }

                    if (devCharacter.commonInfo.id >= 0)
                    {
                        query.CommandText = "SELECT * FROM CharacterCommon WHERE Common_ID=" + devCharacter.commonInfo.id;

                        using (SQLiteDataReader CommonRead = query.ExecuteReader())
                        {
                            Common_SexTB.Text = CommonRead["Char_Sex"].ToString();
                            Common_LevelTB.Text = CommonRead["Char_Level"].ToString();
                            Common_RaceDD.Text = CommonRead["Char_Race_ID"].ToString();
                            Common_ClassDD.Text = CommonRead["Char_Class_ID"].ToString();
                            Common_AlignDD.Text = CommonRead["Char_Align_ID"].ToString();
                        }
                    }

                    query.Dispose();
                }

                conn.Close();
            }
        }

        public void SaveCommonDetails()
        {
            using (SQLiteConnection conn = new SQLiteConnection("Data Source=|DataDirectory|/charDev.db;Version=3;"))
            {
                conn.Open();

                using (SQLiteCommand query = conn.CreateCommand())
                {
                    if (devCharacter.commonInfo.id >= 0)
                    {
                        query.CommandText += "UPDATE CharacterCommon SET ";

                        if (Common_SexTB.Text != devCharacter.commonInfo.charSex)
                        {
                            query.CommandText += "Char_Sex = '" + Common_SexTB.Text + "', ";
                        }

                        if (int.Parse(Common_LevelTB.Text) != devCharacter.commonInfo.level)
                        {
                            query.CommandText += "Char_Level = " + Common_LevelTB.Text + ", ";
                        }

                        if (Common_RaceDD.SelectedIndex + 1 != devCharacter.commonInfo.charRace.id)
                        {
                            query.CommandText += "Char_Race_ID = " + (Common_RaceDD.SelectedIndex + 1).ToString() + ", ";
                        }

                        if (Common_ClassDD.SelectedIndex + 1 != devCharacter.commonInfo.charClass.id)
                        {
                            query.CommandText += "Char_Class_ID = '" + (Common_ClassDD.SelectedIndex + 1).ToString() + "', ";
                        }

                        if (Common_AlignDD.SelectedIndex + 1 != devCharacter.commonInfo.charAlign.id)
                        {
                            query.CommandText += "Char_Align_ID = '" + (Common_AlignDD.SelectedIndex + 1).ToString() + "', ";
                        }

                        if (query.CommandText.Contains(","))
                        {
                            query.CommandText = query.CommandText.Substring(0, query.CommandText.LastIndexOf(","));
                            
                            query.CommandText += " WHERE Common_ID=" + devCharacter.commonInfo.id;
                            query.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        query.CommandText += "INSERT INTO CharacterCommon (";
                        query.CommandText += "Char_Sex, ";
                        query.CommandText += "Char_Level, ";
                        query.CommandText += "Char_Race_ID, ";
                        query.CommandText += "Char_Class_ID, ";
                        query.CommandText += "Char_Align_ID, ";
                        query.CommandText += "Character_ID";
                        query.CommandText += ") VALUES (";
                        query.CommandText += "@Sex, ";
                        query.CommandText += "@Level, ";
                        query.CommandText += "@Race, ";
                        query.CommandText += "@Class, ";
                        query.CommandText += "@Align, ";
                        query.CommandText += "@CharID";
                        query.CommandText += ");";

                        query.Parameters.Add(new SQLiteParameter("@Sex",    Common_SexTB.Text));
                        query.Parameters.Add(new SQLiteParameter("@Level",  Common_LevelTB.Text));
                        query.Parameters.Add(new SQLiteParameter("@Race",   Common_RaceDD.SelectedIndex    + 1));
                        query.Parameters.Add(new SQLiteParameter("@Class",  Common_ClassDD.SelectedIndex   + 1));
                        query.Parameters.Add(new SQLiteParameter("@Align",  Common_AlignDD.SelectedIndex   + 1));
                        query.Parameters.Add(new SQLiteParameter("@CharID", devCharacter.id));

                        query.ExecuteNonQuery();

                        devCharacter.commonInfo.id = (int)conn.LastInsertRowId;
                        CommonIDTB.Text = devCharacter.commonInfo.id.ToString();
                    }

                    query.Dispose();
                }

                conn.Close();
            }
        }

          /////////////////////////////////////////
         //  Ability/Skills Page/Tab Functions  //
        /////////////////////////////////////////

        public void SetAbilityScores()
        {
            charStats = new List<Stat>();
            AbilityTable.Rows.Clear();
            SkillTable.Rows.Clear();

            using (SQLiteConnection conn = new SQLiteConnection("Data Source=|DataDirectory|/charDev.db;Version=3;"))
            {
                conn.Open();

                using (SQLiteCommand query = conn.CreateCommand())
                {
                    query.CommandText += "SELECT * FROM RPG WHERE RPG_ID = " + devCharacter.rpg.id.ToString();

                    using (SQLiteDataReader statType = query.ExecuteReader())
                    {
                        statType.Read();
                        devCharacter.rpg.statSys.id = int.Parse(statType["StatType_ID"].ToString());
                    }

                    query.CommandText = "SELECT * FROM StatList JOIN StatType WHERE StatList.StatType_ID = StatType.StatType_ID AND StatType.StatType_ID = " + devCharacter.rpg.statSys.id.ToString();

                    using (SQLiteDataReader statList = query.ExecuteReader())
                    {
                        while (statList.Read())
                        {                            
                            charStats.Add(new Stat() {  name    = statList["Stat_Name"].ToString(),
                                                        id      = int.Parse(statList["Stat_ID"].ToString()),
                                                        value   = int.Parse(statList["Stat_BaseValue"].ToString())
                                                        });

                            if (!int.TryParse(statList["Stat_Parent"].ToString(), out charStats.Last().parent))
                            {
                                charStats.Last().mod = CalcAbilityMod(charStats.Last().value);
                                charStats.Last().parent = -1;
                            }
                        }
                    }

                    query.Dispose();
                }

                conn.Close();
            }

            AbilityTable.Width = new Unit(25, UnitType.Percentage);
            SkillTable.Width = new Unit(40, UnitType.Percentage);
            AbilityTable.HorizontalAlign = HorizontalAlign.Center;
            SkillTable.HorizontalAlign = HorizontalAlign.Center;

            foreach (Stat newStat in charStats)
            {
                TableRow statRow = new TableRow();

                statRow.Cells.Add(new TableCell() { Text = newStat.name  });
                statRow.Cells.Add(new TableCell() { ID = "Stat_" + newStat.id.ToString(), Text = newStat.value.ToString() });
                
                if (newStat.parent == -1)
                {
                    statRow.Cells.Add(new TableCell() { Text = newStat.mod.ToString() });

                    Button upAbilityBtn     = new Button();
                    Button downAbilityBtn   = new Button();
                    TableCell upBtnCell     = new TableCell();
                    TableCell downBtnCell   = new TableCell();


                    upAbilityBtn.CommandArgument    = newStat.id.ToString();
                    downAbilityBtn.CommandArgument  = newStat.id.ToString();
                    upAbilityBtn.Command    += new CommandEventHandler(IncreaseAbility);
                    downAbilityBtn.Command  += new CommandEventHandler(DecreaseAbility);

                    upBtnCell.Controls.Add(upAbilityBtn);
                    downBtnCell.Controls.Add(downAbilityBtn);
                    statRow.Cells.Add(upBtnCell);
                    statRow.Cells.Add(downBtnCell);

                    abilityList.Add(newStat.id, statRow);
                }
                else
                {
                    int modVal = 0;

                    for (int i = 0; i < charStats.Count; i++)
                    {
                        if (charStats[i].id == newStat.parent)
                        {
                            modVal = charStats[i].mod;
                            charStats[i].subStats.Add(newStat);
                            break;
                        }
                    }

                    statRow.Cells.Add(new TableCell() { Text = "=" });
                    statRow.Cells.Add(new TableCell() { Text = modVal.ToString() });
                    statRow.Cells.Add(new TableCell() { Text = "+" });
                    statRow.Cells.Add(new TableCell() { Text = newStat.value.ToString() });

                    statRow.Cells[1].Text = (modVal + newStat.value + newStat.mod).ToString();
                    skillList.Add(newStat.id, statRow);
                }
            }

            AbilityTable.Rows.AddRange(abilityList.Values.ToArray());
            SkillTable.Rows.AddRange(skillList.Values.ToArray());
        }

        public void UpdateMods()
        {
            foreach (Stat ability in charStats)
            {
                if (ability.parent == -1)
                {
                    ability.mod = CalcAbilityMod(ability.value);

                    abilityList[ability.id].Cells[1].Text = ability.value.ToString();
                    abilityList[ability.id].Cells[2].Text = ability.mod.ToString();

                    foreach (Stat skill in ability.subStats)
                    {
                        skillList[skill.id].Cells[1].Text = (skill.value + ability.mod).ToString();
                        skillList[skill.id].Cells[4].Text = ability.mod.ToString();
                    }
                }
            }            
        }

        public void IncreaseAbility(object sender, CommandEventArgs e)
        {
            int ability = (int)e.CommandArgument;

            foreach (Stat stat in charStats)
            {
                if (stat.id == ability)
                {
                    stat.value++;
                    break;
                }
            }

            UpdateMods();
        }

        public void DecreaseAbility(object sender, CommandEventArgs e)
        {
            int ability = (int)e.CommandArgument;

            foreach (Stat stat in charStats)
            {
                if (stat.id == ability)
                {
                    if (stat.value > 0)
                    {
                        stat.value--;
                        break;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            UpdateMods();
        }

        public void SaveAbilityScores()
        {

        }

        public int CalcAbilityMod(int abilityScore)
        {
            int mod = 0;
            
            if (abilityScore < 10)
            {
                mod = (10 - abilityScore) / 2;
                mod *= -1;
            }
            else if (abilityScore > 11)
            {
                mod = (abilityScore - 10) / 2;
            }

            return mod;
        }
    }
}