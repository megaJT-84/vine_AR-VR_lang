using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.UI;
#if WINDOWS_UWP
using Windows.Storage;
#endif

namespace holoutils
{
    /// <summary>
    /// Component that Logs data to a CSV.
    /// Assumes header is fixed.
    /// Copy and paste this logger to create your own CSV logger.
    /// CSV Logger breaks data up into settions (starts when application starts) which are folders
    /// and instances which are files
    /// A session starts when the application starts, it ends when the session ends.
    /// 
    /// In Editor, writes to MyDocuments/SessionFolderRoot folder
    /// On Device, saves data in the Pictures/SessionFolderRoot
    /// 
    /// How to use:
    /// Find the csvlogger
    /// if it has not started a CSV, create one.
    /// every frame, log stuff
    /// Flush data regularly
    /// 
    /// **Important: Requires the PicturesLibrary capability!**
    /// </summary>
    public class Holo_log : MonoBehaviour

    {
        [SerializeField] List<GameObject> target_Object;
        #region Constants to modify
        private const string File_suffix = "Memory_Palace_Data";
        private const string data_extension = ".csv";
        private const string CSVHeader = "Timestamp, tooltip, object_name," +
                                        "position, rotation, scale";
        private const string SessionFolderRoot = "Exp_record";
        #endregion

        #region private members
        private string m_sessionPath;
        private string m_filePath;
        private string m_recording_time;
        private string m_sessionId;

        private StringBuilder m_csvData;
        #endregion
        #region public members
        public string RecordingInstance => m_recording_time;
        //UnityEngine.TouchScreenKeyboard keyboard;
        //public static string subject_name_holo = "";
        private string user_in;

        private string Row_data = "";

#if WINDOWS_UWP
                StorageFile DataWritetoFile = null;
#endif


        #endregion

        // Use this for initialization
        async void Start()
        {
            await MakeNewSession();
        }

        async Task MakeNewSession()
        {
            m_sessionId = "Recorded Scene " + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            string rootPath = "";
#if WINDOWS_UWP
                    StorageFolder sessionParentFolder = await KnownFolders.PicturesLibrary.CreateFolderAsync(SessionFolderRoot,CreationCollisionOption.OpenIfExists);
                    StorageFile DataWritetoFile = await sessionParentFolder.CreateFileAsync("Memory_Palace_Data.csv");
                    await FileIO.WriteTextAsync(DataWritetoFile, "it's working here");
                    rootPath = sessionParentFolder.Path;
#else
            rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), SessionFolderRoot);
            if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);
#endif
            m_sessionPath = Path.Combine(rootPath, m_sessionId);
            Directory.CreateDirectory(m_sessionPath);
            Debug.Log("CSVLogger logging data to " + m_sessionPath);
        }

        async public void FinalizeRecording()
        {
            string rootPath = "";
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

                    Vector3 objRot = Quaternion.ToEulerAngles(target_Object[i].transform.localRotation);
                    string scene_info = label + attachedObjname;
                    string scene_data_rot = objRot.ToString();
                    string scene_data_pos = target_Object[i].transform.localPosition.ToString();
                    string scene_data_scale = target_Object[i].transform.localScale.ToString();

                    Row_data = (System.DateTime.UtcNow.ToString("MM_dd_HH:fmm_ss") + "," + label + "," + attachedObjname + ","
                        + target_Object[i].transform.localPosition.x + "__" + target_Object[i].transform.localPosition.y + "__" + target_Object[i].transform.localPosition.z + ","
                        + objRot.x + "__" + objRot.y + "__" + objRot.z + ","
                        + target_Object[i].transform.localScale.x + "__" + target_Object[i].transform.localScale.y + "__" + target_Object[i].transform.localScale.z) + "\n";
                    Add_DatatoRow(Row_data);
                }
            }

            //await EndTheCSV();
#if WINDOWS_UWP
                    StorageFolder sessionParentFolder = await KnownFolders.PicturesLibrary.GetFolderAsync(SessionFolderRoot);
                    StorageFile DataWritetoFile = await sessionParentFolder.CreateFileAsync("Memory_Palace_Data_4.csv");
                    await FileIO.WriteTextAsync(DataWritetoFile, m_csvData.ToString());                                                                                                                                                                                                     
#endif
        }

        public void StartNewCSV()
        {
            //var user_in = GetComponent<Text>();
            //string subject_name_unity = user_in.ToString();
            //keyboard = TouchScreenKeyboard.Open("Enter your name");
            //subject_name_holo = keyboard.text;

            m_recording_time = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            var filename = m_recording_time + "__" + File_suffix + ".csv";
            m_filePath = Path.Combine(m_sessionPath, filename);
            if (m_csvData != null)
            {
                EndCSV();
            }
            m_csvData = new StringBuilder();
            m_csvData.AppendLine(CSVHeader);
        }


        public void EndCSV()
        {
            if (m_csvData == null)
            {
                return;
            }
            using (var csvWriter = new StreamWriter(m_filePath, true))
            {
                csvWriter.Write(m_csvData.ToString());
            }
            m_recording_time = null;
            m_csvData = null;
        }

        public async Task EndTheCSV()
        {
#if WINDOWS_UWP
                    StorageFolder sessionParentFolder = await KnownFolders.PicturesLibrary.GetFolderAsync(SessionFolderRoot);
                    StorageFile DataWritetoFile = await sessionParentFolder.CreateFileAsync("Memory_Palace_Data_4.csv");
                    await FileIO.WriteTextAsync(DataWritetoFile, m_csvData.ToString());                                                                                                                                                                                                     
#endif
        }

        async void OnDestroy()
        {
            await EndTheCSV();
        }

        public void Add_Row_CSV(List<String> rowData)
        {
            Add_DatatoRow(string.Join(",", rowData.ToArray()));
        }

        public void Add_DatatoRow(string data_stream)
        {
            m_csvData.AppendLine(data_stream);
        }

        /// <summary>
        /// Writes all current data to current file
        /// </summary>

        private void Flushdata()
        {
            using (var csvWriter = new StreamWriter(m_filePath, true))
            {
                csvWriter.Write(m_csvData.ToString());
            }
            m_csvData.Clear();
        }
        /// <summary>
        /// Returns a row populated with common start data like
        /// recording id, session id, timestamp
        /// </summary>
        /// <returns></returns>
        public List<String> RowWithStartData()
        {
            List<String> rowData = new List<String>();
            rowData.Add(Time.timeSinceLevelLoad.ToString("##.000"));
            rowData.Add(m_recording_time);
            rowData.Add(m_recording_time);
            return rowData;
        }

    }

}