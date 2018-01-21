using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Capture;

namespace QAFrameServerValidator
{
    public class CameraUtil
    {     
        private FrameReaderUtil fr;
        private uint numberOfFramesToCollect;
        private static ManualResetEvent mediaCaptureReady = new ManualResetEvent(false);
        private List<double> frameTimeStampsWithPropertiesList;
        public bool lastStatus;
        public bool profileStatus;
        public bool isColorCamera;


        public bool Start_PNP(string cameraType, string format, uint fps, uint x_resolution, uint y_resolution, List<Control> controls,  bool isPropertiesWhileStreamingTestRunning)
        {
            fr = new FrameReaderUtil();
            fr.SetSettings(cameraType, format, fps, x_resolution, y_resolution, numberOfFramesToCollect, controls, isPropertiesWhileStreamingTestRunning);
            Task.Run(async () =>
            {
                bool bSuccess = await fr.SelectCameraSource(isColorCamera);
                if (bSuccess) { bSuccess = await fr.InitializeMediaCapture(isColorCamera); Console.WriteLine("initializeMediaCapture bool: " + bSuccess); }
                if (bSuccess) { bSuccess = await fr.SetFrameFormat(isColorCamera); Console.WriteLine("setFrameFormat bool: " + bSuccess); }
                mediaCaptureReady.Set();
                if (bSuccess) { bSuccess = await fr.CreateFrameReader(isColorCamera); Console.WriteLine("createFrameReader bool: " + bSuccess); }
                lastStatus = bSuccess;
            }).GetAwaiter().GetResult();
            if (lastStatus)
            {
                fr.TerminateTest();
            }
            Task.Run(async () =>
            {
                await fr.CleanUp();
            }).GetAwaiter().GetResult();
            return true;
        }

        public bool StartWhilePropertiesStreaming(string cameraType, string format, uint fps, uint x_resolution, uint y_resolution, List<Control> controls)
        {
            frameTimeStampsWithPropertiesList = new List<double>();
            if (cameraType.Contains("Color") || cameraType.Contains("RGB"))
            {
                isColorCamera = true;
            }
            else
            {
                isColorCamera = false;
            }
            fr = new FrameReaderUtil();
            fr.SetSettingsWithPropertiesWhileStreaming(cameraType, format, fps, x_resolution, y_resolution, numberOfFramesToCollect, controls);
            Task.Run(async () =>
            {
                bool bSuccess = await fr.SelectCameraSource(isColorCamera);
                if (bSuccess) { bSuccess = await fr.InitializeMediaCapture(isColorCamera); Console.WriteLine("initializeMediaCapture bool: " + bSuccess); }
                if (bSuccess) { bSuccess = await fr.SetFrameFormat(isColorCamera); Console.WriteLine("setFrameFormat bool: " + bSuccess); }
                mediaCaptureReady.Set();
                if (bSuccess) { bSuccess = await fr.CreateFrameReader(isColorCamera); Console.WriteLine("createFrameReader bool: " + bSuccess); }
                lastStatus = bSuccess;
            }).GetAwaiter().GetResult();
            if (lastStatus)
            {
                fr.TerminateTest();
            }
            Task.Run(async () =>
            {
                await fr.CleanUp();
            }).GetAwaiter().GetResult();

            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Start Time Until First Frame Arrived :" + fr.delay);
            Console.BackgroundColor = ConsoleColor.Black;

            Logger.AppendInfo("Start Time Until First Frame Arrived :" + fr.delay);
            Logger.AppendMessage("Is Media Capture = Null  " + fr.VerifyIfMediaCaptureDisposed());
            bool isFrameDeltaOk;
            bool isDeltaBetweenSequenceFramesOk = true;
            bool isFPSPassed = true;
            if (fr.ifAllFramesArrived)
            {
                try
                {
                    fr.analyser.framesTimeStampList = fr.GetFramesTime().ToList();
                    if (fr.productName.Contains("SR300"))
                    {
                        //calculate FPS for all frames
                        //Console.WriteLine("OVERALL FPS:");
                        //isFrameDeltaOk = fr.analyser.CalculateFPS(cameraType, (int)fps, (int)fr.frameCount, (float)SetControl.SR300DepthAccuracyDefaultValue, (float)SetControl.SR300DepthMotionVSRangeTradeoffDefaultValue, SetControl.SR300ColorExposureDefaultValue/**, fr.analyser.framesTimeStampList*/);//calculate overall FPS
                        //Console.BackgroundColor = ConsoleColor.DarkMagenta;
                        //Console.WriteLine("FPS TEST Result is :" + isFrameDeltaOk);

                        //Analyze FPS according to properties                                                                                                                                                                                                                                                       //calculate FPS based on properties
                        if (fr.cameratype == "Depth")
                        {
                            for (int i = 0; i < fr.SR300DepthFrameList.Count() - 1; i++)
                            {
                                if (fr.SR300DepthFrameList[i].properties.Equals(fr.SR300DepthFrameList[i + 1].properties))
                                {
                                    frameTimeStampsWithPropertiesList.Add(fr.SR300DepthFrameList[i].timeStamp);
                                }
                                else
                                {
                                    if (frameTimeStampsWithPropertiesList.Count() > 2)
                                    {
                                        Console.WriteLine("FPS for frames with control values " + fr.SR300DepthFrameList[i].properties);
                                        Logger.AppendInfo("FPS for frames with control values " + fr.SR300DepthFrameList[i].properties);
                                        fr.analyser.framesTimeStampList = frameTimeStampsWithPropertiesList;
                                        isFrameDeltaOk = fr.analyser.CalculateFPS(cameraType, (int)fps, frameTimeStampsWithPropertiesList.Count, (float)fr.SR300DepthFrameList[i].properties.accuracy, (float)fr.SR300DepthFrameList[i].properties.motionVsRange, SetControl.SR300ColorExposureDefaultValue);
                                        frameTimeStampsWithPropertiesList.Clear();
                                        if (!isFrameDeltaOk)
                                        {
                                            isFPSPassed = false;
                                        }
                                        Console.BackgroundColor = ConsoleColor.DarkMagenta;
                                        Console.WriteLine("FPS TEST Result is :" + isFrameDeltaOk);
                                        Console.BackgroundColor = ConsoleColor.Black;
                                        Logger.AppendInfo("FPS TEST Result is :" + isFrameDeltaOk);
                                        bool tempisDeltaBetweenSequenceFramesOk = fr.analyser.IsFrameDeltaBetweenFramesOKPropertiesWhileStreamingSR300(false, (int)fps, (int)SetControl.SR300ColorExposureDefaultValue, (float)fr.SR300DepthFrameList[i].properties.motionVsRange, fr.SR300DepthFrameList[i].properties.accuracy);
                                        if (!tempisDeltaBetweenSequenceFramesOk)
                                        {
                                            isDeltaBetweenSequenceFramesOk = false;
                                        }
                                        Console.BackgroundColor = ConsoleColor.Blue;
                                        Console.WriteLine("Is delta between each 2 sequence frames ok: " + tempisDeltaBetweenSequenceFramesOk);
                                        Logger.AppendInfo("Is delta between each 2 sequence frames ok: " + tempisDeltaBetweenSequenceFramesOk);
                                        Console.BackgroundColor = ConsoleColor.Black;
                                    }
                                }
                            }
                        }
                        else //is color camera
                        {
                            for (int i = 0; i < fr.SR300ColorFrameList.Count() - 1; i++)
                            {
                                if (fr.SR300ColorFrameList[i].properties.Equals(fr.SR300ColorFrameList[i + 1].properties))
                                {
                                    frameTimeStampsWithPropertiesList.Add(fr.SR300ColorFrameList[i].timeStamp);
                                }
                                else
                                {
                                    if (frameTimeStampsWithPropertiesList.Count() > 2)
                                    {
                                        Logger.AppendInfo("FPS for frames with control values " + fr.SR300ColorFrameList[i].properties);
                                        Console.WriteLine("FPS for frames with control values " + fr.SR300ColorFrameList[i].properties);
                                        fr.analyser.framesTimeStampList = frameTimeStampsWithPropertiesList;
                                        isFrameDeltaOk = fr.analyser.CalculateFPS(cameraType, (int)fps, frameTimeStampsWithPropertiesList.Count, (float)SetControl.SR300DepthAccuracyDefaultValue, (float)SetControl.SR300DepthMotionVSRangeTradeoffDefaultValue, (int)Math.Log(fr.SR300ColorFrameList[i].properties.manualExp / 10000.0, 2));
                                        frameTimeStampsWithPropertiesList.Clear();
                                        if (!isFrameDeltaOk)
                                        {
                                            isFPSPassed = false;
                                        }
                                        bool tempisDeltaBetweenSequenceFramesOk = fr.analyser.IsFrameDeltaBetweenFramesOKPropertiesWhileStreamingSR300(true, (int)fps, (int)Math.Log(fr.SR300ColorFrameList[i].properties.manualExp / 10000.0, 2), (float)SetControl.SR300DepthMotionVSRangeTradeoffDefaultValue, (int)SetControl.SR300DepthAccuracyDefaultValue);
                                        if (!tempisDeltaBetweenSequenceFramesOk)
                                        {
                                            isDeltaBetweenSequenceFramesOk = false;
                                        }
                                        Console.BackgroundColor = ConsoleColor.Blue;
                                        Console.WriteLine("Is delta between each 2 sequence frames ok: " + tempisDeltaBetweenSequenceFramesOk);
                                        Logger.AppendInfo("Is delta between each 2 sequence frames ok: " + tempisDeltaBetweenSequenceFramesOk);
                                        Console.BackgroundColor = ConsoleColor.Black;


                                        Console.BackgroundColor = ConsoleColor.DarkMagenta;
                                        Console.WriteLine("FPS TEST Result is :" + isFrameDeltaOk);
                                        Console.BackgroundColor = ConsoleColor.Black;
                                        Logger.AppendInfo("FPS TEST Result is :" + isFrameDeltaOk);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Frame count less than 2.framecount = "+ frameTimeStampsWithPropertiesList.Count());
                                    }
                                }
                            }
                        }
                    }
                    else if (fr.productName.Contains("DS5") || fr.productName.Contains("400") || fr.productName.Contains("410") || fr.productName.Contains("430") || fr.productName.Contains("420"))
                    {
                        //Console.WriteLine("OVERALL FPS:");
                        //int expectedFps = fr.analyser.GetExpectedFpsDs5((int)fps, SetControl.DS5ExposureDefaultValue);
                        //isFrameDeltaOk = fr.analyser.CheckDS5FPS(expectedFps, (int)fr.frameCount, fr.analyser.framesTimeStampList.ToArray());
                        //isDeltaBetweenSequenceFramesOk = fr.analyser.IsFrameDeltaBetweenFramesOKPropertiesWhileStreamingDS5((int)fps, (float)SetControl.SR300DepthMotionVSRangeTradeoffDefaultValue, fr.analyser.framesTimeStampList.ToArray());
                        //Console.BackgroundColor = ConsoleColor.Blue;
                        //Console.WriteLine("Is delta between each 2 sequence frames ok: " + isDeltaBetweenSequenceFramesOk);
                        //Console.BackgroundColor = ConsoleColor.Black;
                        

                        //Analyze FPS according to properties
                        if (fr.cameratype == "Depth")
                        {
                            int expectedFps;
                            for (int i = 0; i < fr.DS5DepthFrameList.Count() - 1; i++)
                            {
                                Logger.AppendInfo("FPS for frames with control values " + fr.DS5DepthFrameList[i].properties);
                                if (fr.DS5DepthFrameList[i].properties.Equals(fr.DS5DepthFrameList[i + 1].properties))
                                {
                                    frameTimeStampsWithPropertiesList.Add(fr.DS5DepthFrameList[i].timeStamp);
                                }
                                else
                                {
                                    if (frameTimeStampsWithPropertiesList.Count() > 2)
                                    {
                                        //Console.WriteLine("FPS for frames with control values " + fr.DS5DepthFrameList[i].properties);
                                        expectedFps = fr.analyser.GetExpectedFpsDs5((int)fps, fr.DS5DepthFrameList[i].properties.manualExposure);
                                        isFrameDeltaOk = fr.analyser.CheckDS5FPS(expectedFps, frameTimeStampsWithPropertiesList.Count(), frameTimeStampsWithPropertiesList.ToArray());
                                        frameTimeStampsWithPropertiesList.Clear();
                                        if (!isFrameDeltaOk)
                                        {
                                            isFPSPassed = false;
                                        }
                                        Console.BackgroundColor = ConsoleColor.DarkMagenta;
                                        Console.WriteLine("FPS TEST Result is :" + isFrameDeltaOk);
                                        Console.BackgroundColor = ConsoleColor.Black;
                                        Logger.AppendInfo("FPS TEST Result is :" + isFrameDeltaOk);
                                        bool tempIsDeltaBetweenSequenceFramesOk = fr.analyser.IsFrameDeltaBetweenFramesOKPropertiesWhileStreamingDS5((int)fps, fr.DS5DepthFrameList[i].properties.manualExposure, frameTimeStampsWithPropertiesList.ToArray());
                                        if (!tempIsDeltaBetweenSequenceFramesOk)
                                        {
                                            isDeltaBetweenSequenceFramesOk = false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    Console.BackgroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("First Frame Arrived After 500 Milliseconds: " + fr.isFirstFrameArrivedLate);
                    Console.BackgroundColor = ConsoleColor.Black;
                    Logger.AppendInfo("First Frame Arrived After 500 Milliseconds: " + fr.isFirstFrameArrivedLate);
                    Logger.AppendInfo(" isDeltaBetweenSequenceFramesOk = " + isDeltaBetweenSequenceFramesOk + " isFPSAccordingToPropertiesPassed " + isFPSPassed);
                    Console.WriteLine(" isDeltaBetweenSequenceFramesOk = " + isDeltaBetweenSequenceFramesOk + " isFPSAccordingToPropertiesPassed " + isFPSPassed);
                    return isDeltaBetweenSequenceFramesOk && isFPSPassed;
                }
                catch (Exception e)
                {
                    Console.WriteLine("==> " + e.Message);
                    profileStatus = false;
                    return profileStatus;
                }
            }
            else
            {
                Console.WriteLine("Not All Frames Arrived!");
                Logger.AppendInfo("Not All Frames Arrived!");
                lastStatus = false;
            }
            return lastStatus;
        }
        
        public bool StartWhileMeasurementTempStreaming(string cameraType, string format, uint fps, uint x_resolution, uint y_resolution, List<Control> controls, uint numberOfFramesToCollect_, bool isPropertiesWhileStreamingTestRunning)
        {
            if (cameraType.Contains("Color") || cameraType.Contains("RGB"))
            {
                isColorCamera = true;
            }

            numberOfFramesToCollect = numberOfFramesToCollect_;

            fr = new FrameReaderUtil();
            fr.SetSettings(cameraType, format, fps, x_resolution, y_resolution, numberOfFramesToCollect, controls, isPropertiesWhileStreamingTestRunning);
            fr.IntializeTempTest(); //initialize the temperature list

            Task.Run(async () =>
            {

                bool bSuccess = await fr.SelectCameraSource(isColorCamera);
                if (bSuccess) { bSuccess = await fr.InitializeMediaCapture(isColorCamera); Console.WriteLine("initializeMediaCapture bool: " + bSuccess); }
                if (bSuccess) { bSuccess = await fr.SetFrameFormat(isColorCamera); Console.WriteLine("setFrameFormat bool: " + bSuccess); }
                mediaCaptureReady.Set();
                //here I call the callback (first we check which type of camera)
                if (bSuccess) { bSuccess = await fr.CreateFrameReaderTemperature(isColorCamera); Console.WriteLine("createFrameReader bool: " + bSuccess); }
                lastStatus = bSuccess;
            }).GetAwaiter().GetResult();


            //wait until the function ends (frames arrived, or stopped) and then kill function (may do it by timeout)
            if (lastStatus)
            {
                fr.TerminateTest();
            }


            if (fr.GetIsSuccess())
            {
                Console.WriteLine("The IMUTemperatureStreamingTest was successfull!");
                throw new Exception("too hot!! burning in here!!");
            }


            //Task.Run(async () =>
            //{
            //    await fr.SelectCameraSource(false);
            //    await fr.InitializeMediaCapture(false);
            //    fr.CallSetControl(controls);
            //    testStatus = fr.CheckFrameFormatAfterSettingFaceAuth();
            //    testStatus = await fr.CreateFrameReader_DoValidationWhileStreaming();
            //}).GetAwaiter().GetResult();


            Task.Run(async () =>
            {
                await fr.CleanUp();
            }).GetAwaiter().GetResult();

            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Start Time Until First Frame Arrived :"+fr.delay);
            Console.BackgroundColor = ConsoleColor.Black;

            //Logger.AppendInfo("Start Time Until First Frame Arrived :" + fr.delay);

            Logger.AppendDebug("Is Media Capture = Null : " + fr.VerifyIfMediaCaptureDisposed());
            Logger.AppendDebug("fr.GetFramesTime().ToList() Size:" + fr?.GetFramesTime()?.Count);
            if (fr.ifAllFramesArrived)
            {

            }
            else
            {
                Console.WriteLine("Not All Frames Arrived!");
                Logger.AppendInfo("Not All Frames Arrived!");
                lastStatus = false;
            }
            return lastStatus;
        }
        
        public bool Start(string cameraType, string format, uint fps, uint x_resolution, uint y_resolution, List<Control> controls, uint numberOfFramesToCollect_, bool isPropertiesWhileStreamingTestRunning)
        {
            if (cameraType.Contains("Color") || cameraType.Contains("RGB"))
            {
                isColorCamera = true;
            }
            numberOfFramesToCollect = numberOfFramesToCollect_;
            fr = new FrameReaderUtil();

            fr.SetSettings(cameraType, format, fps, x_resolution, y_resolution, numberOfFramesToCollect, controls, isPropertiesWhileStreamingTestRunning);
            Task.Run(async () =>
            {
                bool bSuccess = await fr.SelectCameraSource(isColorCamera);
                if (bSuccess) { bSuccess = await fr.InitializeMediaCapture(isColorCamera); Console.WriteLine("initializeMediaCapture bool: " + bSuccess); }
                if (bSuccess) { bSuccess = await fr.SetFrameFormat(isColorCamera); Console.WriteLine("setFrameFormat bool: " + bSuccess); }
                mediaCaptureReady.Set();
                if (bSuccess) { bSuccess = await fr.CreateFrameReader(isColorCamera); Console.WriteLine("createFrameReader bool: " + bSuccess); }
                lastStatus = bSuccess;
            }).GetAwaiter().GetResult();

            if (lastStatus)
            {
                fr.TerminateTest();
            }
            Task.Run(async () =>
            {
                await fr.CleanUp();
            }).GetAwaiter().GetResult();
            return lastStatus;
        }
        
        public bool FPS_Validation(string cameraType, string format, uint fps, uint x_resolution, uint y_resolution, List<Control> controls)
        {
            if (fr.ifAllFramesArrived)
            {
                int value_exposure = 0, value_mvr, value_accuracy;
                bool isDeltaBetweenSequenceFramesOk = true;
               
                if (fr.ifAllFramesArrived)
                {
                    lastStatus = true;
                    try
                    {                      
                        bool isFrameDeltaOk = false;
                        #region SR300
                        if (fr.productName.Contains("SR300"))
                        {
                            value_exposure = (int)controls.Where(x => x.Name.ToUpper().Contains("EXPOSURE")).Select(X => X.Value).DefaultIfEmpty(float.NaN).First();
                            value_mvr = (int)controls.Where(x => x.Name.ToUpper().Contains("MOTION")).Select(X => X.Value).DefaultIfEmpty(float.NaN).First();
                            value_accuracy = 3;

                            isFrameDeltaOk = fr.analyser.CalculateFPS(cameraType, (int)fps, (int)numberOfFramesToCollect, value_accuracy, value_mvr, value_exposure);

                            Console.BackgroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("Is total time  ok: " + isFrameDeltaOk);
                            Console.BackgroundColor = ConsoleColor.Black;
                            Logger.AppendInfo("Is total time  ok: " + isFrameDeltaOk);

                            isDeltaBetweenSequenceFramesOk = fr.analyser.IsFrameDeltaOK(cameraType.Contains("Depth") ? false : true, (int)fps, value_exposure, value_mvr, value_accuracy);

                            Console.BackgroundColor = ConsoleColor.Blue;
                            Console.WriteLine("Is delta between each 2 sequence frames ok: " + isDeltaBetweenSequenceFramesOk);
                            Console.BackgroundColor = ConsoleColor.Black;
                            Logger.AppendInfo("Is delta between each 2 sequence frames ok: " + isDeltaBetweenSequenceFramesOk);

                            ////Check HW Timestamp
                            if (fr.lst_AllFrames.Count > 2)
                            { 
                                Logger.AppendInfo("**********Check HW Timestamp*******************");
                                isDeltaBetweenSequenceFramesOk &= fr.analyser.HW_IsFrameDeltaOK(cameraType.Contains("Depth") ? false : true, (int)fps, value_exposure, value_mvr, value_accuracy);
                                Logger.AppendInfo("**********HW and System TIMESTAMP isDeltaBetweenSequenceFramesOk*******************" + isDeltaBetweenSequenceFramesOk);

                                isFrameDeltaOk = isFrameDeltaOk && isDeltaBetweenSequenceFramesOk;
                            }

                            Console.WriteLine("Test " + isFrameDeltaOk + ":  " + x_resolution + "x" + y_resolution + "@fps" + fps);
                            Logger.AppendInfo("Test Result: " + isFrameDeltaOk + ":  " + x_resolution + "x" + y_resolution + "@fps" + fps);
                        }
                        #endregion
                        else if (fr.productName.Contains("DS5") || fr.productName.Contains("400") || fr.productName.Contains("410") || fr.productName.Contains("430") || fr.productName.Contains("420") || fr.productName.Contains("415"))
                        {
                            value_exposure = (int)controls.Where(x => x.Name.ToUpper().Contains("EXPOSURE")).Select(X => X.Value).DefaultIfEmpty(400).First();

                            Console.WriteLine("Frame buffer size: " + fr.lst_Frames.Count);
                            Console.WriteLine("");
                            Console.WriteLine("****Data Analysis ****");
                            Logger.AppendInfo("Frame buffer size: " + fr.lst_Frames.Count);
                            Logger.NewLine();
                            Logger.AppendInfo("****Data Analysis ****");

                            isFrameDeltaOk = fr.analyser.Analysis_SR4xx_FrameData((int)fps, value_exposure, fr.lst_Frames);
                            lastStatus &= isFrameDeltaOk;

                            Console.WriteLine("Profile Status: "+ isFrameDeltaOk);
                            Logger.AppendInfo("Profile Status: "+ isFrameDeltaOk);
                        }
                        
                        if (!fr.productName.Contains("SR300") && value_exposure > 110)
                        {
                            profileStatus = isFrameDeltaOk;
                        }
                        else
                        {
                            profileStatus = isFrameDeltaOk && (!fr.isFirstFrameArrivedLate);
                        }
                        return profileStatus;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("==> " + e.Message);
                        profileStatus = false;
                        return profileStatus;
                    }
                }
                else
                {
                    Console.WriteLine("Not All Frames Arrived!");
                    Logger.AppendInfo("Not All Frames Arrived!");
                    lastStatus = false;
                }
            }
            return lastStatus;
        }


        //**************
        //WinHello Start
        //**************
        public bool Start(List<TestSetup.ControlValues> controls)
        {
            bool testStatus = true;
            bool lastTestStatus = true;

            //***********************************************************************************
            ///Test scenario 1: IRPairTypr value with FaceAuth

            Console.WriteLine("****************************************************************************");
            Console.WriteLine("Scenario 1: IRPairTypr value with FaceAuth");
            Logger.AppendMessage("****************************************************************************");
            Logger.AppendMessage("Scenario 1: IRPairTypr value with FaceAuth");

            fr = new FrameReaderUtil();
            fr.resetFrameCounter();
            fr.SetSettingsSpecificGroupAndSource("Depth", "Infrared");
            Task.Run(async () =>
            {
                await fr.SelectCameraSource(false);
                await fr.InitializeMediaCapture(false);
                fr.SetFaceAuthControl(controls);
                fr.CallSetControl(controls);
                lastStatus = fr.CheckFrameFormatAfterSettingFaceAuth();
                if (lastStatus) { lastStatus = await fr.CreateFrameReader_GetFrameMetadata(); }
            }).GetAwaiter().GetResult();

            testStatus = lastStatus;
            if (lastStatus)
            {
                fr.TerminateTestParallel();
            }

            Task.Run(async () =>
            {
                await fr.CleanUp();
            }).GetAwaiter().GetResult();


            if (testStatus)
            {
                testStatus = fr.IsIRPairTypeValueOk_FaceAuthOn();
                lastTestStatus &= testStatus;

                //***********************************************************************************
                ///Test scenario 2: IRPairType valure without FaceAuth,Check that after dispose the control faceauth is off

                Console.WriteLine("*******************************************************************************");
                Console.WriteLine("Scenario 2: IRPairType value without FaceAuth,Check that after dispose the control faceauth is off");
                Logger.AppendMessage("*******************************************************************************");
                Logger.AppendMessage("Scenario 2: IRPairType value without FaceAuth,Check that after dispose the control faceauth is off");

                fr = new FrameReaderUtil();
                fr.SetSettingsSpecificGroupAndSource("Depth", "Infrared");
                Task.Run(async () =>
                {
                    await fr.SelectCameraSource(false);
                    await fr.InitializeMediaCapture(false);
                    fr.CallSetControl(controls);
                    lastStatus = fr.CheckFrameFormatAfterSettingFaceAuth();
                    if (lastStatus) { lastStatus = await fr.CreateFrameReader_GetFrameMetadata(); }
                }).GetAwaiter().GetResult();
                testStatus = lastStatus;
                if (lastStatus)
                {
                    fr.TerminateTestParallel();
                }

                Task.Run(async () =>
                {
                    await fr.CleanUp();
                }).GetAwaiter().GetResult();


                if (testStatus)
                { testStatus = fr.IsIRPairTypeValueOk_FaceAuthOff() && testStatus; }
            }
            lastTestStatus &= testStatus;


            //***********************************************************************************
            ///Test scenario 3: config resultion not 360x360,60fps then set FaceAuth ,FaceAuth shouldn't work 
            fr = new FrameReaderUtil();
            fr.resetFrameCounter();
            fr.SetSettings("Depth", "L8", 30, 640, 480, 40, null, false);
            Task.Run(async () =>
            {
                await fr.SelectCameraSource(false);
                await fr.InitializeMediaCapture(false);
                fr.CallSetControl(controls);
                fr.SetFaceAuthControl(controls);
                await fr.SetFrameFormat(false);
                lastStatus = !fr.CheckFrameFormatAfterSettingFaceAuth();
                Console.WriteLine("Changing format after setting faceAuth:" + lastStatus);
                lastStatus = await fr.CreateFrameReader_GetFrameMetadata();
            }).GetAwaiter().GetResult();

            testStatus = lastStatus;
            if (lastStatus)
            {
                fr.TerminateTestParallel();
            }

            Task.Run(async () =>
            {
                await fr.CleanUp();
            }).GetAwaiter().GetResult();

            if (testStatus)
            { testStatus = fr.IsIRPairTypeValueOk_FaceAuthOff() && testStatus; }

            lastTestStatus &= testStatus;
            

            //***********************************************************************************
            ///Test scenario 4: config resultion not 360x360,60fps .setting FaceAuth then set format .FaceAuthshould not work
            Console.WriteLine("****************************************************************************");
            Console.WriteLine("Scenario 4 : set FaceAuth then Change Configuration .FaceLogin should not work");
            Logger.AppendMessage("****************************************************************************");
            Logger.AppendMessage("Scenario 4 : set FaceAuth then Change Configuration .FaceLogin should not work");

            fr = new FrameReaderUtil();
            fr.resetFrameCounter();
            fr.SetSettingsSpecificGroupAndSource("Depth", "Infrared");
            Task.Run(async () =>
            {
                await fr.SelectCameraSource(false);
                await fr.InitializeMediaCapture(false);
                fr.CallSetControl(controls);
                fr.SetFaceAuthControl(controls);
                lastStatus = fr.CheckFrameFormatAfterSettingFaceAuth();

                Console.WriteLine("change config");
                fr.SetSettings("Depth", "L8", 30, 640, 480, 40, null, false);               
                await fr.SetFrameFormat(false);

                if (lastStatus) { lastStatus = await fr.CreateFrameReader_GetFrameMetadata(); }
            }).GetAwaiter().GetResult();
            testStatus = lastStatus;
            if (lastStatus)
            {
                fr.TerminateTestParallel();
            }

            Task.Run(async () =>
            {
                await fr.CleanUp();
            }).GetAwaiter().GetResult();

            if (testStatus)
            { testStatus = fr.IsIRPairTypeValueOk_FaceAuthOff() && testStatus; }

            Console.WriteLine("Test 4 Status " + testStatus);

            lastTestStatus &= testStatus;

            //***********************************************************************************
            ///Test scenario 5: 
            Console.WriteLine("****************************************************************************");
            Console.WriteLine("Scenario 5 : try to set face login while streaming ");
            Logger.AppendMessage("****************************************************************************");
            Logger.AppendMessage("Scenario 5 : try to set face login while streaming ");


            fr = new FrameReaderUtil();
            fr.SetSettingsSpecificGroupAndSource("Depth", "Infrared");
            Task.Run(async () =>
            {
                await fr.SelectCameraSource(false);
                await fr.InitializeMediaCapture(false);
                fr.CallSetControl(controls);
                testStatus = fr.CheckFrameFormatAfterSettingFaceAuth();
                testStatus = await fr.CreateFrameReader_DoValidationWhileStreaming(); 
            }).GetAwaiter().GetResult();

            if (testStatus)
            {
                fr.TerminateTestParallel();
            }

            Task.Run(async () =>
            {
                await fr.CleanUp();
            }).GetAwaiter().GetResult();


            if (testStatus)
            { testStatus = fr.IsIRPairTypeValueOk_FaceAuthOff() && testStatus; }

            Console.WriteLine("test 5 Status: " + testStatus);

            lastTestStatus &= testStatus;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Final Test Status: " + lastTestStatus);
            Console.ForegroundColor = ConsoleColor.White;
            return lastTestStatus;            
        }
        
        public bool StartIDLEPowerTest()
        {
            fr = new FrameReaderUtil();
            fr.resetFrameCounter();

            bool bSuccess = true;
            isColorCamera = false;

            // check IDLE Power 
            
            Task.Run(async () =>
            {
                bSuccess = await fr.SelectCameraSource();
                if (bSuccess) { await fr.InitializeMediaCapture(false); Console.WriteLine("initializeMediaCapture bool: " + bSuccess); }
                mediaCaptureReady.Set();
            }).GetAwaiter().GetResult();

            fr.SetSettings("Depth", "D16", 90, 1280, 720, 50, null, false);

            fr.isFishEyeCamera = false;
            Task.Run(async () =>
            {
                if (bSuccess) { bSuccess = await fr.CreateFrameReader(isColorCamera); Console.WriteLine("createFrameReader bool: " + bSuccess); }
                lastStatus = bSuccess;
            }).GetAwaiter().GetResult();


            // check IDLE Power x2 *****************1.while stream 2. after stream

            fr.SetSettings("Depth", "D16", 6, 424, 240, 50, null, false);


            Task.Run(async () =>
            {
                if (bSuccess) { bSuccess = await fr.CreateFrameReader(isColorCamera); Console.WriteLine("createFrameReader bool: " + bSuccess); }
                lastStatus = bSuccess;
            }).GetAwaiter().GetResult();


            // check IDLE Power x2 *****************1.while stream 2. after stream


            return bSuccess;
        }

        public bool StartMultiSensor()
        {
            fr = new FrameReaderUtil();
            fr.resetFrameCounter();
            
            bool bSuccess =true;
            isColorCamera = false;
            

            fr.SetSettingsSpecificGroupAndSource("Cameras", "Depth");
            Task.Run(async () =>
            {
                bSuccess = await fr.SelectCameraSource();
                if (bSuccess) { await fr.InitializeMediaCapture(false); Console.WriteLine("initializeMediaCapture bool: " + bSuccess); }
                mediaCaptureReady.Set();              
            }).GetAwaiter().GetResult();

            fr.SetSettingsSpecificGroupAndSource("Cameras", "Custom");
            Task.Run(async () =>
            {
                bSuccess = await fr.SelectCameraSource();
                if (bSuccess) { await fr.InitializeMediaCapture(false); Console.WriteLine("initializeMediaCapture bool: " + bSuccess); }
                fr.SetControlValue();
            }).GetAwaiter().GetResult();

            fr.isFishEyeCamera = false;
            Task.Run(async () => 
            {
                if (bSuccess) { bSuccess = await fr.CreateFrameReader(isColorCamera); Console.WriteLine("createFrameReader bool: " + bSuccess); }
                lastStatus = bSuccess;
            }).GetAwaiter().GetResult();

            fr.isFishEyeCamera = true;
            Task.Run(async () =>
            {
                if (bSuccess) { bSuccess = await fr.CreateFrameReader(isColorCamera); Console.WriteLine("createFrameReader bool: " + bSuccess); }
                lastStatus &= bSuccess;
            }).GetAwaiter().GetResult();


            if (lastStatus)
            {
                fr.TerminateTestParallel();
            }

            Task.Run(async () =>
            {
                await fr.CleanUp();
            }).GetAwaiter().GetResult();

            return true;
        }

        public MediaCapture GetDepthMediaCapture()
        {
            return fr.GetDepthMediaCapture();
        }
        public MediaCapture GetColorMediaCapture()
        {
            return fr.GetColorMediaCapture();
        }
   
        public void FinishedSettingControls()
        {
            fr.isSettingControlsFinished = true;
            GetFinishedSettingControls().Set();
        }
        public string getProductName()
        {
            return fr.productName;
        } 
        public ManualResetEvent getMediaCaptureReady()
        {
            return mediaCaptureReady;
        }
        public ManualResetEvent GetFinishedSettingControls()
        {
            return fr.GetFinishedSettingControls();
        }
        public ManualResetEvent GetFinishedStreaming()
        {
                return fr.GetFinishedStreaming();
        }
        
    }
}
