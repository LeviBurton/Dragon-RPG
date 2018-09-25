using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[Serializable] public class Event_Heal : UnityEvent<float> { }
[Serializable] public class Event_Damage : UnityEvent<float> { }

[Serializable] public class Event_EncounterStart : UnityEvent<EncounterSystem> { }
[Serializable] public class Event_EncounterEnd : UnityEvent<EncounterSystem> { }

[Serializable] public class Event_LocationEntryStart : UnityEvent<LocationEntryPoint> { }
[Serializable] public class Event_LocationEntryEnter : UnityEvent<LocationEntryPoint> { }
[Serializable] public class Event_LocationEntryExit : UnityEvent<LocationEntryPoint> { }

