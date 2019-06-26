using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TensorFlow;
using Mono.Options;
namespace Tensor
{
    public class Detector
    {
        private static IEnumerable<CatalogItem> _catalog;
        private static string _currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string _input_relative = "test_images/input.jpg";
        private static string _output_relative = "test_images/output.jpg";
        private static string _input = Path.Combine(_currentDir, _input_relative);
        private static string _output = Path.Combine(_currentDir, _output_relative);
        private static string _catalogPath;
        private static string _modelPath;

        private static double MIN_SCORE_FOR_OBJECT_HIGHLIGHTING = 0.5;

        static OptionSet options = new OptionSet()
        {
            {"input_image=", "Specifies the path to an image ", v => _input = v},
            {"output_image=", "Specifies the path to the output image with detected objects", v => _output = v},
            {"catalog=", "Specifies the path to the .pbtxt objects catalog", v => _catalogPath = v},
            {"model=", "Specifies the path to the trained model", v => _modelPath = v},
            {"h|help", v => Help()}
        };


        private static void Help()
        {
            options.WriteOptionDescriptions(Console.Out);
        }


        public void Detect(ref Image i)
        {

     

            if (_catalogPath == null)
            {
                _catalogPath = Utils.DownloadDefaultTexts();
            }

            if (_modelPath == null)
            {
                _modelPath = Utils.DownloadDefaultModel();
            }
            _catalog = CatalogUtil.ReadCatalogItems(_catalogPath);
            
            string modelFile = _modelPath;

            using (var graph = new TFGraph())
            {
                var model = File.ReadAllBytes(modelFile);
                graph.Import(new TFBuffer(model));

                using (var session = new TFSession(graph))
                {
                    var bytes = ImageUtil.ImageToBytes(i);
                    var tensor = ImageUtil.CreateTensorFromPic(bytes, TFDataType.UInt8);
                 
                        var runner = session.GetRunner();


                        runner
                            .AddInput(graph["image_tensor"][0], tensor)
                            .Fetch(
                                graph["detection_boxes"][0],
                                graph["detection_scores"][0],
                                graph["detection_classes"][0],
                                graph["num_detections"][0]);
                        var output = runner.Run();

                        var boxes = (float[,,])output[0].GetValue(jagged: false);
                        var scores = (float[,])output[1].GetValue(jagged: false);
                        var classes = (float[,])output[2].GetValue(jagged: false);
                        var num = (float[])output[3].GetValue(jagged: false);

                        
                        DrawBoxesInImage(boxes, scores, classes, ref i, MIN_SCORE_FOR_OBJECT_HIGHLIGHTING);
                }
            }
        }


        private static void DrawBoxes(float[,,] boxes, float[,] scores, float[,] classes, string inputFile, string outputFile, double minScore)
        {
            var x = boxes.GetLength(0);
            var y = boxes.GetLength(1);
            var z = boxes.GetLength(2);

            float ymin = 0, xmin = 0, ymax = 0, xmax = 0;

            using (var editor = new ImageEditor(inputFile, outputFile))
            {
                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        if (scores[i, j] < minScore) continue;

                        for (int k = 0; k < z; k++)
                        {
                            var box = boxes[i, j, k];
                            switch (k)
                            {
                                case 0:
                                    ymin = box;
                                    break;
                                case 1:
                                    xmin = box;
                                    break;
                                case 2:
                                    ymax = box;
                                    break;
                                case 3:
                                    xmax = box;
                                    break;
                            }

                        }

                        int value = Convert.ToInt32(classes[i, j]);
                        CatalogItem catalogItem = _catalog.FirstOrDefault(item => item.Id == value);
                        editor.AddBox(xmin, xmax, ymin, ymax, $"{catalogItem.DisplayName} : {(scores[i, j] * 100).ToString("0")}%");
                    }
                }
            }
        }
        private static void DrawBoxesInImage(float[,,] boxes, float[,] scores, float[,] classes,ref Image image, double minScore)
        {
            var x = boxes.GetLength(0);
            var y = boxes.GetLength(1);
            var z = boxes.GetLength(2);

            float ymin = 0, xmin = 0, ymax = 0, xmax = 0;

            using (var editor = new ImageEditor(image))
            {
                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        if (scores[i, j] < minScore) continue;

                        for (int k = 0; k < z; k++)
                        {
                            var box = boxes[i, j, k];
                            switch (k)
                            {
                                case 0:
                                    ymin = box;
                                    break;
                                case 1:
                                    xmin = box;
                                    break;
                                case 2:
                                    ymax = box;
                                    break;
                                case 3:
                                    xmax = box;
                                    break;
                            }

                        }

                        int value = Convert.ToInt32(classes[i, j]);
                        CatalogItem catalogItem = _catalog.FirstOrDefault(item => item.Id == value);
                        editor.AddBox(xmin, xmax, ymin, ymax, $"{catalogItem.DisplayName} : {(scores[i, j] * 100).ToString("0")}%");
                    }
                }
            }
        }
    }



}



