<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="About.aspx.cs" Inherits="jterry.scripting.web.About" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        About
    </h2>
    <p>Script Editor runs <a href="http://ironpython.net/" target="_blank">Iron Python</a> scripts on the web server, 
    allowing developers to test their .NET API's directly in a browser.</p>

    <p>To use the Script Editor, developers should know a little bit of Python. The Iron Python Dot Net 
    <a href="http://ironpython.net/documentation/dotnet/dotnet.html" target="_blank">documentation</a> would be
    a good place to start.</p>

    <p>The example script provided should contain enough of the Python syntax to get started.</p>
</asp:Content>
