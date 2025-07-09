using UnityEngine;

namespace DefaultNamespace
{
	
	[RequireComponent(typeof(PositionSaver))]
	public class EditorMover : MonoBehaviour
	{
		private PositionSaver _save;
		private float _currentDelay;

		//todo comment: Что произойдёт, если _delay > _duration?
		// Записи положения объекта просто не произойдёт
		[SerializeField ,Range(0.2f, 1.0f)]
		private float _delay = 0.5f;
		[SerializeField, Min(0.2f)]
		private float _duration = 5f;

		private void Start()
		{
            //todo comment: Почему этот поиск производится здесь, а не в начале метода Update?
            // Потому-что достаточно один раз присвоить компонент PositionSaver, а не каждый кадр
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

            //todo comment: Почему не написать (_delay -= Time.deltaTime;) по аналогии с полем _duration?
            // Если написать так, то значение _delay будет уменьшаться до нуля и записи будут происходить слишком часто
            _currentDelay -= Time.deltaTime;
			if (_currentDelay <= 0f)
			{
				_currentDelay = _delay;
				_save.Records.Add(new PositionSaver.Data
				{
					Position = transform.position,
					//todo comment: Для чего сохраняется значение игрового времени?
					// Вероятно, для будующей интерполяции
					Time = Time.time,
				});
			}
		}
	}
}