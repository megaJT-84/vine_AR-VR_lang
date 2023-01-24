using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using MRTK.Tutorials.GettingStarted;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.IO;
using Unity.XR.CoreUtils;
using System.Linq;

#if !UNITY_EDITOR && UNITY_METRO
using System.Threading.Tasks;
using Windows.Storage;
#endif

public class ExpWriter_PC_Unity_Editor : MonoBehaviour
{
    [SerializeField] List<GameObject> target_Object;
    private string m_dataFolderPath;
    private string m_dataFileName = "MemoryPalaceData ";
    private string m_dataExtension = ".csv";
    private string SessionFolderRoot = "Exp_data";
    private string m_sessionPath;
    private string m_experimentData;
    public string ExperimentData
    {
        get
        {
            return m_experimentData;
        }
    }



    public void FinalizeRecording()
    {
        var list_length = target_Object.Count;
        for (int i = 0; i < list_length; i++)
        {
            if (target_Object != null && target_Object.Count > 0 && target_Object[i].tag == "semantic_cues")
            {
                string label = "";
                string attachedObjname = "";
                if (target_Object[i].transform.GetComponentInChildren<ToolTip>() != null)
                {
                    ToolTip tt = target_Object[i].transform.GetComponentInChildren<ToolTip>();
                    label = tt.ToolTipText;
                    attachedObjname = target_Object[i].name;

                    Debug.Log(label + " " + attachedObjname);
                }
                else
                {
                    label = "none";
                    attachedObjname = target_Object[i].name;

                    Debug.Log(label + " " + attachedObjname);
                }


                Vector3 objRot = Quaternion.ToEulerAngles(target_Object[i].transform.rotation);
                string scene_info = label + attachedObjname;
                string scene_data_rot = objRot.ToString();
                string scene_data_pos = target_Object[i].transform.position.ToString();
                string scene_data_scale = target_Object[i].transform.localScale.ToString();

                SaveExperimentData(System.DateTime.UtcNow.ToString("_MMddyyyy_HHmmss") + "," + label + "," + attachedObjname + "," + target_Object[i].transform.localPosition.x + "__" + target_Object[i].transform.localPosition.y + "__" + target_Object[i].transform.localPosition.z + ","
                    + objRot.x + "__" + objRot.y + "__" + objRot.z + ","
                    + target_Object[i].transform.localScale.x + "__" + target_Object[i].transform.localScale.y + "__" + target_Object[i].transform.localScale.z);
            }
        }
    }

    // Update is called once per frame

    private void SaveExperimentData(string data)
    {
#if !UNITY_EDITOR && UNITY_METRO
        Task<Task> task = Task<Task>.Factory.StartNew(
            async () =>
            {
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(m_dataFolderPath);
                StorageFile file = await folder.CreateFileAsync(m_dataFileName, CreationCollisionOption.OpenIfExists);

                List<string> temp = new List<string>();
                temp.Add(data);

                await FileIO.AppendLinesAsync(file, temp);
            }
        );

        task.Wait();
        task.Result.Wait();
#else
        string m_dataFileName_f = m_dataFileName + System.DateTime.UtcNow.ToString("_MMddyyyy_HHmmss") + m_dataExtension;
        Stream stream = new FileStream(Path.Combine(m_dataFolderPath, m_dataFileName_f), FileMode.Append, FileAccess.Write);
        using (StreamWriter streamWriter = new StreamWriter(stream))
        {
            streamWriter.WriteLine(data);
        }

        stream.Dispose();
#endif

        Debug.Log("Saved data: " + data);

    }


    private void Start()
    {
#if !UNITY_EDITOR && UNITY_METRO
        m_dataFolderPath = ApplicationData.Current.RoamingFolder.Path;
#else
        m_dataFolderPath = "C:\\Users\\vine2\\Desktop\\ExperimentData";
#endif
        Debug.Log("Data folder path: " + m_dataFolderPath);

        DontDestroyOnLoad(gameObject);

    }
}