using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
    public class CSV_log : MonoBehaviour

    {
        #region Constants to modify
        [SerializeField] List<GameObject> target_Object;
        private const string DataSuffix = "data";
        private const string CSVHeader = "Timestamp, Tooltip attached, Objectt Name," + "position(x,y,z), rotation (x,y,z), scale(x,y,z)";
        private const string SessionFolderRoot = "Exp_record";
        private string m_dataFolderPath;
        private string m_dataFileName = "Memory_Palace_Data ";
        private string m_dataExtension = ".csv";
        #endregion

        #region private members
        private string m_recordingId;
        private string m_sessionId;
        private StringBuilder m_csvData;
        #endregion
        #region public members
        public string RecordingInstance => m_recordingId;
        #endregion

        // Use this for initialization
        async void Start()
        {
            await MakeNewSession();
        }

        async Task MakeNewSession()
        {
            m_sessionId = "Recorded Scene " + DateTime.Now.ToString( "yyyy_MM_dd_HH_mm_ss");
            string rootPath = "";

#if ENABLE_WINMD_SUPPORT
    UnityEngine.WSA.Application.InvokeOnUIThread(async () =>  
    {  
        var folderPicker = new Windows.Storage.Pickers.FolderPicker();  
        folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;  
        folderPicker.FileTypeFilter.Add("*");  
  
        Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();  
        if (folder != null)  
        {  
            // Application now has read/write access to all contents in the picked folder  
            // (including other sub-folder contents)  
            Windows.Storage.AccessCache.StorageApplicationPermissions.  
            FutureAccessList.AddOrReplace("PickedFolderToken", folder);  
        }  
    }, false);  
#else
            rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), SessionFolderRoot);
            if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);
#endif
            m_dataFolderPath = Path.Combine(rootPath, m_sessionId);
            Directory.CreateDirectory(m_dataFolderPath);
            Debug.Log(rootPath);
            Debug.Log("CSVLogger logging data to " + m_dataFolderPath);
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


                    Vector3 objRot = Quaternion.ToEulerAngles(target_Object[i].transform.localRotation);
                    string scene_info = label + attachedObjname;
                    string scene_data_rot = objRot.ToString();
                    string scene_data_pos = target_Object[i].transform.localPosition.ToString();
                    string scene_data_scale = target_Object[i].transform.localScale.ToString();

                    Write_Data(System.DateTime.UtcNow.ToString("_MMddyyyy_HHmmss") + "," + label + "," + attachedObjname + "," 
                        + target_Object[i].transform.localPosition.x + "__" + target_Object[i].transform.localPosition.y + "__" + target_Object[i].transform.localPosition.z + ","
                        + objRot.x + "__" + objRot.y + "__" + objRot.z + ","
                        + target_Object[i].transform.localScale.x + "__" + target_Object[i].transform.localScale.y + "__" + target_Object[i].transform.localScale.z);
                }
            }
        }

        private void Write_Data(string data)
        {
            string m_dataFileName_f = m_dataFileName + System.DateTime.UtcNow.ToString("_MMddyyyy_HHmmss") + m_dataExtension;
            Stream stream = new FileStream(Path.Combine(m_dataFolderPath, m_dataFileName_f), FileMode.Append, FileAccess.Write);
            using (StreamWriter streamWriter = new StreamWriter(stream))
            {
                streamWriter.WriteLine(data);
            }
            stream.Dispose();

            Debug.Log("Saved data: " + data);

        }

        //public void FlushData()
        //{
        //    string m_filePath = m_dataFolderPath + m_dataFileName;
        //    using (var csvWriter = new StreamWriter(m_filePath, true))
        //    {
        //        csvWriter.Write(m_csvData.ToString());
        //    }
        //    m_csvData.Clear();
        //}


        async void SaveFile()
        {
#if ENABLE_WINMD_SUPPORT
    if (Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.ContainsItem("PickedFolderToken"))   
    {  
        Windows.Storage.StorageFolder storageFolder = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFolderAsync("PickedFolderToken");  
        Windows.Storage.StorageFile sampleFile = await storageFolder.CreateFileAsync("sample.txt", Windows.Storage.CreationCollisionOption.OpenIfExists);  
        await Windows.Storage.FileIO.WriteTextAsync(sampleFile, "Swift as a shadow");  
    }  
#endif
        }

        /// <summary>
        /// Returns a row populated with common start data like
        /// recording id, session id, timestamp
        /// </summary>
        /// <returns></returns>

    }
}
