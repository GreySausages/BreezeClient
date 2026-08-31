using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(GameObject))]
[HarmonyPatch("CreatePrimitive", MethodType.Normal)]
internal class MenuFixer : MonoBehaviour
{
    private static void Postfix(GameObject __result)
    {
        __result.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
    }
}