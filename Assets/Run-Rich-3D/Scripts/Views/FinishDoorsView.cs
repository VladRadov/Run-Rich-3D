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

        private const string ClosedLaneName = "LaneX5";

        private readonly List<Gate> _gates = new List<Gate>();
        private Vector3 _inward = Vector3.right;
        private Vector3 _pathOrigin;
        private float _originForward;
        private bool _bound;

        internal void Bind(Vector3 inwardWorld, Vector3 pathOrigin, float originForward)
        {
            _inward = inwardWorld.sqrMagnitude > 0.01f ? inwardWorld.normalized : transform.right;
            _pathOrigin = pathOrigin;
            _originForward = originForward;
            _gates.Clear();
            LastTrigger = originForward;
            StopForward = originForward;
            CollectGates();
            _bound = true;
            Close();
        }

        internal float LastTrigger { get; private set; }

        internal float StopForward { get; private set; }

        internal void UpdateOpen(float playerForward)
        {
            if (!_bound)
            {
                return;
            }

            for (int i = 0; i < _gates.Count; i++)
            {
                _gates[i].SetOpen(OpenAmount(playerForward, _gates[i].Trigger));
            }
        }

        internal void Close()
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

            float afterLastOpen = LastTrigger + 1.2f;
            float beforeClosed = closedTrigger - _stopGap;
            StopForward = beforeClosed > afterLastOpen ? beforeClosed : afterLastOpen;
            if (StopForward > closedTrigger - 0.8f)
            {
                StopForward = closedTrigger - 0.8f;
            }
        }

        private float MeasureDoorTrigger(Transform node)
        {
            var triggers = new List<float>();
            CollectDoorTriggers(node, triggers);
            if (triggers.Count == 0)
            {
                return TriggerOf(node.position);
            }

            float sum = 0f;
            for (int i = 0; i < triggers.Count; i++)
            {
                sum += triggers[i];
            }

            return sum / triggers.Count;
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

            float angle = Vector3.SignedAngle(closedDir, _inward, Vector3.up);
            if (Mathf.Abs(angle) < 12f || Mathf.Abs(angle) > 150f)
            {
                angle = _openAngle * Mathf.Sign(Vector3.Cross(closedDir, _inward).y);
            }

            angle = Mathf.Clamp(angle, -_openAngle, _openAngle);
            leaves.Add(new Leaf(leaf, leaf.localRotation, Quaternion.Euler(0f, angle, 0f), TriggerOf(leaf.position)));
            return true;
        }

        private float TriggerOf(Vector3 worldPos)
        {
            Vector3 delta = worldPos - _pathOrigin;
            delta.y = 0f;
            return _originForward + Vector3.Dot(delta, _inward);
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

        private static bool IsClosedLane(string name)
        {
            return string.Equals(name, ClosedLaneName, System.StringComparison.OrdinalIgnoreCase);
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

            internal Gate(Leaf[] leaves, float trigger)
            {
                _leaves = leaves;
                Trigger = trigger;
            }

            internal float Trigger { get; }

            internal void SetOpen(float open)
            {
                float t = Mathf.Clamp01(open);
                if (Mathf.Abs(t - _open) < 0.001f)
                {
                    return;
                }

                _open = t;
                float eased = t * t * (3f - 2f * t);
                for (int i = 0; i < _leaves.Length; i++)
                {
                    _leaves[i].Apply(eased);
                }
            }
        }

        private readonly struct Leaf
        {
            private readonly Transform _transform;
            private readonly Quaternion _closed;
            private readonly Quaternion _delta;

            internal Leaf(Transform transform, Quaternion closed, Quaternion delta, float trigger)
            {
                _transform = transform;
                _closed = closed;
                _delta = delta;
                Trigger = trigger;
            }

            internal float Trigger { get; }

            internal void Apply(float t)
            {
                if (_transform != null)
                {
                    _transform.localRotation = _closed * Quaternion.Slerp(Quaternion.identity, _delta, t);
                }
            }
        }
    }
}
