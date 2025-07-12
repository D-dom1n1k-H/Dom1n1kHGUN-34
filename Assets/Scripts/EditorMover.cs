using UnityEngine;

namespace DefaultNamespace
{

    [RequireComponent(typeof(PositionSaver))]
    public class EditorMover : MonoBehaviour
    {
        private PositionSaver _save;
        private float _currentDelay;

        //todo comment: ��� ���������, ���� _delay > _duration?
        // ������ ��������� ������� ������ �� ���������
        [SerializeField, Range(0.2f, 1.0f)]
        private float _delay = 0.5f;
        [SerializeField, Min(0.2f)]
        private float _duration = 5f;

        private void Start()
        {
            //todo comment: ������ ���� ����� ������������ �����, � �� � ������ ������ Update?
            // ������-��� ���������� ���� ��� ��������� ��������� PositionSaver, � �� ������ ����
            if (_duration < _delay)
            {
                _duration = _delay * 5;
            }

            _save = GetComponent<PositionSaver>();
            _save.Records.Clear();
        }

        private void Update()
        {
            _duration -= Time.deltaTime;
            if (_duration <= 0f)
            {
                enabled = false;
                Debug.Log($"<b>{name}</b> finished", this);
                return;
            }

            //todo comment: ������ �� �������� (_delay -= Time.deltaTime;) �� �������� � ����� _duration?
            // ���� �������� ���, �� �������� _delay ����� ����������� �� ���� � ������ ����� ����������� ������� �����
            _currentDelay -= Time.deltaTime;
            if (_currentDelay <= 0f)
            {
                _currentDelay = _delay;
                _save.Records.Add(new PositionSaver.Data
                {
                    Position = transform.position,
                    //todo comment: ��� ���� ����������� �������� �������� �������?
                    // ��������, ��� �������� ������������
                    Time = Time.time,
                });
            }
        }
    }
}