<%@ Page Title="Script Editor" Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.master" 
    CodeBehind="ScriptEditor.aspx.cs" Inherits="jterry.scripting.web.ScriptEditor" ValidateRequest="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <link href="<%=Page.ResolveUrl("~/Content/themes/base/minified/jquery-ui.min.css")%>" rel="stylesheet" type="text/css" />
    <link href="<%=Page.ResolveUrl("~/Scripts/codemirror/lib/codemirror.css")%>" rel="stylesheet" type="text/css" />

    <script src="<%=Page.ResolveUrl("~/Scripts/jquery-2.0.1.min.js")%>" type="text/javascript"></script>
    <script src="<%=Page.ResolveUrl("~/Scripts/jquery-ui-1.10.3.min.js")%>" type="text/javascript"></script>
    <script src="<%=Page.ResolveUrl("~/Scripts/codemirror/lib/codemirror.js")%>" type="text/javascript"></script>
    <script src="<%=Page.ResolveUrl("~/Scripts/codemirror/mode/python/python.js")%>" type="text/javascript"></script>
    <script src="<%=Page.ResolveUrl("~/Scripts/codemirror/addon/edit/matchbrackets.js")%>" type="text/javascript"></script>
    <script src="<%=Page.ResolveUrl("~/Scripts/codemirror/addon/fold/foldcode.js")%>" type="text/javascript"></script>
    <script src="<%=Page.ResolveUrl("~/Scripts/codemirror/addon/fold/brace-fold.js")%>" type="text/javascript"></script>
    <script src="<%=Page.ResolveUrl("~/Scripts/codemirror/addon/fold/xml-fold.js")%>" type="text/javascript"></script>

    <script type="text/javascript">

        $(document).ready(function () {
            var editor = CodeMirror.fromTextArea(document.getElementById("MainContent__script"), {
                mode: { name: "python",
                    version: 2,
                    singleLineStringErrors: false
                },
                lineNumbers: true,
                indentUnit: 4,
                tabMode: "shift",
                matchBrackets: true
            });

            editor.setSize(900, 300)
        });
    </script>

    <style type="text/css">
        .scriptEditor  {
            font-size: 10pt;
        }
        
        .scriptResults  {
            width: 100%;
            height: 100px;
            font-size: 8pt;
            border: 0;
        }
        
        legend { 
          padding-top: 0;
          padding-bottom; }
        fieldset, td { 
          padding-top: 0;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div>
        <fieldset>
        <legend>Script</legend>
        
        <asp:TextBox ID="_script" runat="server" TextMode="MultiLine" CssClass="scriptEditor"></asp:TextBox>
        
        <asp:Button ID="_btnRunScript" runat="server" onclick="_btnRunScript_Click" Text="Run Script" />

        </fieldset>

        <fieldset>
        <legend>Output</legend>
        
        <asp:TextBox ID="_output" runat="server" CssClass="scriptResults" ReadOnly="true" 
                TextMode="MultiLine"></asp:TextBox>
        
        </fieldset>
    </div>
</asp:Content>
