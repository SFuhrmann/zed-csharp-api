//======= Copyright (c) Stereolabs Corporation, All rights reserved. ===============
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace sl
{
    /// \ingroup Fusion_group
    /// <summary>
    /// Holds Fusion process data and functions
    /// </summary>
    public class Fusion
    {
        /// <summary>
        /// DLL name, used for extern calls to the wrapper.
        /// </summary>
        const string nameDll = sl.ZEDCommon.NameDLL;

        #region DLL Calls

        /************************************************************************
         * Fusion API Initialisation
         ************************************************************************/

        [DllImport(nameDll, EntryPoint = "sl_fusion_read_configuration_file")]
        private static extern FUSION_ERROR_CODE dllz_fusion_read_configuration_file(System.Text.StringBuilder jsonConfigFileName, COORDINATE_SYSTEM coordinateSystem, UNIT unit, [In, Out] FusionConfiguration[] configs, ref int nbCameras);

        [DllImport(nameDll, EntryPoint = "sl_fusion_get_sender_state")]
        private static extern SENDER_ERROR_CODE dllz_fusion_get_sender_state(ref CameraIdentifier uuid);

        [DllImport(nameDll, EntryPoint = "sl_fusion_init")]
        private static extern FUSION_ERROR_CODE dllz_fusion_init(ref InitFusionParameters initFusionParams);

        [DllImport(nameDll, EntryPoint = "sl_fusion_process")]
        private static extern FUSION_ERROR_CODE dllz_fusion_process();

        [DllImport(nameDll, EntryPoint = "sl_fusion_subscribe")]
        private static extern FUSION_ERROR_CODE dllz_fusion_subscribe(ref CameraIdentifier uuid, System.Text.StringBuilder jsonConfigFileName, ref Vector3 poseTranslation, ref System.Numerics.Quaternion poseRotation);

        [DllImport(nameDll, EntryPoint = "sl_fusion_update_pose")]
        private static extern FUSION_ERROR_CODE dllz_fusion_update_pose(ref CameraIdentifier uuid, ref Vector3 poseTranslation, ref System.Numerics.Quaternion poseRotation);

        /************************************************************************
         * Body Tracking Fusion
         ************************************************************************/

        [DllImport(nameDll, EntryPoint = "sl_fusion_enable_body_tracking")]
        private static extern FUSION_ERROR_CODE dllz_fusion_enable_body_tracking(ref BodyTrackingFusionParameters btfParams);

        [DllImport(nameDll, EntryPoint = "sl_fusion_disable_body_tracking")]
        private static extern void dllz_fusion_disable_body_tracking();

        [DllImport(nameDll, EntryPoint = "sl_fusion_retrieve_bodies")]
        private static extern FUSION_ERROR_CODE dllz_fusion_retrieve_bodies(ref Bodies bodies, ref BodyTrackingFusionRuntimeParameters rtparams, CameraIdentifier uuid);

        [DllImport(nameDll, EntryPoint = "sl_fusion_get_process_metrics")]
        private static extern FUSION_ERROR_CODE dllz_fusion_get_process_metrics(ref FusionMetrics metrics);

        /************************************************************************
         * Positional Tracking
         ************************************************************************/

        [DllImport(nameDll, EntryPoint = "sl_fusion_enable_positional_tracking")]
        private static extern FUSION_ERROR_CODE dllz_fusion_enable_positional_tracking(ref sl_PositionalTrackingFusionParameters ptfParams);

        [DllImport(nameDll, EntryPoint = "sl_fusion_get_position")]
        private static extern POSITIONAL_TRACKING_STATE dllz_fusion_get_position(ref Pose pose, REFERENCE_FRAME referenceFrame, UNIT unit, ref CameraIdentifier uuid, POSITION_TYPE retrieveType);

        [DllImport(nameDll, EntryPoint = "sl_fusion_disable_positional_tracking")]
        private static extern void dllz_fusion_disable_positional_tracking();

        [DllImport(nameDll, EntryPoint = "sl_fusion_close")]
        private static extern void dllz_fusion_close();

        /************************************************************************
         * GNSS Fusion
         ************************************************************************/

        [DllImport(nameDll, EntryPoint = "sl_fusion_ingest_gnss_data")]
        private static extern void dllz_fusion_ingest_gnss_data(ref GNSSData data);

        [DllImport(nameDll, EntryPoint = "sl_fusion_get_current_gnss_data")]
        private static extern POSITIONAL_TRACKING_STATE dllz_fusion_get_current_gnss_data(ref GNSSData data);

        [DllImport(nameDll, EntryPoint = "sl_fusion_get_geo_pose")]
        private static extern GNSS_CALIBRATION_STATE dllz_fusion_get_geo_pose(ref GeoPose pose);

        [DllImport(nameDll, EntryPoint = "sl_fusion_geo_to_camera")]
        private static extern GNSS_CALIBRATION_STATE dllz_fusion_geo_to_camera(ref LatLng inLatLng, out Pose outPose);

        [DllImport(nameDll, EntryPoint = "sl_fusion_camera_to_geo")]
        private static extern GNSS_CALIBRATION_STATE dllz_fusion_camera_to_geo(ref Pose inPose, out GeoPose outGeoPose);

        [DllImport(nameDll, EntryPoint = "sl_fusion_get_current_timestamp")]
        private static extern ulong dllz_fusion_get_current_timestamp();

        [DllImport(nameDll, EntryPoint = "sl_fusion_get_current_gnss_calibration_std")]
        private static extern GNSS_CALIBRATION_STATE dllz_fusion_get_current_gnss_calibration_std(ref float yawStd, ref Vector3 positionStd);

        [DllImport(nameDll, EntryPoint = "sl_fusion_get_geo_tracking_calibration")]
        private static extern void dllz_fusion_get_geo_tracking_calibration(ref Vector3 position, ref System.Numerics.Quaternion rotation);

        #endregion

        /// <summary>
        /// DLL-friendly version of GNSSCalibrationParameters (found in ZEDCommon.cs).
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct sl_GNSSCalibrationParameters
        {
            /// <summary>
            /// This parameter defines the target yaw uncertainty at which the calibration process between GNSS and VIO concludes. The unit of this parameter is in radian. By default, the threshold is set at 0.1 radians.
            /// </summary>
            public float targetYawUncertainty;
            /// <summary>
            /// When this parameter is enabled (set to true), the calibration process between GNSS and VIO accounts for the uncertainty in the determined translation, thereby facilitating the calibration termination. 
            /// The maximum allowable uncertainty is controlled by the 'target_translation_uncertainty' parameter.
            /// By default, it is set to false.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public bool enableTranslationUncertaintyTarget;
            /// <summary>
            /// This parameter defines the target translation uncertainty at which the calibration process between GNSS and VIO terminates. By default, the threshold is set at 10 centimeters (10e-2).
            /// </summary>
            public float targetTranslationUncertainty;
            /// <summary>
            /// This initialization parameter determines whether reinitialization should be performed between GNSS and VIO fusion when a significant disparity is detected between GNSS data and the current fusion data. 
            /// It becomes particularly crucial during prolonged GNSS signal loss scenarios.
            /// By default, it is set to true.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public bool enableReinitialization;
            /// <summary>
            /// This parameter determines the threshold for GNSS/VIO reinitialization. If the fused position deviates beyond the region defined by the product of the GNSS covariance and the gnss_vio_reinit_threshold, a reinitialization will be triggered.
            /// By default, it is set to 5.
            /// </summary>
            public float gnssVioReinitThreshold;
            /// <summary>
            /// If this parameter is set to true, the fusion algorithm will used a rough VIO / GNSS calibration at first and then refine it. This allow you to quickly get a fused position.
            /// By default, it is set to true.
            /// </summary>     
            [MarshalAs(UnmanagedType.U1)]
            public bool enableRollingCalibration;

            public sl_GNSSCalibrationParameters(GNSSCalibrationParameters gnssCalibrationParameters)
            {
                targetYawUncertainty = gnssCalibrationParameters.targetYawUncertainty;
                enableTranslationUncertaintyTarget = gnssCalibrationParameters.enableTranslationUncertaintyTarget;
                targetTranslationUncertainty = gnssCalibrationParameters.targetTranslationUncertainty;
                enableReinitialization = gnssCalibrationParameters.enableReinitialization;
                gnssVioReinitThreshold = gnssCalibrationParameters.gnssVioReinitThreshold;
                enableRollingCalibration = gnssCalibrationParameters.enableRollingCalibration;
            }
        };

        /// <summary>
        /// DLL-friendly version of PositionalTrackingFusionParameters (found in ZEDCommon.cs).
        /// </summary>
        public struct sl_PositionalTrackingFusionParameters
        {
            /// <summary>
            /// If the GNSS should be enabled.
            /// </summary>
            public bool enableGNSSFusion;
            /// <summary>
            /// GNSS calibration parameter. Determine target threshold for GNSS / VIO calibration.
            /// </summary>
            public sl_GNSSCalibrationParameters gnssCalibrationParameters;
            /// <summary>
            /// Constructor
            /// </summary>
            public sl_PositionalTrackingFusionParameters(PositionalTrackingFusionParameters positionalTrackingFusionParameters)
            {
                enableGNSSFusion = positionalTrackingFusionParameters.enableGNSSFusion;
                gnssCalibrationParameters = new sl_GNSSCalibrationParameters(positionalTrackingFusionParameters.gnssCalibrationParameters);
            }
        }


        ~Fusion()
        {
            Close();
        }

        public void Close()
        {
            dllz_fusion_close();
        }

        /// <summary>
        /// Read a Configuration JSON file to configure a fusion process.
        /// </summary>
        /// <param name="jsonConfigFileName">The name of the JSON file containing the configuration.</param>
        /// <param name="coordinateSystem">The COORDINATE_SYSTEM in which you want the World Pose to be in.</param>
        /// <param name="unit">The UNIT in which you want the World Pose to be in.</param>
        /// <param name="configs"> An array of \ref FusionConfiguration for all the camera present in the file.</param>
        /// <param name="nbCameras">Number of cameras</param>
        public void ReadConfigurationFile(System.Text.StringBuilder jsonConfigFileName, COORDINATE_SYSTEM coordinateSystem, UNIT unit, ref FusionConfiguration[] configs, ref int nbCameras)
        {
            configs = new FusionConfiguration[(int)Constant.MAX_FUSED_CAMERAS];
            dllz_fusion_read_configuration_file(jsonConfigFileName, coordinateSystem, unit, configs, ref nbCameras);
        }

        /// <summary>
        /// Returns the state of a connected data sender.
        /// </summary>
        /// <param name="uuid">Identifier of the camera.</param>
        /// <returns>State of the sender </returns>
        public SENDER_ERROR_CODE GetSenderState(ref CameraIdentifier uuid)
        {
            return dllz_fusion_get_sender_state(ref uuid);
        }

        /// <summary>
        /// FusionHandler initialisation. Initializes memory/generic datas
        /// </summary>
        /// <param name="initFusionParameters"></param>
        /// <returns></returns>
        public FUSION_ERROR_CODE Init(ref InitFusionParameters initFusionParameters)
        {
            return dllz_fusion_init(ref initFusionParameters);
        }

        /// <summary>
        /// processes the fusion.
        /// </summary>
        /// <returns></returns>
        public FUSION_ERROR_CODE Process()
        {
            return dllz_fusion_process();
        }

        /// <summary>
        ///  adds a camera to the multi camera handler
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="jsonConfigFileName"></param>
        /// <param name="poseTranslation"></param>
        /// <param name="poseRotation"></param>
        /// <returns></returns>
        public FUSION_ERROR_CODE Subscribe(ref CameraIdentifier uuid, string jsonConfigFileName, ref Vector3 poseTranslation, ref System.Numerics.Quaternion poseRotation)
        {
            return dllz_fusion_subscribe(ref uuid, new System.Text.StringBuilder(jsonConfigFileName, jsonConfigFileName.Length), ref poseTranslation, ref poseRotation);
        }

        /// <summary>
        /// updates the camera World pose
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="poseTranslation"></param>
        /// <param name="poseRotation"></param>
        /// <returns></returns>
        public FUSION_ERROR_CODE UpdatePose(ref CameraIdentifier uuid, ref Vector3 poseTranslation, ref System.Numerics.Quaternion poseRotation)
        {
            return dllz_fusion_update_pose(ref uuid, ref poseTranslation, ref poseRotation);
        }

        /************************************************************************
         * Body Tracking Fusion
         ************************************************************************/
        /// <summary>
        /// enables the body tracking module
        /// </summary>
        /// <param name="btfParams"></param>
        /// <returns></returns>
        public FUSION_ERROR_CODE EnableBodyTracking(ref BodyTrackingFusionParameters btfParams)
        {
            return dllz_fusion_enable_body_tracking(ref btfParams);
        }

        /// <summary>
        /// disable the body tracking module
        /// </summary>
        public void DisableBodyTracking()
        {
            dllz_fusion_disable_body_tracking();
        }

        /// <summary>
        /// retrieves the body data, can be the fused data (default), or the raw data provided by a specific sender
        /// </summary>
        /// <param name="bodies"></param>
        /// <param name="rtparams"></param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public FUSION_ERROR_CODE RetrieveBodies(ref Bodies bodies, ref BodyTrackingFusionRuntimeParameters rtparams, CameraIdentifier uuid)
        {
            return dllz_fusion_retrieve_bodies(ref bodies, ref rtparams, uuid);
        }

        /// <summary>
        /// gets the metrics of the Fusion process, for the fused data as well as individual camera provider data
        /// </summary>
        /// <param name="metrics"></param>
        /// <returns></returns>
        public FUSION_ERROR_CODE GetProcessMetrics(ref FusionMetrics metrics)
        {
            return dllz_fusion_get_process_metrics(ref metrics);
        }

        /************************************************************************
         * Positional Tracking
         ************************************************************************/

        /// <summary>
        /// enables positional tracking module
        /// </summary>
        /// <param name="ptfParams"></param>
        /// <returns></returns>
        public FUSION_ERROR_CODE EnablePositionalTracking(ref PositionalTrackingFusionParameters ptfParams)
        {
            sl_PositionalTrackingFusionParameters sl_PositionalTrackingFusionParameters = new sl_PositionalTrackingFusionParameters(ptfParams);
            sl.FUSION_ERROR_CODE err = (sl.FUSION_ERROR_CODE)dllz_fusion_enable_positional_tracking(ref sl_PositionalTrackingFusionParameters);
            return err;
        }

        /// <summary>
        /// Gets the Fused Position of the camera system
        /// </summary>
        /// <param name="pose">contains the camera pose in world position (world position is given by the calibration of the cameras system)</param>
        /// <param name="referenceFrame">defines the reference from which you want the pose to be expressed. Default : \ref REFERENCE_FRAME "REFERENCE_FRAME::WORLD".</param>
        /// <param name="coordinateSystem"></param>
        /// <param name="unit"></param>
        /// <param name="uuid"></param>
        /// <param name="retrieveType"></param>
        /// <returns></returns>
        public POSITIONAL_TRACKING_STATE GetPosition(ref Pose pose, REFERENCE_FRAME referenceFrame, UNIT unit, ref CameraIdentifier uuid, POSITION_TYPE retrieveType)
        {
            return dllz_fusion_get_position(ref pose, referenceFrame, unit, ref uuid, retrieveType);
        }

        /// <summary>
        /// Disables the positional tracking 
        /// </summary>
        public void DisablePositionalTracking()
        {
            dllz_fusion_disable_positional_tracking();
        }

        /************************************************************************
         * GNSS Fusion
         ************************************************************************/

        /// <summary>
        /// ingests GNSS data from an external sensor into the fusion module
        /// </summary>
        /// <param name="data">the current GNSS data to combine with the current positional tracking data</param>
        public void IngestGNSSData(ref GNSSData data)
        {
            dllz_fusion_ingest_gnss_data(ref data);
        }

        /// <summary>
        /// returns the current GNSS data
        /// </summary>
        /// <param name="data">the current GNSS data </param>
        /// <returns></returns>
        public POSITIONAL_TRACKING_STATE GetCurrentGNSSData(ref GNSSData data)
        {
            return dllz_fusion_get_current_gnss_data(ref data);
        }

        /// <summary>
        /// returns the current GeoPose
        /// </summary>
        /// <param name="pose">the current GeoPose</param>
        /// <returns></returns>
        public GNSS_CALIBRATION_STATE GetGeoPose(ref GeoPose pose)
        {
            return dllz_fusion_get_geo_pose(ref pose);
        }

        /// <summary>
        /// Converts latitude / longitude into position in sl::Fusion coordinate system.
        /// </summary>
        /// <param name="inLatLng"></param>
        /// <param name="outPose"></param>
        /// <returns></returns>
        public GNSS_CALIBRATION_STATE GeoToCamera(ref LatLng inLatLng, out Pose outPose)
        {
            return dllz_fusion_geo_to_camera(ref inLatLng, out outPose);
        }

        /// <summary>
        /// Converts a position in sl::Fusion coordinate system in real world coordinate. 
        /// </summary>
        /// <param name="inPose"></param>
        /// <param name="outGeoPose"></param>
        /// <returns></returns>
        public GNSS_CALIBRATION_STATE CameraToGeo(ref Pose inPose, out GeoPose outGeoPose)
        {
            return dllz_fusion_camera_to_geo(ref inPose, out outGeoPose);
        }

        /// <summary>
        /// Gets the current timestamp.
        /// </summary>
        /// <returns></returns>
        public ulong GetCurrentTimestamp()
        {
            return dllz_fusion_get_current_timestamp();
        }

        /// <summary>
        /// Get the current calibration uncertainty defined during calibration process
        /// </summary>
        /// <param name="yawStd">yaw uncertainty</param>
        /// <param name="positionStd">position uncertainty</param>
        /// <returns></returns>
        public GNSS_CALIBRATION_STATE GetCurrentGNSSCalibrationStd(ref float yawStd, ref Vector3 positionStd)
        {
            return dllz_fusion_get_current_gnss_calibration_std(ref yawStd, ref positionStd);
        }

        /// <summary>
        /// Get the calibration found between VIO and GNSS
        /// </summary>
        /// <param name="position">calibration found between VIO and GNSS (Translation)</param>
        /// <param name="rotation">calibration found between VIO and GNSS (Rotation)</param>
        public void GetGeoTrackingCalibration(ref Vector3 position, ref Quaternion rotation)
        {
            dllz_fusion_get_geo_tracking_calibration(ref position, ref rotation);
        }
    }
}