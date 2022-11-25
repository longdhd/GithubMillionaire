using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;
using TMPro;
using System.Text;

public class ProfilerUI : MonoBehaviour
{
    private TextMeshProUGUI m_Text;
    ProfilerRecorder m_GCMemoryRecorder;
    // Start is called before the first frame update
    void Awake()
    {
        m_Text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        m_GCMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory");
    }

    // Update is called once per frame
    void Update()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"GC Memory\t\t{m_GCMemoryRecorder.LastValue}");
        m_Text.text = stringBuilder.ToString();
    }

    private void OnDisable()
    {
        m_GCMemoryRecorder.Dispose();
    }
}
