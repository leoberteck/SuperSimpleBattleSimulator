using SuperSimpleBattleSimulator.Common;
using SuperSimpleBattleSimulator.Delegates;
using SuperSimpleBattleSimulator.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace SuperSimpleBattleSimulator.ViewModel
{
    public class UnitViewModel : ViewModelBase
    {
        private static Random rand = new Random();

        /// <summary>
        /// Defines unit hitpoints
        /// </summary>
        private double health;
        public double Health
        {
            get { return health; }
            set
            {
                if (health != value)
                {
                    health = value;
                    RaisePropertyChanged("Health");
                }
            }
        }
        /// <summary>
        /// Defines unit attack power
        /// </summary>
        private double attack;
        public double Attack
        {
            get { return attack; }
            set
            {
                if (attack != value)
                {
                    attack = value;
                    RaisePropertyChanged("Attack");
                }
            }
        }
        /// <summary>
        /// Defines unit resistence agaist attack
        /// </summary>
        private double armor;
        public double Armor
        {
            get { return armor; }
            set
            {
                if (armor != value)
                {
                    armor = value;
                    RaisePropertyChanged("Armor");
                }
            }
        }
        /// <summary>
        /// Defines how often unit will attack.
        /// A unit attack speed is a random number 
        /// between (500 - AttackSpeed) and (1000 - AttackSpeed) miliseconds.
        /// </summary>
        private int attackSpeed;
        public int AttackSpeed
        {
            get { return attackSpeed; }
            set
            {
                if (attackSpeed != value)
                {
                    attackSpeed = value;
                    RaisePropertyChanged("AttackSpeed");
                }
            }
        }
        /// <summary>
        /// Each team 50 to distrubute among its units
        /// </summary>
        public int Points { get; set; }
        public int UsedPoints { get; set; }
        /// <summary>
        /// Each unit must be defined with minimum and max values for 
        /// Health, Attack, Armor and AttackSpeed
        /// </summary>
        public double MinHealth { get; private set; }
        public double MinAttack { get; private set; }
        public double MinArmor { get; private set; }
        public double MaxHealth { get; private set; }
        public double MaxAttack { get; private set; }
        public double MaxArmor { get; private set; }
        public int MaxAttackSpeed { get; private set; }
        public UnitType UnitType { get; private set; }
        public UnitTeam Team { get; private set; }
        public DispatcherTimer _timer { get; set; }
        public event DeathEventHandler UnitDied;
        public event AttackEventHandler UnitAttack;

        public RelayCommand AddHealthPoint { get; set; }
        public RelayCommand RemoveHealthPoint { get; set; }
        public RelayCommand AddAttackPoint { get; set; }
        public RelayCommand RemoveAttackPoint { get; set; }
        public RelayCommand AddArmorPoint { get; set; }
        public RelayCommand RemoveArmorPoint { get; set; }
        public RelayCommand AddAttackSpeedPoint { get; set; }
        public RelayCommand RemoveAttackSpeedPoint { get; set; }

        public UnitViewModel(double _minHealth
                        , double _minAttack
                        , double _minArmor
                        , double _maxHealth
                        , double _maxAttack
                        , double _maxArmor
                        , int _maxAttackSpeed
                        , UnitType _unityType
                        , UnitTeam _team
                        , int _points)
        {
            _timer = new DispatcherTimer();
            MinHealth = Health = _minHealth;
            MinAttack = Attack = _minAttack;
            MinArmor = Armor = _minArmor;
            MaxHealth = _maxHealth;
            MaxAttack = _maxAttack;
            MaxArmor = _maxArmor;
            MaxAttackSpeed = _maxAttackSpeed;
            UnitType = _unityType;
            Team = _team;
            Points = _points;
            AttackSpeed = 0;
            PropertyChanged += Unity_PropertyChanged;
            AddHealthPoint = new RelayCommand(OnAddHealthPoint);
            RemoveHealthPoint = new RelayCommand(OnRemoveHealthPoint);
            AddAttackPoint = new RelayCommand(OnAddAttackPoint);
            RemoveAttackPoint = new RelayCommand(OnRemoveAttackPoint);
            AddArmorPoint = new RelayCommand(OnAddArmorPoint);
            RemoveArmorPoint = new RelayCommand(OnRemoveArmorPoint);
            AddAttackSpeedPoint = new RelayCommand(OnAddAttackSpeedPoint);
            RemoveAttackSpeedPoint = new RelayCommand(OnRemoveAttackSpeedPoint);
        }

        /// <summary>
        /// Callback that listens for changed on the binding properties.
        /// When the Health property of the unit goes to 0 the unit is dead.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Unity_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Health" && Health <= 0)
            {
                Dead();// XD
            }
        }

        /// <summary>
        /// Start a sequence of random attacks.
        /// </summary>
        public void StartAttack()
        {
            var miliseconds = rand.Next(500 - AttackSpeed, 1000 - AttackSpeed);
            _timer.Interval = new TimeSpan(0, 0, 0, 0, miliseconds);
            _timer.Tick += _attack_timer_Tick;
            _timer.Start();
        }

        /// <summary>
        /// Fires the attack event. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _attack_timer_Tick(object sender, object e)
        {
            var miliseconds = rand.Next(500 - AttackSpeed, 1000 - AttackSpeed);
            _timer.Interval = new TimeSpan(0, 0, 0, 0, miliseconds);
            UnitAttack?.Invoke(this, Attack, UnitType, Team);
        }

        /// <summary>
        /// When the unit dies, stop the attack timer, unubscribe from the timer event and
        /// fires the death event.
        /// </summary>
        public void Dead()
        {
            _timer.Stop();
            _timer.Tick -= _attack_timer_Tick;
            UnitDied?.Invoke(this, Team);
        }

        /// <summary>
        /// Performs a validations when the user tries to change some of the units skill points.
        /// The new value (value + change) of the skill must be between the mininum and maximum value
        /// for that skill and team must still have points left to expend.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxValue"></param>
        /// <param name="minValue"></param>
        /// <param name="change"></param>
        /// <returns></returns>
        private double TryChangeDouble(double value, double maxValue, double minValue, double change)
        {
            var newVal = value + change;
            var newPoints = Points + (change < 0 ? 1 : -1);
            if (newVal <= maxValue && newVal >= minValue && newPoints >= 0 && newPoints <= MainViewModel.MAX_TEAM_POINTS)
            {
                value = newVal;
                Points = newPoints;
                UsedPoints += (change < 0 ? -1 : 1);
                ObserverHelper<MainViewModel, int>.NotifyObservers(this, Points);
            }
            return value;
        }

        /// <summary>
        /// Callback for the edit buttons on the edit form.
        /// </summary>
        private void OnAddHealthPoint() { Health = TryChangeDouble(Health, MaxHealth, MinHealth, 5); }
        private void OnRemoveHealthPoint() { Health = TryChangeDouble(Health, MaxHealth, MinHealth, -5); }
        private void OnAddAttackPoint() { Attack = TryChangeDouble(Attack, MaxAttack, MinAttack, 1); }
        private void OnRemoveAttackPoint() { Attack = TryChangeDouble(Attack, MaxAttack, MinAttack, -1); }
        private void OnAddArmorPoint() { Armor = TryChangeDouble(Armor, MaxArmor, MinArmor, 1); }
        private void OnRemoveArmorPoint() { Armor = TryChangeDouble(Armor, MaxArmor, MinArmor, -1); }
        private void OnAddAttackSpeedPoint() { AttackSpeed = Convert.ToInt32(TryChangeDouble(AttackSpeed, MaxAttackSpeed, 0, 25)); }
        private void OnRemoveAttackSpeedPoint() { AttackSpeed = Convert.ToInt32(TryChangeDouble(AttackSpeed, MaxAttackSpeed, 0, -25)); }

        /// <summary>
        /// Simple factory to generate new units.
        /// </summary>
        /// <param name="_team">The team of the new unit</param>
        /// <param name="_points">The team points avaliable</param>
        /// <returns>A new instance of UnitViewModel</returns>
        public static UnitViewModel GetNewInfantry(UnitTeam _team, int _points)
        {
            return new UnitViewModel(50, 15, 20, 100, 30, 50, 200, UnitType.Infantry, _team, _points);
        }

        public static UnitViewModel GetNewRanged(UnitTeam _team, int _points)
        {
            return new UnitViewModel(30, 35, 10, 100, 40, 50, 500, UnitType.Ranged, _team, _points);
        }

        public static UnitViewModel GetNewCavalry(UnitTeam _team, int _points)
        {
            return new UnitViewModel(60, 20, 10, 150, 50, 30, 100, UnitType.Cavalary, _team, _points);
        }
    }
}
