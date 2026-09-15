using UniRx;

namespace RunRich3D.Models
{
    internal sealed class GameLoopModel
    {
        internal IReadOnlyReactiveProperty<int> LevelNumber => _levelNumber;

        private readonly IntReactiveProperty _levelNumber = new IntReactiveProperty(1);

        internal void AdvanceLevel()
        {
            _levelNumber.Value += 1;
        }
    }
}
