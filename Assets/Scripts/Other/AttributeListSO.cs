using UnityEngine;

[CreateAssetMenu(fileName = "AttributeList", menuName = "ScriptableObjects/AttributeList")]
public class AttributeListSO : ScriptableObject
{
    [SerializeField] Attribute[] attributes;
    public void SetAttributes(ref Attribute[] _a){
        // _a = attributes;
        _a = new Attribute[attributes.Length];
        for (int i = 0; i < attributes.Length; i++){
            _a[i] = new Attribute(attributes[i].GetName(), attributes[i].GetBaseValue());
        }
    }
}
