#if WINDOWS_UWP

//Get local folder
StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

//Create file
StorageFile textFileForWrite = await storageFolder.CreateFileAsync("LocalText.txt");

//Write to file
await FileIO.WriteTextAsync(textFileForWrite, "Text written to file from code");

.....

//Get file
StorageFile textFileForRead = await storageFolder.GetFileAsync("LocalText.txt");

//Read file
string plainText = "";
plaintext = await FileIO.ReadTextAsync(textFileForRead);

#endif