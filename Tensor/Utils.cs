using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Reflection;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.GZip;
namespace Tensor
{
    public static class Utils
    {
      

        public static string DownloadDefaultTexts(string dir = null)
        {
            if(string.IsNullOrWhiteSpace(dir))
                dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+ "/Downloaded";

            AppSettingsReader reader = new AppSettingsReader();
            string defaultTextsUrl = reader.GetValue("DefaultTextsUrl", typeof(string)).ToString();
            var textsFile = Path.Combine(dir, "mscoco_label_map.pbtxt");
            var wc = new WebClient();
           
            wc.DownloadFile(defaultTextsUrl, textsFile);
            return textsFile;

        }

      

        public static string DownloadDefaultModel(string dir = null)
        {
            if (string.IsNullOrWhiteSpace(dir))
                dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Downloaded";
            AppSettingsReader reader = new AppSettingsReader();
            string defaultModelUrl = reader.GetValue("DefaultModelUrl", typeof(string)).ToString();

            var modelFile = Path.Combine(dir, "faster_rcnn_inception_resnet_v2_atrous_coco_11_06_2017/frozen_inference_graph.pb");
             string zipfile = Path.Combine(dir, "faster_rcnn_inception_resnet_v2_atrous_coco_11_06_2017.tar.gz");
           
            if (File.Exists(modelFile))
                return modelFile;

            if (!File.Exists(zipfile))
            {
               
                var wc = new WebClient();
                wc.DownloadFile(new Uri(defaultModelUrl), zipfile);

                ExtractToDirectory(zipfile, dir);
                File.Delete(zipfile);
            }

            return dir;

        }

       
        private static void ExtractToDirectory(string file, string targetDir)
        {
            Console.WriteLine("Extracting");

            using (Stream inStream = File.OpenRead(file))
            using (Stream gzipStream = new GZipInputStream(inStream))
            {
                TarArchive tarArchive = TarArchive.CreateInputTarArchive(gzipStream);
                tarArchive.ExtractContents(targetDir);
            }
        }

    }
}