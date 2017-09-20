using System;
using System.Data.SQLite;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CharacterCreator
{
    public partial class MainPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Page_Init(object sender, EventArgs e)
        {
            PopulateMenu();

            charListDD.SelectedIndexChanged += new EventHandler(LoadCharChanged);
            charListDD.AutoPostBack = true;

            CharTypeRBList.SelectedIndexChanged += new EventHandler(CharTypeChanged);
            CharTypeRBList.AutoPostBack = true;

            RPGDD.SelectedIndexChanged += new EventHandler(RPGChanged);
            RPGDD.AutoPostBack = true;
            
            RefreshBtn.Command  += new CommandEventHandler(PopulateMenu);
            ClearBtn.Command    += new CommandEventHandler(ClearCharInfo);
            EditBtn.Command     += new CommandEventHandler(EditCharacter);
            CopyBtn.Command     += new CommandEventHandler(CopyCharacter);
        }

        public void PopulateMenu(object sender, CommandEventArgs e)
        {
            PopulateMenu();
        }

        public void PopulateMenu()
        {
            charListDD.Items.Clear();
            CharTypeRBList.Items.Clear();
            RPGDD.Items.Clear();

            charListDD.Items.Add(new ListItem("", "0"));
            RPGDD.Items.Add(new ListItem("", "0"));
            
            LoadCharChanged();

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

                            if (CharTypeRBList.Items.Count > 0)
                            {
                                CharTypeRBList.SelectedIndex = 0;
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
                        
                        query.CommandText = "SELECT * FROM Character " +
                                            "LEFT JOIN CharacterRace " + 
                                            "ON Character.Char_Race_ID = CharacterRace.Char_Race_ID " +
                                            "LEFT JOIN CharacterClass " +
                                            "ON Character.Char_Class_ID = CharacterClass.Char_Class_ID " +
                                            "ORDER BY Character_Name;";

                        using (SQLiteDataReader CharListRead = query.ExecuteReader())
                        {
                            while (CharListRead.Read())
                            {
                                charListDD.Items.Add(new ListItem(CharListRead["Character_Name"].ToString() + " - Lvl " + 
                                                                    CharListRead["Character_Level"].ToString() + " " + 
                                                                    CharListRead["Char_Race_Name"].ToString() + " " +
                                                                    CharListRead["Char_Class_Name"].ToString(), 
                                                                    CharListRead["Character_ID"].ToString()));
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

        public void LoadCharChanged(object sender, EventArgs e)
        {
            LoadCharChanged();
        }

        public void LoadCharChanged()
        {
            if (charListDD.SelectedIndex > 0)
            {
                try
                {
                    using (SQLiteConnection conn = new SQLiteConnection("Data Source=|DataDirectory|/charDev.db;Version=3;"))
                    {
                        conn.Open();

                        using (SQLiteCommand query = conn.CreateCommand())
                        {
                            query.CommandText = "SELECT * FROM Character JOIN Campaign " +
                                                "WHERE Character.Campaign_ID = Campaign.Campaign_ID AND Character_ID = " + charListDD.SelectedValue + ";";

                            using (SQLiteDataReader reader = query.ExecuteReader())
                            {
                                reader.Read();

                                NameTB.Text     = reader["Character_Name"].ToString();
                                PlayerTB.Text   = reader["Character_Player"].ToString();
                                SexTB.Text      = reader["Character_Sex"].ToString();
                                DescTB.Text     = reader["Character_Desc"].ToString();

                                CharTypeRBList.SelectedValue = reader["Char_Type_ID"].ToString();
                                CharTypeChanged();

                                RPGDD.SelectedValue = reader["RPG_ID"].ToString();
                                RPGChanged();

                                CampaignDD.SelectedValue    = reader["Campaign_ID"].ToString();

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

                                CharacterID.Value = charListDD.SelectedValue;
                            }
                            
                            query.Dispose();
                        }

                        conn.Close();
                    }

                    ToggleFields(false);
                }
                catch (Exception err)
                {
                    DescTB.Text += err.ToString();
                }
            }
            else
            {
                CharacterID.Value = "-1";
                ClearCharInfo();
                
                ToggleFields(true);
            }
        }

        public void ToggleFields(bool toggle)
        {
            if (toggle)
            {
                EditBtn.Text = "Create";
                ClearBtn.Text = "Clear";
            }
            else
            {
                EditBtn.Text = "Edit";
                ClearBtn.Text = "Delete";
            }

            NameTB.Enabled = toggle;
            PlayerTB.Enabled = toggle;
            SexTB.Enabled = toggle;
            DescTB.Enabled = toggle;

            CharTypeRBList.Enabled = toggle;

            RPGDD.Enabled = toggle;
            CampaignDD.Enabled = toggle;
            LevelDD.Enabled = toggle;
            RaceDD.Enabled = toggle;
            ClassDD.Enabled = toggle;
            AlignDD.Enabled = toggle;
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
        
        public void ClearCharInfo(object sender, CommandEventArgs e)
        {
            ClearCharInfo();
        }

        public void ClearCharInfo()
        {
            if (CharacterID.Value != "-1")
            {
                try
                {
                    using (SQLiteConnection conn = new SQLiteConnection("Data Source=|DataDirectory|/charDev.db;Version=3;"))
                    {
                        conn.Open();

                        using (SQLiteCommand query = conn.CreateCommand())
                        {
                            query.CommandText += "DELETE FROM Character WHERE Character_ID = " + CharacterID.Value;
                            query.ExecuteNonQuery();
                            query.Dispose();

                            charListDD.SelectedIndex = 0;
                            CharacterID.Value = "-1";
                        }

                        conn.Close();
                    }
                }
                catch (Exception err)
                {
                    DescTB.Text += err.ToString();
                }
            }

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
                RPGChanged();
            }
        }
        
        public void EditCharacter(object sender, CommandEventArgs e)
        {
            if (charListDD.SelectedIndex > 0)
            {
                CharacterID.Value = charListDD.SelectedValue;
            }
            else if (CreationFieldCheck())
            {
                CreateCharacter();
            }
            else
            {
                DescTB.Text += "...CHARACTER NOT CREATED!";
                return;
            }

            if (CharacterID.Value != "-1")
            {
                Response.Redirect("CharacterEditor.aspx?id=" + CharacterID.Value);
            }
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
            else if (RPGDD.Items.Count == 0 || RPGDD.SelectedIndex < 1)
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

        public void CreateCharacter()
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection("Data Source=|DataDirectory|/charDev.db;Version=3;"))
                {
                    conn.Open();

                    using (SQLiteCommand query = conn.CreateCommand())
                    {
                        string campaignID = CampaignDD.SelectedValue;

                        if (CampaignDD.SelectedIndex == 0)
                        {
                            query.CommandText += "INSERT INTO Campaign (Campaign_Name, RPG_ID) VALUES ('New " + RPGDD.SelectedItem.Text + " Campaign', " + RPGDD.SelectedValue + ");";
                            query.ExecuteNonQuery();

                            campaignID = conn.LastInsertRowId.ToString();
                        }

                        query.CommandText = "INSERT INTO Character (";
                        query.CommandText += "Character_Name, ";
                        query.CommandText += "Character_Player, ";
                        query.CommandText += "Character_Sex, ";
                        query.CommandText += "Character_Desc, ";
                        query.CommandText += "Campaign_ID, ";
                        query.CommandText += "Character_Level, ";
                        query.CommandText += "Char_Type_ID, ";
                        query.CommandText += "Char_Race_ID, ";
                        query.CommandText += "Char_Class_ID, ";
                        query.CommandText += "Char_Align_ID";
                        query.CommandText += ") VALUES (";
                        query.CommandText += "@Name, ";
                        query.CommandText += "@Player, ";
                        query.CommandText += "@Sex, ";
                        query.CommandText += "@Desc, ";
                        query.CommandText += "@Campaign, ";
                        query.CommandText += "@Level, ";
                        query.CommandText += "@Type, ";
                        query.CommandText += "@Race, ";
                        query.CommandText += "@Class, ";
                        query.CommandText += "@Align";
                        query.CommandText += ");";

                        query.Parameters.Add(new SQLiteParameter("@Name",       NameTB.Text));
                        query.Parameters.Add(new SQLiteParameter("@Player",     PlayerTB.Text));
                        query.Parameters.Add(new SQLiteParameter("@Sex",        SexTB.Text));
                        query.Parameters.Add(new SQLiteParameter("@Desc",       DescTB.Text));
                        query.Parameters.Add(new SQLiteParameter("@Campaign",   campaignID));

                        query.Parameters.Add(new SQLiteParameter("@Level",  LevelDD.SelectedValue));
                        query.Parameters.Add(new SQLiteParameter("@Type",   CharTypeRBList.SelectedValue));
                        query.Parameters.Add(new SQLiteParameter("@Race",   RaceDD.SelectedValue));
                        query.Parameters.Add(new SQLiteParameter("@Class",  ClassDD.SelectedValue));
                        query.Parameters.Add(new SQLiteParameter("@Align",  AlignDD.SelectedValue));

                        query.ExecuteNonQuery();

                        CharacterID.Value = conn.LastInsertRowId.ToString();

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

        public void CopyCharacter(object sender, CommandEventArgs e)
        {
            try
            { 
                using (SQLiteConnection conn = new SQLiteConnection("Data Source=|DataDirectory|/charDev.db;Version=3;"))
                {
                    conn.Open();

                    using (SQLiteCommand query = conn.CreateCommand())
                    {
                        query.CommandText = "INSERT INTO Character (";
                        query.CommandText += "Character_Name, ";
                        query.CommandText += "Character_Player, ";
                        query.CommandText += "Character_Sex, ";
                        query.CommandText += "Character_Desc, ";
                        query.CommandText += "Campaign_ID, ";
                        query.CommandText += "Character_Level, ";
                        query.CommandText += "Char_Type_ID, ";
                        query.CommandText += "Char_Race_ID, ";
                        query.CommandText += "Char_Class_ID, ";
                        query.CommandText += "Char_Align_ID";
                        query.CommandText += ") SELECT ";
                        query.CommandText += "Character_Name || ' (Copy)', ";
                        query.CommandText += "Character_Player, ";
                        query.CommandText += "Character_Sex, ";
                        query.CommandText += "Character_Desc, ";
                        query.CommandText += "Campaign_ID, ";
                        query.CommandText += "Character_Level, ";
                        query.CommandText += "Char_Type_ID, ";
                        query.CommandText += "Char_Race_ID, ";
                        query.CommandText += "Char_Class_ID, ";
                        query.CommandText += "Char_Align_ID ";
                        query.CommandText += "FROM Character ";
                        query.CommandText += "WHERE Character_ID = " + CharacterID.Value + ";";
                        
                        query.ExecuteNonQuery();

                        string newCharID = conn.LastInsertRowId.ToString();
                        PopulateMenu();

                        charListDD.SelectedValue = newCharID;
                        LoadCharChanged();                        

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
}