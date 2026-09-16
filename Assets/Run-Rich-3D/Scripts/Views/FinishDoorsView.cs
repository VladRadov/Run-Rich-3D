using System.Collections.Generic;
using UnityEngine;

namespace RunRich3D.Views
{
    public sealed class FinishDoorsView : MonoBehaviour
    {
        [SerializeField] private float _openStart = 5.5f;
        [SerializeField] private float _openEnd = 0.35f;
        [SerializeField] private float _openAngle = 95f;
        [SerializeField] private float _stopGap = 2.8f;
        [SerializeField] private float _passEpsilon = 0.05f;
        [SerializeField] private string _closedLaneName = "LaneX5";

        private readonly List<Gate> _gates = new List<Gate>();
        private Vector3 _pathForward = Vector3.forward;
        private Vector3 _inward = Vector3.right;
        private Vector3 _pathOrigin;
        private float _originForward;
        private float _afterLastOpenPadding = 1.2f;
        private float _closedLaneClamp = 0.8f;
        private bool _bound;

        public void BindSettings(
            float openStart,
            float openEnd,
            float openAngle,
            float stopGap,
            float afterLastOpenPadding,
            float closedLaneClamp,
            float passEpsilon,
            string closedLaneName)
        {
            _openStart = openStart;
            _openEnd = openEnd;
            _openAngle = openAngle;
            _stopGap = stopGap;
            _afterLastOpenPadding = afterLastOpenPadding;
            _closedLaneClamp = closedLaneClamp;
            _passEpsilon = passEpsilon;
            if (!string.IsNullOrEmpty(closedLaneName))
            {
                _closedLaneName = closedLaneName;
            }
        }

        public void Bind(Vector3 pathForwardWorld, Vector3 pathOrigin, float originForward)
        {
            Vector3 forward = pathForwardWorld;
            forward.y = 0f;
            _pathForward = forward.sqrMagnitude > 0.01f ? forward.normalized : Vector3.forward;
            Vector3 inward = Vector3.Cross(Vector3.up, _pathForward);
            _inward = inward.sqrMagnitude > 0.01f ? inward.normalized : Vector3.right;
            _pathOrigin = pathOrigin;
            _originForward = originForward;
            _gates.Clear();
            LastTrigger = originForward;
            StopForward = originForward;
            CollectGates();
            _bound = true;
            Close();
        }

        public float LastTrigger { get; private set; }

        public float StopForward { get; private set; }

        public int DoorMultiplier(float playerForward)
        {
            int passed = 0;
            for (int i = 0; i < _gates.Count; i++)
            {
                if (playerForward + _passEpsilon >= _gates[i].Trigger)
                {
                    passed++;
                }
            }

            if (passed < 1)
            {
                passed = 1;
            }

            return passed + 1;
        }

        public int UpdateOpen(float playerForward)
        {
            if (!_bound)
            {
                return 0;
            }

            int opened = 0;
            for (int i = 0; i < _gates.Count; i++)
            {
                if (_gates[i].SetOpen(OpenAmount(playerForward, _gates[i].Trigger)))
                {
                    opened++;
                }
            }

            return opened;
        }

        public void Close()
        {
            if (!_bound)
            {
                return;
            }

            for (int i = 0; i < _gates.Count; i++)
            {
                _gates[i].SetOpen(0f);
            }
        }

        private float OpenAmount(float playerForward, float gateForward)
        {
            float ahead = gateForward - playerForward;
            if (ahead <= _openEnd)
            {
                return 1f;
            }

            if (ahead >= _openStart)
            {
                return 0f;
            }

            return 1f - (ahead - _openEnd) / (_openStart - _openEnd);
        }

        private void CollectGates()
        {
            bool hasClosedLane = false;
            float closedTrigger = _originForward;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (IsClosedLane(child.name))
                {
                    closedTrigger = MeasureDoorTrigger(child);
                    hasClosedLane = true;
                    continue;
                }

                var leaves = new List<Leaf>();
                CollectDoors(child, leaves);
                if (leaves.Count == 0)
                {
                    continue;
                }

                float trigger = AverageTrigger(leaves);
                _gates.Add(new Gate(leaves.ToArray(), trigger));
                if (trigger > LastTrigger)
                {
                    LastTrigger = trigger;
                }
            }

            if (!hasClosedLane)
            {
                StopForward = LastTrigger;
                return;
            }

            float afterLastOpen = LastTrigger + _afterLastOpenPadding;
            float beforeClosed = closedTrigger - _stopGap;
            StopForward = beforeClosed > afterLastOpen ? beforeClosed : afterLastOpen;
            float closedLimit = closedTrigger - _closedLaneClamp;
            if (closedLimit > LastTrigger && StopForward > closedLimit)
            {
                StopForward = closedLimit;
            }

            if (StopForward < LastTrigger)
            {
                StopForward = LastTrigger;
            }
        }

        private float MeasureDoorTrigger(Transform node)
        {
            var triggers = new List<float>();
            CollectDoorTriggers(node, triggers);
            if (triggers.Count == 0)
            {
                CollectMeshTriggers(node, triggers);
            }

            if (triggers.Count == 0)
            {
                return TriggerOf(node.position);
            }

            float max = triggers[0];
            for (int i = 1; i < triggers.Count; i++)
            {
                if (triggers[i] > max)
                {
                    max = triggers[i];
                }
            }

            return max;
        }

        private void CollectMeshTriggers(Transform node, List<float> triggers)
        {
            if (node.GetComponent<MeshFilter>() != null)
            {
                triggers.Add(TriggerOf(node.position));
            }

            for (int i = 0; i < node.childCount; i++)
            {
                CollectMeshTriggers(node.GetChild(i), triggers);
            }
        }

        private void CollectDoorTriggers(Transform node, List<float> triggers)
        {
            if (IsBakedPlaque(node.name))
            {
                return;
            }

            if (IsSwingingDoor(node.name))
            {
                triggers.Add(TriggerOf(node.position));
                return;
            }

            for (int i = 0; i < node.childCount; i++)
            {
                CollectDoorTriggers(node.GetChild(i), triggers);
            }
        }

        private void CollectDoors(Transform node, List<Leaf> leaves)
        {
            if (IsBakedPlaque(node.name))
            {
                return;
            }

            if (IsSwingingDoor(node.name))
            {
                AddDoor(node, leaves);
                return;
            }

            for (int i = 0; i < node.childCount; i++)
            {
                CollectDoors(node.GetChild(i), leaves);
            }
        }

        private void AddDoor(Transform door, List<Leaf> leaves)
        {
            if (door.childCount == 0)
            {
                TryAddLeaf(door, leaves);
                return;
            }

            bool addedChild = false;
            for (int i = 0; i < door.childCount; i++)
            {
                Transform child = door.GetChild(i);
                if (child.GetComponent<MeshFilter>() == null)
                {
                    continue;
                }

                if (TryAddLeaf(child, leaves))
                {
                    addedChild = true;
                }
            }

            if (!addedChild)
            {
                TryAddLeaf(door, leaves);
            }
        }

        private bool TryAddLeaf(Transform leaf, List<Leaf> leaves)
        {
            Vector3 closedDir = LeafDirection(leaf);
            closedDir.y = 0f;
            if (closedDir.sqrMagnitude < 0.0001f)
            {
                return false;
            }

            closedDir.Normalize();
            float angle = Vector3.SignedAngle(closedDir, _pathForward, Vector3.up);
            if (Mathf.Abs(angle) < 12f || Mathf.Abs(angle) > 150f)
            {
                angle = _openAngle * LeafOpenSign(leaf.position);
            }

            angle = Mathf.Clamp(angle, -_openAngle, _openAngle);
            leaves.Add(new Leaf(leaf, leaf.localRotation, Quaternion.Euler(0f, angle, 0f), TriggerOf(leaf.position)));
            return true;
        }

        private float LeafOpenSign(Vector3 worldPos)
        {
            Vector3 delta = worldPos - _pathOrigin;
            delta.y = 0f;
            float lateral = Vector3.Dot(delta, _inward);
            return lateral >= 0f ? -1f : 1f;
        }

        private float TriggerOf(Vector3 worldPos)
        {
            Vector3 delta = worldPos - _pathOrigin;
            delta.y = 0f;
            return _originForward + Vector3.Dot(delta, _pathForward);
        }

        private static float AverageTrigger(List<Leaf> leaves)
        {
            float sum = 0f;
            for (int i = 0; i < leaves.Count; i++)
            {
                sum += leaves[i].Trigger;
            }

            return sum / leaves.Count;
        }

        private static Vector3 LeafDirection(Transform leaf)
        {
            var filter = leaf.GetComponent<MeshFilter>();
            if (filter != null && filter.sharedMesh != null)
            {
                return leaf.TransformVector(filter.sharedMesh.bounds.center);
            }

            return leaf.TransformVector(Vector3.right);
        }

        private bool IsClosedLane(string name)
        {
            return string.Equals(name, _closedLaneName, System.StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsBakedPlaque(string name)
        {
            return name == "Door";
        }

        private static bool IsSwingingDoor(string name)
        {
            return name.StartsWith("Door", System.StringComparison.OrdinalIgnoreCase) && name != "Door";
        }

        private sealed class Gate
        {
            private readonly Leaf[] _leaves;
            private float _open = -1f;

            public Gate(Leaf[] leaves, float trigger)
            {
                _leaves = leaves;
                Trigger = trigger;
            }

            public float Trigger { get; }

            public bool SetOpen(float open)
            {
                float t = Mathf.Clamp01(open);
                if (Mathf.Abs(t - _open) < 0.001f)
                {
                    return false;
                }

                bool started = _open <= 0.001f && t > 0.001f;
                _open = t;
                float eased = t * t * (3f - 2f * t);
                for (int i = 0; i < _leaves.Length; i++)
                {
                    _leaves[i].Apply(eased);
                }

                return started;
            }
        }

        private readonly struct Leaf
        {
            private readonly Transform _transform;
            private readonly Quaternion _closed;
            private readonly Quaternion _delta;

            public Leaf(Transform transform, Quaternion closed, Quaternion delta, float trigger)
            {
                _transform = transform;
                _closed = closed;
                _delta = delta;
                Trigger = trigger;
            }

            public float Trigger { get; }

            public void Apply(float t)
            {
                if (_transform != null)
                {
                    _transform.localRotation = _closed * Quaternion.Slerp(Quaternion.identity, _delta, t);
                }
            }
        }
    }
}
