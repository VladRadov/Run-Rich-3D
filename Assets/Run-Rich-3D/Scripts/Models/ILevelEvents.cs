using System;
using UniRx;

namespace RunRich3D.Models
{
    internal interface ILevelEvents
    {
        IObservable<int> FinishReached { get; }
        IObservable<int> PickupCollected { get; }
        IObservable<int> WealthGained { get; }
        IObservable<int> WealthLost { get; }
        IObservable<Unit> FlagRaised { get; }
        IObservable<Unit> DoorOpened { get; }
    }
}
