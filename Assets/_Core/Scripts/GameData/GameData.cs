using RPG.Character;
using RPG.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class GameData
{
    public HeroData hero;
    public List<HeroData> partyHeroes = new List<HeroData>();
}

