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
    public partial class CharacterCreator : Page
    {
        public object CharacterID
        {
            get { return ViewState["ID"]; }
            set { ViewState["ID"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Page_Init(object sender, EventArgs e)
        {
            PopulateMenu();

            CharTypeRBList.SelectedIndexChanged += new EventHandler(CharTypeChanged);
            CharTypeRBList.AutoPostBack = true;

            RPGDD.SelectedIndexChanged += new EventHandler(RPGChanged);
            RPGDD.AutoPostBack = true;

            try
            {
                string rawId = Request.QueryString["id"];
                CharacterID = int.Parse(rawId);

                if ((int)CharacterID >= 0)
                {
                    CharIDTB.Text = CharacterID.ToString();

                    using (SQLiteConnection conn = new SQLiteConnection("Data Source=|DataDirectory|/charDev.db;Version=3;"))
                    {
                        conn.Open();

                        using (SQLiteCommand query = conn.CreateCommand())
                        {
                            query.CommandText = "SELECT * FROM Character WHERE Character_ID = " + CharacterID.ToString() + ";";

                            using (SQLiteDataReader reader = query.ExecuteReader())
                            {
                                reader.Read();

                                NameTB.Text = reader["Character_Name"].ToString();
                                DescTB.Text += reader["Character_Desc"].ToString();
                                PlayerTB.Text = reader["Player_Name"].ToString();
                                
                                CharTypeRBList.SelectedValue = reader["CharacterType_ID"].ToString();
                                CharTypeChanged();

                                RPGDD.SelectedValue = reader["RPG_ID"].ToString();
                                RPGChanged();
                                
                                if (reader["Campaign_ID"].ToString() != null)
                                {
                                    CampaignDD.SelectedValue = reader["Campaign_ID"].ToString();
                                }
                            }

                            query.Dispose();
                        }

                        conn.Close();
                    }
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

        public void PopulateMenu()
        {
            CharTypeRBList.Items.Clear();
            RPGDD.Items.Clear();

            RPGDD.Items.Add(new ListItem(""));

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
                                CharTypeRBList.Items.Add(new ListItem(CharTypeRead["CharacterType_Name"].ToString(), CharTypeRead["CharacterType_ID"].ToString()));
                            }
                        }

                        query.CommandText = "SELECT * FROM RPG;";

                        using (SQLiteDataReader RPGRead = query.ExecuteReader())
                        {
                            while (RPGRead.Read())
                            {
                                RPGDD.Items.Add(new ListItem(RPGRead["RPG_Name"].ToString(), RPGRead["RPG_ID"].ToString()));
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

        public void CharTypeChanged(object sender, EventArgs e)
        {
            CharTypeChanged();
        }

        public void CharTypeChanged()
        {
            if (CharTypeRBList.SelectedItem.Text == "NPC")
            {
                PlayerTB.Enabled = false;
                PlayerTB.Text = "";
            }
            else if (!PlayerTB.Enabled)
            {
                PlayerTB.Enabled = true;
            }
        }

        public void RPGChanged(object sender, EventArgs e)
        {
            RPGChanged();
        }

        public void RPGChanged()
        {
            CampaignDD.Items.Clear();
            CampaignDD.Items.Add("New Campaign");
            
            if (RPGDD.SelectedIndex > 0)
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
                            query.CommandText = "SELECT * FROM Campaign WHERE RPG_ID = " + RPGDD.SelectedValue + ";";

                            using (SQLiteDataReader CampaignListing = query.ExecuteReader())
                            {
                                while (CampaignListing.Read())
                                {
                                    CampaignDD.Items.Add(new ListItem(CampaignListing["Camp_DM"].ToString() + " - " + CampaignListing["Camp_Name"].ToString(), CampaignListing["Campaign_ID"].ToString()));
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

        public void ResetStartMenu(object sender, CommandEventArgs e)
        {
            NameTB.Text = "";
            PlayerTB.Text = "";
            DescTB.Text += "";

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

        public void SaveChanges(object sender, CommandEventArgs e)
        {
            try
            {
                if (CreationFieldCheck())
                {
                    SaveMainDetails();
                    SaveCommonDetails();
                    //SaveAbilityScores();
                }
            }
            catch (Exception err)
            {
                DescTB.Text += err.ToString();
            }
        }

        public void SaveMainDetails()
        {

        }
        
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
                    query.CommandText += "RPG_ID=" + RPGDD.SelectedIndex.ToString();

                    using (SQLiteDataReader AlignRead = query.ExecuteReader())
                    {
                        while (AlignRead.Read())
                        {
                            Common_RaceDD.Items.Add(AlignRead["Char_Race_Name"].ToString());
                        }
                    }

                    query.CommandText = "SELECT * FROM CharacterClass JOIN RPGClass WHERE ";
                    query.CommandText += "CharacterClass.Char_Class_ID = RPGClass.Char_Class_ID AND ";
                    query.CommandText += "RPG_ID=" + RPGDD.SelectedIndex.ToString();

                    using (SQLiteDataReader AlignRead = query.ExecuteReader())
                    {
                        while (AlignRead.Read())
                        {
                            Common_ClassDD.Items.Add(AlignRead["Char_Class_Name"].ToString());
                        }
                    }

                    query.CommandText = "SELECT * FROM CharacterAlign JOIN RPGAlign WHERE ";
                    query.CommandText += "CharacterAlign.Char_Align_ID = RPGAlign.Char_Align_ID AND ";
                    query.CommandText += "RPG_ID=" + RPGDD.SelectedIndex.ToString();

                    using (SQLiteDataReader AlignRead = query.ExecuteReader())
                    {
                        while(AlignRead.Read())
                        {
                            Common_AlignDD.Items.Add(AlignRead["Char_Align_Name"].ToString());
                        }
                    }

                    /*
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
                    */

                    query.Dispose();
                }

                conn.Close();
            }
        }

        public void SaveCommonDetails()
        {
            /*
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
                        //query.Parameters.Add(new SQLiteParameter("@CharID", devCharacter.id));

                        query.ExecuteNonQuery();

                        //devCharacter.commonInfo.id = (int)conn.LastInsertRowId;
                        //CommonIDTB.Text = devCharacter.commonInfo.id.ToString();
                    }

                    query.Dispose();
                }

                conn.Close();
            }
            */
        }
        
        public void SetAbilityScores()
        {
            /*
            AbilityTable.Rows.Clear();
            SkillTable.Rows.Clear();

            using (SQLiteConnection conn = new SQLiteConnection("Data Source=|DataDirectory|/charDev.db;Version=3;"))
            {
                conn.Open();

                using (SQLiteCommand query = conn.CreateCommand())
                {
                    query.CommandText += "SELECT * FROM RPG WHERE RPG_ID = " + RPGDD.SelectedIndex.ToString();

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
            */
        }

        public void UpdateMods()
        {
            /*
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
            */            
        }

        public void IncreaseAbility(object sender, CommandEventArgs e)
        {
            /*
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
            */
        }

        public void DecreaseAbility(object sender, CommandEventArgs e)
        {
            /*
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
            */
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