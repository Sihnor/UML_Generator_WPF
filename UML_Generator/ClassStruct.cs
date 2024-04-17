using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UML_Generator
{
    class ClassStruct
    {
        private string ID;
        private string Classname;
        private string OriginalBaseClass;
        private string CurrentBaseClass;
        private List<string> VariableNames;
        private List<string> FunctionNames;

        public ClassStruct(string iD, string className, string baseClass)
        {
            this.ID = iD;
            this.Classname = className;
            this.OriginalBaseClass = baseClass;
            this.CurrentBaseClass = baseClass;
            this.VariableNames = new List<string>();
            this.FunctionNames = new List<string>();
        }
        public ClassStruct(string iD, string className, string baseClass, List<string> variableNames, List<string> functionNames)
        {
            this.ID = iD;
            this.Classname = className;
            this.OriginalBaseClass = baseClass;
            this.CurrentBaseClass = baseClass;
            this.VariableNames = variableNames;
            this.FunctionNames = functionNames;
        }

        public bool AddVariable(string variable)
        {
            if (this.VariableNames.Contains(variable)) return false;

            this.VariableNames.Add(variable);

            return true;
        }

        public void ReplaceVariableList(List<string> list)
        {
            this.VariableNames = list;
        }

        public bool AddFunction(string function)
        {
            if (this.FunctionNames.Contains(function)) return false;

            this.FunctionNames.Add(function);

            return true;
        }

        public void ReplaceFunctionList(List<string> list)
        {
            this.FunctionNames = list;
        }

        public string GetID()
        {
            return this.ID;
        }

        public string GetClassName()
        {
            return this.Classname;
        }

        public string GetOriginalBaseClass()
        {
            return this.OriginalBaseClass;
        }

        public void ResetBaseClass()
        {
            this.CurrentBaseClass = this.OriginalBaseClass;
        }

        public void SetCurrentBaseClass(string baseClass)
        {
            this.CurrentBaseClass = baseClass;
        }

        public string GetCurrentBaseClass()
        {
            return this.CurrentBaseClass;
        }

        public List<string> GetVariableList()
        {
            return this.VariableNames;
        }

        public List<string> GetFunctionList()
        {
            return this.FunctionNames;
        }

        public string PrintClass()
        {
            string fullfunction = "";

            fullfunction += "using System.Collections;" + System.Environment.NewLine;
            fullfunction += "using System.Collections.Generic;" + System.Environment.NewLine;
            fullfunction += "using UnityEngine;" + System.Environment.NewLine;
            fullfunction += System.Environment.NewLine;

            fullfunction += $"public class {this.GetClassName()} : {this.GetCurrentBaseClass()}" + System.Environment.NewLine;
            fullfunction += "{" + System.Environment.NewLine;

            fullfunction += System.Environment.NewLine;

            foreach (string varaible in this.VariableNames)
            {
                fullfunction += varaible + ";" + System.Environment.NewLine;
            }

            foreach (string function in this.FunctionNames)
            {
                fullfunction += System.Environment.NewLine;
                fullfunction += function + System.Environment.NewLine;
                fullfunction += "{" + System.Environment.NewLine;
                fullfunction += System.Environment.NewLine;
                fullfunction += "}" + System.Environment.NewLine;
            }

            fullfunction += "}" + System.Environment.NewLine;

            return fullfunction;
        }
    }
}
