using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
namespace QAFrameServerValidator
{
    public class FPS_Analysis
    {
        public static List<String> syncAvgAnalysis ;
        public string profileName="";
        public List<double> framesTimeStampList;
        public List<Utils.REAL_SENSE_SR300_DEPTH_MMETADATA> framesTimeStampList_HW;
        private List<double> deltaList = new List<double>();
        public FPS_Analysis()
        {
            syncAvgAnalysis = new List<string>();
            framesTimeStampList = new List<double>();
            framesTimeStampList_HW = new List<Utils.REAL_SENSE_SR300_DEPTH_MMETADATA>(); 
        }
        public double GetExpectedColorFpsInExposureControl(int fps, double exposureValue)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("EXPOSREVALUE = " + exposureValue);
            Console.ForegroundColor = ConsoleColor.White;
            double expectedValue = 1 / Math.Pow(2.0, exposureValue);
            return fps < expectedValue ? fps : expectedValue;
        }

        public double GetExpectedDepthFpsResultInMotionVsRangeTradeoff(string IntensityFPS, int fps, float accuracyValue, float MotionVsRangeTradeoffValue)
        {           
            int realAccuracyValue = GetRealAccuracyValue(accuracyValue);
            double Min_MvR_Exp = 1.0;
            double expectedDepthExposureTime = -1;
            expectedDepthExposureTime = Min_MvR_Exp + MotionVsRangeTradeoffValue * 0.09;
            expectedDepthExposureTime = expectedDepthExposureTime * realAccuracyValue;

            double idleTime = 0;
            double userExposureTime = 1000.0 / fps;
            if (userExposureTime > expectedDepthExposureTime)
            {
                idleTime = userExposureTime - expectedDepthExposureTime;
                if (idleTime >= 1 && idleTime < 1.8)
                {
                    expectedDepthExposureTime = userExposureTime + 1.8 - idleTime;
                }
            }

            if (fps >= 30)
            {
                if (fps < (1000.0 / expectedDepthExposureTime))
                {
                    expectedDepthExposureTime = 1000.0 / fps;
                }
            }
            else 
            {
                if (fps < (1000 / (expectedDepthExposureTime + 18)))
                    expectedDepthExposureTime = 1000.0 / fps;
                else
                    expectedDepthExposureTime = expectedDepthExposureTime + 18;
            }
            double expectedFps = 1000.0 / expectedDepthExposureTime;
            return expectedFps;
        }

        public int GetRealAccuracyValue(float value)
        {
            switch ((int)value)
            {
                case 1: return 11;
                case 2: return 10;
                case 3: return 10;
                default: return -1;
            }
        }


        public bool IsFrameDeltaBetweenFramesOKPropertiesWhileStreamingSR300(bool ifColorProfile, int fps, int exposure, float MvsR, int accuracy)
        {
            Logger.AppendDebug("IsFrameDeltaOK validation:\n");
            if (framesTimeStampList.Count < 2)
            {
                Logger.AppendInfo("The size of TimeStampList is less than 2");
                return true;
            }

            double first, second, delta;
            for (int i = 0; i < framesTimeStampList.Count - 1; i++)
            {
                first = framesTimeStampList[i];
                second = framesTimeStampList[i + 1];
                delta = second - first;
                deltaList.Add(delta);
            }

            double acceptedThreshold = 1.10;
            double expectedFps;

            if (ifColorProfile)
                expectedFps = GetExpectedColorFpsInExposureControl(fps, exposure);
            else
                expectedFps = GetExpectedDepthFpsResultInMotionVsRangeTradeoff("", fps, (float)accuracy, (float)MvsR);


            Logger.AppendInfo("Excpected FPS: " + expectedFps);
            double expectedResult = (1.0 / expectedFps) * 1000.0;
            Logger.AppendInfo("Excpected Result: " + expectedResult);
            int numberOfTimesThatFrameDeltaIsNotWithinBounds = 0;
            //check delta 
            double AverageDelta = 0;
            double maxDelta = deltaList[1];
            double minDelta = deltaList[1];
            for (int i = 1; i < deltaList.Count; i++)
            {
                AverageDelta += deltaList[i];

                if (deltaList[i] > maxDelta)
                    maxDelta = deltaList[i];

                if (deltaList[i] < minDelta)
                    minDelta = deltaList[i];

                if (deltaList[i] < expectedResult / acceptedThreshold || deltaList[i] > expectedResult * acceptedThreshold)  //SOME DELTA IS OUT OF RANGE
                {
                    numberOfTimesThatFrameDeltaIsNotWithinBounds++;
                }
            }
            int thresholdNumberOfFramesThatAreNotInBounds = (int)((double)framesTimeStampList.Count * .05);
            return numberOfTimesThatFrameDeltaIsNotWithinBounds < thresholdNumberOfFramesThatAreNotInBounds;
        }

        public bool IsFrameDeltaBetweenFramesOKPropertiesWhileStreamingDS5(int fps, double exposure, double[] framesArray)
        {
            double expectedFps = (1 / (double)GetExpectedFpsDs5(fps, exposure)) * 1000;
            double threshold = fps < 30 ? 0.1 : 0.05;
            double upper = expectedFps + expectedFps * threshold;
            double lower = expectedFps - expectedFps * threshold;

            Logger.AppendInfo($"IsFrameDeltaBetweenFramesOK: FPS - {fps}, Exposure - {exposure}, Actual FPS - {expectedFps}");
            int numberOfTimesThatFrameDeltaIsNotWithinBounds = 0;
            for (int i = 2; i < framesArray.Count(); i++)
            {
                if ((framesArray[i] - framesArray[i - 1]) < lower || (framesArray[i] - framesArray[i - 1]) > upper)
                {
                    numberOfTimesThatFrameDeltaIsNotWithinBounds++;
                }
            }
            int thresholdNumberOfFramesThatAreNotInBounds = (int)((double)framesArray.Count() * .05);
            return numberOfTimesThatFrameDeltaIsNotWithinBounds < thresholdNumberOfFramesThatAreNotInBounds;
        }

        public bool IsFrameDeltaOK(bool ifColorProfile, int fps, int exposure, float MvsR, int accuracy)
        {          
            Logger.AppendInfo("IsFrameDeltaOK validation:\n");
            if (framesTimeStampList.Count < 2)
            {
                Logger.AppendInfo("The size of TimeStampList is less than 2");
                return false;
            }

            deltaList = new List<double>();
            double first, second, delta;
            for (int i = 0; i < framesTimeStampList.Count - 1; i++)
            {
                first = framesTimeStampList[i];
                second = framesTimeStampList[i + 1];
                delta = second - first;
                deltaList.Add(delta);
            }

            double acceptedThreshold = 1.10;
            bool isTestPassed = true;
            double expectedFps;

            if (ifColorProfile)
                expectedFps = GetExpectedColorFpsInExposureControl(fps, exposure);
            else
                expectedFps = GetExpectedDepthFpsResultInMotionVsRangeTradeoff("", fps, (float)accuracy, (float)MvsR);


            Logger.AppendInfo("Excpected FPS: " + expectedFps);
            double expectedResult = (1.0 / expectedFps) * 1000.0;
            Logger.AppendInfo("Excpected Result: " + expectedResult);

            //check delta 
            bool isDeltaArrayOk = true;
            double AverageDelta = 0;
            double maxDelta = deltaList[1];
            double minDelta = deltaList[1];
            for (int i = 1; i < deltaList.Count; i++)
            {
                Logger.AppendInfo("delta is: " + deltaList[i]);             
                AverageDelta += deltaList[i];

                if (deltaList[i] > maxDelta)
                    maxDelta = deltaList[i];

                if (deltaList[i] < minDelta)
                    minDelta = deltaList[i];

                if (deltaList[i] < expectedResult / acceptedThreshold || deltaList[i] > expectedResult * acceptedThreshold)  //SOME DELTA IS OUT OF RANGE
                {
                    Logger.AppendInfo("Failed! Delta = " + Convert.ToString(deltaList[i]) + ((deltaList[i] < expectedResult / acceptedThreshold) ? (" < " + Convert.ToString(expectedResult / acceptedThreshold)) : (" > " + Convert.ToString(expectedResult * acceptedThreshold))) + " ; AcceptedThreshold = " + Convert.ToString(acceptedThreshold));
                    isDeltaArrayOk = false;
                }
            }
            AverageDelta = AverageDelta / (deltaList.Count - 3);
            if (AverageDelta < expectedResult / acceptedThreshold || AverageDelta > expectedResult * acceptedThreshold)  //AVERAGE IS OUT OF RANGE
            {
                Logger.AppendInfo("Failed! Average Delta = " + Convert.ToString(AverageDelta) + ((AverageDelta < expectedResult / acceptedThreshold) ? (" < " + Convert.ToString(expectedResult / acceptedThreshold)) : (" > " + Convert.ToString(expectedResult * acceptedThreshold))) + " ; AcceptedThreshold = " + Convert.ToString(acceptedThreshold));
                isTestPassed = false;
            }
            isTestPassed = isDeltaArrayOk ? isTestPassed : false;

            Logger.AppendInfo("Configured FPS = " + fps + " , [expectedFps(depth formula) = " + expectedFps + " , expectedDelta(1000/expectedFps) = " + expectedResult + "] , realMaxDelta = " + maxDelta + " , realMinDelta = " + minDelta + " , Avg Delta : " + AverageDelta);

            return isTestPassed;

        }


        public bool HW_IsFrameDeltaOK(bool ifColorProfile, int fps, int exposure, float MvsR, int accuracy)
        {
            Logger.AppendInfo("IsFrameDeltaOK validation:\n");
            if (framesTimeStampList_HW.Count < 2)
            {
                Logger.AppendInfo("The size of TimeStampList is less than 2");
                return false;
            }

            double first, second, delta;
            deltaList = new List<double>();
            Logger.AppendInfo("HW timestamp size is :"+ framesTimeStampList_HW.Count);
            for (int i = 0; i < framesTimeStampList_HW.Count - 1; i++)
            {
                first = framesTimeStampList_HW[i].timestamp;
                second = framesTimeStampList_HW[i + 1].timestamp;
                delta = (second - first)/100000;
                deltaList.Add(delta);
            }

            double acceptedThreshold = 1.10;
            bool isTestPassed = true;
            double expectedFps;

            if (ifColorProfile)
                expectedFps = GetExpectedColorFpsInExposureControl(fps, exposure);
            else
                expectedFps = GetExpectedDepthFpsResultInMotionVsRangeTradeoff("", fps, (float)accuracy, (float)MvsR);


            Logger.AppendInfo("Excpected FPS: " + expectedFps);
            double expectedResult = (1.0 / expectedFps) * 1000.0;
            Logger.AppendInfo("Excpected Result: " + expectedResult);

            //check delta 
            bool isDeltaArrayOk = true;
            double AverageDelta = 0;
            double maxDelta = deltaList[1];
            double minDelta = deltaList[1];
            Logger.AppendInfo("Delta HW timestamp size is :" + deltaList.Count);
            for (int i = 1; i < deltaList.Count-1; i++)
            {
                Logger.AppendInfo("delta is: " + deltaList[i]);
                Logger.AppendInfo("HW timestamp " + framesTimeStampList_HW[i-1].timestamp + "  frameCounter"+ framesTimeStampList_HW[i-1].frameCounter);
                Logger.AppendInfo("HW timestamp "+ framesTimeStampList_HW[i].timestamp+ "  frameCounter"+ framesTimeStampList_HW[i].frameCounter);
                AverageDelta += deltaList[i];

                if (deltaList[i] > maxDelta)
                    maxDelta = deltaList[i];

                if (deltaList[i] < minDelta)
                    minDelta = deltaList[i];

                if (deltaList[i] < expectedResult / acceptedThreshold || deltaList[i] > expectedResult * acceptedThreshold)  //SOME DELTA IS OUT OF RANGE
                {
                    Logger.AppendInfo("Failed! Delta = " + Convert.ToString(deltaList[i]) + ((deltaList[i] < expectedResult / acceptedThreshold) ? (" < " + Convert.ToString(expectedResult / acceptedThreshold)) : (" > " + Convert.ToString(expectedResult * acceptedThreshold))) + " ; AcceptedThreshold = " + Convert.ToString(acceptedThreshold));
                    isDeltaArrayOk = false;
                }
            }
            AverageDelta = AverageDelta / (deltaList.Count - 3);
            if (AverageDelta < expectedResult / acceptedThreshold || AverageDelta > expectedResult * acceptedThreshold)  //AVERAGE IS OUT OF RANGE
            {
                Logger.AppendInfo("Failed! Average Delta = " + Convert.ToString(AverageDelta) + ((AverageDelta < expectedResult / acceptedThreshold) ? (" < " + Convert.ToString(expectedResult / acceptedThreshold)) : (" > " + Convert.ToString(expectedResult * acceptedThreshold))) + " ; AcceptedThreshold = " + Convert.ToString(acceptedThreshold));
                isTestPassed = false;
            }
            isTestPassed = isDeltaArrayOk ? isTestPassed : false;

            Logger.AppendInfo("Configured FPS = " + fps + " , [expectedFps(depth formula) = " + expectedFps + " , expectedDelta(1000/expectedFps) = " + expectedResult + "] , realMaxDelta = " + maxDelta + " , realMinDelta = " + minDelta + " , Avg Delta : " + AverageDelta);

            return isTestPassed;
        }

        public bool CalculateFPS(string testType, int fps, int NumberOfFramesArrived, float accuracy, float MvsR, double currExposure)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("currExposure = " + currExposure);
            Logger.AppendInfo("Calculate FPS Validation:\n");
            NumberOfFramesArrived = NumberOfFramesArrived - 1;
            int countOfFramesArrived = framesTimeStampList.Count;
            if (countOfFramesArrived < 2)
            {
                Logger.AppendInfo("The size of TimeStampList is less than 2");
                return false;
            }

            double duration = framesTimeStampList[countOfFramesArrived - 1] - framesTimeStampList[1];
            double realFPS = ((double)NumberOfFramesArrived / (duration)) * 1000.0;

            //===>FW calculation double CameraFPS = ((double)NumberOfFramesArrived / (_TimeStampAndArrivalTimeList.back().first - _TimeStampAndArrivalTimeList.front().first)) * 10000000.0;

            double expectedFps;

            if (testType.ToLower().Contains("color") || testType.ToLower().Contains("rgb"))
                expectedFps = GetExpectedColorFpsInExposureControl(fps, (int)currExposure);
            else
                expectedFps = GetExpectedDepthFpsResultInMotionVsRangeTradeoff(testType, fps, accuracy, MvsR);
            Console.WriteLine("Expected FPS = " + expectedFps + " actual fps = " + realFPS);
            Logger.AppendInfo("Expected FPS = " + expectedFps + " actual fps = " + realFPS);
            return CheckFpsTreshold(realFPS, expectedFps);
        }

        private bool CheckFpsTreshold(double realFps, double expectedFps)
        {
            if (expectedFps == double.NaN)
            {
                expectedFps = realFps;
            }
            double p = 100 - (realFps / expectedFps) * 100;
            double acceptedThreshold = 10;

            if (p > acceptedThreshold)
            {
                Logger.AppendInfo("FPS TRASHOLD - Failed. Actual FPS:" + realFps + " ; Expected FPS: " + expectedFps + " (" + p + " percent)");
                return false;
            }
            else
            {
                return true;
            }
        }


        public bool CheckDS5FPS(int fps, int framesNumber, double[] framesArray)
        {
            framesNumber = framesArray.Count();
            int fn = framesNumber - 1;
            double a = framesArray[framesNumber - 1];
            double b = framesArray[1];
            double c = (double)fn / (double)fps;
            double d = framesArray[framesNumber - 1] - framesArray[1];
            double e = c * 1000;
            double threshold;
            threshold = fps == 15 || fps == 10 ? 0.1 : 0.05;
            double f = e * threshold;
            Logger.AppendDebug("Expected fps: " + e);
            Logger.AppendDebug("Actual fps : " + d);
            Console.WriteLine("Expected fps: " + e);
            Console.WriteLine("Actual fps : " + d);
            if ((c * 1000 + f) < d || (c * 1000 - f) > d)
            {
                Logger.AppendInfo("FAILED!!");
                return false;
            }
            Logger.AppendInfo("PASSED!!");
            return true;
        }

        public int GetExpectedFpsDs5(int fps, double exposure)
        {
            if (exposure >= 20 && exposure <= 11000)
                return fps < 90 ? fps : 90;
            if (exposure >= 11020 && exposure <= 16500)
                return fps < 60 ? fps : 60;
            if (exposure >= 16520 && exposure <= 33200)
                return fps < 30 ? fps : 30;
            if (exposure >= 33220 && exposure <= 66500)
                return fps < 15 ? fps : 15;
            if (exposure >= 66520 && exposure <= 166000)
                return fps < 6 ? fps : 6;
            return 0;
        }

        public bool CheckFpsBetweenFramesDS5(int fps, double exposure, double[] framesArray)
        {
            double expectedFps = (1 / (double)GetExpectedFpsDs5(fps, exposure))*1000;
            double threshold = fps < 30 ? 0.1 : 0.05;
            double upper = expectedFps + expectedFps * threshold;
            double lower = expectedFps- expectedFps * threshold;
            bool isDeltaOk = true;

            Logger.AppendDebug($"CheckFpsBetweenFramesDS5: FPS - {fps}, Exposure - {exposure}, Actual FPS - {expectedFps}");

            for (int i = 2; i < framesArray.Count(); i++)
            {
                if((framesArray[i]- framesArray[i-1] ) < lower || (framesArray[i] - framesArray[i - 1]) > upper)
                {
                    isDeltaOk = false;
                    Logger.AppendDebug($"Failed! Delta between: frame{i} - fps:{framesArray[i]}  and frame{i-1} - fps:{framesArray[i-1]},delta: {framesArray[i] - framesArray[i - 1]}");
                }
            }
            return isDeltaOk;
        }
        
        public bool HW_CheckFpsBetweenFramesDS5(int fps, double exposure, double[] framesArray)
        {
            Console.WriteLine("HW_CheckFpsBetweenFramesDS5");
            double expectedFps = (1 / (double)GetExpectedFpsDs5(fps, exposure)) * 1000;
            double threshold = fps < 30 ? 0.1 : 0.05;
            double upper = expectedFps + expectedFps * threshold;
            double lower = expectedFps - expectedFps * threshold;
            bool isDeltaOk = true;

            Logger.AppendDebug($"HW_CheckFpsBetweenFramesDS5: FPS - {fps}, Exposure - {exposure}, Actual FPS - {expectedFps}");

            for(int j = 0; j < framesArray.Count(); j++)
            {
                framesArray[j] = framesArray[j] / 1000;
            }

            for (int i = 2; i < framesArray.Count(); i++)
            {
                if ((framesArray[i] - framesArray[i - 1]) < lower || (framesArray[i] - framesArray[i - 1]) > upper)
                {
                    isDeltaOk = false;
                    Logger.AppendDebug($"Failed! Delta between: frame{i} - OPTime:{((int)framesArray[i]).ToString("X")}   and frame{i - 1} - OPTime:{((int)framesArray[i - 1]).ToString("X")}, delta: {framesArray[i] - framesArray[i - 1]}");
                }
            }
            return isDeltaOk;
        }

        public bool HW_CheckFpsBetweenFramesDS5_(int fps, double exposure, List<object> framesArray)
        {
            Console.WriteLine("HW_CheckFpsBetweenFramesDS5");
            double expectedFps = (1 / (double)GetExpectedFpsDs5(fps, exposure)) * 1000;
            double threshold = fps < 30 ? 0.1 : 0.05;
            double upper = (expectedFps + expectedFps * threshold)*1000;
            double lower = (expectedFps - expectedFps * threshold)*1000;
            bool isDeltaOk = true;

            Console.WriteLine($"HW_CheckFpsBetweenFramesDS5_: FPS - {fps}, Exposure - {exposure}, Actual FPS - {expectedFps}");
            Logger.AppendDebug($"HW_CheckFpsBetweenFramesDS5_: FPS - {fps}, Exposure - {exposure}, Actual FPS - {expectedFps}");

            Console.WriteLine($"threshold: upper - {upper}, lower - {lower}");
            Logger.AppendDebug($"threshold: upper - {upper}, lower - {lower}");
            
            for (int i = 2; i < framesArray.Count(); i++)
            {
                double delta = Convert.ToDouble(framesArray[i]) - Convert.ToDouble(framesArray[i - 1]);
                if (delta < lower || delta > upper)
                {
                    isDeltaOk = false;
                    Logger.AppendDebug($"Failed! Delta between: frame{i} - OPTime:{framesArray[i]}   and frame{i - 1} - OPTime:{framesArray[i - 1]}, delta: {delta}");
                    Console.WriteLine($"Failed! Delta between: frame{i} - OPTime:{framesArray[i]}   and frame{i - 1} - OPTime:{framesArray[i - 1]}, delta: {delta}");
                }
            }
            return isDeltaOk;
        }
        public bool HW_CheckFpsBetweenFramesDS5_FullData(int fps, double exposure, List<Utils.REAL_SENSE_RS400_DEPTH_METADATA_INTEL_CAPTURE_TIME> framesArray)
        {
            double expectedFps = (1 / (double)GetExpectedFpsDs5(fps, exposure)) ;
            double threshold = fps < 30 ? 0.1 : 0.05;
            double upper = (expectedFps + expectedFps * threshold)*1000000.0;
            double lower = (expectedFps - expectedFps * threshold)*1000000.0;
            bool isDeltaOk = true;

            Logger.AppendDebug($"upper: {upper}, lower: {lower}");
            Logger.AppendDebug($"HW_CheckFpsBetweenFramesDS5_FullData: FPS - {fps}, Exposure - {exposure}, Actual FPS - {expectedFps}");
            
            for (int i = 2; i < framesArray.Count(); i++)
            {
                Console.WriteLine("delta "+ (framesArray[i].opticalTimestamp - framesArray[i - 1].opticalTimestamp));
                if ((framesArray[i].opticalTimestamp - framesArray[i - 1].opticalTimestamp) < lower || (framesArray[i].opticalTimestamp - framesArray[i - 1].opticalTimestamp) > upper)
                {
                    isDeltaOk = false;
                    Logger.AppendDebug($"Failed! Delta between: frame{framesArray[i].frameCounter} - fps:{framesArray[i].opticalTimestamp}  and frame{framesArray[i-1].frameCounter} - fps:{framesArray[i - 1].opticalTimestamp}, delta: {framesArray[i].opticalTimestamp - framesArray[i - 1].opticalTimestamp}");
                }
            }
            return isDeltaOk;
        }
        
        public bool Analysis_SR4xx_FrameData(int fps, double exposure, List<Utils.FRAME_DATA_RS400> lst_frames)
        {
            Logger.AppendDebug("CheckFpsBetweenFramesDS5 - list size :"+ lst_frames.Count);
            Console.WriteLine("CheckFpsBetweenFramesDS5 - list size :" + lst_frames.Count);
            Console.WriteLine("exposure value is :" + exposure);
            Logger.AppendDebug("exposure value is :" + exposure);
            double expectedFps = GetExpectedFpsDs5(fps, exposure);
            if (expectedFps == 0)
            {
                expectedFps = (int)Math.Min((int)fps, Math.Pow(2, Math.Abs(exposure)));
                expectedFps = (1 / (double)expectedFps) * 1000;
            }
            else
            {
                expectedFps = (1 / (double)expectedFps) * 1000;
            }
            Logger.AppendDebug("Execpted FPS: " + expectedFps);
            double threshold = fps < 30 ? 0.1 : 0.05;
            double upper = (expectedFps + expectedFps * threshold) * 1000;
            double lower = (expectedFps - expectedFps * threshold) * 1000;
            bool frameStatus = true;
            bool profileStatus = true;

            Console.WriteLine($"CheckFpsBetweenFramesDS5: FPS - {fps}, Exposure - {exposure}, Actual FPS - {expectedFps}");
            Logger.AppendDebug($"CheckFpsBetweenFramesDS5: FPS - {fps}, Exposure - {exposure}, Actual FPS - {expectedFps}");

            Console.WriteLine($"threshold: upper - {upper}, lower - {lower}");
            Logger.AppendDebug($"threshold: upper - {upper}, lower - {lower}");
            uint frameLost = 0;
            for (int i = 1; i < lst_frames.Count; i++)
            {
                frameStatus = true;
                
                Console.WriteLine("Type: "+ lst_frames[i].SensorType+"   Check frameCounter " + lst_frames[i].FrameCounter);
                Logger.AppendDebug("Type: " + lst_frames[i].SensorType + "   Check frameCounter " + lst_frames[i].FrameCounter);
                if (lst_frames[i].FrameCounter - lst_frames[i - 1].FrameCounter > 1)
                {
                    Console.WriteLine($"Error on frameCounter: frame-index {i - 1}: {lst_frames[i - 1].FrameCounter}, frame_index {i}: {lst_frames[i].FrameCounter} ");
                    Logger.AppendError($"Error on frameCounter: frame_index {i - 1}: {lst_frames[i - 1].FrameCounter}, frame_index {i}: {lst_frames[i].FrameCounter}");
                    frameStatus &= false;
                    frameLost += lst_frames[i].FrameCounter - lst_frames[i - 1].FrameCounter;
                }

                Console.WriteLine("Check callBackTime "+ lst_frames[i].callBackTime);
                Logger.AppendDebug("Check callBackTime "+ lst_frames[i].callBackTime);
                if (lst_frames[i].callBackTime - lst_frames[i - 1].callBackTime > 2)
                {
                    Console.WriteLine($"Error on callBackTime:  frame_index {i - 1}: {lst_frames[i - 1].callBackTime}, frame_index {i}: {lst_frames[i].callBackTime}, elapsed time: {lst_frames[i].callBackTime - lst_frames[i - 1].callBackTime}");
                    Logger.AppendError($"Error on callBackTime: frame_index {i - 1}: {lst_frames[i - 1].callBackTime}, frame_index {i}: {lst_frames[i].callBackTime}, elapsed time: {lst_frames[i].callBackTime - lst_frames[i - 1].callBackTime}");
                    frameStatus &= false;
                }

                Console.WriteLine("Check hwTimeStamp "+ lst_frames[i].hwTimeStamp);
                Logger.AppendDebug("Check hwTimeStamp "+ lst_frames[i].hwTimeStamp);
                double hw_delta = Convert.ToDouble(lst_frames[i].hwTimeStamp) - Convert.ToDouble(lst_frames[i - 1].hwTimeStamp);
                if (hw_delta < lower || hw_delta > upper)
                {
                    Console.WriteLine($"Error on hwTimeStamp: frame_index {i - 1}: {lst_frames[i - 1].hwTimeStamp}, frame_index {i}: {lst_frames[i].hwTimeStamp}, delta : {hw_delta}");
                    Logger.AppendError($"Error on hwTimeStamp: frame_index {i - 1}: {lst_frames[i - 1].hwTimeStamp}, frame_index {i}: {lst_frames[i].hwTimeStamp}, delta : {hw_delta}");
                    frameStatus &= false;
                }

                Console.WriteLine("Check systemRelativeTime "+ lst_frames[i].systemRelativeTime);
                Logger.AppendDebug("Check systemRelativeTime "+lst_frames[i].systemRelativeTime);
                double system_delta = (lst_frames[i].systemRelativeTime - lst_frames[i-1].systemRelativeTime)*1000;
                if (system_delta < lower || system_delta > upper)
                {
                    Console.WriteLine($"Error on systemRelativeTime:  frame_index {i - 1}: {lst_frames[i - 1].systemRelativeTime}, frame_index {i}: {lst_frames[i].systemRelativeTime}, delta : {system_delta}");
                    Logger.AppendError($"Error on systemRelativeTime: frame_index {i - 1}: {lst_frames[i - 1].systemRelativeTime},  frame_index {i}: {lst_frames[i].systemRelativeTime}, delta : {system_delta}");
                    frameStatus &= false;
                }
                Logger.AppendMessage("Frame Status: "+ frameStatus);
                Logger.AppendMessage("---");
                profileStatus &= frameStatus;
            }

            Console.WriteLine($"frames lost  {((double)frameLost/ (double)lst_frames.Count)*100}%");
            Logger.AppendDebug($"frames lost { ((double)frameLost / (double)lst_frames.Count)*100}%");


            
            return profileStatus;
        }

        public static int CalculateHowLongToRunThePropertiesWhileStreamingTest(List<Control> controlsL, int FPS)
        {
            int howLongToRunTest = 0;
            for (int i = 0; i < controlsL.Count(); i++)
            {
                switch (controlsL[i].Key)
                {
                    case Types.ControlKey.DS5_DEPTH_EXPOSURE:
                        {
                            howLongToRunTest += CalculateHowLongToRunThePropertiesWhileStreamingTestHelper(SetControl.DS5ExposureMin, SetControl.DS5ExposureMax, SetControl.DS5ExposureStep, FPS);
                            break;
                        }
                    case Types.ControlKey.DS5_DEPTH_LASER_ON_OFF:
                        {
                            howLongToRunTest += CalculateHowLongToRunThePropertiesWhileStreamingTestHelper(SetControl.DS5LaserOnOffMin, SetControl.DS5LaserOnOffMax, SetControl.DS5LaserOnOffStep, FPS);
                            break;
                        }
                    case Types.ControlKey.DS5_DEPTH_LASER_POWER:
                        {
                            howLongToRunTest += CalculateHowLongToRunThePropertiesWhileStreamingTestHelper(SetControl.DS5LaserPowerMin, SetControl.DS5LaserPowerMax, SetControl.DS5LaserPowerStep, FPS);
                            break;
                        }
                    case Types.ControlKey.COLOR_EXPOSURE:
                        {
                            howLongToRunTest += CalculateHowLongToRunThePropertiesWhileStreamingTestHelper(SetControl.SR300ColorExposureMin, SetControl.SR300ColorExposureMax, SetControl.SR300ColorExposureStep, FPS);
                            break;
                        }
                    case Types.ControlKey.COLOR_BRIGHTNESS:
                        {
                            howLongToRunTest += CalculateHowLongToRunThePropertiesWhileStreamingTestHelper(SetControl.SR300ColorBrightnessMin, SetControl.SR300ColorBrightnessMax, SetControl.SR300ColorBrightnessStep, FPS);
                            break;
                        }
                    case Types.ControlKey.COLOR_CONTRAST:
                        {
                            howLongToRunTest += CalculateHowLongToRunThePropertiesWhileStreamingTestHelper(SetControl.SR300ColorContrastMin, SetControl.SR300ColorContrastMax, SetControl.SR300ColorContrastStep, FPS);
                            break;
                        }
                    case Types.ControlKey.COLOR_HUE:
                        {
                            howLongToRunTest += CalculateHowLongToRunThePropertiesWhileStreamingTestHelper(SetControl.SR300ColorHueMin, SetControl.SR300ColorHueMax, SetControl.SR300ColorHueStep, FPS);
                            break;
                        }
                    case Types.ControlKey.COLOR_WHITE_BALANCE:
                        {
                            howLongToRunTest += CalculateHowLongToRunThePropertiesWhileStreamingTestHelper(SetControl.SR300ColorWhiteBalanceMin, SetControl.SR300ColorWhiteBalanceMax, SetControl.SR300ColorWhiteBalanceStep, FPS);
                            break;
                        }
                    case Types.ControlKey.COLOR_BACK_LIGHT_COMPENSATION:
                        {
                            howLongToRunTest += CalculateHowLongToRunThePropertiesWhileStreamingTestHelper(SetControl.SR300ColorBacklightCompensationMin, SetControl.SR300ColorBacklightCompensationMax, SetControl.SR300ColorBacklightCompensationStep, FPS);
                            break;
                        }
                    case Types.ControlKey.COLOR_EXPOSURE_PRIORITY:
                        {
                            howLongToRunTest += CalculateHowLongToRunThePropertiesWhileStreamingTestHelper(SetControl.SR300ColorExposurePriorityMin, SetControl.SR300ColorExposurePriorityMax, SetControl.SR300ColorExposurePriorityStep, FPS);
                            break;
                        }
                    case Types.ControlKey.COLOR_POWER_LINE_FREQUENCY:
                        {
                            howLongToRunTest += CalculateHowLongToRunThePropertiesWhileStreamingTestHelper(SetControl.SR300ColorPowerLineFrequencyMin, SetControl.SR300ColorPowerLineFrequencyMax, SetControl.SR300ColorPowerLineFrequencyStep, FPS);
                            break;
                        }
                    case Types.ControlKey.DEPTH_MOTION_VS_RANGE_TRADE:
                        {
                            howLongToRunTest += CalculateHowLongToRunThePropertiesWhileStreamingTestHelper(SetControl.SR300DepthMotionVSRangeTradeoffMin, SetControl.SR300DepthMotionVSRangeTradeoffMax, SetControl.SR300DepthMotionVSRangeTradeoffStep, FPS);
                            break;
                        }
                    case Types.ControlKey.DEPTH_LASER_POWER:
                        {
                            howLongToRunTest += CalculateHowLongToRunThePropertiesWhileStreamingTestHelper(SetControl.SR300DepthLaserPowerMin, SetControl.SR300DepthLaserPowerMax, SetControl.SR300DepthLaserPowerStep, FPS);
                            break;
                        }
                    case Types.ControlKey.DEPTH_ACCURACY:
                        {
                            howLongToRunTest += CalculateHowLongToRunThePropertiesWhileStreamingTestHelper(SetControl.SR300DepthAccuracyMin, SetControl.SR300DepthAccuracyMax, SetControl.SR300DepthAccuracyStep, FPS);
                            break;
                        }
                    case Types.ControlKey.DEPTH_FILTER_OPTION:
                        {
                            howLongToRunTest += CalculateHowLongToRunThePropertiesWhileStreamingTestHelper(SetControl.SR300DepthFilterOptionMin, SetControl.SR300DepthFilterOptionMax, SetControl.SR300DepthFilterOptionStep, FPS);
                            break;
                        }
                    case Types.ControlKey.DEPTH_CONFIDENCE_THRESHOLD:
                        {
                            howLongToRunTest += CalculateHowLongToRunThePropertiesWhileStreamingTestHelper(SetControl.SR300DepthConfidenceThresholdMin, SetControl.SR300DepthConfidenceThresholdMax, SetControl.SR300DepthConfidenceThresholdStep, FPS);
                            break;
                        }
                    case Types.ControlKey.DEPTH_PRESET:
                        {
                            howLongToRunTest += 10000;//constant 10000 ms
                            break;
                        }
                    case Types.ControlKey.COLOR_GAMMA:
                        {
                            howLongToRunTest += CalculateHowLongToRunThePropertiesWhileStreamingTestHelper(SetControl.SR300ColorGammaMin, SetControl.SR300ColorGammaMax, SetControl.SR300ColorGammaStep, FPS);
                            break;
                        }
                    case Types.ControlKey.COLOR_SATURATION:
                        {
                            howLongToRunTest += CalculateHowLongToRunThePropertiesWhileStreamingTestHelper(SetControl.SR300ColorSaturationMin, SetControl.SR300ColorSaturationMax, SetControl.SR300ColorSaturationStep, FPS);
                            break;
                        }
                    case Types.ControlKey.COLOR_SHARPNESS:
                        {
                            howLongToRunTest += CalculateHowLongToRunThePropertiesWhileStreamingTestHelper(SetControl.SR300ColorSharpnessMin, SetControl.SR300ColorSharpnessMax, SetControl.SR300ColorSharpnessStep, FPS);
                            break;
                        }
                    case Types.ControlKey.COLOR_GAIN:
                        {
                            howLongToRunTest += CalculateHowLongToRunThePropertiesWhileStreamingTestHelper(SetControl.SR300ColorGainMin, SetControl.SR300ColorGainMax, SetControl.SR300ColorGainStep, FPS);
                            break;
                        }
                    case Types.ControlKey.DS5_DEPTH_GAIN:
                        {
                            break;
                        }
                    case Types.ControlKey.COLOR_PRIVACY:
                        {
                            break;
                        }
                    case Types.ControlKey.DEPTH_PRIVACY:
                        {
                            howLongToRunTest += CalculateHowLongToRunThePropertiesWhileStreamingTestHelper(SetControl.SR300DepthPrivacyMin, SetControl.SR300DepthPrivacyMax, SetControl.SR300DepthPrivacyStep, FPS);
                            break;
                        }
                }
            }
            howLongToRunTest += 1000000;//give a buffer
            Console.WriteLine("Running the test for " + howLongToRunTest + " ms");
            return howLongToRunTest;
        }
        private static int CalculateHowLongToRunThePropertiesWhileStreamingTestHelper(double min, double max, double step, int FPS)
        {
            return (int)(((max - min) / step) * SetControl.calculateMillisecondsToSleepBetweenSets((uint)FPS)) + 1000 /**1000 buffer time*/;
        }

        public static double GetExpectedColorFps_DS5(int fps, double exposureValue)
        {
            double expectedValue =  Math.Pow(2, exposureValue) * 1000;
            expectedValue = (fps < expectedValue) ? fps : expectedValue;
            return expectedValue;
        }


        private bool isSubTypeExist(List<string> subTypes,string type)
        {
            foreach(var s in subTypes)
            {
                if (type.Equals(s)) return true;
            }

            return false;
        }
        private int subTypeNum(List<Utils.FRAME_DATA_RS400> lst_Frames)
        {
            try
            {
                List<string> subTypes = new List<string>();
                foreach(var f in lst_Frames)
                {
                    if (isSubTypeExist(subTypes, f.subType.ToString()) == false)
                        subTypes.Add(f.subType.ToString());
                }

                return subTypes.Count;
            }
            catch (Exception ex)
            {
                return -1;
                
            }
        }
        public bool Check_HW_SyncTimeStamp_Color_Depth(int fps, List<Utils.FRAME_DATA_RS400> lst_Frames)
        {
            bool status1 = true, status2 = true, status3 = true, status4 = true,status5=true;
            double threshold = 0.1 * fps ;
            List<Utils.FRAME_DATA_RS400> type1Frames = null;
            List<Utils.FRAME_DATA_RS400> type2Frames = null;
            List<Utils.FRAME_DATA_RS400> type3Frames = null;

            List<Utils.FRAME_DATA_RS400> depthFrames = null;
            List<Utils.FRAME_DATA_RS400> irFrames = null;
            List<Utils.FRAME_DATA_RS400> colorFrames = null;

            string subType1="", subType2="", subType3="";
            GetSubType(lst_Frames, out subType1, out subType2,out subType3);
            
            if(subType1!="")
                type1Frames = SortFramesByType(subType1, lst_Frames);
            if (subType2 != "")
                type2Frames = SortFramesByType(subType2, lst_Frames);
            if (subType3 != "")
                type3Frames = SortFramesByType(subType3, lst_Frames);

            if (type1Frames != null)
            {
                Logger.AppendDebug($"*****Validate {type1Frames[0].SensorType.ToString()} {type1Frames[0].subType.ToString()} Frames*****");
                status1 = CheckSequenceFramesDelta(fps, type1Frames);
                Logger.AppendDebug("");
            }
            if (type2Frames != null)
            {
                Logger.AppendDebug($"*****Validate {type2Frames[0].SensorType.ToString()} {type2Frames[0].subType.ToString()} Frames*****");
                status2 = CheckSequenceFramesDelta(fps, type2Frames);
                Logger.AppendDebug("");
            }

            if (type3Frames != null)
            {
                Logger.AppendDebug($"*****Validate {type3Frames[0].SensorType.ToString()} {type3Frames[0].subType.ToString()} Frames*****");
                status3 = CheckSequenceFramesDelta(fps, type3Frames);
                Logger.AppendDebug("");
            }



            switch(subType1)
            {
                case "YUY2":
                    colorFrames = type1Frames;
                    break;
                case "D16":
                    depthFrames = type1Frames;
                    break;
                case "L8":
                    irFrames = type1Frames;
                    break;
            }

            switch (subType2)
            {
                case "YUY2":
                    colorFrames = type2Frames;
                    break;
                case "D16":
                    depthFrames = type2Frames;
                    break;
                case "L8":
                    irFrames = type2Frames;
                    break;
            }

            switch (subType3)
            {
                case "YUY2":
                    colorFrames = type3Frames;
                    break;
                case "D16":
                    depthFrames = type3Frames;
                    break;
                case "L8":
                    irFrames = type3Frames;
                    break;
            }



            // check sync between depth and color            
            if (depthFrames != null && colorFrames != null)
            {
                Logger.AppendDebug("**** Depth and Color Sync validation ****");
                List<FrameSyncPair> FramesPairs = new List<FrameSyncPair>();
                status4 = Check_Sync(fps, depthFrames, colorFrames, out FramesPairs);
                Logger.AppendDebug("");
            }

            // check sync between IR and color
            if (irFrames != null && colorFrames != null)
            {
                Logger.AppendDebug("**** IR and Color Sync validation ****");
                List<FrameSyncPair> FramesPairs2 = new List<FrameSyncPair>();
                status5 = Check_Sync(fps, irFrames, colorFrames, out FramesPairs2);
                Logger.AppendDebug("");
            }

            return status1 & status2 & status3 & status4 & status5;
        }
        
        public void GetSubType(List<Utils.FRAME_DATA_RS400> lst_Frames, out string type1, out string type2,out string type3)
        {
            int subTypesNum = subTypeNum(lst_Frames);
            type1 = lst_Frames.Where(x => x.subType != null).First().subType;
            string temp = type1;
            type2 = lst_Frames.Where(x => x.subType != temp).First().subType;
            type3 = "";
            if (subTypesNum == 3)
            {
                string temp2 = type2;
                type3 = lst_Frames.Where(x => x.subType != temp && x.subType != temp2).First().subType;
            }
        }    
        public List<Utils.FRAME_DATA_RS400>  SortFramesByType(string type, List<Utils.FRAME_DATA_RS400> lst_frames)
        {
            List<Utils.FRAME_DATA_RS400> typeFrames= new List<Utils.FRAME_DATA_RS400>();            
            if (lst_frames != null)
            {
                foreach (var f in lst_frames)
                {
                    //if (f.SensorType.ToString() == type)
                    if (f.subType == type)
                    {
                        typeFrames.Add(f);
                    }
                }
            }
            Console.WriteLine($"list of {type} frames , count: {typeFrames.Count()} ");
            Logger.AppendDebug($"list of {type} frames , count: {typeFrames.Count()} ");
            return typeFrames;
        }
        public bool CheckSequenceFramesDelta(int _fps, List<Utils.FRAME_DATA_RS400> frames)
        {
            double threshold = _fps < 30 ? 0.1 : 0.05;
            double fps = (double) 1 / _fps;
            double lower = fps * 1000 - threshold * (fps * 1000);
            double upper = fps * 1000 + threshold * (fps * 1000);
            bool status = true;
            Console.WriteLine("fps- "+ fps + "  Upper bond: " + upper + "  Lower bond: " + lower);
            Logger.AppendDebug ("fps- " + fps + "  Upper bond: " + upper + "   Lower bond: " + lower);
            int totalLost = 0;
            Logger.AppendDebug($"First 3 frames counter: {frames[0].SensorType}, {frames[0].FrameCounter}, {frames[1].FrameCounter}, {frames[2].FrameCounter}");
            for (int i=1 ; i<frames?.Count-3 ; i++)
            {
                var sys_delta = (frames[i + 1].systemRelativeTime - frames[i].systemRelativeTime);
                var delta=(Convert.ToDouble( frames[i + 1].hwTimeStamp) - Convert.ToDouble(frames[i].hwTimeStamp))/1000;

                
                if (frames[i+1].FrameCounter -1 !=  frames[i].FrameCounter)
                {
                    Logger.AppendError($"Frame counter not sequenced: {frames[i].SensorType}, {frames[i].FrameCounter}, next: { frames[i + 1].FrameCounter}");
                }
                if (frames[i].FrameCounter< frames[i-1].FrameCounter)
                {
                    Logger.AppendError($"current frame counter is: {frames[i].FrameCounter}, previous: { frames[i - 1].FrameCounter}");
                }
                if (delta < lower || delta > upper)
                {
                    Console.WriteLine($"Error on  HW time: {frames[i].SensorType}, {frames[i].subType}, {frames[i].FrameCounter}, {frames[i].hwTimeStamp}");
                    Logger.AppendError($"{frames[i].SensorType}, {frames[i].FrameCounter}-{frames[i].hwTimeStamp},  previous counter {frames[i-1].FrameCounter}-{frames[i-1].hwTimeStamp},  next counter {frames[i+1].FrameCounter}-{frames[i + 1].hwTimeStamp},next->next counter {frames[i + 2].FrameCounter}-{frames[i + 2].hwTimeStamp}, Delta: {delta}");
                    status = false;
                    //the if condition to skip counter reset bug!
                    if (frames[i].FrameCounter < frames[i + 1].FrameCounter) 
                        totalLost += (int) (frames[i + 1].FrameCounter - frames[i].FrameCounter - 1);
                }
            }
            Console.WriteLine("@@@ expected frames " + _fps * (uint.Parse(ConfigurationSettings.AppSettings["streamTimeInMilliseconds"]) / 1000));
            if (totalLost > 0)
            {
                Logger.AppendDebug($"*** Total lost is: { totalLost } ***");
                Logger.AppendDebug($"*** Percent: { ((double)totalLost / (double)(_fps*(uint.Parse (ConfigurationSettings.AppSettings["streamTimeInMilliseconds"])/1000) )) * 100}% ***");
            }
            return status;
        }
        public void SystemTimestampAdjustment(double adj,  List<FrameSyncPair> lst)
        {
            foreach(var f in lst)
            {
                f.frame1.systemTime = f.frame1.systemTime - adj;
            }
        }

        public bool Validate_Synced_Pairs(List<FrameSyncPair> framesPairs, out List<string> error)
        {
            try
            {
                bool status = true;
                string errorMessage;
                int counter = 0;
                error = new List<string>();
                foreach(var p in framesPairs)
                {
                    if (counter < 6)
                    {
                        counter++;
                        continue;
                    }

                    
                    if (p.frame1.trigger != 1)
                    {
                        status = false;
                        errorMessage = p.frame1.subType + " Sync Trigger equal to 0";
                        error.Add(errorMessage);
                    }

                    if (p.frame2.trigger != 1)
                    {
                        status = false;
                        errorMessage = p.frame1.subType + " Sync Trigger equal to 0";
                        error.Add(errorMessage);
                    }

                    if (!status) break;
                }

                return status;
            }
            catch (Exception ex)
            {
                error = null;
                return false;
            }
        }

        public bool Check_Sync(int fps, List<Utils.FRAME_DATA_RS400> type1Frames, List<Utils.FRAME_DATA_RS400> type2Frames,out List<FrameSyncPair> FramesPairs)
        {
            bool status;
            List<string> error;
            

            Console.WriteLine($"Depth list size is {type1Frames?.Count} , Color list size is {type2Frames?.Count}");
            Logger.AppendDebug($"Depth list size is {type1Frames?.Count} , Color list size is {type2Frames?.Count}");
            
            int size = type1Frames.Count < type2Frames.Count ? type1Frames.Count : type2Frames.Count;

            List<FrameToPair> startFirst;
            List<FrameToPair> startSecond;

            var threshold = (((double)1000 / fps) * 0.1) * 1000;

            Console.WriteLine("Threshold in microseconds : " + threshold);
            Logger.AppendDebug("Threshold in microseconds : " + threshold);
            
            ///find out which stream start first
            if (Convert.ToDouble(type1Frames[0].hwTimeStamp) < Convert.ToDouble(type2Frames[0].hwTimeStamp))
            {
                startFirst = ConvertList_FRAME_DATA_RS400_To_FrameToPair(type1Frames);
                startSecond = ConvertList_FRAME_DATA_RS400_To_FrameToPair(type2Frames);
                Console.WriteLine($"{type1Frames[0].SensorType.ToString()} started first");
                Logger.AppendDebug($"{type1Frames[0].SensorType.ToString()} started first");
            }
            else
            {
                startFirst = ConvertList_FRAME_DATA_RS400_To_FrameToPair(type2Frames);
                startSecond = ConvertList_FRAME_DATA_RS400_To_FrameToPair(type1Frames);
                Console.WriteLine($"{type2Frames[0].SensorType.ToString()} started first");
                Logger.AppendDebug($"{type2Frames[0].SensorType.ToString()} started first");
            }
            
            FramesPairs = new List<FrameSyncPair>();
            foreach (var sf in startSecond)
            {
                double delta = -1;
                FrameSyncPair tempPairs = new FrameSyncPair();
                tempPairs.frame1 = sf;
                tempPairs.frame2 = new FrameToPair(new Utils.FRAME_DATA_RS400());
                foreach (var ff in startFirst)
                {
                    //double dTemp = Math.Abs(sf.systemTime - ff.systemTime);
                    double dTemp = Math.Abs(sf.hwTime - ff.hwTime);
                    if (delta == -1  && !ff.isPairFound && dTemp < threshold)
                    {
                        //tempPairs.frame2.isPairFound = false;
                        tempPairs.frame2 = ff;
                        tempPairs.frame2.isPairFound = true;
                        delta = dTemp;
                    }
                    else if (dTemp < delta && ff.isPairFound /*&& dTemp < threshold*1.2*/)
                    {
                        Console.WriteLine($"@@@@@@ {ff.counter}");
                        Logger.AppendDebug($"@@@@@ {ff.counter}");

                        tempPairs.frame2.isPairFound = false;
                        tempPairs.frame2 = ff;
                        tempPairs.frame2.isPairFound = true;
                        delta = dTemp;
                    }                  
                }
                tempPairs.frame1.isPairFound = true;
                if(tempPairs.frame1.isPairFound && tempPairs.frame2.isPairFound)
                    FramesPairs.Add(tempPairs);
            }
         
            if (FramesPairs.Count == 0)
            {
                Console.WriteLine("********** No Synced Found **********");
                Logger.AppendDebug("********** No Synced Found **********");
                return false;
            }
            int i = 0;
            foreach(var f in FramesPairs)
            {
                if(f.frame1.systemTime!=0 && f.frame2.systemTime !=0)
                {
                    break;
                }
                i++;
            }
            double adjustment = FramesPairs[i].frame1.systemTime - FramesPairs[i].frame2.systemTime;
            Console.WriteLine("***" + adjustment);
            Console.WriteLine("***" + FramesPairs[i].frame1.systemTime);
            SystemTimestampAdjustment(adjustment, FramesPairs);
            Console.WriteLine("***" + FramesPairs[i].frame1.systemTime);

            int matchedPairsFound = 0;
            double maxHwDelta = 0, maxSysDelta = 0, minHwDelta = threshold*1000, minSysDelta = threshold*1000, avgHwDelta = 0, avgSysDelta = 0, sumHw = 0, sumSys = 0; 
            foreach(var p in FramesPairs)
            {
                Logger.AppendDebug($"Delta Pair Frame : {p.frame1.sensor} - {p.frame1.subType} - {p.frame1.counter} - {p.frame1.systemTime} - {p.frame1.trigger},  Frame : {p.frame2.sensor} - {p.frame2.subType} - {p.frame2.counter} - {p.frame2.systemTime} - {p.frame2.trigger}, Delta (microSec){p.HwTimeDelta}, System Delta {p.SystemTimeDelta*1000}");
                if (p.HwTimeDelta > 0 && p.HwTimeDelta < threshold)
                {
                    matchedPairsFound++;
                    if(p.HwTimeDelta > maxHwDelta)
                    {
                        maxHwDelta = p.HwTimeDelta;
                    }
                    if(p.SystemTimeDelta > maxSysDelta)
                    {
                        maxSysDelta = p.SystemTimeDelta;
                    }
                    if(p.HwTimeDelta<minHwDelta)
                    {
                        minHwDelta = p.HwTimeDelta;
                    }
                    if(p.SystemTimeDelta<minSysDelta && p.SystemTimeDelta!=0)
                    {
                        minSysDelta = p.SystemTimeDelta;
                    }
                    sumHw += p.HwTimeDelta;
                    sumSys += p.SystemTimeDelta;
                }
                else 
                {
                    Logger.AppendError("HW TimeStamp delta out of range: " + p.HwTimeDelta);
                }
                if(p.SystemTimeDelta < 0 && p.SystemTimeDelta > threshold)
                {
                    Logger.AppendError("HW TimeStamp delta out of range: " + p.SystemTimeDelta * 1000);
                }               
            }
            avgHwDelta = sumHw / matchedPairsFound;
            avgSysDelta = sumSys / matchedPairsFound;

            Logger.AppendInfo($" Matched Pairs found {matchedPairsFound}, number of frames received {size}");
            Console.WriteLine($" Matched Pairs found {matchedPairsFound}, number of frames received {size}");

            syncAvgAnalysis.Add($"{profileName} - HW Delta  Max: {maxHwDelta}, Min: {minHwDelta}, Average: {avgHwDelta} - System Delta  Max: {maxSysDelta*1000}, Min: {minSysDelta*1000}, Average: {avgSysDelta*1000}, threshold {threshold}");

            status = Validate_Synced_Pairs(FramesPairs, out error);

            foreach(var e in error)
            {
                Logger.AppendDebug(e.ToString());
            }

            return status;
        }

        public void PrintSyncAvgAnalysis()
        {
            foreach(string s in syncAvgAnalysis)
            {
                Logger.AppendDebug(s);
                Console.WriteLine(s);
            }
        }
        public void Check_Sync_Color_Depth(int fps,List<Utils.FRAME_DATA_RS400> depthFrames, List<Utils.FRAME_DATA_RS400> colorFrames)
        {
            Console.WriteLine($"Depth list size is {depthFrames.Count} , Color list size is {colorFrames.Count}");
            Logger.AppendDebug($"Depth list size is {depthFrames.Count} , Color list size is {colorFrames.Count}");

            int size = depthFrames.Count < colorFrames.Count ? depthFrames.Count : colorFrames.Count;

            List<Utils.FRAME_DATA_RS400> startFirst;
            List<Utils.FRAME_DATA_RS400> startSecond;

            var threshold = (((double)1 / fps) * 0.1)*1000;
            int startPairIndex = 0;

            Console.WriteLine("Threshold in Milliseconds : "+ threshold);
            Logger.AppendDebug("Threshold in Milliseconds : "+ threshold);

            ///find out whihc stream start first
            if (Convert.ToDouble(depthFrames[0].hwTimeStamp) < Convert.ToDouble(colorFrames[0].hwTimeStamp))
            {
                startFirst = depthFrames;
                startSecond = colorFrames;
                Console.WriteLine("Depth started first");
                Logger.AppendDebug("Depth started first");
            }
            else
            {
                startFirst = colorFrames;
                startSecond = depthFrames;
                Console.WriteLine("Color started first");
                Logger.AppendDebug("Color started first");
            }

            ///find out first matched index of pair color and depth frames 
            for (int i = 0; i < startFirst.Count; i++)
            {
                if( Math.Abs(startFirst[i].systemRelativeTime-startSecond[0].systemRelativeTime) < threshold)
                {
                    startPairIndex = i;
                    Console.WriteLine("start index : " + startPairIndex);
                    Logger.AppendDebug("start index : " + startPairIndex);
                    break;
                }
            }


            if(startSecond.Count + startPairIndex > startFirst.Count)
            {
                size = startFirst.Count - (startSecond.Count + startPairIndex - startFirst.Count);
                Console.WriteLine("loop size : " + size);
                Logger.AppendDebug("loop size : " + size);
            }
           
                    
            for (int i=0; i < size; i++)
            {
                double delta = Math.Abs(startFirst[i + startPairIndex].systemRelativeTime - startSecond[0].systemRelativeTime);
                Console.WriteLine("Delta between frames: "+delta);
                Logger.AppendDebug("Delta between frames: " + delta);

                if (delta > threshold)
                {
                    Console.WriteLine ("Sync Failed !!");
                    Logger.AppendError("Sync Failed !!");

                    Console.WriteLine($"Frame {startFirst[i + startPairIndex].SensorType}, start stream: { startFirst[i + startPairIndex].FrameCounter} -  Frame {startSecond[i].SensorType} second stream: { startSecond[i].FrameCounter}");
                    Console.WriteLine("Frame system time First stream: " + startFirst[i + startPairIndex].systemRelativeTime + " -  Frame system time second stream: " + colorFrames[i].systemRelativeTime);

                    Logger.AppendDebug($"Frame {startFirst[i + startPairIndex].SensorType}, start stream: { startFirst[i + startPairIndex].FrameCounter} -  Frame {startSecond[i].SensorType} second stream: { startSecond[i].FrameCounter}");
                    Logger.AppendDebug("Frame system time First stream: " + startFirst[i + startPairIndex].systemRelativeTime + " -  Frame system time second stream: " + startSecond[i].systemRelativeTime);

                }
            }
        }
        //helper method
        public List<FrameToPair> ConvertList_FRAME_DATA_RS400_To_FrameToPair(List<Utils.FRAME_DATA_RS400> lst)
        {
            List<FrameToPair> temp = new List<FrameToPair>();
            foreach (Utils.FRAME_DATA_RS400 f in lst)
            {
                //Logger.AppendDebug($"frame type: {f.sensorType},  frame counter: {f.FrameCounter}, trigger: {f.ConfigurationMetadata.trigger}, hw time: {f.hwTimeStamp}");
                //Console.WriteLine($"frame type: {f.sensorType},  frame counter: {f.FrameCounter}, trigger: {f.ConfigurationMetadata.trigger}");
                temp.Add(new FrameToPair(f));
            }
            return temp;
        }
        
    }

    public class FrameSyncPair
    {    
        public FrameToPair frame1;
        public FrameToPair frame2;
        public double SystemTimeDelta
        {
            get { return Math.Abs(frame1.systemTime - frame2.systemTime); }
        }

        public double HwTimeDelta
        {            
            get
            {
                //Console.WriteLine($"{frame1.counter}. {frame1.hwTime} - {frame2.counter}. {frame2.hwTime} = {Math.Abs(frame2.hwTime - frame1.hwTime)}");
                //Logger.AppendDebug($"{frame1.counter}. {frame1.hwTime} - {frame2.counter}. {frame2.hwTime} = {Math.Abs(frame2.hwTime - frame1.hwTime)}");
                return (Math.Abs(frame1.hwTime - frame2.hwTime))/*/100*/;
            }
        }
        public FrameSyncPair()
        {
        }
    }

    public class FrameToPair
    {
        public uint counter;
        public double systemTime;
        public double hwTime;
        public string sensor;
        public bool isPairFound;
        public int trigger;
        public string subType;
        public FrameToPair(Utils.FRAME_DATA_RS400 frame)
        {
            counter = frame.FrameCounter;
            systemTime = frame.systemRelativeTime;
            hwTime = Convert.ToDouble(frame.hwTimeStamp);
            sensor = frame.SensorType.ToString();
            isPairFound = false;
            trigger = frame.ConfigurationMetadata.trigger;
            subType = frame.subType;
        }
    }

   



}
