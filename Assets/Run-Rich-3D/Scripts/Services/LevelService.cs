using UnityEngine;
using Zenject;
using RunRich3D.Controllers;
using RunRich3D.Models;
using RunRich3D.Settings;
using RunRich3D.Views;

namespace RunRich3D.Services
{
    public sealed class LevelService : MonoBehaviour, IGameService
    {
        [Header("References")]
        [SerializeField] private Transform _levelRoot;
        [SerializeField] private Transform _pickupRoot;
        [SerializeField] private Transform _finishRoot;
        [SerializeField] private FlagView[] _flagViews;
        [SerializeField] private GateView _gateView;
        [SerializeField] private FinishDoorsView _finishDoorsView;
        [SerializeField] private GameObject _legacyPath;
        [SerializeField] private LevelSettings _settings;

        private PlayerService _playerService;
        private LevelModel _model;
        private LevelController _controller;
        private PickupPools _pickupPools;

        public ILevelEvents Events => _controller;
        public Transform LevelRoot => _levelRoot;

        [Inject]
        public void Construct(LevelSettings settings, PlayerService playerService)
        {
            if (settings != null)
            {
                _settings = settings;
            }

            _playerService = playerService;
        }

        public void Initialize()
        {
            if (_legacyPath != null)
            {
                _legacyPath.SetActive(false);
            }

            LevelWorldTuning world = _settings.World;
            var path = new PathBend(_settings.CreatePathSegments(), _settings.PathSurfaceY);
            LevelLayout layout = _settings.CreateLayout(path);
            _model = new LevelModel(layout, _settings.PickupHalfWidth, _settings.PickupHalfDepth, _settings.GateDepth);
            _playerService.BindPath(path);

            Transform pickupRoot = _pickupRoot != null ? _pickupRoot : _levelRoot;
            _pickupPools?.Dispose();
            _pickupPools = new PickupPools(pickupRoot, _settings.DollarPrefab, _settings.BottlePrefab, _settings.MoneySpinDegreesPerSecond);
            var builder = new LevelWorldBuilder(pickupRoot, CreateCatalog(), world, path, _pickupPools, _settings.PlusSignTexture);
            LevelWorldBuilder.BuiltLevel visuals = builder.BuildRuntimePickups(layout);
            FlagView[] flags = _flagViews != null ? _flagViews : new FlagView[0];
            FinishDoorsView finishDoors = BindFinishDoors(path);

            _controller = new LevelController(
                _model,
                _playerService.Model,
                visuals.Pickups,
                flags,
                _gateView,
                finishDoors,
                _settings.FlagRaiseStart,
                _settings.FlagRaiseEnd,
                _settings.FlagRaiseDetectThreshold,
                _settings.MinDoorMultiplier);
            _controller.Initialize();
        }

        public void Dispose()
        {
            _controller?.Dispose();
            _controller = null;
            _pickupPools?.Dispose();
            _pickupPools = null;
        }

        public FlagView[] BakeStaticWorld(Transform staticRoot, LevelWorldBuilder.Catalog catalog)
        {
            if (_settings == null)
            {
                return new FlagView[0];
            }

            LevelWorldTuning world = _settings.World;
            PathSegment[] segments = _settings.CreatePathSegments();
            var path = new PathBend(segments, 0f);
            LevelLayout layout = _settings.CreateLayout(path);
            var builder = new LevelWorldBuilder(staticRoot, catalog, world, path);
            return builder.BakeStatic(layout);
        }

        private FinishDoorsView BindFinishDoors(PathBend path)
        {
            FinishDoorsView view = _finishDoorsView;
            Transform finish = _finishRoot != null
                ? _finishRoot
                : view != null ? view.transform : null;
            if (view == null && finish != null)
            {
                view = EntityViewFactory.CreateOn<FinishDoorsView>(finish.gameObject);
                _finishDoorsView = view;
            }

            if (view == null)
            {
                return null;
            }

            view.BindSettings(
                _settings.DoorOpenStart,
                _settings.DoorOpenEnd,
                _settings.DoorOpenAngle,
                _settings.DoorStopGap,
                _settings.AfterLastOpenPadding,
                _settings.ClosedLaneClamp,
                _settings.DoorPassEpsilon,
                _settings.ClosedLaneName);
            float finishZ = _settings.FinishZ;
            PathPose pose = path.Sample(finishZ, 0f);
            Vector3 pathForward = Quaternion.Euler(0f, pose.YawDegrees, 0f) * Vector3.forward;
            view.Bind(pathForward, new Vector3(pose.X, pose.Y, pose.Z), finishZ);
            return view;
        }

        private LevelWorldBuilder.Catalog CreateCatalog()
        {
            return new LevelWorldBuilder.Catalog(
                _settings.DollarPrefab,
                _settings.BottlePrefab,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                _settings.MoneyMaterial,
                _settings.BottleMaterial,
                null,
                null,
                null,
                null,
                null,
                null,
                null);
        }
    }
}
