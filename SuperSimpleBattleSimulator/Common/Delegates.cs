using SuperSimpleBattleSimulator.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSimpleBattleSimulator.Delegates
{
    public delegate void AttackEventHandler(object sender, double AttackPower, UnitType UnityType, UnitTeam Team);
    public delegate void DeathEventHandler(object sender, UnitTeam Team);
}
