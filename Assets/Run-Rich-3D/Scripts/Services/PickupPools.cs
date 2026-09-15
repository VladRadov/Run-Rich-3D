using UnityEngine;
using RunRich3D.Views;

namespace RunRich3D.Services
{
    internal sealed class PickupPools
    {
        internal PickupPools(Transform parent, GameObject moneyPrefab, GameObject bottlePrefab, float spinDegrees)
        {
            if (moneyPrefab != null)
            {
                Money = new PrefabPool<PickupSpinView>(parent, moneyPrefab, view =>
                {
                    view.BindSpin(spinDegrees);
                });
            }

            if (bottlePrefab != null)
            {
                Bottles = new PrefabPool<LevelPieceView>(parent, bottlePrefab, view =>
                {
                    view.Bind();
                });
            }
        }

        internal PrefabPool<PickupSpinView> Money { get; }
        internal PrefabPool<LevelPieceView> Bottles { get; }

        internal void ReleaseAll()
        {
            Money?.ReleaseAll();
            Bottles?.ReleaseAll();
        }

        internal void Dispose()
        {
            Money?.Dispose();
            Bottles?.Dispose();
        }
    }
}
