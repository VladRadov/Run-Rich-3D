using UnityEngine;

namespace RunRich3D.Views
{
    public sealed class GateView : MonoBehaviour
    {
        [SerializeField] private GameObject[] _leftPieces;
        [SerializeField] private GameObject[] _rightPieces;

        public void Bind(GameObject[] leftPieces, GameObject[] rightPieces)
        {
            _leftPieces = leftPieces;
            _rightPieces = rightPieces;
            ShowAll();
        }

        public void HidePassedSide(bool left)
        {
            SetPieces(left ? _leftPieces : _rightPieces, false);
        }

        public void ShowAll()
        {
            SetPieces(_leftPieces, true);
            SetPieces(_rightPieces, true);
        }

        private static void SetPieces(GameObject[] pieces, bool visible)
        {
            if (pieces == null)
            {
                return;
            }

            for (int i = 0; i < pieces.Length; i++)
            {
                if (pieces[i] != null)
                {
                    pieces[i].SetActive(visible);
                }
            }
        }
    }
}
