<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CharacterEditor.aspx.cs" Inherits="CharacterCreator.CharacterCreator" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Character Creator - Editor Page</title>
        <link href="~/content/CreatorStyle.css" type="text/css" media="screen" rel="stylesheet" />
        <link href="~/content/bootstrap.css" type="text/css" media="screen"  rel="stylesheet" />
        <script src="Scripts/jquery-1.10.2.js"></script>
        <script src="Scripts/bootstrap.js"></script>
        <script src="Scripts/angular.js"></script>
        <script src="Scripts/characterInfo.js"></script>
    </head>
    <body>
        <form id="form1" class="form-horizontal row" runat="server">
            <div id="IDShortcuts" hidden="hidden">
                <asp:HiddenField ID="CharacterID"   Value="-1"  runat="server" />
                <asp:HiddenField ID="Changes"       Value=""    runat="server" />
            </div>
            
            <script>
                $(document).ready(function () { 
                    $("#form1 :input").change(function () {
                        alert("HEY!");

                        // do whatever you need to do when something's changed.
                        // perhaps set up an onExit function on the window
                        document.getElementById("Changes").value = "Yup";

                        return false;
                    });
                });
            </script>

            <div class="container">
                <asp:LinkButton ID="SaveBtn" ToolTip="Save Changes" runat="server">
                    <i class="glyphicon glyphicon-floppy-disk"></i>
                </asp:LinkButton>
                <asp:LinkButton ID="CancelBtn" ToolTip="Cancel Changes" runat="server">
                    <i class="glyphicon glyphicon-floppy-remove"></i>
                </asp:LinkButton>

                <ul id="PrimaryMenus" class="nav nav-pills center-pills">
			        <li class="active">
                        <a  href="#1a" data-toggle="tab">Start Menu</a>
			        </li>
			        <li>
                        <a href="#2a" data-toggle="tab">Campaign Details</a>
			        </li>
  		            <li>
                      <a href="#3a" data-toggle="tab">Character Sheet</a>
			        </li>
                    <li>
                        <a  href="#1b" data-toggle="tab">Ability Scores</a>
			        </li>
			        <li>
                        <a href="#2b" data-toggle="tab">Skill List</a>
			        </li>
			        <li>
                        <a href="#3b" data-toggle="tab">Feats</a>
			        </li>
  		            <li>
                      <a href="#4b" data-toggle="tab">Bio/Notes</a>
			        </li>
		        </ul>

			    <div class="tab-content clearfix">
			        <div class="tab-pane active" id="1a">                        
                        <asp:Table ID="mainInfoTable" runat="server">
                            <asp:TableRow runat="server" CssClass="tableRow mandatoryField" BorderWidth="10px">
                                <asp:TableCell runat="server" CssClass="fieldLabel">
                                    <asp:Label ID="NameLB" runat="server" Text="Character Name:" />
                                </asp:TableCell>
                                <asp:TableCell runat="server">
                                    <asp:TextBox ID="NameTB" runat="server" />
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow runat="server" CssClass="tableRow mandatoryField" BorderWidth="10px">
                                <asp:TableCell runat="server" CssClass="fieldLabel">
                                    <asp:Label ID="CharTypeLB" runat="server" Text="Character Type:" />
                                </asp:TableCell>
                                <asp:TableCell runat="server">
                                    <asp:RadioButtonList ID="CharTypeRBList" runat="server" RepeatDirection="Horizontal" Width="40%">
                                    </asp:RadioButtonList>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow runat="server" CssClass="tableRow" BorderWidth="10px">
                                <asp:TableCell runat="server" CssClass="fieldLabel">
                                    <asp:Label ID="PlayerLB" runat="server" Text="Player Name:" />
                                </asp:TableCell>
                                <asp:TableCell runat="server">
                                    <asp:TextBox ID="PlayerTB" runat="server" />
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow runat="server" CssClass="tableRow mandatoryField" BorderWidth="10px">
                                <asp:TableCell runat="server" CssClass="fieldLabel">                
                                    <asp:Label ID="RPGLB" runat="server" Text="RPG:" />
                                </asp:TableCell>
                                <asp:TableCell runat="server">
                                    <asp:DropDownList ID="RPGDD" runat="server" />
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow runat="server" CssClass="tableRow" BorderWidth="10px">
                                <asp:TableCell runat="server" CssClass="fieldLabel">
                                    <asp:Label ID="CampaignLB" runat="server" Text="Current Campaign:" />
                                </asp:TableCell>
                                <asp:TableCell runat="server">
                                    <asp:DropDownList ID="CampaignDD" runat="server" />
                                </asp:TableCell>
                            </asp:TableRow>                            
                            <asp:TableRow runat="server" CssClass="tableRow" BorderWidth="10px">
                                <asp:TableCell runat="server" CssClass="fieldLabel">
                                    <asp:Label ID="LevelLB" runat="server" Text="Level:" />
                                </asp:TableCell>
                                <asp:TableCell runat="server">
                                    <asp:DropDownList ID="LevelDD" runat="server" />
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow runat="server" CssClass="tableRow" BorderWidth="10px">
                                <asp:TableCell runat="server" CssClass="fieldLabel">
                                    <asp:Label ID="RaceLB" runat="server" Text="Race:" />
                                </asp:TableCell>
                                <asp:TableCell runat="server">
                                    <asp:DropDownList ID="RaceDD" runat="server" />
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow runat="server" CssClass="tableRow" BorderWidth="10px">
                                <asp:TableCell runat="server" CssClass="fieldLabel">
                                    <asp:Label ID="ClassLB" runat="server" Text="Class:" />
                                </asp:TableCell>
                                <asp:TableCell runat="server">
                                    <asp:DropDownList ID="ClassDD" runat="server" />
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow runat="server" CssClass="tableRow" BorderWidth="10px">
                                <asp:TableCell runat="server" CssClass="fieldLabel">
                                    <asp:Label ID="AlignLB" runat="server" Text="Alignment:" />
                                </asp:TableCell>
                                <asp:TableCell runat="server">
                                    <asp:DropDownList ID="AlignDD" runat="server" />
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow runat="server" CssClass="tableRow" BorderWidth="10px">
                                <asp:TableCell runat="server" CssClass="fieldLabel">
                                    <asp:Label ID="SexLB" runat="server" Text="Sex:" />
                                </asp:TableCell>
                                <asp:TableCell runat="server">
                                    <asp:TextBox ID="SexTB" runat="server" />
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow runat="server" CssClass="tableRow" BorderWidth="10px">
                                <asp:TableCell runat="server" CssClass="fieldLabel">
                                    <asp:Label ID="DescLB" runat="server" Text="Description/Backstory:" />
                                </asp:TableCell>
                                <asp:TableCell runat="server">
                                    <asp:TextBox ID="DescTB" runat="server" TextMode="multiline" Columns="20" Rows="5" />
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
				    </div>

				    <div class="tab-pane" id="2a">                    
                        <asp:Table ID="CampaignTable" runat="server">
                            <asp:TableRow runat="server" CssClass="tableRow" BorderWidth="10px">
                                <asp:TableCell runat="server" CssClass="fieldLabel">
                                    <asp:Label ID="Label2" runat="server" Text="DM Name:" />
                                </asp:TableCell>
                                <asp:TableCell runat="server">
                                    <asp:TextBox ID="Camp_DMTB" runat="server" />
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow runat="server" CssClass="tableRow" BorderWidth="10px">
                                <asp:TableCell runat="server" CssClass="fieldLabel">
                                    <asp:Label ID="Label1" runat="server" Text="Campaign Name:" />
                                </asp:TableCell>
                                <asp:TableCell runat="server">
                                    <asp:TextBox ID="Camp_NameTB" runat="server" />
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow runat="server" CssClass="tableRow" BorderWidth="10px">
                                <asp:TableCell runat="server" CssClass="fieldLabel">
                                    <asp:Label ID="Label3" runat="server" Text="Campaign Type:" />
                                </asp:TableCell>
                                <asp:TableCell runat="server">
                                    <asp:TextBox ID="Camp_TypeTB" runat="server" />
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow runat="server" CssClass="tableRow" BorderWidth="10px">
                                <asp:TableCell runat="server" CssClass="fieldLabel">                
                                    <asp:Label ID="Label4" runat="server" Text="Campaign Desc:" />
                                </asp:TableCell>
                                <asp:TableCell runat="server">
                                    <asp:TextBox ID="Camp_DescTB" runat="server" TextMode="multiline" Columns="20" Rows="5" />
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow runat="server" CssClass="tableRow" BorderWidth="10px">
                                <asp:TableCell runat="server" CssClass="fieldLabel">                
                                    <asp:Label ID="Label9" runat="server" Text="Campaign Notes:" />
                                </asp:TableCell>
                                <asp:TableCell runat="server">
                                    <asp:TextBox ID="Camp_ExtraTB" runat="server" TextMode="multiline" Columns="20" Rows="5" />
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
				    </div>
                    
                    <div class="tab-pane" id="3a">
                        <h3 align="center">Coming Soon...</h3>
				    </div>

			        <div class="tab-pane" id="1b">                        
                        <asp:Table ID="AbilityTable" runat="server">
                        </asp:Table>
				    </div>

				    <div class="tab-pane" id="2b">                    
                        <asp:Table ID="SkillTable" runat="server">
                        </asp:Table>
				    </div>

                    <div class="tab-pane" id="3b">            
                        <asp:Table ID="FeatTable" runat="server">
                        </asp:Table>
				    </div>

                    <div class="tab-pane" id="4b">
                        <h3 align="center">Coming Soon...</h3>
				    </div>
			    </div>
            </div>
        </form>
    </body>
</html>
