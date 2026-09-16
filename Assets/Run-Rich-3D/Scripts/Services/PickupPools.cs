using UnityEngine;
using RunRich3D.Views;

namespace RunRich3D.Services
{
    public sealed class PickupPools
    {
        public PickupPools(Transform parent, GameObject moneyPrefab, GameObject bottlePrefab, float spinDegrees)
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

        public PrefabPool<PickupSpinView> Money { get; }
        public PrefabPool<LevelPieceView> Bottles { get; }

        public void ReleaseAll()
        {
            Money?.ReleaseAll();
            Bottles?.ReleaseAll();
        }

        public void Dispose()
        {
            Money?.Dispose();
            Bottles?.Dispose();
        }
    }
}
