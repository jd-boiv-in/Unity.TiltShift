using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;

namespace JD.TiltShifts.Editor
{
    // Fix when managed stripping is high
    public class TiltShiftBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log($"Tilt Shift Build Processor");
            
            // Ensure shaders are included and instancing variants as well
            var asset = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/GraphicsSettings.asset");
            if (asset == null)
            {
                Debug.LogError($"Cannot load GraphicsSettings.asset");
                return;
            }
            
            var serObj = new SerializedObject(asset);
            serObj.UpdateIfRequiredOrScript();

            var includedShadersProp = serObj.FindProperty("m_AlwaysIncludedShaders");
            if (includedShadersProp == null)
            {
                Debug.LogError($"Cannot find m_AlwaysIncludedShaders property");
                return;
            }
            
            var shaders = new[]
            {
                "Hidden/PostFX/TiltShift",
            };
            
            foreach (var name in shaders)
            {
                var shader = Shader.Find(name);
                if (shader == null)
                {
                    Debug.LogError($"Cannot find shader with name");
                    return;
                }

                AddShader(includedShadersProp, shader);
            }
            
            serObj.ApplyModifiedPropertiesWithoutUndo();
        }
        
        private void AddShader(SerializedProperty includedShadersProp, Shader shader)
        {
            // Add shader if not present
            for (int i = 0, count = includedShadersProp.arraySize; i < count; ++i)
            {
                var element = includedShadersProp.GetArrayElementAtIndex(i);
                if (element.objectReferenceValue == shader)
                {
                    return; // Shader added already
                }
            }

            includedShadersProp.arraySize++;
            var shaderProp = includedShadersProp.GetArrayElementAtIndex(includedShadersProp.arraySize - 1);
            shaderProp.objectReferenceValue = shader;
        }
    }
}