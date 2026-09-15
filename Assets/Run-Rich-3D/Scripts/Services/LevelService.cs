using UnityEngine;
using RunRich3D.Controllers;
using RunRich3D.Models;
using RunRich3D.Views;

namespace RunRich3D.Services
{
    public sealed class LevelService : MonoBehaviour, IGameService
    {
        [SerializeField] private Transform _levelRoot;
        [SerializeField] private PlayerService _playerService;
        [SerializeField] private GameObject _legacyPath;
        [SerializeField] private GameObject _dollarPrefab;
        [SerializeField] private GameObject _bottlePrefab;
        [SerializeField] private Mesh _groundMesh;
        [SerializeField] private Mesh _boxMesh;
        [SerializeField] private Mesh _flagMesh;
        [SerializeField] private Mesh _choiceDoorMesh;
        [SerializeField] private Mesh _partyMesh;
        [SerializeField] private Mesh _studyMesh;
        [SerializeField] private Mesh _finishBlueMesh;
        [SerializeField] private Mesh _finishGreenMesh;
        [SerializeField] private Mesh _finishOrangeMesh;
        [SerializeField] private Mesh _finishYellowMesh;
        [SerializeField] private Mesh _doorPoorMesh;
        [SerializeField] private Mesh _doorRichMesh;
        [SerializeField] private Mesh _doorMillionMesh;
        [SerializeField] private Material _pathMaterial;
        [SerializeField] private Material _moneyMaterial;
        [SerializeField] private Material _bottleMaterial;
        [SerializeField] private Material _flagMaterial;
        [SerializeField] private Material _choiceMaterial;
        [SerializeField] private Material _finishMaterial;
        [SerializeField] private Material _goodDoorMaterial;
        [SerializeField] private Material _poorDoorMaterial;
        [SerializeField] private Font _labelFont;

        private LevelModel _model;
        private LevelController _controller;

        internal ILevelEvents Events => _controller;

        public void Initialize()
        {
            if (_legacyPath != null)
            {
                _legacyPath.SetActive(false);
            }

            LevelLayout layout = LevelLayout.CreateFirst();
            _model = new LevelModel(layout);

            var builder = new LevelWorldBuilder(_levelRoot, CreateCatalog());
            LevelPieceView[] pickupViews = builder.Build(layout);

            _controller = new LevelController(_model, _playerService.Model, pickupViews);
            _controller.Initialize();
        }

        public void Dispose()
        {
            _controller?.Dispose();
            _controller = null;
        }

        private LevelWorldBuilder.Catalog CreateCatalog()
        {
            return new LevelWorldBuilder.Catalog(
                _dollarPrefab,
                _bottlePrefab,
                _groundMesh,
                _boxMesh,
                _flagMesh,
                _choiceDoorMesh,
                _partyMesh,
                _studyMesh,
                _finishBlueMesh,
                _finishGreenMesh,
                _finishOrangeMesh,
                _finishYellowMesh,
                _doorPoorMesh,
                _doorRichMesh,
                _doorMillionMesh,
                _pathMaterial,
                _moneyMaterial,
                _bottleMaterial,
                _flagMaterial,
                _choiceMaterial,
                _finishMaterial,
                _goodDoorMaterial,
                _poorDoorMaterial,
                _labelFont);
        }
    }
}
