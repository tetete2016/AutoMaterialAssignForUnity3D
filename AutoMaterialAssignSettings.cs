using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "AutoAssignSettings", order = 1)]
public class AutoMaterialAssignSettings : ScriptableObject {
    public List<Material> materials;
    public int slot;
}
