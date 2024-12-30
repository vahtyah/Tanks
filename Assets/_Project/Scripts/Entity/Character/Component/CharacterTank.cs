using UnityEngine;

public class CharacterTank : MonoBehaviour
{
    [SerializeField] private GameObject turret;
    [SerializeField] private GameObject wheels;
    [SerializeField] private GameObject body;
    
    public GameObject Turret => turret;
    public GameObject Wheels => wheels;
    public GameObject Body => body;
}
