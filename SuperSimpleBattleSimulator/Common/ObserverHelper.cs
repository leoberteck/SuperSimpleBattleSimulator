using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSimpleBattleSimulator.Common
{

    /// <summary>
    /// Class that dispenses the use of IObservable. 
    /// It manages Observable classes of any type.
    /// </summary>
    /// <typeparam name="T">Type of the observer class</typeparam>
    /// <typeparam name="U">Type of the parameter to be sent on the NotifyObservers method</typeparam>
    public static class ObserverHelper<T, U> where T : IObserver<U>
    {
        private static List<T> listObservers { get; set; }
        static ObserverHelper()
        {
            listObservers = new List<T>();
        }

        public static void Register(T observer) {
            if (!listObservers.Contains(observer))
                listObservers.Add(observer);
        }

        public static void UnRegister(T observer)
        {
            listObservers.Remove(observer);
        }

        public static void NotifyObservers(object sender, U parameter) {
            listObservers.ForEach(t => t.Update(sender, parameter));
        }
    }

    public interface IObserver<U> : IDisposable
    {
        void Update(object sender, U parameter);
    }
}
