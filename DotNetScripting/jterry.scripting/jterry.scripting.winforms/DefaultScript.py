import clr
import System

clr.AddReference("System.Core")
clr.AddReference("System.Windows.Forms")
clr.ImportExtensions(System.Linq)

from System import DateTime
from System.Windows.Forms import Application, Form, Button

print "Old Title " + scriptEditor.Text
scriptEditor.Text = "Script Editor " + DateTime.Now.ToString()
print "New Title " + scriptEditor.Text