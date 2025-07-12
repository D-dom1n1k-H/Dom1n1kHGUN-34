using System;
using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(PositionSaver))]
    public class ReplayMover : MonoBehaviour
    {
        private PositionSaver _save;

        private int _index;
        private PositionSaver.Data _prev;
        private float _duration;

        private void Start()
        {
            ////todo comment: ����� ����� ��� ��������?
            //// �������� �������� ������ �� null, ���� ��������� PositionSaver �� ��� �������� ��� � �� ��� �������
            if (!TryGetComponent(out _save) || _save.Records.Count == 0)
            {
                Debug.LogError("Records incorrect value", this);
                //todo comment: ��� ���� ����������� ���� ���������?
                // ����� �� �������� Update, ���� ��� ������ ��� ���������������
                enabled = false;
            }
        }

        private void Update()
        {
            var curr = _save.Records[_index];
            //todo comment: ��� ��������� ��� ������� (� ����� �����)? 
            // ���������, ��� ������� ������� ����� ������ ������� � ������� ������. ��� ����� ��� ����, ����� ������������� ������� ������� � ������ ������ �������
            // ���� ������� ������� ����� ������ ������� � ������� ������, �� �� ��������� � ��������� ������
            if (Time.time > curr.Time)
            {
                _prev = curr;
                _index++;
                //todo comment: ��� ���� ����� ��� ��������?
                // ��� ���� ���-�� ���������� ���������������, ����� ��� ������ ���� ���������
                if (_index >= _save.Records.Count)
                {
                    enabled = false;
                    Debug.Log($"<b>{name}</b> finished", this);
                }
            }
            //todo comment: ��� ���� ������������ ��� ���������� (��� � ���������� ��� �����������)?
            // ��� ����� ����� ������� � ��������� ������, ������� ������������ ��� ������������ ����� ����� ��������� �������
            // �����, ������ �� ������ "����������������" ����� ���������
            var delta = (Time.time - _prev.Time) / (curr.Time - _prev.Time);
            //todo comment: ����� ����� ��� ��������?
            // ��� ������ ��������� �� delta �������� ������ (IsNaN -> Is Not a Number), ���� ��� �� ��� �������� �� 0f
            if (float.IsNaN(delta)) delta = 0f;
            //todo comment: �������, ��� ���������� � ���� ������� ��� ��������, ��������� ��� ��������
            // ��� ������ ������������� ����������� ����� �������. ��������:
            // A - ��� ���������� ������� �������, B - ��� ������� ������� �������, C - ��� ������� ����� A � B
            // A = 0; B = 3;
            // ���� delta = 0.5, �� ������ ��������� �� �������� ���� ����� A � B, ���� delta = 0.75, �� ������ ��������� �� 75% ���� ����� A � B
            // Vector3.Lerp(0, 3, 0.5f) | Vector3.Lerp(0, 3, 0.75f) ����� �������������� 1.5 � 2.25
            transform.position = Vector3.Lerp(_prev.Position, curr.Position, delta);
        }
    }
}