using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;
using static MineCombat.EventManager;

namespace MineCombat
{
    public class Entity : Properties
    {
        private double _maxHealth;
        private double _health;
        private bool _alive = true;
        private object _lock = new();

        protected ITags tags;

        internal Entity(double maxHealth, ITags tags)
        {
            _maxHealth = Math.Min(uint.MaxValue, Math.Max(1, maxHealth));
            _health = _maxHealth;
            this.tags = tags;
        }
        internal Entity(double maxHealth, double health, ITags tags)
        {
            _maxHealth = Math.Min(uint.MaxValue, Math.Max(1, maxHealth));
            _health = Math.Min(_maxHealth, Math.Max(1, health));
            this.tags = tags;
        }
        internal Entity(double maxHealth) : this(maxHealth, ConstTags.Empty) { }
        internal Entity(double maxHealth, double health) : this(maxHealth, health, ConstTags.Empty) { }

        public double GetMaxHealth()
        {
            lock (_lock) { return _maxHealth; }
        }

        public double GetHealth()
        {
            lock (_lock) { return _health; }
        }

        public void SetMaxHealth(double maxHealth)
        {
            lock (_lock)
            {
                _maxHealth = Math.Min(uint.MaxValue, Math.Max(1, maxHealth));
                _health = Math.Min(_health, _maxHealth);
            }
        }

        public void SetHealth(double health)
        {
            lock (_lock)
            {
                _health = Math.Min(_maxHealth, health);
                if (_health <= 0)
                {
                    _alive = false;
                    _health = 0;
                }
            }
        }

        public void ChangeMaxHealth(Process<double> process)
        {
            lock (_lock)
            {
                process(ref _maxHealth);
                _maxHealth = Math.Min(uint.MaxValue, Math.Max(1, _maxHealth));
                _health = Math.Min(_health, _maxHealth);
            }
        }

        public void ChangeHealth(Process<double> process)
        {
            lock (_lock)
            {
                process(ref _health);
                _health = Math.Min(_maxHealth, _health);
                if (_health <= 0)
                {
                    _alive = false;
                    _health = 0;
                }
            }
        }

        public bool ApplyDamage(double damage)
        {
            lock (_lock)
            {
                _health -= damage;
                if (_health <= 0)
                {
                    _alive = false;
                    Die();
                    _health = 0;
                }
                return _alive;
            }
        }

        public bool IsAlive()
        {
            lock (_lock) { return _alive; }
        }

        public void SetAlive(bool alive)
        {
            lock (_lock)
            {
                _alive = alive;
                if (alive)
                    Revive();
                else Die();
            }
        }

        public virtual void Die() { }
        public virtual void Revive() { }
    }

    public class Player : Entity
    {
#nullable enable
        public readonly string Name;
        public Slots<Card> Inventory { get; private set; }
        public Slots<Card> Situation { get; private set; }
        public Card? ArmorSlot;
        private Dictionary<Material, uint> _material_bag = new(23);

        internal Player(string name, double maxHealth, ITags tags, uint ivt = 36, uint sta = 3) : base(maxHealth, tags)
        {
            Name = name;
            Inventory = new(ivt);
            Situation = new(sta);
        }

        internal Player(string name, double maxHealth, uint ivt = 36, uint sta = 3) : base(maxHealth)
        {
            Name = name;
            Inventory = new(ivt);
            Situation = new(sta);
        }

        internal void Play(uint index, Box<Entity>? targets)
        {
            Card card = Inventory[index];

            Assert.IsNotNull(card, "The card is Null");
            Events.Trigger("CardDurabilityDamaged", (card, (uint)1));
            CombatManager.Play(this, card, targets);
        }


        public override void Die()
        {
            UnityEngine.Debug.Log("The Player " + this.Name + "is Dead");
            Events.Trigger("CombatantDied", this);
        }
#nullable disable
    }
}
