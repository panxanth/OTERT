using System;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.JScript;

namespace ExpressionParser {

    public class Evaluator {

        public static int EvalToInteger(string statement) {
            string s = EvalToString(statement).Split(new char[] {'.',','})[0];
            return Int32.Parse(s.ToString());
        }

        public static double EvalToDouble(string statement) {
            string s = EvalToString(statement);
            string k = s.Replace(".", ",");
            return double.Parse(k);
        }

        public static string EvalToString(string statement) {
            object o = EvalToObject(statement);
            return o.ToString();
        }

        public static object EvalToObject(string statement) {
            return _evaluatorType.InvokeMember(
                        "Eval",
                        BindingFlags.InvokeMethod,
                        null,
                        _evaluator,
                        new object[] { statement }
                     );
        }

        static Evaluator() {
            ICodeCompiler compiler;
            compiler = new JScriptCodeProvider().CreateCompiler();

            CompilerParameters parameters;
            parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;

            CompilerResults results;
            results = compiler.CompileAssemblyFromSource(parameters, _jscriptSource);

            Assembly assembly = results.CompiledAssembly;
            _evaluatorType = assembly.GetType("Evaluator.Evaluator");

            _evaluator = Activator.CreateInstance(_evaluatorType);
        }

        private static object _evaluator = null;
        private static Type _evaluatorType = null;
        private static readonly string _jscriptSource =

            @"package Evaluator{
          
               class Evaluator{
               
                  public function Eval(expr : String) : String { 
                     return eval(expr); 
                  }
               }
            }";
    }
}