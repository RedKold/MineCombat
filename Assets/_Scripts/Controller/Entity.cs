using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MineCombat
{
    public class Entity : Properties
    {
        private double _maxHealth;
        private double _health;
        private bool _alive = true;
        private object _lock = new();

        internal Entity(double maxHealth)
        {
            _maxHealth = Math.Max(1, maxHealth);
            _health = _maxHealth;
        }

        internal Entity(double maxHealth, double health)
        {
            _maxHealth = Math.Max(1, maxHealth);
            _health = Math.Min(_maxHealth, Math.Max(1, health));
        }

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
                _maxHealth = Math.Max(1, maxHealth);
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
                _maxHealth = Math.Max(1, _maxHealth);
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

        public void ApplyDamage(double damage)
        {
            lock (_lock)
            {
                _health -= damage;
                if (_health <= 0)
                {
                    _alive = false;
                    _health = 0;
                }
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
            }
        }
    }
}
