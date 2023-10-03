using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Briefcase_Demo_Functions.DataModel;
using Google.FlatBuffers;
using SmartCamera;

namespace Briefcase_Demo_Functions
{
    public static class Telemetry_Processor
    {
        private const string Signalr_Hub = "SignalRHub";
        private static ILogger _logger = null;

        [FunctionName("Telemetry_Processor")]
        public static async Task Run([EventHubTrigger("%EventHubName%", ConsumerGroup = "%TELEMETRY_CG%", Connection = "EventHubConnectionString")] EventData[] eventData,
            [SignalR(HubName = "SignalRHub")] IAsyncCollector<SignalRMessage> signalRMessage, 
            ILogger log)
        {
            var exceptions = new List<Exception>();
            _logger = log;
            List<string> deivceIdFilterList = System.Environment.GetEnvironmentVariable("DeviceIdFilter").Split(';').ToList();

            INFERENCE_RESULT inferenceResults = null;

            foreach (EventData ed in eventData)
            {
                //try
                {
                    DateTime now = DateTime.Now;

                    // Replace these two lines with your processing logic.
                    //dynamic jsonData = JsonConvert.DeserializeObject(ed.EventBody.ToString());
                    var jObject = JObject.Parse(ed.EventBody.ToString());

                    // Example
                    //{
                    //    "backdoor-EA_Main/placeholder": {
                    //        "DeviceID": "sid-100A50500A2013099964012000000000",
                    //        "ModelID": "0300000001100100",
                    //        "Image": false,
                    //        "Inferences": [
                    //            {
                    //                "T": "20230508222535894",
                    //                "O": "AKBMPgAsEz8AACQ8AKCJPQAAAAAAkK4+AADQOwBg7T0AADw9AAAAAAAAAAAAgII9AMB0PQCAvjwAgL48AIC+PACAvjwAgL48AIC+PACAvjwAgL48AIC+PACAvjwAgL48AIC+PACAvjwAgL48AIC+PACAvjwAgL48AIC+PACAvjwAgL48AIC+PACAvjwAgL48AIC+PACAvjwAgL48AIC+PACAvjwAgL48AIC+PACAvjwAgL48AIC+PACAvjwAgL48AIC+PACAvjwAgL48AIC+PACAvjwAgL48AIC+PACAvjwAgL48AIC+PACAvjwAgL48AIC+PACAvjwAgL48AIC+PAAY2j4AQBY9ACBOPgAAaTwAAAAAAIDJPQAAAAAAaCA/AARGPwAAAAAAAMM+ACDKPgCADT0AAIo7AACKOwAAijsAAIo7AACKOwAAijsAAIo7AACKOwAAijsAAIo7AACKOwAAijsAAIo7AACKOwAAijsAAIo7AACKOwAAijsAAIo7AACKOwAAijsAAIo7AACKOwAAijsAAIo7AACKOwAAijsAAIo7AACKOwAAijsAAIo7AACKOwAAijsAAIo7AACKOwAAijsAAIo7AACKOwAAijsAAIo7AACKOwAAijsAAIo7AACKOwAAijsAAIo7AACKOwAAijsAAIo7AACKOwAAijsAoPA+AARlPwD+fz8A6PA+AFAMPgAYIz8A4Ho+AFR7PwAQlj4AHA4/AKjOPgBgCz8AYMg9AOCEPQDghD0A4IQ9AOCEPQDghD0A4IQ9AOCEPQDghD0A4IQ9AOCEPQDghD0A4IQ9AOCEPQDghD0A4IQ9AOCEPQDghD0A4IQ9AOCEPQDghD0A4IQ9AOCEPQDghD0A4IQ9AOCEPQDghD0A4IQ9AOCEPQDghD0A4IQ9AOCEPQDghD0A4IQ9AOCEPQDghD0A4IQ9AOCEPQDghD0A4IQ9AOCEPQDghD0A4IQ9AOCEPQDghD0A4IQ9AOCEPQDghD0A4IQ9AOCEPQDghD0A4IQ9AGwIPwBomD4ACF8/AJiaPgAYtT4AAGQ/AP5/PwD+fz8AvHc/AJgUPwD+fz8AADI/AIBfPQDAUD0AwFA9AMBQPQDAUD0AwFA9AMBQPQDAUD0AwFA9AMBQPQDAUD0AwFA9AMBQPQDAUD0AwFA9AMBQPQDAUD0AwFA9AMBQPQDAUD0AwFA9AMBQPQDAUD0AwFA9AMBQPQDAUD0AwFA9AMBQPQDAUD0AwFA9AMBQPQDAUD0AwFA9AMBQPQDAUD0AwFA9AMBQPQDAUD0AwFA9AMBQPQDAUD0AwFA9AMBQPQDAUD0AwFA9AMBQPQDAUD0AwFA9AMBQPQDAUD0AwFA9AMBQPQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH8/AAAoPgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQA=="
                    //            }
                    //        ],
                    //        "project_id": "00g4cyn9mh63nqgza697"
                    //    }
                    //}

                    if (jObject["backdoor-EA_Main/placeholder"] != null) {
                        //_logger.LogInformation($"Telemetry Message : {ed.EventBody.ToString()}");

                        String deviceId = jObject["backdoor-EA_Main/placeholder"]["DeviceID"].ToString();

                        if (!deivceIdFilterList.Contains(deviceId))
                        {
                            _logger.LogDebug($"Skipping {deviceId}");
                            continue;
                        }

                        if (jObject["backdoor-EA_Main/placeholder"]["Inferences"] != null)
                        {

                            inferenceResults = new INFERENCE_RESULT();

                            foreach (var inferenceResult in jObject["backdoor-EA_Main/placeholder"]["Inferences"])
                            {
                                DateTime inferenceTime;
                                if (false == DateTime.TryParseExact(inferenceResult["T"].ToString(),
                                                                    "yyyyMMddHHmmssfff",
                                                                    null,
                                                                    System.Globalization.DateTimeStyles.None,
                                                                    out inferenceTime))
                                {
                                    _logger.LogError($"failed to parse inference result time");
                                }

                                if ((now - inferenceTime).Hours > 1)
                                {
                                    //continue;
                                }

                                List<INFERENCE_RESULT_ITEM> deserializedBuffer = Deserialize(inferenceResult["O"].ToString());

                                inferenceResults.DeviceId = deviceId;
                                inferenceResults.ModelId = jObject["backdoor-EA_Main/placeholder"]["ModelID"].ToString();
                                inferenceResults.Image = jObject["backdoor-EA_Main/placeholder"]["Image"].ToObject<bool>();

                                inferenceResults.T = inferenceResult["T"].ToString();

                                inferenceResults.inferenceResults = deserializedBuffer;
     
                                _logger.LogInformation($"Inference Result : {inferenceResults.DeviceId} {inferenceResults.ModelId} {inferenceTime}");

                                foreach (var item in inferenceResults.inferenceResults)
                                {
                                    _logger.LogInformation($"    Class {item.Class} Conf : {item.Confidence.ToString("0.00")} [{item.x},{item.y}] - [{item.X},{item.Y}]");
                                }
                            }
                        }
                    }

                    //if (inferenceResults != null && inferenceResults.inferenceResults.Count > 0)
                        if (inferenceResults != null)
                        {
                            // send to SignalR Hub
                            var data = JsonConvert.SerializeObject(inferenceResults);

                        _logger.LogInformation($"SignalR Message (EventHub): {data.Length}");
                        await signalRMessage.AddAsync(new SignalRMessage
                        {
                            Target = "deviceTelemetry",
                            Arguments = new[] { data }
                        });
                    }
                    await Task.Yield();
                }
                //catch (Exception e)
                //{
                //    // We need to keep processing the rest of the batch - capture this exception and continue.
                //    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                //    exceptions.Add(e);
                //}
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }

        private static List<INFERENCE_RESULT_ITEM> Deserialize(string inferenceData)
        {
            byte[] buf = Convert.FromBase64String(inferenceData);
            ObjectDetectionTop objectDetectionTop = ObjectDetectionTop.GetRootAsObjectDetectionTop(new ByteBuffer(buf));
            ObjectDetectionData objectData = objectDetectionTop.Perception ?? new ObjectDetectionData();
            int resNum = objectData.ObjectDetectionListLength;
            _logger.LogInformation($"NumOfDetections: {resNum.ToString()}");

            List<INFERENCE_RESULT_ITEM> inferenceResults = new List<INFERENCE_RESULT_ITEM>();
            for (int i = 0; i < resNum; i++)
            {
                GeneralObject objectList = objectData.ObjectDetectionList(i) ?? new GeneralObject();
                BoundingBox unionType = objectList.BoundingBoxType;
                if (unionType == BoundingBox.BoundingBox2d)
                {
                    var bbox2d = objectList.BoundingBox<BoundingBox2d>().Value;
                    INFERENCE_RESULT_ITEM data = new INFERENCE_RESULT_ITEM();
                    data.Class = objectList.ClassId;
                    data.Confidence = (float)Math.Round(objectList.Score, 6, MidpointRounding.AwayFromZero);
                    data.x = bbox2d.Left;
                    data.y = bbox2d.Top;
                    data.X = bbox2d.Right;
                    data.Y = bbox2d.Bottom;

                    inferenceResults.Add(data);
                }
            }
            return inferenceResults;
        }

    }
}
