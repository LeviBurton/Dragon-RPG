using RPG.Character;
using RPG.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class GameData
{
    public List<HeroData> allHeroes = new List<HeroData>();
    public List<HeroData> currentHeroes = new List<HeroData>();
}

