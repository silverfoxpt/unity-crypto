//TAKE FROM MNIST DATABASE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MNIST.IO;
using System.Linq;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class UnpackTrainingData : MonoBehaviour
{
    
    List<TestCase> tests = new List<TestCase>();

    [SerializeField] private bool forceDestroyPreviousSave = false;

    private void Start()
    {
        /*if (forceDestroyPreviousSave)
        {
            var data = FileReaderMNIST.LoadImagesAndLables(
                "./Assets/Train NN/train-labels-idx1-ubyte.gz",
                "./Assets/Train NN/train-images-idx3-ubyte.gz");

            tests = data.ToList();
            TrainData dat = new TrainData(); dat.tests = tests;

            SaveData(dat, "NNTrainTest");
            Debug.Log(tests.Count.ToString() + " ; forced");
        }
        else
        {
            var openedTests = LoadData("NNTrainTest");
            TrainData dat = (TrainData) openedTests;
            tests = dat.tests;

            Debug.Log(tests.Count);
        }*/
    }

    public void SaveData(object objectToSave, string fileName)
    {
        string FullFilePath = Application.persistentDataPath + "/" + fileName + ".bin";

        BinaryFormatter Formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(FullFilePath, FileMode.Create);

        Formatter.Serialize(fileStream, objectToSave);
        fileStream.Close();
    }

    public object LoadData(string fileName)
    {
        string FullFilePath = Application.persistentDataPath + "/" + fileName + ".bin";

        if (File.Exists(FullFilePath))
        {
            BinaryFormatter Formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(FullFilePath, FileMode.Open);
            object obj = Formatter.Deserialize(fileStream);
            fileStream.Close();

            return obj;
        }
        else
        {
            return null;
        }
    }
}
