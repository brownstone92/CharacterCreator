using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SQLite;

namespace CharacterCreator
{
    public partial class CharacterCreator : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                string rawId = Request.QueryString["id"];
                CharacterID.Value = rawId;

                PopulateMenu();
                PopulateFields();

                CharTypeRBList.SelectedIndexChanged += new EventHandler(CharTypeChanged);
                CharTypeRBList.AutoPostBack = true;

                RPGDD.SelectedIndexChanged += new EventHandler(RPGChanged);
                RPGDD.AutoPostBack = true;

                CampaignDD.SelectedIndexChanged += new EventHandler(CampaignChanged);
                CampaignDD.AutoPostBack = true;

                SaveBtn.Command += new CommandEventHandler(SaveChanges);
                CancelBtn.Command += new CommandEventHandler(CancelChanges);
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

            RPGDD.Items.Add(new ListItem("", "0"));
            RPGChanged();

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
                                CharTypeRBList.Items.Add(new ListItem(CharTypeRead["Char_Type_Name"].ToString(), CharTypeRead["Char_Type_ID"].ToString()));
                            }
                        }

                        query.CommandText = "SELECT * FROM RPG ORDER BY RPG_Name;";

                        using (SQLiteDataReader RPGRead = query.ExecuteReader())
                        {
                            while (RPGRead.Read())
                            {
                                RPGDD.Items.Add(new ListItem(RPGRead["RPG_Name"].ToString(), RPGRead["RPG_ID"].ToString()));
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

        public void PopulateFields()
        {
            try
            {
                if (int.Parse(CharacterID.Value) >= 0)
                {
                    using (SQLiteConnection conn = new SQLiteConnection("Data Source=|DataDirectory|/charDev.db;Version=3;"))
                    {
                        conn.Open();

                        using (SQLiteCommand query = conn.CreateCommand())
                        {
                            query.CommandText = "SELECT * FROM Character JOIN Campaign WHERE Character.Campaign_ID = Campaign.Campaign_ID AND Character_ID = " + CharacterID.Value + ";";

                            using (SQLiteDataReader reader = query.ExecuteReader())
                            {
                                reader.Read();

                                NameTB.Text = reader["Character_Name"].ToString();
                                DescTB.Text = reader["Character_Desc"].ToString();
                                PlayerTB.Text = reader["Character_Player"].ToString();

                                CharTypeRBList.SelectedValue = reader["Char_Type_ID"].ToString();
                                CharTypeChanged();

                                RPGDD.SelectedValue = reader["RPG_ID"].ToString();
                                RPGChanged();

                                if (reader["Character_Level"].ToString() != "")
                                {
                                    LevelDD.SelectedValue = reader["Character_Level"].ToString();
                                }

                                if (reader["Char_Race_ID"].ToString() != "")
                                {
                                    RaceDD.SelectedValue = reader["Char_Race_ID"].ToString();
                                }

                                if (reader["Char_Class_ID"].ToString() != "")
                                {
                                    ClassDD.SelectedValue = reader["Char_Class_ID"].ToString();
                                }

                                if (reader["Char_Align_ID"].ToString() != "")
                                {
                                    AlignDD.SelectedValue = reader["Char_Align_ID"].ToString();
                                }

                                CampaignDD.SelectedValue = reader["Campaign_ID"].ToString();
                                CampaignChanged();
                            }

                            query.Dispose();
                        }

                        conn.Close();
                    }
                }
                else
                {

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
            LevelDD.Items.Clear();
            RaceDD.Items.Clear();
            ClassDD.Items.Clear();
            AlignDD.Items.Clear();

            CampaignDD.Items.Add(new ListItem("New Campaign", "0"));
            LevelDD.Items.Add(new ListItem("", "0"));
            RaceDD.Items.Add(new ListItem("", "0"));
            ClassDD.Items.Add(new ListItem("", "0"));
            AlignDD.Items.Add(new ListItem("", "0"));

            LevelDD.Enabled = true;
            RaceDD.Enabled = true;
            ClassDD.Enabled = true;
            AlignDD.Enabled = true;

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
                            query.CommandText = "SELECT * FROM Campaign WHERE RPG_ID = " + RPGDD.SelectedValue + " ORDER BY Campaign_Name;";

                            using (SQLiteDataReader CampaignListing = query.ExecuteReader())
                            {
                                while (CampaignListing.Read())
                                {
                                    CampaignDD.Items.Add(new ListItem(CampaignListing["Campaign_DM"].ToString() + "; '" + CampaignListing["Campaign_Name"].ToString() + "'", 
                                                                        CampaignListing["Campaign_ID"].ToString()));
                                }
                            }

                            query.CommandText = "SELECT * FROM RPG WHERE RPG_ID = " + RPGDD.SelectedValue + ";";

                            using (SQLiteDataReader LevelCap = query.ExecuteReader())
                            {
                                int cap = 0;

                                if (LevelCap.Read())
                                {
                                    cap = int.Parse(LevelCap["RPG_Level_Cap"].ToString());
                                }

                                if (cap > 0)
                                {
                                    for (int i = 1; i <= cap; i++)
                                    {
                                        LevelDD.Items.Add(new ListItem(i.ToString(), i.ToString()));
                                    }
                                }
                                else
                                {
                                    LevelDD.Items[0].Text = "N/A";
                                    LevelDD.Enabled = false;
                                }
                            }

                            query.CommandText = "SELECT * FROM CharacterRace JOIN RPGRace " +
                                                "WHERE CharacterRace.Char_Race_ID = RPGRace.Char_Race_ID " +
                                                "AND RPG_ID = " + RPGDD.SelectedValue + " ORDER BY Char_Race_Name;";

                            using (SQLiteDataReader RaceListing = query.ExecuteReader())
                            {
                                while (RaceListing.Read())
                                {
                                    RaceDD.Items.Add(new ListItem(RaceListing["Char_Race_Name"].ToString(), RaceListing["Char_Race_ID"].ToString()));
                                }

                                if (RaceDD.Items.Count < 2)
                                {
                                    RaceDD.Items[0].Text = "N/A";
                                    RaceDD.Enabled = false;
                                }
                            }

                            query.CommandText = "SELECT * FROM CharacterClass JOIN RPGClass " +
                                                "WHERE CharacterClass.Char_Class_ID = RPGClass.Char_Class_ID " +
                                                "AND RPG_ID = " + RPGDD.SelectedValue + " ORDER BY Char_Class_Name;";

                            using (SQLiteDataReader ClassListing = query.ExecuteReader())
                            {
                                while (ClassListing.Read())
                                {
                                    ClassDD.Items.Add(new ListItem(ClassListing["Char_Class_Name"].ToString(), ClassListing["Char_Class_ID"].ToString()));
                                }

                                if (ClassDD.Items.Count < 2)
                                {
                                    ClassDD.Items[0].Text = "N/A";
                                    ClassDD.Enabled = false;
                                }
                            }

                            query.CommandText = "SELECT * FROM CharacterAlign JOIN RPGAlign " +
                                                "WHERE CharacterAlign.Char_Align_ID = RPGAlign.Char_Align_ID " +
                                                "AND RPG_ID = " + RPGDD.SelectedValue + ";";

                            using (SQLiteDataReader AlignListing = query.ExecuteReader())
                            {
                                while (AlignListing.Read())
                                {
                                    AlignDD.Items.Add(new ListItem(AlignListing["Char_Align_Name"].ToString(), AlignListing["Char_Align_ID"].ToString()));
                                }

                                if (AlignDD.Items.Count < 2)
                                {
                                    AlignDD.Items[0].Text = "N/A";
                                    AlignDD.Enabled = false;
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

        public void CampaignChanged(object sender, EventArgs e)
        {
            CampaignChanged();
        }

        public void CampaignChanged()
        {
            Camp_NameTB.Text = "";
            Camp_DMTB.Text = "";
            Camp_TypeTB.Text = "";
            Camp_DescTB.Text = "";
            Camp_ExtraTB.Text = "";
            
            if (CampaignDD.SelectedIndex > 0)
            {
                try
                {
                    using (SQLiteConnection conn = new SQLiteConnection("Data Source=|DataDirectory|/charDev.db;Version=3;"))
                    {
                        conn.Open();

                        using (SQLiteCommand query = conn.CreateCommand())
                        {
                            query.CommandText = "SELECT * FROM Campaign WHERE Campaign_ID = " + CampaignDD.SelectedValue + ";";

                            using (SQLiteDataReader reader = query.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    Camp_NameTB.Text    = reader["Campaign_Name"].ToString();
                                    Camp_DMTB.Text      = reader["Campaign_DM"].ToString();
                                    Camp_TypeTB.Text    = reader["Campaign_Type"].ToString();
                                    Camp_DescTB.Text    = reader["Campaign_Desc"].ToString();
                                    Camp_ExtraTB.Text   = reader["Campaign_Extra"].ToString();
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

        public void CancelChanges(object sender, CommandEventArgs e)
        {
            PopulateFields();
        }

        public bool CreationFieldCheck()
        {
            bool pass = true;

            if (NameTB.Text == "")
            {
                pass = false;
            }
            else if (LevelDD.Items.Count > 1 && LevelDD.SelectedIndex < 1)
            {
                pass = false;
            }
            else if (RaceDD.Items.Count > 1 && RaceDD.SelectedIndex < 1)
            {
                pass = false;
            }
            else if (ClassDD.Items.Count > 1 && ClassDD.SelectedIndex < 1)
            {
                pass = false;
            }
            else if (AlignDD.Items.Count > 1 && AlignDD.SelectedIndex < 1)
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
                    //SaveAbilityScores();

                    Changes.Value = "";
                }
            }
            catch (Exception err)
            {
                DescTB.Text += err.ToString();
            }
        }

        public void SaveMainDetails()
        {
            DescTB.Text += "Attempting to Save...";

            if (Changes.Value.Length > 0)
            {
                try
                {
                    using (SQLiteConnection conn = new SQLiteConnection("Data Source=|DataDirectory|/charDev.db;Version=3;"))
                    {
                        conn.Open();

                        using (SQLiteCommand query = conn.CreateCommand())
                        {
                            query.CommandText += "UPDATE Character SET ";
                            query.CommandText += "Character_Name = '" + NameTB.Text + "' ";
                            query.CommandText += "WHERE Character_ID = " + CharacterID.Value;

                            query.ExecuteNonQuery();
                            query.Dispose();
                        }

                        conn.Close();
                    }
                }
                catch (Exception err)
                {
                    DescTB.Text += err.ToString();
                }

                DescTB.Text += "Save Complete!";
            }
            else
            {
                DescTB.Text += "Save Failed!";
            }
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
                    query.CommandText += "SELECT * FROM RPG WHERE RPG_ID = " + RPGDD.SelectedValue;

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