using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace cajero_server
{
    class Program
    {


        public static void Main()
        {
            ReconstruccionArchivos();
            var path = Path.Combine(Directory.GetCurrentDirectory());
            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = path,
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                Filter = "*.txt"
            };
            
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            
            watcher.EnableRaisingEvents = true;
            Console.ReadKey();
        }
        

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            ReconstruccionArchivos();
            Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
        }
        
        public static void ReconstruccionArchivos()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "prueba.txt");
            var archivo = File.ReadAllLines(path);
            var cuentas = new List<string>();
            archivo.ToList().ForEach(line => {
                var cuenta = line.Split(',')[7];
                if (!cuentas.Contains(cuenta))
                    cuentas.Add(cuenta);
            });

            StringBuilder trans = new StringBuilder();

            cuentas.ForEach(cuenta =>
            {
                var pathCuenta = Path.Combine(Directory.GetCurrentDirectory(), cuenta + ".txt");

                archivo.ToList().ForEach(line =>
                {
                    var transacciones = line.Split(',');
                    if (transacciones[7] == cuenta)
                    {
                        trans.Append(transacciones[6]);
                        //Byte[] info = new UTF8Encoding(true).GetBytes(transacciones[6]+"\r\n");
                        //fs.Write(info, 0, info.Length);
                    }
                });

                using (FileStream fs = File.Create(pathCuenta))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes(trans.ToString());
                    fs.Write(info, 0, info.Length);
                }
            });
            
            //var cuentas = Path.Combine(Directory.GetCurrentDirectory(), archivo[0] + ".txt");
            //File.WriteAllText(cuentas, "contenido");
        }

    }
}
