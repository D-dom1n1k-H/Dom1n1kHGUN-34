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
            ////todo comment: зачем нужны эти проверки?
            //// Проверки избегают ошибки на null, если компонент PositionSaver не был добавлен или в нём нет записей
            if (!TryGetComponent(out _save) || _save.Records.Count == 0)
			{
				Debug.LogError("Records incorrect value", this);
                //todo comment: Для чего выключается этот компонент?
                // Чтобы не вызывать Update, если нет данных для воспроизведения
                enabled = false;
			}
		}

		private void Update()
		{
			var curr = _save.Records[_index];
            //todo comment: Что проверяет это условие (с какой целью)? 
            // Проверяет, что текущее игровое время больше времени в текущей записи. Это нужно для того, чтобы воспроизвести позицию объекта в нужный момент времени
            // Если текущее игровое время больше времени в текущей записи, то мы переходим к следующей записи
            if (Time.time > curr.Time)
			{
				_prev = curr;
				_index++;
                //todo comment: Для чего нужна эта проверка?
                // Для того что-бы остановить воспроизведение, когда все записи были проиграны
                if (_index >= _save.Records.Count)
				{
					enabled = false;
					Debug.Log($"<b>{name}</b> finished", this);
				}
			}
            //todo comment: Для чего производятся эти вычисления (как в дальнейшем они применяются)?
            // Это время между текущим и следующим кадром, которое используется для интерполяции между двумя позициями объекта
			// Иначе, объект бы просто "телепортировался" между позициями
            var delta = (Time.time - _prev.Time) / (curr.Time - _prev.Time);
            //todo comment: Зачем нужна эта проверка?
            // Эта строка проверяет ли delta является числом (IsNaN -> Is Not a Number), если нет то она меняется на 0f
            if (float.IsNaN(delta)) delta = 0f;
            //todo comment: Опишите, что происходит в этой строчке так подробно, насколько это возможно
            // Эта строка интерполирует перемещение между кадрами. Например:
            // A - это предыдущая позиция объекта, B - это текущая позиция объекта, C - это позиция между A и B
            // A = 0; B = 3;
            // Если delta = 0.5, то объект находится на половине пути между A и B, если delta = 0.75, то объект находится на 75% пути между A и B
            // Vector3.Lerp(0, 3, 0.5f) | Vector3.Lerp(0, 3, 0.75f) вернёт соответственно 1.5 и 2.25
            transform.position = Vector3.Lerp(_prev.Position, curr.Position, delta);
		}
	}
}