using Microsoft.VisualBasic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace UML_Generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private XmlNodeList xmlFile;
        private List<ClassStruct> AllClasses = new List<ClassStruct>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "XML-Dateien (*.xml)|*.xml|Alle Dateien (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;

                this.OpenXMLDocument(fileName);

                this.FileName.Text = fileName;
            }
        }

        private void OpenXMLDocument(string fileName)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(fileName);

            //XmlNodeList elements = xmlDocument.SelectNodes("//mxCell");
            xmlFile = xmlDocument.SelectNodes("//mxCell");

            this.LoadAllClassnames();

            this.LoadAllFunctions();

            this.LoadAllVariables();

            this.PrintToFormsClasses();
        }

        private void LoadAllClassnames()
        {
            foreach (XmlNode node in xmlFile)
            {
                if (node.Attributes!["style"] == null) continue;
                if (!node.Attributes["style"]!.Value.Contains("swimlane;")) continue;

                string id = node.Attributes["id"]!.Value;
                string classname = node.Attributes["value"]!.Value;

                this.ReplaceStrings(ref classname);

                ClassStruct classStruct = new ClassStruct(id, classname, "MonoBehaviour");
                this.AllClasses.Add(classStruct);
            }
        }

        private void LoadAllFunctions()
        {
            foreach (ClassStruct classStruct in AllClasses)
            {
                foreach (XmlNode node in xmlFile)
                {
                    if (node.Attributes["parent"] == null) continue;
                    if (node.Attributes["value"] == null) continue;
                    if (node.Attributes["value"].Value == "") continue;
                    if (classStruct.GetID() != node.Attributes["parent"].Value) continue;
                    if (!node.Attributes["value"].Value.Contains("(")) continue;

                    // Get access specifiers
                    string accessSpecifier = GetAccessSpecifier(node.Attributes["value"]!);

                    // Get Return Type
                    string returnType = GetDataType(node.Attributes["value"]);

                    // Get function Name
                    string functionName = "";
                    int lastIndex = node.Attributes["value"].Value.LastIndexOf(":");
                    int startFunctionNameIndex = Array.FindIndex<char>(node.Attributes["value"].Value.ToCharArray(), char.IsLetter);
                    int endFunctionNameIndex = node.Attributes["value"].Value.IndexOf("(");
                    if (lastIndex != -1) functionName += node.Attributes["value"].Value.Substring(startFunctionNameIndex, endFunctionNameIndex - startFunctionNameIndex);

                    // Get Paramerter List
                    string parameterList = "";
                    int startParamerterIndex = node.Attributes["value"].Value.IndexOf("(");
                    int endParameterIndex = node.Attributes["value"].Value.IndexOf(")");
                    parameterList = node.Attributes["value"].Value.Substring(startParamerterIndex, endParameterIndex - startParamerterIndex + 1);
                    if (endParameterIndex - startParamerterIndex != 1) parameterList = FormatParameterList(parameterList);

                    string function = accessSpecifier + returnType + functionName + parameterList;

                    classStruct.AddFunction(function);
                }
            }
        }

        private void LoadAllVariables()
        {
            foreach (ClassStruct classStruct in AllClasses)
            {
                foreach (XmlNode node in this.xmlFile)
                {
                    if (node.Attributes!["parent"] == null) continue;
                    if (node.Attributes["value"] == null) continue;
                    if (node.Attributes["value"]!.Value == "") continue;
                    if (classStruct.GetID() != node.Attributes["parent"]!.Value) continue;
                    if (node.Attributes["value"]!.Value.Contains("(")) continue;

                    // Get access specifiers
                    string accessSpecifier = GetAccessSpecifier(node.Attributes["value"]!);

                    // Get Data Type
                    string variableType = GetDataType(node.Attributes["value"]!);
                    this.ReplaceStrings(ref variableType);

                    // Get variable Name
                    string variableName = "";
                    int lastIndex = node.Attributes["value"]!.Value.LastIndexOf(":");
                    int startVariableNameIndex = Array.FindIndex<char>(node.Attributes["value"]!.Value.ToCharArray(), char.IsLetter);
                    if (lastIndex != -1) variableName += node.Attributes["value"].Value.Substring(startVariableNameIndex, lastIndex - startVariableNameIndex);


                    string variable = accessSpecifier + variableType + variableName;
                    classStruct.AddVariable(variable);
                    //int lastIndex = node.Attributes["value"].Value.LastIndexOf(":");
                    //if (lastIndex != -1) property += node.Attributes["value"].Value.Substring(lastIndex);
                    //if (lastIndex != -1) property += node.Attributes["value"].Value.Substring(0, lastIndex);
                }
            }
        }

        string GetAccessSpecifier(XmlAttribute node)
        {
            if (node.Value.Contains("-")) return "private: ";
            if (node.Value.Contains("+")) return "public: ";
            if (node.Value.Contains("#")) return "protected: ";

            return "";
        }

        string GetDataType(XmlAttribute node)
        {
            string dataType = "";
            int lastIndex = node.Value.LastIndexOf(":");
            if (lastIndex != -1) dataType += node.Value.Substring(lastIndex + 1);
            dataType = dataType.Trim();
            dataType += " ";

            return dataType;
        }

        private string FormatParameterList(string list)
        {
            // Entferne Klammern und Leerzeichen
            string cleanInput = list.Replace("(", "").Replace(")", "").Trim();

            // Teile den Eingabestring in einzelne Parameter auf
            string[] parameters = cleanInput.Split(',');

            // Initialisiere den Ergebnisstring
            string result = "";

            // Durchlaufe die einzelnen Parameter
            foreach (string parameter in parameters)
            {
                // Entferne Leerzeichen
                string cleanParameter = parameter.Trim();

                // Teile den Parameter in Name und Typ auf
                string[] parts = cleanParameter.Split(':');

                // Extrahiere Name und Typ
                string name = parts[0].Trim();
                string type = parts[1].Trim();

                // Füge den Parameter zur Ergebnisliste hinzu
                result += $"{type} {name}, ";
            }
            // Entferne das letzte Komma und Leerzeichen
            result = result.TrimEnd(' ', ',');

            result = result.Insert(0, "(");
            result = result.Insert(result.Length, ")");

            return result;
        }

        private void ReplaceStrings(ref string name)
        {
            name = name.Replace("<div>", "");
            name = name.Replace("</div>", "");
            name = name.Replace("&lt;", "<");
            name = name.Replace("&gt;", ">");
        }

        private void PrintToFormsClasses()
        {
            foreach (ClassStruct item in this.AllClasses)
            {
                this.ClassesList.Items.Add(item.GetClassName());
            }
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;

            int textLength = textBox.Text.Length;

            double newSize = textBox.FontSize - (textLength * 0.2);

            if (newSize < 12)
            {
                newSize = 12;
            }

            textBox.FontSize = newSize;
        }

        ClassStruct SelectedClass;

        private void ClassesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string curItem = this.ClassesList.SelectedValue.ToString();

            foreach (ClassStruct item in this.AllClasses)
            {
                if (curItem != item.GetClassName()) continue;

                this.SelectedClass = item;
                this.UpdateClassPreview();
            }
        }

        private void SetBaseClassOrigin_Click(object sender, RoutedEventArgs e)
        {
            this.SelectedClass.ResetBaseClass();
            this.UpdateClassPreview();
        }

        private void SetBaseClassMonoBehaviour_Click(object sender, RoutedEventArgs e)
        {
            this.SelectedClass.SetCurrentBaseClass("MonoBehaviour");
            this.UpdateClassPreview();
        }

        private void SetBaseClassScriptableObject_Click(object sender, RoutedEventArgs e)
        {
            this.SelectedClass.SetCurrentBaseClass("ScriptableObject");
            this.UpdateClassPreview();
        }

        private void UpdateClassPreview()
        {
            this.ClassPreview.Text = this.SelectedClass.PrintClass();
        }
    }
}