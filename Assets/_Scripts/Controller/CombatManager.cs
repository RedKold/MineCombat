using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineCombat
{
    public static class CombatManager
    {
        private static Player[] _players = new Player[2];
        public static IReadOnlyList<Player> Players => _players;

        public static void Play(Player instigator, Card card, Box<Entity> targets)
        {
            Box<string> branches = new();
            card.action?.Invoke(instigator, card, targets, branches);
            branches.ForEach(s => { EventManager.SlicedEvents.Trigger("PlayCard", s, (instigator, card)); });
        }
    }
}
