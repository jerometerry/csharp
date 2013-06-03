import clr
clr.AddReference("System.Core")
clr.AddReference("System.Windows.Forms")
import System
from System import DateTime
clr.ImportExtensions(System.Linq)
from System.Windows.Forms import Application, Form, Button

print "Old Title " + scriptEditor.Text
scriptEditor.Text = "Script Editor " + DateTime.Now.ToString()
print "New Title " + scriptEditor.Text