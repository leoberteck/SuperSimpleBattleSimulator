using SuperSimpleBattleSimulator.Common;
using SuperSimpleBattleSimulator.Enum;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace SuperSimpleBattleSimulator.ViewModel
{
    public class MainViewModel : ViewModelBase, Common.IObserver<int>
    {
        public const int MAX_TEAM_POINTS = 100;

        #region Binding Properties
        private ObservableCollection<UnitViewModel> listBlueUnits;
        public ObservableCollection<UnitViewModel> ListBlueUnits
        {
            get { return listBlueUnits; }
            set
            {
                if (listBlueUnits != value)
                {
                    listBlueUnits = value;
                    RaisePropertyChanged("ListBlueUnits");
                }
            }
        }

        private ObservableCollection<UnitViewModel> listRedUnits;
        public ObservableCollection<UnitViewModel> ListRedUnits
        {
            get { return listRedUnits; }
            set
            {
                if (listRedUnits != value)
                {
                    listRedUnits = value;
                    RaisePropertyChanged("ListRedUnits");
                }
            }
        }

        public RelayCommand NewRedInfantryClicked { get; set; }
        public RelayCommand NewRedRangedClicked { get; set; }
        public RelayCommand NewRedCavalryClicked { get; set; }

        public RelayCommand NewBlueInfantryClicked { get; set; }
        public RelayCommand NewBlueRangedClicked { get; set; }
        public RelayCommand NewBlueCavalryClicked { get; set; }

        public RelayCommand StartBattle { get; set; }
        public RelayCommand ClearBattleField { get; set; }
        public RelayCommand<object> UnitClicked { get; set; }
        #endregion

        #region Form Binding Properties
        private UnitViewModel fromViewModel;
        public UnitViewModel FormViewModel
        {
            get { return fromViewModel; }
            set
            {
                if (fromViewModel != value)
                {
                    fromViewModel = value;
                    RaisePropertyChanged("FormViewModel");
                }
            }
        }

        private bool formVisible;
        public bool FormVisible
        {
            get { return formVisible; }
            set
            {
                if (formVisible != value)
                {
                    formVisible = value;
                    RaisePropertyChanged("FormVisible");
                }
            }
        }

        public RelayCommand KillSelectedUnitClicked { get; set; }
        public RelayCommand CancelEditUnitClicked { get; set; }

        private bool isBattleGoingOn;
        public bool IsBattleGoingOn
        {
            get { return isBattleGoingOn; }
            set
            {
                if (isBattleGoingOn != value)
                {
                    isBattleGoingOn = value;
                    RaisePropertyChanged("IsBattleGoingOn");
                }
            }
        }

        private int bluePoints = MAX_TEAM_POINTS;
        public int BluePoints
        {
            get { return bluePoints; }
            set
            {
                if (bluePoints != value)
                {
                    bluePoints = value;
                    RaisePropertyChanged("BluePoints");
                }
            }
        }

        private int redPoints = MAX_TEAM_POINTS;
        public int RedPoints
        {
            get { return redPoints; }
            set
            {
                if (redPoints != value)
                {
                    redPoints = value;
                    RaisePropertyChanged("RedPoints");
                }
            }
        }

        #endregion

        Random _rand = new Random();

        public MainViewModel()
        {
            ListBlueUnits = new ObservableCollection<UnitViewModel>();
            ListRedUnits = new ObservableCollection<UnitViewModel>();

            ListBlueUnits.CollectionChanged += ListUnits_CollectionChanged;
            ListRedUnits.CollectionChanged += ListUnits_CollectionChanged;

            NewRedInfantryClicked = new RelayCommand(OnNewRedInfantryClicked);
            NewRedRangedClicked = new RelayCommand(OnNewRedRangedClicked);
            NewRedCavalryClicked = new RelayCommand(OnNewRedCavalryClicked);
            NewBlueInfantryClicked = new RelayCommand(OnNewBlueInfantryClicked);
            NewBlueRangedClicked = new RelayCommand(OnNewBlueRangedClicked);
            NewBlueCavalryClicked = new RelayCommand(OnNewBlueCavalryClicked);
            UnitClicked = new RelayCommand<object>(OnUnitClicked);
            StartBattle = new RelayCommand(DoStartBattle);
            ClearBattleField = new RelayCommand(DoClearBattleField);
            KillSelectedUnitClicked = new RelayCommand(OnKillSelectedUnitClicked);
            CancelEditUnitClicked = new RelayCommand(OnCancelEditUnitClicked);
            ObserverHelper<MainViewModel, int>.Register(this);
            InitSampleBattleField();
        }

        /// <summary>
        /// Populate the field of battle with some units. 
        /// Fires only when aplication opens.
        /// </summary>
        private void InitSampleBattleField() {
            OnNewBlueCavalryClicked();
            OnNewBlueCavalryClicked();
            OnNewBlueCavalryClicked();
            OnNewBlueCavalryClicked();
            OnNewBlueInfantryClicked();
            OnNewBlueInfantryClicked();
            OnNewBlueInfantryClicked();
            OnNewBlueInfantryClicked();
            OnNewBlueRangedClicked();
            OnNewBlueRangedClicked();
            OnNewBlueRangedClicked();
            OnNewBlueRangedClicked();

            OnNewRedCavalryClicked();
            OnNewRedCavalryClicked();
            OnNewRedCavalryClicked();
            OnNewRedCavalryClicked();
            OnNewRedInfantryClicked();
            OnNewRedInfantryClicked();
            OnNewRedInfantryClicked();
            OnNewRedInfantryClicked();
            OnNewRedRangedClicked();
            OnNewRedRangedClicked();
            OnNewRedRangedClicked();
            OnNewRedRangedClicked();
        }

        /// <summary>
        /// Callback that is fired every time the count of ListBlueUnits or ListRedUnits changes.
        /// If IsBattleGoingOn = true and one of the fields gets empty. The battle is over.
        /// When the battle ends, a message is shown, both fields get cleared and both teams points are restored.
        /// </summary>
        private async void ListUnits_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!((IEnumerable<UnitViewModel>)sender).Any() && IsBattleGoingOn)
            {
                var winningTeam = ListBlueUnits.Any() ? "Blue" : "Red";
                var message = new MessageDialog("The " + winningTeam + " won the battle!!!");
                await message.ShowAsync();
                IsBattleGoingOn = false;
                DoClearBattleField();
            }
        }

        /// <summary>
        /// For every unit added to the field, the MainViewModel listens
        /// to the events of attack and death of the unit.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        private UnitViewModel HookCallBacks(UnitViewModel unit)
        {
            unit.UnitAttack += Unit_UnitAttack;
            unit.UnitDied += Unit_UnitDied;
            return unit;
        }

        /// <summary>
        /// When a unit dies, the MainViewModel unsubscribes the unit events, restores unit points to the 
        /// total of the team points and remove it from the field of battle.
        /// </summary>
        /// <param name="sender">The dead unit</param>
        /// <param name="Team">The team of the dead unit</param>
        private void Unit_UnitDied(object sender, UnitTeam Team)
        {
            var unit = sender as UnitViewModel;
            unit.UnitAttack -= Unit_UnitAttack;
            unit.UnitDied -= Unit_UnitDied;
            if (Team == UnitTeam.Red)
            {
                ListRedUnits.Remove(unit);
                ObserverHelper<MainViewModel, int>.NotifyObservers(unit, RedPoints + unit.UsedPoints);
            }
            else
            {
                ListBlueUnits.Remove(unit);
                ObserverHelper<MainViewModel, int>.NotifyObservers(unit, BluePoints + unit.UsedPoints);
            }
        }

        /// <summary>
        /// When a unit attacks, a random unit from the opposite team is chosen to be the target.
        /// Both unit types are compared to see if a +50% attack bonus can be applied.
        /// The final attack value is defined by : (sender.attack + bonus) - target.armor
        /// </summary>
        /// <param name="sender">The attacking unit</param>
        /// <param name="AttackPower">The attack power of the attacking unit</param>
        /// <param name="UnityType">The type of the attacking unit</param>
        /// <param name="Team">The team of the attacking unit</param>
        private void Unit_UnitAttack(object sender, double AttackPower, UnitType UnityType, UnitTeam Team)
        {
            var targetTeam = Team == UnitTeam.Red ? ListBlueUnits : ListRedUnits;
            if (targetTeam.Any()) {
                var index = _rand.Next(0, targetTeam.Count() - 1);
                var target = targetTeam.ElementAt(index);
                var realAttack = GetAttackBonus(AttackPower, UnityType, target.UnitType);
                realAttack = ((100 - target.Armor) * realAttack) / 100;
                target.Health -= realAttack;
            }
        }

        /// <summary>
        /// Compare two types of unit to see if a attack bonus can be applied
        /// </summary>
        /// <param name="attack">the initial value of the attack power of the attacking unit</param>
        /// <param name="attacker">the type of the attacking unit</param>
        /// <param name="target">the type of the target unit</param>
        /// <returns>return the attack power + bonus (if it can be applied)</returns>
        private double GetAttackBonus(double attack, UnitType attacker, UnitType target)
        {
            if ((attacker == UnitType.Cavalary && target == UnitType.Ranged)
                || (attacker == UnitType.Ranged && target == UnitType.Infantry)
                || (attacker == UnitType.Infantry && target == UnitType.Cavalary)) {
                attack *= 1.5;
            }
            return attack;
        }

        /// <summary>
        /// Callbacks for the unit add buttons
        /// </summary>
        private void OnNewRedInfantryClicked() { ListRedUnits.Add(HookCallBacks(UnitViewModel.GetNewInfantry(UnitTeam.Red, RedPoints))); }
        private void OnNewRedRangedClicked() { ListRedUnits.Add(HookCallBacks(UnitViewModel.GetNewRanged(UnitTeam.Red, RedPoints))); }
        private void OnNewRedCavalryClicked() { ListRedUnits.Add(HookCallBacks(UnitViewModel.GetNewCavalry(UnitTeam.Red, RedPoints))); }
        private void OnNewBlueInfantryClicked() { ListBlueUnits.Add(HookCallBacks(UnitViewModel.GetNewInfantry(UnitTeam.Blue, BluePoints))); }
        private void OnNewBlueRangedClicked() { ListBlueUnits.Add(HookCallBacks(UnitViewModel.GetNewRanged(UnitTeam.Blue, BluePoints))); }
        private void OnNewBlueCavalryClicked() { ListBlueUnits.Add(HookCallBacks(UnitViewModel.GetNewCavalry(UnitTeam.Blue, BluePoints))); }

        /// <summary>
        /// When a unit is clicked a edit form opens
        /// </summary>
        /// <param name="clickedItem">The clicked unit</param>
        private void OnUnitClicked(object clickedItem)
        {
            if (!IsBattleGoingOn) {
                FormViewModel = clickedItem as UnitViewModel;
                FormVisible = true;
            }
        }

        /// <summary>
        /// Starts the battle. Closes the form. disable all buttons.
        /// </summary>
        private async void DoStartBattle()
        {
            if (ListBlueUnits.Any() && ListRedUnits.Any())
            {
                IsBattleGoingOn = true;
                foreach (var one in ListRedUnits)
                {
                    one.StartAttack();
                }
                foreach (var one in ListBlueUnits)
                {
                    one.StartAttack();
                }
                FormVisible = false;
            }
            else
            {
                var message = new MessageDialog("Each team must have at least one unit");
                await message.ShowAsync();
            }
        }

        /// <summary>
        /// Resets the battle field.
        /// </summary>
        private void DoClearBattleField()
        {
            while(ListRedUnits.Any())
            {
                ListRedUnits.First().Dead();
            }
            while(ListBlueUnits.Any())
            {
                ListBlueUnits.First().Dead();
            }
            FormVisible = false;
            BluePoints = MAX_TEAM_POINTS;
            RedPoints = MAX_TEAM_POINTS;
        }

        /// <summary>
        /// Unit has benn killed by the user on the edit form
        /// </summary>
        private void OnKillSelectedUnitClicked()
        {
            if (FormViewModel != null)
            {
                FormViewModel.Dead();
                FormVisible = false;
            }
        }

        /// <summary>
        /// User closed the edit form.
        /// </summary>
        private void OnCancelEditUnitClicked()
        {
            FormVisible = false;
        }
        
        /// <summary>
        /// Callback for when a point is expent or retrieved from a unit.
        /// Updates the avaliable points for all the units in the field.
        /// </summary>
        /// <param name="sender">The object that fired the ObserverHelper.NotifyObservers()</param>
        /// <param name="parameter">The new value for the team points</param>
        public void Update(object sender, int parameter)
        {
            var unit = sender as UnitViewModel;
            if (unit != null)
            {
                if (unit.Team == UnitTeam.Blue)
                {
                    BluePoints = parameter;
                    foreach (var one in ListBlueUnits) { one.Points = BluePoints; }
                }
                else {
                    RedPoints = parameter;
                    foreach (var one in ListRedUnits) { one.Points = RedPoints; }
                }
            }
        }

        /// <summary>
        /// Unresgister the current instance of the viewmodel from the ObserverHelper.
        /// </summary>
        public void Dispose()
        {
            ObserverHelper<MainViewModel, int>.UnRegister(this);
        }

        ~MainViewModel() { Dispose(); }
    }
}