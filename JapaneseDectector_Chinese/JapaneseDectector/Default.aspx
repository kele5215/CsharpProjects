<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="JapaneseDectector._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>日本語・中国語抽出ツール</title>
    <script type="text/javascript">
        function doCheckedChanged() {
            var checkbox;
            var array;
            var cb;

            checkbox = document.getElementById("cbSelectAll");
            if (checkbox != null) {
                array = document.all;
                for (var i = 0; i < array.length; i++) {
                    if (array[i].type == "checkbox") {
                        array[i].checked = checkbox.checked;
                    }
                }
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:TextBox ID="txtPath" Width="300px" runat="server" />
            <asp:Button ID="btnRun" runat="server" Text="日本語抽出" OnClick="btnRun_Click" />
            <asp:Button ID="btnGetWords" runat="server" Text="中国語抽出" OnClick="btnRun_Click_C" />
            <asp:Button ID="btnGet" runat="server" Text="Get拡張子" />
        </div>
        <div>
            <asp:TextBox ID="txtFileSave" Width="300px" runat="server" />
            <asp:Button ID="Button3" runat="server" Text="ファイル抽出"  OnClick="btnFileGet_Click" />
        </div>
        <div>
        </div>
        <div>
            <asp:CheckBox ID="cbSQL" runat="server" Text=".sql" />
            &nbsp;<asp:CheckBox ID="cbCMD" runat="server" Text=".cmd" />
            &nbsp;<asp:CheckBox ID="cbLOG" runat="server" Text=".log" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; |&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:CheckBox ID="cbCS" runat="server" Text=".cs" />
            &nbsp;<asp:CheckBox ID="cbASPX" runat="server" Text=".aspx" />
            &nbsp;<asp:CheckBox ID="cbRESX" runat="server" Text=".resx" />
            &nbsp;<asp:CheckBox ID="cbXAML" runat="server" Text=".xaml" />
            &nbsp;<asp:CheckBox ID="cbJS" runat="server" Text=".js" />
            &nbsp;<asp:CheckBox ID="cbCSS" runat="server" Text=".css" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; |&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:CheckBox ID="cbCONFIG" runat="server" Text=".config" />
            &nbsp;<asp:CheckBox ID="cbINI" runat="server" Text=".ini" />
            &nbsp;<asp:CheckBox ID="cbTXT" runat="server" Text=".txt" />
            &nbsp;<asp:CheckBox ID="cbXML" runat="server" Text=".xml" />
            &nbsp;<asp:CheckBox ID="cbHTM" runat="server" Text=".htm" />
            &nbsp;<asp:CheckBox ID="cbHTML" runat="server" Text=".html" />
            &nbsp;<asp:CheckBox ID="cbH" runat="server" Text=".h" />
            &nbsp;<asp:CheckBox ID="cbCPP" runat="server" Text=".cpp" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; |&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:CheckBox ID="cbISS" runat="server" Text=".iss" />
            <asp:CheckBox ID="cbXSLT" runat="server" Text=".xslt" />
            <asp:CheckBox ID="cbVBS" runat="server" Text=".vbs" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; |&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:CheckBox ID="cbWSF" runat="server" Text=".wsf" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; |&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:CheckBox ID="cbAll" runat="server" Text=".*" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; |&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:CheckBox ID="cbSelectAll" runat="server" onclick="doCheckedChanged();" Text="全選択" />
        </div>
        <asp:TextBox ID="lblResult" runat="server" Width="100%" Rows="60" TextMode="MultiLine"></asp:TextBox>
    </form>
</body>
</html>
