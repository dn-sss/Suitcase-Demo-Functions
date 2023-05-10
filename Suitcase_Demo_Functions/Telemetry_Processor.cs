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
using static Suitcase_Demo_Functions.DataModel;

namespace Suitcase_Demo_Functions
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

            // Custom Vision's Max Detection
            int MAX_DETECTION = 64;
            INFERENCE_RESULT inferenceResults = null;

            foreach (EventData ed in eventData)
            {
                try
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
                            inferenceResults.inferenceResults = new List<INFERENCE_RESULT_ITEM>();

                            foreach (var inferenceResult in jObject["backdoor-EA_Main/placeholder"]["Inferences"])
                            {
                                // parse Custom Vision output.
                                List<float> resultArray = new List<float>();
                                String bse64EncodedString = inferenceResult["O"].ToString();

                                // Decode Base64 encoded string
                                var base64EncodedBytes = Convert.FromBase64String(bse64EncodedString);

                                // 
                                //   0 -  63 : Normalized Start X of Bounding Box Output 
                                //  64 - 127 : Normalized Start Y of Bounding Box Output
                                // 128 - 191 : Normalized End X of Bounding Box Output
                                // 192 - 255 : Normalized End Y of Bounding Box Output
                                // 256 - 319 : Classification (Class ID)
                                // 320 - 383 : Accuracy

                                // Convert to array with Float
                                for (int i = 0; i <= MAX_DETECTION * 6; i++)
                                {
                                    float myFloat = System.BitConverter.ToSingle(base64EncodedBytes, 4 * i);
                                    resultArray.Add(myFloat);
                                }

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
                                    continue;
                                }

                                inferenceResults.DeviceId = deviceId;
                                inferenceResults.ModelId = jObject["backdoor-EA_Main/placeholder"]["ModelID"].ToString();
                                inferenceResults.Image = jObject["backdoor-EA_Main/placeholder"]["Image"].ToObject<bool>();

                                inferenceResults.T = inferenceResult["T"].ToString();
                                // Re-format to INFERENCE_ITEM
                                for (int i = 0; i < MAX_DETECTION; i++)
                                {
                                    if (resultArray[i + MAX_DETECTION * 5] == 0.0)
                                    {
                                        continue;
                                    }

                                    INFERENCE_RESULT_ITEM inferenceResultItem = new INFERENCE_RESULT_ITEM
                                    {
                                        Class = Convert.ToInt32(resultArray[i + MAX_DETECTION * 4]),
                                        Confidence = resultArray[i + MAX_DETECTION * 5],
                                        y = resultArray[i + MAX_DETECTION * 0],
                                        x = resultArray[i + MAX_DETECTION * 1],
                                        Y = resultArray[i + MAX_DETECTION * 2],
                                        X = resultArray[i + MAX_DETECTION * 3],
                                    };

                                    inferenceResults.inferenceResults.Add(inferenceResultItem);
                                }

                                _logger.LogInformation($"Inference Result : {inferenceResults.DeviceId} {inferenceResults.ModelId} {inferenceTime}");

                                foreach (var item in inferenceResults.inferenceResults)
                                {
                                    _logger.LogInformation($"    Class {item.Class} Conf : {item.Confidence.ToString("0.00")} [{item.x},{item.y}] - [{item.X},{item.Y}]");
                                }
                            }
                        }
                    }

                    if (inferenceResults != null && inferenceResults.inferenceResults.Count > 0)
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
                catch (Exception e)
                {
                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                    exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }


    }
}
