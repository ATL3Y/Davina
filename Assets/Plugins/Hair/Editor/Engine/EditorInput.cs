using System.Collections.Generic;
using UnityEngine;

namespace Assets.Hair.Editor.Engine
{
    public class EditorInput
    {
        private readonly List<KeyCode> keyCodes = new List<KeyCode>();

        public void Update()
        {
            var e = Event.current;

            if (e.type == EventType.KeyDown)
            {
                if (!keyCodes.Contains(e.keyCode))
                    keyCodes.Add(e.keyCode);
            }

            if (e.type == EventType.KeyUp)
            {
                if (keyCodes.Contains(e.keyCode))
                    keyCodes.Remove(e.keyCode);
            }
        }

        public bool IsKey(KeyCode code)
        {
            return keyCodes.Contains(code);
        }
    }
}
