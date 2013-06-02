<%@ Page Language="C#" AutoEventWireup="true" 
    CodeBehind="ScriptEditor.aspx.cs" 
    Inherits="jterry.scripting.web.ScriptEditor" 
    ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

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
            var editor = CodeMirror.fromTextArea(document.getElementById("_script"), {
                mode: { name: "python",
                    version: 2,
                    singleLineStringErrors: false
                },
                lineNumbers: true,
                indentUnit: 4,
                tabMode: "shift",
                matchBrackets: true
            });

            editor.setSize(960, 500)

            $("#helpDialog").dialog({
                autoOpen: false,
                width: 800,
                height: 500
            });

            $("#showHelpBtn").click(function () {
                $("#helpDialog").dialog('open');
                return false;
            });
        });
    </script>

    <style type="text/css">
        .scriptEditor  {
            width: 100%;
            height: 800px;
            font-size: 8pt;
        }
        
        .scriptResults  {
            width: 100%;
            height: 50px;
            font-size: 8pt;
            border: 0;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <fieldset>
        <legend>Script</legend>
        
        <asp:TextBox ID="_script" runat="server" TextMode="MultiLine" CssClass="scriptEditor">import clr
clr.AddReference("System.Core")
import System
from System import *
from System.Collections.Generic import List
clr.ImportExtensions(System.Linq)

def printCurrentTime():
    print DateTime.Now.ToString()
    return

def getRepository(name):
    return factory.GetRepository(name)

def getAllEntities(repo):
    return repo.GetAll()

printCurrentTime()
repo = getRepository("Customers")
customers = getAllEntities(repo)
print "Initial customer count: " + customers.Count.ToString()
customer1 = repo.CreateEntity()
customer1.Name = "My Customer"
print "Id: " + customer1.Id.ToString() + " Name: " + customer1.Name
print "Customer count after adding a customer: " + customers.Count.ToString()</asp:TextBox>
        
        <asp:Button ID="_btnRunScript" runat="server" onclick="_btnRunScript_Click" Text="Run Script" />
        <input id="showHelpBtn" type="submit" value="Help" />

        </fieldset>

        <fieldset>
        <legend>Output</legend>
        
        <asp:TextBox ID="_output" runat="server" CssClass="scriptResults" ReadOnly="true" 
                TextMode="MultiLine"></asp:TextBox>
        
        </fieldset>
    </div>

    <div id="helpDialog" title="Script Editor Help">
        <p>Script Editor runs <a href="http://ironpython.net/" target="_blank">Iron Python</a> scripts on the web server, 
        allowing developers to test their .NET API's directly in a browser.</p>

        <p>To use the Script Editor, developers should know a little bit of Python. The example script provided
        should contain enough of the Python syntax to get started.</p>
    </div>
    </form>
</body>
</html>
