<%@ Page Async="true" Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="WebBookStore._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:ListBox ID="ListBox1" runat="server" Height="175px" Width="827px"></asp:ListBox>
    
    </div>
        <p>
            <asp:TextBox ID="searchTextBox" runat="server" Width="817px"></asp:TextBox>
            <asp:Button ID="SearchBT" runat="server" OnClick="SearchBT_Click" Text="Search" />
            <asp:Button ID="ShowAllBT" runat="server" OnClick="ShowAllBT_Click" Text="Show All" />
            <asp:Button ID="AddToCartBT" runat="server" OnClick="AddToCartBT_Click" Text="Add to cart" />
            <asp:Button ID="CeckoutBT" runat="server" OnClick="CeckoutBT_Click" Text="Ceckout" />
        </p>
        <p>
            <div ID="CartLiteral" runat="server"></div>
        </p>
        <p>
            Info:</p>
        <p>
            <div ID="InfoLiteral" runat="server"></div>
        </p>
    </form>
</body>
</html>
