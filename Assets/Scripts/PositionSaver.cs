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
        [SerializeField, ReadOnly,Tooltip("��� ���������� ����� ���� ����� ��������������� ����������� ���� � ���������� � �������� �Create File�")]
        private TextAsset _json;

        [SerializeField, HideInInspector]
        public List<Data> Records;

        private void Awake()
        {


            //todo comment: ��� �����, ���� � ���� ����� ������� �� ������� ����� �� ������?
            // ����� ������ NullReferenceException, �.�. _json ����� ����� null
            if (_json == null)
            {
                gameObject.SetActive(false);
                Debug.LogError("Please, create TextAsset and add in field _json");
                return;
            }

            JsonUtility.FromJsonOverwrite(_json.text, this);
            //todo comment: ��� ���� ����� ��� �������� (��� ��� ��������� ��������)?
            // ��� ��������� �������� ������ ��� ��������� Records �� ����������, � ��� ��� ������ �������� ���� ��� � ������������ � ����������
            if (Records == null)
                Records = new List<Data>(10);
        }

        private void OnDrawGizmos()
        {
            //todo comment: ����� ����� ��� �������� (��� ��� ���������� ��������)?
            // ��������� �������� ������ NullReferenceException, ���� � ��������� ��� ������ � ������ �� ����� ���������
            if (Records == null || Records.Count == 0) return;
            var data = Records;
            var prev = data[0].Position;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(prev, 0.3f);
            //todo comment: ������ �������� ���������� �� � �������� ��������?
            // �������-�� ������� ������� ��� ��������� �����, �� ������� �������� �����
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
            //todo comment: ��� ���������� � ���� ������?
            // �������� ���� Path.txt � ����� Assets, Aplication.dataPath - ��� ���� � ����� Assets
            var stream = File.Create(Path.Combine(Application.dataPath, "Path.txt"));
            //todo comment: ��������� ��� ���� ����� ��� ������? (� ����� ��������� �������, ���������������) 
            // �������: Dispoce ������� ����������� ��� ����������/���������, ������� ������� ����-�� ��� ��� ������ ������� ������ �� �����
            // �����: Dispose ��������� ������ � ������, ��������� ����� File.Create()
            stream.Dispose();
            UnityEditor.AssetDatabase.Refresh();
            //� Unity ����� ������ ������� �� �� ����, ��� ����� ������������ ������� "t:"
            //����� ����������, ����� ���������� ������ ������ (������� � ����-������ ��������, ��������)
            var guids = UnityEditor.AssetDatabase.FindAssets("t:TextAsset");
            foreach (var guid in guids)
            {
                //���� �������� ����� �������� ���� � ������ ����� ��� ����
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                //���� �������� ����� ��������� ��� �����
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                //todo comment: ��� ���� ����� ��� ��������?
                // �������� �������� �� NullReferenceException � ��� ����� ����� ��������� ��� �� ����� ������ ��� �����, ������� ��� �����
                if (asset != null && asset.name == "Path")
                {
                    _json = asset;
                    UnityEditor.EditorUtility.SetDirty(this);
                    UnityEditor.AssetDatabase.SaveAssets();
                    UnityEditor.AssetDatabase.Refresh();
                    //todo comment: ������ �� ����� �������, � �� ���������� �������������?
                    // ������-��� �� ��� ����� ������ �����
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