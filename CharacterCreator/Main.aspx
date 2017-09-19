<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="CharacterCreator.MainPage" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Character Creator - Main Page</title>
        <link href="~/content/CreatorStyle.css" type="text/css" media="screen" rel="stylesheet" />
        <link href="~/content/bootstrap.css" type="text/css" media="screen"  rel="stylesheet" />
        <script src="Scripts/jquery-1.10.2.js"></script>
        <script src="Scripts/bootstrap.js"></script>
        <script src="Scripts/angular.js"></script>
    </head>
    <body>
        <form id="form1" runat="server">
        <div>   
            <asp:HiddenField ID="Character" Value="-1" runat="server" />             
            <asp:Table ID="loadCharTable" runat="server">
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">
                        <asp:Label ID="InstructLB" Text="Load a character from the List below, <br>
                                                    or create a new character with the form to the right..." runat="server" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">
                        <asp:DropDownList ID="charListDD" runat="server" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server">
                    <asp:TableCell runat="server">
                        <asp:Button ID="EditBtn" Text="Edit" runat="server" />
                        <asp:Button ID="CopyBtn" Text="Duplicate" runat="server" />
                        <asp:Button ID="DeleteBtn" Text="Delete" runat="server" />
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>            
        
            <asp:Table ID="newCharTable" runat="server">
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
                        <asp:Label ID="DescLB" runat="server" Text="Description/Backstory:" />
                    </asp:TableCell>
                    <asp:TableCell runat="server">
                        <asp:TextBox ID="DescTB" runat="server" TextMode="multiline" Columns="20" Rows="5" />
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow runat="server" CssClass="tableRow" BorderWidth="10px" HorizontalAlign="Center">
                    <asp:TableCell runat="server" HorizontalAlign="Center" ColumnSpan="2">
                        <asp:Button ID="NewCharBtn" runat="server" Text="Create" />
                        <asp:LinkButton ID="RefreshBtn" OnClientClick="Response.Redirect(Request.RawUrl);" ToolTip="Refresh List" runat="server">
                            <i class="glyphicon glyphicon-refresh"></i>
                        </asp:LinkButton>
                        <asp:Button ID="ClearBtn" runat="server" Text="Clear" />                    
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
        </div>
        </form>
    </body>
</html>
