using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using UnityEngine;
using UnityEditor;

public static class EnemyStateMachineEditor
{
    public static void GenerateScript(FSMGraphSaveData saveData)
    {
        string scriptContent = GenerateScriptContent(saveData);

        string scriptPath = $"Assets/EditorWindow/FSMSystem/BehaviorScripts/{saveData.FileName}.cs";
        
        File.WriteAllText(scriptPath, scriptContent);
        
        AssetDatabase.Refresh();
        
        //AssignScriptableObjectReferences(saveData);
        CompileAndCreateScript(scriptContent, saveData.GameObject, saveData);
            
        Debug.Log($"Script generated at: {scriptPath}");
    }

    private static void AssignScriptableObjectReferences(FSMGraphSaveData saveData)
    {
        EnemyStateMachine enemyStateMachine = saveData.GameObject.GetComponent<EnemyStateMachine>();
        if (enemyStateMachine == null)
        {
            enemyStateMachine = saveData.GameObject.AddComponent<EnemyStateMachine>();
        }

        for (int i = 0; i < saveData.Nodes.Count; i++)
        {
            enemyStateMachine.SetVariableValue(char.ToLowerInvariant(saveData.Nodes[i].Name[0]) + saveData.Nodes[i].Name.Substring(1), saveData.Nodes[i].ScriptableObject);
        }
    }
    
    static void CompileAndCreateScript(string scriptCode, GameObject gameObject, FSMGraphSaveData saveData)
    {
        // Compile the script dynamically
        CodeDomProvider provider = new CSharpCodeProvider();
        CompilerParameters parameters = new CompilerParameters();

        /*string systemRuntimePath = Path.Combine(EditorApplication.applicationContentsPath, "Tools/netcorerun", "System.Runtime.dll");
        parameters.ReferencedAssemblies.Add(systemRuntimePath);
        
        string netstandardPath = Path.Combine(EditorApplication.applicationContentsPath, "Tools/netcorerun", "netstandard.dll");
        parameters.ReferencedAssemblies.Add(netstandardPath);
        
        string systemCollectionsPath = Path.Combine(EditorApplication.applicationContentsPath, "Tools/netcorerun", "System.Private.CoreLib.dll");
        parameters.ReferencedAssemblies.Add(systemCollectionsPath);
        
        string coreModulePath = Path.Combine(EditorApplication.applicationContentsPath, "Managed/UnityEngine", "UnityEngine.CoreModule.dll");
        parameters.ReferencedAssemblies.Add(coreModulePath);
        
        parameters.ReferencedAssemblies.Add("Library/ScriptAssemblies/Assembly-CSharp.dll");*/

        parameters.GenerateInMemory = true;
        //parameters.GenerateExecutable = false;
        CompilerResults results = provider.CompileAssemblyFromSource(parameters, scriptCode);
        
        if (results.Errors.HasErrors)
        {
            foreach (CompilerError error in results.Errors)
            {
                Debug.LogError($"Error {error.ErrorNumber}: {error.ErrorText}");
            }
        }
        else
        {
            // Get the compiled type
            Type newScriptType = results.CompiledAssembly.GetType(saveData.FileName);

            if (newScriptType != null)
            {
                // Create an instance of the new script type
                MonoBehaviour newScriptInstance = (MonoBehaviour)gameObject.AddComponent(newScriptType);

                // Call a method on the new script
                MethodInfo dynamicMethod = newScriptType.GetMethod("SetVariableValue");
                if (dynamicMethod != null)
                {
                    for (int i = 0; i < saveData.Nodes.Count; i++)
                    {
                        dynamicMethod.Invoke(newScriptInstance,new object[]{char.ToLowerInvariant(saveData.Nodes[i].Name[0]) + saveData.Nodes[i].Name.Substring(1), saveData.Nodes[i].ScriptableObject});
                    }
                }
            }
            else
            {
                Debug.LogError("Failed to get the compiled script type.");
            }
        }
    }

    private static string GenerateScriptContent(FSMGraphSaveData saveData)
    {

        string scriptContent = "using UnityEngine;\n";
        scriptContent += "using System;\n";
        scriptContent += "using System.Collections;\n";
        scriptContent += "using System.Collections.Generic;\n";
        scriptContent += "using System.Reflection;\n\n";
        

        scriptContent += $"public class {saveData.FileName} : MonoBehaviour\n";
        scriptContent += "{\n";

        List<FSMNodeSaveData> states = new List<FSMNodeSaveData>();   
        foreach (FSMNodeSaveData state in saveData.Nodes)
        {
            states.Add(state);
        }
        
        foreach (var node in states.Distinct())
        {
            scriptContent += $"\t[Header(\"" + node.Name + "\")]\n";
            foreach (string fullAttribute in node.ScriptableObject.InspectVariables())
            {
                /*string[] result = fullAttribute.Split(',');
                scriptContent += "\t[SerializeField]\n";
                scriptContent += $"\tprivate {result[1]} {result[0]} = {char.ToLowerInvariant(result[2][0]) + result[2].Substring(1)};\n";*/
            }
            
            scriptContent += "\t[SerializeField]\n";
            string variableName = node.DialogueType == FSMDialogueType.State ? node.Name+"State" : node.Name+"Condition";
            scriptContent += $"\tpublic {variableName} {char.ToLowerInvariant(node.Name[0]) + node.Name.Substring(1)};\n";

            scriptContent += "\n";


            /*ScriptableObject actionState = CreateInstance(node.Name+"State");
            scriptContent += $"\n\tprivate {actionState.GetType()} {char.ToLowerInvariant(stateName[0]) + stateName.Substring(1) + "State"};\n\n";
            
            scriptContent += "\t[Header(\""+stateName+" Settings\")]\n\n";
            
            FieldInfo[] fields = GetPublicVariables(actionState);

            // Print the names and values of the public variables
            foreach (FieldInfo field in fields)
            {
                var value = field.GetValue(actionState);
                scriptContent += $"\tpublic {value.GetType()} {field.Name +" = "+ value};\n";
            }*/
        }


        scriptContent += $"\tpublic Dictionary<string, object> GetVariables()";
        scriptContent += "\n\t{\n";
        scriptContent += "\t\tDictionary<string, object> variables = new Dictionary<string, object>();\n";
        scriptContent += $"\t\tType type = GetType();\n";
        scriptContent += $"\t\tFieldInfo[] fields = type.GetFields();\n";
        scriptContent += "\n";
        scriptContent += "\t\tforeach (FieldInfo field in fields)\n";
        scriptContent += "\t\t{\n";
        scriptContent += "\t\t\tobject value = field.GetValue(this);\n";
        scriptContent += "\t\t\tvariables.Add(field.Name, value);\n";
        scriptContent += "\t\t}\n";
        scriptContent += "\t\treturn variables;\n";
        scriptContent += "\t}\n";
            
        scriptContent += "\n";
            
        scriptContent += $"\tpublic void SetVariableValue(string variableName, object newValue)\n";
        scriptContent += "\t{\n";
        scriptContent += "\t\t// Use reflection to set the value of the variable with the given name\n";
        scriptContent += "\t\tSystem.Type type = GetType();\n";
        scriptContent += "\t\tSystem.Reflection.FieldInfo field = type.GetField(variableName);\n";
        scriptContent += "\n";
        scriptContent += "\t\tif (field != null)\n";
        scriptContent += "\t\t{\n";
        scriptContent += "\t\t\tfield.SetValue(this, newValue);\n";
        scriptContent += "\t\t}\n";
        scriptContent += "\t\telse\n";
        scriptContent += "\t\t{\n";
        scriptContent += "\t\t\tDebug.LogError($\"{variableName} does not exist in the ScriptableObject.\");\n";
        scriptContent += "\t\t}\n";
        scriptContent += "\t}\n";
        
        /*List<string> contidions = new List<string>();   
        foreach (var state in _stateMachineData.states)
        {
            contidions.Add(state.condition);
        }

        foreach (var condition in contidions.Distinct())
        {
            ScriptableObject actionState = CreateInstance(condition+"Condition");
            scriptContent += $"\n\tprivate {actionState.GetType()} {char.ToLowerInvariant(condition[0]) + condition.Substring(1) + "Condition"};\n\n";
        }*/
        
        /*scriptContent += "\t\n";
        scriptContent += $"\tpublic void Update()\n";
        scriptContent += "\t{\n";
        scriptContent += "\t}\n\n";
        
        List<string> states2 = new List<string>();

        foreach (var state in _stateMachineData.states)
        {
            if (!states2.Contains(state.name))
            {
                scriptContent += $"\tpublic void {state.name}()\n";
                scriptContent += "\t{\n";
                
                // Generate a call to the SpecificAttackMethod
                string nameCorrection = char.ToLowerInvariant(state.name[0]) + state.name.Substring(1);
                string nameCorrection2 = char.ToLowerInvariant(state.condition[0]) + state.condition.Substring(1) + "Condition";
                            
                scriptContent += $"\t\t{nameCorrection}State.Execute();\n";
                scriptContent += $"\t\tif ("+nameCorrection2+".Condition())\n";
                scriptContent += "\t\t{\n";
                scriptContent += $"\t\t\t{state.nextState}();\n";
                scriptContent += "\t\t}\n";
                scriptContent += "\t}\n\n";
                
                states2.Add(state.name);
            }
            
        }*/

        scriptContent += "}\n";

        return scriptContent;
    }
}

