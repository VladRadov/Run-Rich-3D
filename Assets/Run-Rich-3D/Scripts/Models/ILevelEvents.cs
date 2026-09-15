using System;
using UniRx;

namespace RunRich3D.Models
{
    internal interface ILevelEvents
    {
        IObservable<int> FinishReached { get; }
        IObservable<int> PickupCollected { get; }
    }
}
