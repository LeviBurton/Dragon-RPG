using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public enum ECommandType
    {
        Move,
        Attack,
        Spell,
        Item
    }

    public enum ECommandStatus
    {
        Queued,
        Running,
        Done
    }

    public struct Command 
    {
        public ECommandType CommandType;
        public ECommandStatus CommandStatus;
    }
}
