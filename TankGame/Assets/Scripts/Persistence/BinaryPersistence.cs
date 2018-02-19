using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

namespace TankGame.Persistence
{
    [Serializable]
    public class BinaryPersistence : IPersistence
    {
        public string Extension { get { return ".bin"; } }

        public string FilePath { get; private set;}

        /// <summary>
        /// Initializes the BinaryPersistence object.
        /// </summary>
        /// <param name="path">The path of the save file
        /// without the file extension</param>
        public BinaryPersistence(string path)
        {
            FilePath = path + Extension;
        }

        public void Save<T>(T data)
        {
            using (FileStream stream = File.OpenWrite(FilePath))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(stream, data);

                // Calling the stream.Close() is not necessary when using
                // the 'using' statement. When the execution leaves the
                // stream's scope, the Dispose method is called automatically
                // by stream.Close().
                stream.Close();
            }
        }

        public T Load<T>()
        {
            T data = default(T);

            if (File.Exists(FilePath))
            {
                // If we are not using the 'using' statement we have to make
                // sure that the stream is correctly closed in case of an
                // Exception being thrown. The finally block makes sure that
                // the stream is closed correctly in every case.
                FileStream stream = File.OpenRead(FilePath);
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    data = (T) bf.Deserialize(stream);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                finally
                {
                    stream.Close();
                } 
            }

            return data;
        }
    }
}
