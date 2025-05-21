using System.Collections.Generic;
using UnityEngine;

namespace PSkrzypa.MVVMUI
{
    [CreateAssetMenu(menuName = "Sprites Database")]
    public class SpritesDatabase : SingletonScriptableObject<SpritesDatabase>
    {
        public Dictionary<string, Sprite> spritesDictionary;

        public override void RefreshDatabase()
        {
            throw new System.NotImplementedException();
        }
    }
}
