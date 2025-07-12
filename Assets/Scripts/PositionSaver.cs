using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DefaultNamespace
{
	public class PositionSaver : MonoBehaviour
	{
		[Serializable]
		public struct Data
		{
			public Vector3 Position;
			public float Time;
		}
		[SerializeField ,ReadOnly, Tooltip("Для заполнения этого поля нужно воспользоваться контекстным меню в инспекторе и командой “Create File”")]
		private TextAsset _json;

		[SerializeField, HideInInspector]
		public List<Data> Records;

		private void Awake()
		{


            //todo comment: Что будет, если в теле этого условия не сделать выход из метода?
            // Будет ошибка NullReferenceException, т.к. _json будет равен null
            if (_json == null)
			{
				gameObject.SetActive(false);
				Debug.LogError("Please, create TextAsset and add in field _json");
				return;
			}
			
			JsonUtility.FromJsonOverwrite(_json.text, this);
            //todo comment: Для чего нужна эта проверка (что она позволяет избежать)?
            // Она позволяет избежать ошибки что коллекция Records не существует, а так она просто создаётся один раз и используется в дальнейшем
            if (Records == null)
				Records = new List<Data>(10);
		}

		private void OnDrawGizmos()
		{
            //todo comment: Зачем нужны эти проверки (что они позволляют избежать)?
            // Позволяют избежать ошибки NullReferenceException, если в коллекции нет данных и нечего не будет рисоватся
            if (Records == null || Records.Count == 0) return;
			var data = Records;
			var prev = data[0].Position;
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(prev, 0.3f);
            //todo comment: Почему итерация начинается не с нулевого элемента?
            // Потомуч-то нулевой элемент это стартовая точка, от которой рисуется линия
            for (int i = 1; i < data.Count; i++)
			{
				var curr = data[i].Position;
				Gizmos.DrawWireSphere(curr, 0.3f);
				Gizmos.DrawLine(prev, curr);
				prev = curr;
			}
		}
		
#if UNITY_EDITOR
		[ContextMenu("Create File")]
		private void CreateFile()
		{
            //todo comment: Что происходит в этой строке?
            // Создаётся файл Path.txt в папке Assets, Aplication.dataPath - это путь к папке Assets
            var stream = File.Create(Path.Combine(Application.dataPath, "Path.txt"));
            //todo comment: Подумайте для чего нужна эта строка? (а потом проверьте догадку, закомментировав) 
            // Догадка: Dispoce кажется переводится как освободить/выбрасить, поэтому логично было-бы что эта строка удаляет данные из файла
            // Ответ: Dispose завершает работу с файлом, созданным через File.Create()
            stream.Dispose();
			UnityEditor.AssetDatabase.Refresh();
			//В Unity можно искать объекты по их типу, для этого используется префикс "t:"
			//После нахождения, Юнити возвращает массив гуидов (которые в мета-файлах задаются, например)
			var guids = UnityEditor.AssetDatabase.FindAssets("t:TextAsset");
			foreach (var guid in guids)
			{
				//Этой командой можно получить путь к ассету через его гуид
				var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
				//Этой командой можно загрузить сам ассет
				var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                //todo comment: Для чего нужны эти проверки?
                // Типичная проверка на NullReferenceException и имя файла чтобы убедиться что мы нашли именно тот ассет, который нам нужен
                if (asset != null && asset.name == "Path")
				{
					_json = asset;
					UnityEditor.EditorUtility.SetDirty(this);
					UnityEditor.AssetDatabase.SaveAssets();
					UnityEditor.AssetDatabase.Refresh();
                    //todo comment: Почему мы здесь выходим, а не продолжаем итерироваться?
                    // Потому-что мы уже нашли нужный ассет
                    return;
				}
			}
		}

		[Serializable]
		class Wrapper 
		{ 
			public List<PositionSaver.Data> Records;
        }
		private void OnDestroy()
		{
			if (Records != null && Records.Count > 0)
			{
				var wrapper = new Wrapper { Records = Records };
				string path = UnityEditor.AssetDatabase.GetAssetPath(_json);
				var json = JsonUtility.ToJson(wrapper, true);

				File.WriteAllText(path, json);
				UnityEditor.AssetDatabase.Refresh();
            }	
        }
#endif
	}
}