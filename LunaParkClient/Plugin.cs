using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace LunaParkClient
{
    public class Client : BaseScript
    {
        internal static Client Instance;
        public Client()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
            Instance = this;
        }

        private void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
            FerrisWheel.Init();
            RollerCoaster.Init();
        }

        internal void AddEventHandler(string evt, object action) {
            EventHandlers[evt] += (Action)action;
        }
        internal void AddTick(Func<Task> task)
        {
            Tick += task;
        }
        internal void RemoveTick(Func<Task> task)
        {
            Tick -= task;
        }
    }
    public static class Extensions
    {
        public static float ConvertRadiansToDegrees(this double radians)
        {
            float degrees = (float)((180 / Math.PI) * radians);
            return degrees;
        }

        public static float ConvertDegreesToRadians(this double angle)
        {
            float radians = (float)((Math.PI / 180) * angle);
            return radians;
        }
    }
}
