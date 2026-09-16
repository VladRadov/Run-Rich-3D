using UniRx;
using RunRich3D.Views;

namespace RunRich3D.Controllers
{
    public sealed class LevelPresentationController
    {
        private readonly FlagView[] _flagViews;
        private readonly FinishDoorsView _finishDoors;
        private readonly float _flagRaiseStart;
        private readonly float _flagRaiseEnd;
        private readonly float _raiseStartThreshold;

        public LevelPresentationController(
            FlagView[] flagViews,
            FinishDoorsView finishDoors,
            float flagRaiseStart,
            float flagRaiseEnd,
            float flagRaiseDetectThreshold)
        {
            _flagViews = flagViews;
            _finishDoors = finishDoors;
            _flagRaiseStart = flagRaiseStart;
            _flagRaiseEnd = flagRaiseEnd;
            _raiseStartThreshold = flagRaiseDetectThreshold > 0f ? flagRaiseDetectThreshold : 0.01f;
        }

        public int UpdateFlags(float forward, bool playing)
        {
            if (_flagViews == null)
            {
                return 0;
            }

            int raised = 0;
            for (int i = 0; i < _flagViews.Length; i++)
            {
                float amount = RaiseAmount(forward, _flagViews[i].TriggerZ);
                if (playing && _flagViews[i].Raised <= _raiseStartThreshold && amount > _raiseStartThreshold)
                {
                    raised++;
                }

                _flagViews[i].SetRaised(amount);
            }

            return raised;
        }

        public int UpdateDoors(float forward, bool playing)
        {
            if (_finishDoors == null)
            {
                return 0;
            }

            int opened = _finishDoors.UpdateOpen(forward);
            return playing ? opened : 0;
        }

        public void Reset()
        {
            if (_flagViews != null)
            {
                for (int i = 0; i < _flagViews.Length; i++)
                {
                    _flagViews[i].SetRaised(0f);
                }
            }

            if (_finishDoors != null)
            {
                _finishDoors.Close();
            }
        }

        private float RaiseAmount(float playerZ, float flagZ)
        {
            float ahead = flagZ - playerZ;
            if (ahead <= _flagRaiseEnd)
            {
                return 1f;
            }

            if (ahead >= _flagRaiseStart)
            {
                return 0f;
            }

            return 1f - (ahead - _flagRaiseEnd) / (_flagRaiseStart - _flagRaiseEnd);
        }
    }
}
