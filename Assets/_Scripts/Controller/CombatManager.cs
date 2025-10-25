using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MineCombat.EventManager;

namespace MineCombat
{
    public static class CombatManager
    {
#nullable enable
        private static Context _cnt = new();
        private static Player[] _players = new Player[2];
        public static IReadOnlyList<Player> Players => _players;

        public static void Play(Entity instigator, Card card, Box<Entity>? targets)
        {
            card.Commands[0]?.ForEach(cmd =>{
                SlicedEvents.Trigger("CardPlayed", cmd, (instigator, card, targets, _cnt));
            });
        }
#nullable disable
    }
}
