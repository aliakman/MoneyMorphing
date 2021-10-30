using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Data/Game Data")]           // Oyun verileri için bir scriptable obje oluşturuyorum. Level gibi verileri player pref yerine burada tutuyorum.
public class GameData : ScriptableObject
{
    public int levelValue;
    public int levelTextValue;
    public int tutorialLevelCount;
}
