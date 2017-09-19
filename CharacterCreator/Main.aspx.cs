using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CharacterCreator
{
    public partial class MainPage : Page
    {
        public object CharacterInfo
        {
            get
            {
                return Character.Value;
            }
        }

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
            
            EditBtn.Command += new CommandEventHandler(LoadCharacter);
            ClearBtn.Command += new CommandEventHandler(ClearCharInfo);
            NewCharBtn.Command += new CommandEventHandler(CreateCharacter);
            RefreshBtn.Command += new CommandEventHandler(PopulateMenu);

            DescTB.Text += "RELOADING!!!";
        }

        public void PopulateMenu(object sender, CommandEventArgs e)
        {
            PopulateMenu();
        }

        public void PopulateMenu()
        {
            CharTypeRBList.Items.Clear();
            charListDD.Items.Clear();
            RPGDD.Items.Clear();

            charListDD.Items.Add(new ListItem("", "0"));
            RPGDD.Items.Add(new ListItem("", "0"));

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

                        query.CommandText = "SELECT * FROM Character;";

                        using (SQLiteDataReader CharListRead = query.ExecuteReader())
                        {
                            while (CharListRead.Read())
                            {
                                charListDD.Items.Add(new ListItem(CharListRead["Character_Name"].ToString(), CharListRead["Character_ID"].ToString()));
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
                            query.CommandText = "SELECT * FROM Character WHERE Character_ID = " + charListDD.SelectedValue + ";";

                            using (SQLiteDataReader reader = query.ExecuteReader())
                            {
                                reader.Read();

                                NameTB.Text = reader["Character_Name"].ToString();
                                PlayerTB.Text = reader["Player_Name"].ToString();
                                DescTB.Text = reader["Character_Desc"].ToString();

                                CharTypeRBList.SelectedIndex = int.Parse(reader["CharacterType_ID"].ToString()) - 1;
                                RPGDD.SelectedIndex = int.Parse(reader["RPG_ID"].ToString());

                                RPGChanged();

                                int campaignID = 0;
                                if (int.TryParse(reader["Campaign_ID"].ToString(), out campaignID))
                                {
                                    CampaignDD.SelectedIndex = campaignID;
                                }

                                Character.Value = charListDD.SelectedValue;
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
            else
            {
                Character.Value = "-1";
                ClearCharInfo();
            }
        }

        public void CharTypeChanged(object sender, EventArgs e)
        {
            CharTypeChanged();
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

        public void RPGChanged(object sender, EventArgs e)
        {
            RPGChanged();
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
                            query.CommandText = "SELECT * FROM Campaign WHERE RPG_ID = " + RPGDD.SelectedIndex.ToString() + ";";

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
        
        public void ClearCharInfo(object sender, CommandEventArgs e)
        {
            ClearCharInfo();
        }

        public void ClearCharInfo()
        {
            NameTB.Text = "";
            PlayerTB.Text = "";
            DescTB.Text = "";

            if (CharTypeRBList.Items.Count > 0)
            {
                CharTypeRBList.SelectedIndex = 0;
            }

            if (RPGDD.Items.Count > 0)
            {
                RPGDD.SelectedIndex = 0;
            }
        }
        
        public void LoadCharacter(object sender, CommandEventArgs e)
        {
            if (charListDD.SelectedIndex > 0)
            {
                Character.Value = charListDD.SelectedValue;
                EditCharacter();
            }
        }

        public void EditCharacter()
        {
            Response.Redirect("CharacterEditor.aspx?id=" + Character.Value);
        }
        
        public void CreateCharacter(object sender, CommandEventArgs e)
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

                    query.Parameters.Add(new SQLiteParameter("@Name", NameTB.Text));
                    query.Parameters.Add(new SQLiteParameter("@Desc", DescTB.Text));
                    query.Parameters.Add(new SQLiteParameter("@Player", PlayerTB.Text));
                    query.Parameters.Add(new SQLiteParameter("@Type", CharTypeRBList.SelectedIndex + 1));
                    query.Parameters.Add(new SQLiteParameter("@RPG", RPGDD.SelectedIndex));

                    if (CampaignDD.SelectedIndex > 0)
                    {
                        query.Parameters.Add(new SQLiteParameter("@Campaign", CampaignDD.SelectedIndex));
                    }
                    else
                    {
                        query.Parameters.Add(new SQLiteParameter("@Campaign", null));
                    }

                    query.ExecuteNonQuery();

                    Character.Value = conn.LastInsertRowId.ToString();

                    query.Dispose();
                }

                conn.Close();
            }

            EditCharacter();
        }
    }
}