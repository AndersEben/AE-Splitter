using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AE_Splitter
{
    class Cutter
    {
        public static async Task SplitFile(string inputFile, int chunkSize, string path)
        {

            using (Stream input = File.OpenRead(inputFile))
            {
                int index = 0;

                double number = Convert.ToDouble(input.Length) / Convert.ToDouble(chunkSize);
                int number3 = (int)(Math.Ceiling(number));

                while (input.Position < input.Length)
                {
                    using (Stream output = File.Create(path + "\\" + inputFile.Replace(path,"") + "." + index + ".cut." + number3))
                    {

                        int BUFFER_SIZE = chunkSize;

                        byte[] buffer = new byte[BUFFER_SIZE];

                        int remaining = chunkSize, bytesRead;
                        int ttt;

                        while (remaining > 0 && (bytesRead = input.Read(buffer, 0,ttt=Math.Min(remaining, BUFFER_SIZE))) > 0)
                        {
                            

                            if(bytesRead < BUFFER_SIZE)
                            {
                                byte[] newArray = new byte[bytesRead];
                                newArray = copyArray(buffer, bytesRead);
                                
                                buffer = new byte[bytesRead];
                                newArray.CopyTo(buffer, 0);

                                remaining = bytesRead;
                            }

                            await output.WriteAsync(buffer, 0, buffer.Length);

                            remaining -= bytesRead;
                        }

                    }
                    index++;
                }
            }
        }

        public byte[] addByteToArray(byte[] bArray, byte newByte)
        {
            byte[] newArray = new byte[bArray.Length + 1];
            bArray.CopyTo(newArray, 1);
            newArray[0] = newByte;
            return newArray;
        }

        public static byte[] copyArray(byte[] bArray, int a_lenght)
        {
            byte[] newArray = new byte[a_lenght];

            for (int i = 0; i < a_lenght; i++)
            {
                newArray[i] = bArray[i];
            }

            return newArray;
        }
    }
}
